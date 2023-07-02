namespace WistTypeConverter;

using Backend;
using Backend.Attributes;
using Backend.Interpreter;

[WistLib]
public static class WistTypeConverter
{
    [WistLibFunction]
    public static void ToStr(WistInterpreter i, int paramsCount)
    {
        if (paramsCount != 1)
            throw new WistError("number of parameters must be 1");

        i.Push(new WistConst(i.Pop().ToString()));
    }

    [WistLibFunction]
    public static void ToBool(WistInterpreter i, int paramsCount)
    {
        if (paramsCount != 1)
            throw new WistError("number of parameters must be 1");

        var wistConst = i.Pop();
        var result = wistConst.Type switch
        {
            WistType.Bool => wistConst.GetBool(),
            WistType.String => string.Equals(wistConst.GetString(), "true", StringComparison.OrdinalIgnoreCase),
            WistType.Number => Math.Abs(wistConst.GetNumber() - 1) < 0.000_01,
            _ => throw new WistError($"Cannot convert {wistConst.Type} to bool")
        };

        i.Push(new WistConst(result));
    }

    [WistLibFunction]
    public static void ToNumber(WistInterpreter i, int paramsCount)
    {
        if (paramsCount != 1)
            throw new WistError("number of parameters must be 1");

        var wistConst = i.Pop();
        var result = wistConst.Type switch
        {
            WistType.Bool => wistConst.GetBool() ? 1 : 0,
            WistType.String => double.Parse(wistConst.GetString()),
            WistType.Number => wistConst.GetNumber(),
            _ => throw new WistError($"Cannot convert {wistConst.Type} to number")
        };

        i.Push(new WistConst(result));
    }
}