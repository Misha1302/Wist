namespace WistCompiler;

public static class WistThrowHelper
{
    public static double ThrowGetNumber(WistType type) => throw new WistError($"Cannot get number because type is {type}");
    public static bool ThrowGetBool(WistType type) => throw new WistError($"Cannot get bool because type is {type}");
    public static nint ThrowGetClass(WistType type) => throw new WistError($"Cannot get class because type is {type}");
    public static List<WistConst> ThrowGetList(WistType type) => throw new WistError($"Cannot get list because type is {type}");
}