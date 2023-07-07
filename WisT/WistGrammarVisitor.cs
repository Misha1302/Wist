namespace WisT;

using Antlr4.Runtime.Tree;
using WisT.WistContent;
using WistCompiler;

public class WistGrammarVisitor : WistGrammarBaseVisitor<object?>
{
    private const string This = "this";
    private const string Constructor = "Ctor";
    private const string StartFuncName = "Start";

    private readonly WistImageBuilder _imageBuilder = null!;
    private List<string> _importedCodes = null!;
    private int _lineNumber = 1;
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

    public override object VisitErrorNode(IErrorNode node) =>
        throw new WistError($"Syntax error on line {_lineNumber} - '{node.GetText()}'");

    public override object? VisitNewline(WistGrammarParser.NewlineContext context)
    {
        _lineNumber++;
// #if DEBUG
//         _imageBuilder.SetCurLine(_lineNumber);
//         _imageBuilder.SetLocalsCount();
// #endif
        return default;
    }

    public WistImageObject GetFixedImage() => _imageBuilder.Compile();
}