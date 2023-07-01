namespace WisT;

using System.Text;
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
        result = CreateGenerators(result);
        return result;
    }

    private static string RemoveBlockStarts(string result) => FigureBlockStartRegex().Replace(result, " { ");
    private static string RemoveBlockEnds(string result) => FigureBlockEndRegex().Replace(result, "\n} ");

    private static string RemoveEmptyLines(string code)
    {
        return string.Join("\n",
            code.Split('\n', '\r').Where(x => !string.IsNullOrWhiteSpace(x))
        );
    }

    private static string CreateGenerators(string code)
    {
        var sb = new StringBuilder(code);

        int startInd;
        while ((startInd = code.IndexOf("generator ", StringComparison.Ordinal)) != -1)
        {
            var name = code[(startInd + 10)..code.IndexOf('(', startInd)];
            var bracketsLvl = 0;
            var curInd = code.IndexOf('{', startInd);
            var openBracketInd = curInd;
            do
            {
                var c = code[curInd];


                if (c == '{') bracketsLvl++;
                else if (c == '}') bracketsLvl--;
                curInd++;
            } while (bracketsLvl != 0);

            var s = code[(openBracketInd + 1)..(curInd - 1)];

            sb.Remove(startInd, curInd - startInd);
            code = code.Remove(startInd, curInd - startInd);
            
            sb.Append(WistGenerator.CreateGenerator(s, name));
        }

        return sb.ToString();
    }
}