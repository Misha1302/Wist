namespace WistTypeConverter;

using Backend;

[WistLib]
public static class WistTypeConverter
{
    [WistLibFunction]
    public static void ToStr()
    {
        WistInterpreter.Push(new WistConst(WistInterpreter.Pop().ToString()));
    }

    [WistLibFunction]
    public static void ToBool()
    {
        var wistConst = WistInterpreter.Pop();
        var result = wistConst.Type switch
        {
            WistType.Bool => wistConst.GetBool(),
            WistType.String => string.Equals(wistConst.GetString(), "true", StringComparison.OrdinalIgnoreCase),
            WistType.Number => Math.Abs(wistConst.GetNumber() - 1) < 0.000_01,
            _ => throw new WistException($"Cannot convert {wistConst.Type} to bool")
        };

        WistInterpreter.Push(new WistConst(result));
    }

    [WistLibFunction]
    public static void ToNumber()
    {
        var wistConst = WistInterpreter.Pop();
        var result = wistConst.Type switch
        {
            WistType.Bool => wistConst.GetBool() ? 1 : 0,
            WistType.String => double.Parse(wistConst.GetString()),
            WistType.Number => wistConst.GetNumber(),
            _ => throw new WistException($"Cannot convert {wistConst.Type} to number")
        };

        WistInterpreter.Push(new WistConst(result));
    }
}