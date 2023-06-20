namespace WisT;

using Backend;

// ReSharper disable UnusedMember.Global
public static class BuildInFunctions
{
    public static void Print()
    {
        Console.WriteLine(Interpreter.Pop());
        Interpreter.Push(WistConst.CreateNull());
    }

    public static void Input()
    {
        Interpreter.Push((WistConst)(Console.ReadLine() ?? string.Empty));
    }

    public static void Exit()
    {
        Interpreter.Exit();
    }
}