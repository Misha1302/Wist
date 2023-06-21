namespace WisT;

using Backend;

[WistLib]
public static class BuildInFunctions
{
    [WistLibFunction]
    public static void Print()
    {
        Console.WriteLine(Interpreter.Pop());
        Interpreter.Push(WistConst.CreateNull());
    }

    [WistLibFunction]
    public static void Input()
    {
        Interpreter.Push((WistConst)(Console.ReadLine() ?? string.Empty));
    }

    [WistLibFunction]
    public static void Exit()
    {
        Interpreter.Exit();
    }
}