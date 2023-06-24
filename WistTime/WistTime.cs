namespace WistTime;

using Backend;

[WistLib]
public static class WistTime
{
    [WistLibFunction]
    public static void GetTime()
    {
        WistInterpreter.Push(new WistConst(DateTimeOffset.Now.ToUnixTimeMilliseconds()));
    }
}