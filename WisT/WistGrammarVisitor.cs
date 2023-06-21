namespace WisT;

using System.Globalization;
using Antlr4.Runtime;
using Backend;
using WisT.WistGrammar;

public class WistGrammarVisitor : WistGrammarBaseVisitor<object?>
{
    private WistImageBuilder _imageBuilder = null!;
    private int _needResultLevel;
    private WistLibsManager _wistLibsManager = null!;
    private string _path = string.Empty;

    private WistGrammarVisitor(WistImageBuilder imageBuilder, WistLibsManager wistLibsManager, string s)
    {
        _imageBuilder = imageBuilder;
        _wistLibsManager = wistLibsManager;
        _needResultLevel = 0;
    }

    public WistGrammarVisitor()
    {
    }

    public WistImageObject CompileCode(WistGrammarParser.ProgramContext program, string path)
    {
        _path = path;
        _imageBuilder = new WistImageBuilder();
        _wistLibsManager = new WistLibsManager();
        _needResultLevel = 0;
        _wistLibsManager.AddLibByType(typeof(BuildInFunctions));
        
        _imageBuilder.CallFunc("start");

        Visit(program);

        return GetFixedImage();
    }

    public override object? VisitAssigment(WistGrammarParser.AssigmentContext context)
    {
        _needResultLevel++;
        var type = context.TYPE()?.GetText();
        var name = context.IDENTIFIER().GetText();

        if (context.elementOfArray() != null)
            throw new NotImplementedException();

        if (type == "let")
            _imageBuilder.CreateVar(name);
        else if (type == "var")
            throw new NotImplementedException();

        var expressionContext = context.expression();
        Visit(expressionContext);
        _imageBuilder.SetVar(name);

        _needResultLevel--;

        return default;
    }

    public override object? VisitCmpExpression(WistGrammarParser.CmpExpressionContext context)
    {
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);

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
                throw new NotImplementedException();
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
        else throw new NotImplementedException();

        return default;
    }

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

    public override object? VisitFuncDecl(WistGrammarParser.FuncDeclContext context)
    {
        _imageBuilder.CreateFunction(context.IDENTIFIER(0).GetText());

        // handle parameters
        for (var i = context.IDENTIFIER().Length - 1; i >= 1; i--)
        {
            var name = context.IDENTIFIER(i).GetText();
            _imageBuilder.CreateVar(name);
            _imageBuilder.SetVar(name);
        }

        Visit(context.block());

        _imageBuilder.PushConst(WistConst.CreateNull());
        _imageBuilder.Ret();

        return default;
    }

    public override object? VisitReturn(WistGrammarParser.ReturnContext context)
    {
        Visit(context.expression());
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


    public override object? VisitFunctionCall(WistGrammarParser.FunctionCallContext context)
    {
        _needResultLevel++;
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);
        _needResultLevel--;


        var text = context.IDENTIFIER().GetText();
        var m = _wistLibsManager.TryGetFunction(text);

        if (m is not null)
            _imageBuilder.CallExternalMethod(m);
        else _imageBuilder.CallFunc(text);

        if (_needResultLevel == 0)
            _imageBuilder.Drop();

        return default;
    }


    public override object? VisitIdentifierExpression(WistGrammarParser.IdentifierExpressionContext context)
    {
        _imageBuilder.LoadVar(context.IDENTIFIER().GetText());
        return default;
    }


    public override object? VisitAddExpression(WistGrammarParser.AddExpressionContext context)
    {
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);

        if (context.ADD_OP().GetText() == "+")
            _imageBuilder.Add();
        else
            _imageBuilder.Sub();

        return default;
    }

    public override object? VisitMulExpression(WistGrammarParser.MulExpressionContext context)
    {
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);

        if (context.MUL_OP().GetText() == "*")
            _imageBuilder.Mul();
        else
            _imageBuilder.Div();

        return default;
    }

    public override object? VisitDllImport(WistGrammarParser.DllImportContext context)
    {
        var s = context.STRING().GetText()[1..^1];
        if (s.EndsWith("dll"))
            _wistLibsManager.AddLib(s);
        else
            CompileOtherCode(File.ReadAllText(Path.Combine(_path, s)));

        return default;
    }

    private void CompileOtherCode(string s)
    {
        var visitor = new WistGrammarVisitor(_imageBuilder, _wistLibsManager, s);

        var inputStream = new AntlrInputStream(s);
        var simpleLexer = new WistGrammarLexer(inputStream);
        var commonTokenStream = new CommonTokenStream(simpleLexer);
        var simpleParser = new WistGrammarParser(commonTokenStream);
        var simpleContext = simpleParser.program();

        visitor.Visit(simpleContext);
    }

    public WistImageObject GetFixedImage() => _imageBuilder.Compile();
}