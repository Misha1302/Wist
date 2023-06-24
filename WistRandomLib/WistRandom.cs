namespace WistRandomLib;

using Backend;

[WistLib]
public static class WistRandom
{
    private static Random _random = new();

    [WistLibFunction]
    public static void SetSeed()
    {
        _random = new Random((int)WistInterpreter.Pop().GetNumber());
    }

    [WistLibFunction]
    public static void RandomInteger()
    {
        var max = WistInterpreter.Pop().GetNumber();
        var min = WistInterpreter.Pop().GetNumber();
        WistInterpreter.Push(new WistConst(Random.Shared.Next((int)min, (int)max + 1)));
    }

    [WistLibFunction]
    public static void RandomReal()
    {
        var max = WistInterpreter.Pop().GetNumber();
        var min = WistInterpreter.Pop().GetNumber();
        WistInterpreter.Push(new WistConst(_random.NextDouble() * (max - min) + min));
    }
}