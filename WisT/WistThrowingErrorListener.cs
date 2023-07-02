namespace WisT;

using Antlr4.Runtime;
using Backend;

public class WistThrowingErrorListener : BaseErrorListener, IAntlrErrorListener<int>
{
    public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg,
        RecognitionException e)
    {
        SyntaxError(line, charPositionInLine, msg);
    }

    public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine,
        string msg,
        RecognitionException e)
    {
        SyntaxError(line, charPositionInLine, msg);
    }


    private static void SyntaxError(int line, int charPositionInLine, string msg)
    {
        throw new WistException($"line={line}; chars={charPositionInLine}; msg={msg}");
    }
}