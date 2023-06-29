namespace WistTime;

using Backend;
using Backend.Attributes;
using Backend.Interpreter;

[WistLib]
public static class WistTime
{
    [WistLibFunction]
    public static void GetTime(WistInterpreter i)
    {
        i.Push(new WistConst(DateTimeOffset.Now.ToUnixTimeMilliseconds()));
    }
}