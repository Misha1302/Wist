namespace WisT;

using WisT.WistContent;
using WistCompiler;

public class WistGrammarVisitor : WistGrammarBaseVisitor<object?>
{
    private const string MainFuncName = "Main";
    
    private readonly List<WistImageClass> _classes = new();
    private WistImageClass CurClass => _classes[^1];

    public override object? VisitFuncDecl(WistGrammarParser.FuncDeclContext context)
    {
        if (context.IDENTIFIER(0).GetText() == MainFuncName)
        {
            _instructions.Add();
        }
    }
}