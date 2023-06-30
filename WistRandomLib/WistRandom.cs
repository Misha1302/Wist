namespace WistRandomLib;

using Backend;
using Backend.Attributes;
using Backend.Interpreter;

[WistLib]
public static class WistRandom
{
    private static Random _random = new();

    [WistLibFunction]
    public static void SetSeed(WistInterpreter i, int paramsCount)
    {
        if (paramsCount != 1)
            throw new WistException("number of parameters must be 1");
        
        _random = new Random((int)i.Pop().GetNumber());
    }

    [WistLibFunction]
    public static void RandomInteger(WistInterpreter i, int paramsCount)
    {
        if (paramsCount != 2)
            throw new WistException("number of parameters must be 2");
        
        var max = i.Pop().GetNumber();
        var min = i.Pop().GetNumber();
        i.Push(new WistConst(Random.Shared.Next((int)min, (int)max + 1)));
    }

    [WistLibFunction]
    public static void RandomReal(WistInterpreter i, int paramsCount)
    {
        if (paramsCount != 2)
            throw new WistException("number of parameters must be 2");
        
        var max = i.Pop().GetNumber();
        var min = i.Pop().GetNumber();
        i.Push(new WistConst(_random.NextDouble() * (max - min) + min));
    }
}