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

        Visit(context.expression());
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
            _image.PushConst(new WistConst(decimal.Parse(i.GetText().Replace("_", ""), NumberStyles.Any, CultureInfo.InvariantCulture)));
        else if (context.STRING() is { } s)
            _image.PushConst(new WistConst(s.GetText()));
        else throw new NotImplementedException();

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
        var m = typeof(BuildInFunctions).GetMethod(context.IDENTIFIER().GetText(),
            BindingFlags.Static | BindingFlags.Public);

        foreach (var expressionContext in context.expression())
            Visit(expressionContext);

        _image.CallExternalMethod(m ?? throw new InvalidOperationException());

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