namespace WistTime;

using Backend;
using Backend.Attributes;
using Backend.Interpreter;

[WistLib]
public static class WistTime
{
    [WistLibFunction]
    public static void GetTime(WistInterpreter i, int paramsCount)
    {
        if (paramsCount != 0)
            throw new WistException("number of parameters must be 0");

        i.Push(new WistConst(DateTimeOffset.Now.ToUnixTimeMilliseconds()));
    }
}