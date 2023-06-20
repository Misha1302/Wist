namespace WisT;

using System.Globalization;
using System.Reflection;
using Backend;
using WisT.WistGrammar;

public class WistGrammarVisitor : WistGrammarBaseVisitor<object?>
{
    private readonly WistImage _image = new();

    public override object? VisitAssigment(WistGrammarParser.AssigmentContext context)
    {
        var type = context.TYPE()?.GetText();
        var name = context.IDENTIFIER().GetText();

        if (context.elementOfArray() != null)
            throw new NotImplementedException();

        if (type == "let")
            _image.CreateVar(name);
        else if (type == "var")
            throw new NotImplementedException();

        var expressionContext = context.expression();
        Visit(expressionContext);
        _image.SetVar(name);

        return default;
    }

    public override object? VisitCmpExpression(WistGrammarParser.CmpExpressionContext context)
    {
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);

        switch (context.CMP_OP().GetText())
        {
            case ">":
                _image.GreaterThan();
                break;
            case "<":
                _image.LessThan();
                break;
            case "==":
                _image.Cmp();
                break;
            case "!=":
                _image.NotCmp();
                break;
            case "<=":
                _image.LessOrEquals();
                break;
            case ">=":
                _image.GreaterOrEquals();
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

        _image.SetLabel(startName);
        Visit(context.expression());
        _image.JmpIfFalse(endName);

        Visit(context.block());

        _image.Jmp(startName);
        _image.SetLabel(endName);

        return default;
    }

    public override object? VisitConstant(WistGrammarParser.ConstantContext context)
    {
        if (context.NUMBER() is { } i)
            _image.PushConst(new WistConst(double.Parse(i.GetText().Replace("_", ""), NumberStyles.Any,
                CultureInfo.InvariantCulture)));
        else if (context.STRING() is { } s)
            _image.PushConst(new WistConst(s.GetText()));
        else if (context.BOOL() is { } b)
            _image.PushConst(new WistConst(b.GetText() == "true"));
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

        _image.SetLabel(startName);
        var lineContext2 = context.expression();
        if (lineContext2 != null)
            Visit(lineContext2); // i < 10
        _image.JmpIfFalse(endName);

        Visit(context.block());
        var lineContext3 = context.assigment(1);
        if (lineContext3 != null)
            Visit(lineContext3); // i = i + 1

        _image.Jmp(startName);
        _image.SetLabel(endName);

        return default;
    }

    public override object? VisitFuncDecl(WistGrammarParser.FuncDeclContext context)
    {
        _image.CreateFunction(context.IDENTIFIER(0).GetText());

        // handle parameters
        for (var i = context.IDENTIFIER().Length - 1; i >= 1; i--)
        {
            var name = context.IDENTIFIER(i).GetText();
            _image.CreateVar(name);
            _image.SetVar(name);
        }

        Visit(context.block());

        return default;
    }

    public override object? VisitReturn(WistGrammarParser.ReturnContext context)
    {
        Visit(context.expression());
        _image.Ret();

        return default;
    }

    public override object? VisitIfBlock(WistGrammarParser.IfBlockContext context)
    {
        var endIfName = $"end_if{Guid.NewGuid()}";
        var elseEndName = $"else_end{Guid.NewGuid()}";


        // if condition
        Visit(context.expression());

        _image.JmpIfFalse(endIfName);
        // if block
        Visit(context.block());

        _image.Jmp(elseEndName);
        _image.SetLabel(endIfName);

        // else block
        var elseIfBlockContext = context.elseIfBlock();
        if (elseIfBlockContext != null)
            Visit(elseIfBlockContext);

        _image.SetLabel(elseEndName);

        return default;
    }

    public override object? VisitRemExpression(WistGrammarParser.RemExpressionContext context)
    {
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);

        _image.Rem();
        return default;
    }


    public override object? VisitFunctionCall(WistGrammarParser.FunctionCallContext context)
    {
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);


        var text = context.IDENTIFIER().GetText();
        var m = typeof(BuildInFunctions).GetMethod(text, BindingFlags.Static | BindingFlags.Public);

        if (m is not null)
            _image.CallExternalMethod(m);
        else _image.CallFunc(text);

        return default;
    }


    public override object? VisitIdentifierExpression(WistGrammarParser.IdentifierExpressionContext context)
    {
        _image.LoadVar(context.IDENTIFIER().GetText());
        return default;
    }


    public override object? VisitAddExpression(WistGrammarParser.AddExpressionContext context)
    {
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);

        if (context.ADD_OP().GetText() == "+")
            _image.Add();
        else
            _image.Sub();

        return default;
    }

    public override object? VisitMulExpression(WistGrammarParser.MulExpressionContext context)
    {
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);

        if (context.MUL_OP().GetText() == "*")
            _image.Mul();
        else
            _image.Div();

        return default;
    }


    public WistImage GetImage() => _image;
}