namespace WisT;

using System.Globalization;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Backend;
using WisT.WistContent;

public class WistGrammarVisitor : WistGrammarBaseVisitor<object?>
{
    private const string This = "this";
    private const string Constructor = "Ctor";

    private WistImageBuilder _imageBuilder = null!;
    private List<string> _importedCodes = null!;
    private IParseTree? _methodCall;
    private int _needResultLevel;
    private string _path = string.Empty;
    private WistLibsManager _wistLibsManager = null!;

    private WistGrammarVisitor(WistImageBuilder imageBuilder, WistLibsManager wistLibsManager,
        List<string> importedCodes)
    {
        _imageBuilder = imageBuilder;
        _wistLibsManager = wistLibsManager;
        _needResultLevel = 0;
        _methodCall = null!;
        _importedCodes = importedCodes;
    }

    public WistGrammarVisitor()
    {
    }

    public override object? VisitClassDecl(WistGrammarParser.ClassDeclContext context)
    {
        _imageBuilder.CreateClass(
            context.IDENTIFIER(0).GetText(),
            context.IDENTIFIER().Skip(1).Select(x => x.GetText()).Append("parents").ToList(),
            new List<string>()
        );

        Visit(context.block());

        return default;
    }

    public override object? VisitClassInitExpression(WistGrammarParser.ClassInitExpressionContext context)
        => Visit(context.classInit());

    public override object? VisitClassInit(WistGrammarParser.ClassInitContext context)
    {
        // handle params
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);

        _imageBuilder.InstantiateClass(context.IDENTIFIER().GetText());
        _imageBuilder.Dup();
        _imageBuilder.CallMethod(Constructor, context.expression().Length);

        return default;
    }

    public WistImageObject CompileCode(WistGrammarParser.ProgramContext program, string path, bool retImage = true)
    {
        _path = path;
        _imageBuilder = new WistImageBuilder();
        _wistLibsManager = new WistLibsManager();
        _importedCodes = new List<string>();
        _needResultLevel = 0;
        _wistLibsManager.AddLibByType(typeof(WistBuildInFunctions));

        Visit(program);

        return retImage ? GetFixedImage() : default!;
    }

    public override object? VisitAssigment(WistGrammarParser.AssigmentContext context) =>
        Visit(
            (IParseTree)context.varAssigment() ??
            (IParseTree)context.elementOfArrayAssigment() ??
            (IParseTree)context.fieldAssigment()
        );

    public override object? VisitFieldAssigment(WistGrammarParser.FieldAssigmentContext context)
    {
        Visit(context.expression(0)); // class

        var assigmentSign = context.ASSIGMENT_SIGN().GetText();
        if (assigmentSign != "=")
        {
            Visit(context.expression(0)); // class
            _imageBuilder.LoadField(context.IDENTIFIER().GetText()); // field
            Visit(context.expression(1));
            HandleMulOrAddOp(assigmentSign[..^1]);
        }
        else
        {
            Visit(context.expression(1)); // value
        }

        _imageBuilder.SetField(context.IDENTIFIER().GetText());
        return default;
    }

    public override object? VisitMethodCall(WistGrammarParser.MethodCallContext context)
    {
        _methodCall = context.expression();

        Visit(context.call());

        return default;
    }

    public override object? VisitFieldExpression(WistGrammarParser.FieldExpressionContext context)
    {
        Visit(context.expression()); // class
        _imageBuilder.LoadField(context.IDENTIFIER().GetText());
        return default;
    }

    public override object? VisitVarAssigment(WistGrammarParser.VarAssigmentContext context)
    {
        _needResultLevel++;
        var type = context.TYPE()?.GetText();
        var name = context.IDENTIFIER().GetText();

        if (type == "let")
            _imageBuilder.CreateLocal(name);
        else if (type == "var")
            _imageBuilder.CreateGlobal(name);


        var assigmentSign = context.ASSIGMENT_SIGN().GetText();
        if (assigmentSign != "=")
        {
            _imageBuilder.LoadGlobalOrLocal(name);
            Visit(context.expression());
            HandleMulOrAddOp(assigmentSign[..^1]);
        }
        else
        {
            Visit(context.expression()); // value
        }

        _imageBuilder.SetGlobalOrLocal(name);

        _needResultLevel--;

        return default;
    }

    public override object? VisitElementOfArrayAssigment(WistGrammarParser.ElementOfArrayAssigmentContext context)
    {
        Visit(context.expression(0)); // array

        var assigmentSign = context.ASSIGMENT_SIGN().GetText();
        if (assigmentSign != "=")
        {
            Visit(context.expression(0)); // array
            Visit(context.expression(1)); // index
            _imageBuilder.PushElem(); // push elem

            Visit(context.expression(2)); // value
            HandleMulOrAddOp(assigmentSign[..^1]);
        }

        Visit(context.expression(1)); // index

        _imageBuilder.SetElem();
        return default;
    }

    public override object? VisitCmpExpression(WistGrammarParser.CmpExpressionContext context)
    {
        _needResultLevel++;
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);
        _needResultLevel--;

        switch (context.CMP_OP().GetText())
        {
            case ">":
                _imageBuilder.GreaterThan();
                break;
            case "<":
                _imageBuilder.LessThan();
                break;
            case "==":
                _imageBuilder.Cmp();
                break;
            case "!=":
                _imageBuilder.NotCmp();
                break;
            case "<=":
                _imageBuilder.LessOrEquals();
                break;
            case ">=":
                _imageBuilder.GreaterOrEquals();
                break;
            default:
                throw new WistException("Unknown op");
        }

        return default;
    }

    public override object? VisitWhileBlock(WistGrammarParser.WhileBlockContext context)
    {
        var startName = $"while_start_{Guid.NewGuid()}";
        var endName = $"while_end_{Guid.NewGuid()}";

        _imageBuilder.SetLabel(startName);
        Visit(context.expression());
        _imageBuilder.JmpIfFalse(endName);

        Visit(context.block());

        _imageBuilder.Jmp(startName);
        _imageBuilder.SetLabel(endName);

        return default;
    }

    public override object? VisitConstant(WistGrammarParser.ConstantContext context)
    {
        if (context.NUMBER() is { } i)
            _imageBuilder.PushConst(new WistConst(double.Parse(i.GetText().Replace("_", ""), NumberStyles.Any,
                CultureInfo.InvariantCulture)));
        else if (context.STRING() is { } s)
            _imageBuilder.PushConst(new WistConst(s.GetText()[1..^1]));
        else if (context.BOOL() is { } b)
            _imageBuilder.PushConst(new WistConst(b.GetText() == "true"));
        else if (context.NULL() is not null)
            _imageBuilder.PushConst(WistConst.CreateNull());
        else throw new WistException("Unknown const");

        return default;
    }

    public override object? VisitBoolExpression(WistGrammarParser.BoolExpressionContext context)
    {
        Visit(context.expression(0));
        Visit(context.expression(1));
        switch (context.BOOL_OP().GetText())
        {
            case "and":
                _imageBuilder.And();
                break;
            case "or":
                _imageBuilder.Or();
                break;
            case "xor":
                _imageBuilder.Xor();
                break;
            default:
                throw new WistException("Unknown operator");
        }

        return default;
    }

    public override object? VisitNotExpression(WistGrammarParser.NotExpressionContext context)
    {
        Visit(context.expression());
        _imageBuilder.Not();
        return default;
    }

    public override object? VisitElseIfBlock(WistGrammarParser.ElseIfBlockContext context) =>
        Visit((IParseTree)context.block() ?? context.ifBlock());

    public override object? VisitParenthesizedExpression(WistGrammarParser.ParenthesizedExpressionContext context) =>
        Visit(context.expression());

    public override object? VisitLoopBlock(WistGrammarParser.LoopBlockContext context)
    {
        var startName = $"loop_start_{Guid.NewGuid()}";
        var endName = $"loop_end_{Guid.NewGuid()}";


        var lineContext = context.assigment(0);
        if (lineContext != null)
            Visit(lineContext); // let i = 0

        _imageBuilder.SetLabel(startName);
        var lineContext2 = context.expression();
        if (lineContext2 != null)
            Visit(lineContext2); // i < 10
        _imageBuilder.JmpIfFalse(endName);

        Visit(context.block());
        var lineContext3 = context.assigment(1);
        if (lineContext3 != null)
            Visit(lineContext3); // i = i + 1

        _imageBuilder.Jmp(startName);
        _imageBuilder.SetLabel(endName);

        return default;
    }

    public override object? VisitMethodDecl(WistGrammarParser.MethodDeclContext context)
    {
        var methodName = context.IDENTIFIER(0).GetText();
        _imageBuilder.CreateMethod(methodName, context.IDENTIFIER().Length - 1);


        _imageBuilder.CreateLocal(This);
        _imageBuilder.SetLocal(This);
        for (var i = context.IDENTIFIER().Length - 1; i >= 1; i--)
        {
            var name = context.IDENTIFIER(i).GetText();
            _imageBuilder.CreateLocal(name);
            _imageBuilder.SetLocal(name);
        }

        Visit(context.block());

        if (methodName == Constructor)
            _imageBuilder.LoadLocal(This);
        else
            _imageBuilder.PushConst(WistConst.CreateNull());

        _imageBuilder.Ret();

        return default;
    }

    public override object? VisitFuncDecl(WistGrammarParser.FuncDeclContext context)
    {
        _imageBuilder.CreateFunction(context.IDENTIFIER(0).GetText(), context.IDENTIFIER().Length - 1);

        // handle parameters
        for (var i = context.IDENTIFIER().Length - 1; i >= 1; i--)
        {
            var name = context.IDENTIFIER(i).GetText();
            _imageBuilder.CreateLocal(name);
            _imageBuilder.SetLocal(name);
        }

        Visit(context.block());

        _imageBuilder.PushConst(WistConst.CreateNull());
        _imageBuilder.Ret();

        return default;
    }

    public override object? VisitReturn(WistGrammarParser.ReturnContext context)
    {
        _needResultLevel++;
        Visit(context.expression());
        _needResultLevel--;

        _imageBuilder.Ret();

        return default;
    }

    public override object? VisitIfBlock(WistGrammarParser.IfBlockContext context)
    {
        var endIfName = $"end_if{Guid.NewGuid()}";
        var elseEndName = $"else_end{Guid.NewGuid()}";


        // if condition
        Visit(context.expression());

        _imageBuilder.JmpIfFalse(endIfName);
        // if block
        Visit(context.block());

        _imageBuilder.Jmp(elseEndName);
        _imageBuilder.SetLabel(endIfName);

        // else block
        var elseIfBlockContext = context.elseIfBlock();
        if (elseIfBlockContext != null)
            Visit(elseIfBlockContext);

        _imageBuilder.SetLabel(elseEndName);

        return default;
    }

    public override object? VisitRemExpression(WistGrammarParser.RemExpressionContext context)
    {
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);

        _imageBuilder.Rem();
        return default;
    }


    public override object? VisitFunctionExpression(WistGrammarParser.FunctionExpressionContext context)
        => Visit(context.call());

    public override object? VisitMethodExpression(WistGrammarParser.MethodExpressionContext context)
    {
        _methodCall = context.expression();
        Visit(context.call());
        return default;
    }

    public override object? VisitCall(WistGrammarParser.CallContext context)
    {
        _needResultLevel++;
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);
        _needResultLevel--;


        var name = context.IDENTIFIER().GetText();

        if (_methodCall is null)
        {
            var m = _wistLibsManager.TryGetFunction(name);

            if (m is not null)
                _imageBuilder.CallExternalMethod(m);
            else _imageBuilder.CallFunc(name, context.expression().Length);
        }
        else
        {
            _needResultLevel++;
            Visit(_methodCall);
            _needResultLevel--;

            _imageBuilder.Dup();
            _methodCall = null;
            _imageBuilder.CallMethod(name, context.expression().Length);
        }

        if (_needResultLevel == 0)
            _imageBuilder.Drop();

        return default;
    }


    public override object? VisitIdentifierExpression(WistGrammarParser.IdentifierExpressionContext context)
    {
        _imageBuilder.LoadGlobalOrLocal(context.IDENTIFIER().GetText());
        return default;
    }


    public override object? VisitAddExpression(WistGrammarParser.AddExpressionContext context)
    {
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);

        var text = context.ADD_OP().GetText();
        HandleAddOp(text);

        return default;
    }

    private void HandleAddOp(string text)
    {
        if (text == "+")
            _imageBuilder.Add();
        else
            _imageBuilder.Sub();
    }

    public override object? VisitMulExpression(WistGrammarParser.MulExpressionContext context)
    {
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);

        var text = context.MUL_OP().GetText();
        HandleMulOp(text);

        return default;
    }

    private void HandleMulOp(string text)
    {
        if (text == "*")
            _imageBuilder.Mul();
        else
            _imageBuilder.Div();
    }

    private void HandleMulOrAddOp(string text)
    {
        switch (text)
        {
            case "*":
                _imageBuilder.Mul();
                break;
            case "/":
                _imageBuilder.Div();
                break;
            case "+":
                _imageBuilder.Add();
                break;
            case "-":
                _imageBuilder.Sub();
                break;
            default:
                throw new WistException($"Unknown operation {text}");
        }
    }

    public override object? VisitDllImport(WistGrammarParser.DllImportContext context)
    {
        var path = context.STRING().GetText()[1..^1];

        if (path.EndsWith("dll"))
        {
            if (_importedCodes.Contains(path)) goto end;

            _wistLibsManager.AddLib(path);
            _importedCodes.Add(path);
        }
        else
        {
            var fullPath = Path.GetFullPath(Path.Combine(_path, path));
            if (_importedCodes.Contains(fullPath)) goto end;

            CompileOtherCode(WistPreprocessor.Preprocess(File.ReadAllText(fullPath)));
            _importedCodes.Add(fullPath);
        }

        end:
        return default;
    }

    public override object? VisitGotoLabel(WistGrammarParser.GotoLabelContext context)
    {
        _imageBuilder.Jmp(context.IDENTIFIER().GetText());
        return default;
    }

    public override object? VisitSetLabel(WistGrammarParser.SetLabelContext context)
    {
        _imageBuilder.SetLabel(context.IDENTIFIER().GetText());
        return default;
    }

    public override object? VisitArrayInit(WistGrammarParser.ArrayInitContext context)
    {
        _imageBuilder.PushList();
        _imageBuilder.SetFirstRegister();

        var length = context.expression().Length;

        for (var i = 0; i < length; i++)
        {
            _imageBuilder.LoadFirstRegister();
            Visit(context.expression(i));
            _imageBuilder.AddElem();
        }

        _imageBuilder.LoadFirstRegister();

        return default;
    }

    public override object? VisitElementOfArrayExpression(WistGrammarParser.ElementOfArrayExpressionContext context)
    {
        Visit(context.expression(0)); // array
        Visit(context.expression(1)); // index
        _imageBuilder.PushElem();

        return default;
    }

    private void CompileOtherCode(string s)
    {
        var visitor = new WistGrammarVisitor(_imageBuilder, _wistLibsManager, _importedCodes);

        var inputStream = new AntlrInputStream(s);
        var simpleLexer = new WistGrammarLexer(inputStream);
        var commonTokenStream = new CommonTokenStream(simpleLexer);
        var simpleParser = new WistGrammarParser(commonTokenStream);
        var simpleContext = simpleParser.program();

        visitor.Visit(simpleContext);
    }

    public override object? VisitInitFunc(WistGrammarParser.InitFuncContext context)
    {
        _imageBuilder.CallFunc(context.STRING().GetText()[1..^1], 0);
        return default;
    }

    public WistImageObject GetFixedImage() => _imageBuilder.Compile();
}