namespace WisT;

using System.Text.RegularExpressions;

public static partial class WistPreprocessor
{
    [GeneratedRegex("\\s*\\n*\\s*\\}.*\\n*\\s*")]
    private static partial Regex FigureBlockEndRegex();
    
    [GeneratedRegex("\\s*\\n*\\s*\\{[^\\}\\n\\r]*\\n*\\s*")]
    private static partial Regex FigureBlockStartRegex();

    public static string Preprocess(string code)
    {
        var result = RemoveEmptyLines(code);
        result = RemoveBlockEnds(result);
        result = RemoveBlockStarts(result);
        return result;
    }

    private static string RemoveBlockStarts(string result) => FigureBlockStartRegex().Replace(result, " { ");
    private static string RemoveBlockEnds(string result) => FigureBlockEndRegex().Replace(result, "} ");

    private static string RemoveEmptyLines(string code)
    {
        return string.Join("\n",
            code.Split('\n', '\r').Where(x => !string.IsNullOrWhiteSpace(x))
        );
    }
}