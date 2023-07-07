namespace WistCompiler;

using System.Globalization;

public static class WistToStringManager
{
    public static string ToStr(WistConst c)
    {
        return c.Type switch
        {
            WistType.Bool => c.GetBool().ToString(),
            WistType.Number => c.GetNumber().ToString("F5", CultureInfo.InvariantCulture),
            WistType.String => c.GetString(),
            WistType.List => string.Join(", ", c.GetClass()),
            WistType.Null => "None",
            WistType.Class => "<<Class>>",
            _ => throw new WistError($"Unknown type to convert to string - {c.Type}")
        };
    }
}