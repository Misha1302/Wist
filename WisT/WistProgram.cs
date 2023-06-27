namespace WisT;

using System.Text.RegularExpressions;

public partial class WistPreprocessor
{
    [GeneratedRegex("\\}.*\n\\s")]
    private static partial Regex FigureBlockEndRegex();

    public static string Preprocess(string code, string path)
    {
        var result = string.Join("\n",
            File.ReadAllText(path).Split('\n', '\r').Where(x => !string.IsNullOrWhiteSpace(x))
        );
        return FigureBlockEndRegex().Replace(result, "} ");
    }
}