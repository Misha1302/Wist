namespace Backend.Interpreter;

public static class WistExtension
{
    public static int GetWistHashCode(this string s) => WistHashCode.Instance.GetHashCode(s);
    public static int GetWistHashCode(this long s) => WistHashCode.GetHashCode(s);
}