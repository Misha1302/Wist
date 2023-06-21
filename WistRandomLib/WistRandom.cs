namespace WistRandomLib;

using Backend;

[WistLib]
public static class WistRandom
{
    private static Random _random = new();

    [WistLibFunction]
    public static void SetSeed()
    {
        _random = new Random((int)Interpreter.Pop().GetNumber());
    }

    [WistLibFunction]
    public static void RandomInteger()
    {
        var max = Interpreter.Pop().GetNumber();
        var min = Interpreter.Pop().GetNumber();
        Interpreter.Push((WistConst)Random.Shared.Next((int)min, (int)max));
    }

    [WistLibFunction]
    public static void RandomReal()
    {
        var max = Interpreter.Pop().GetNumber();
        var min = Interpreter.Pop().GetNumber();
        Interpreter.Push((WistConst)(_random.NextDouble() * (max - min) + min));
    }
}