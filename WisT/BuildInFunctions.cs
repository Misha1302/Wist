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
        // there is no need for Interpreter.Push(WistConst.CreateNull()) because the program will already exit
    }

    [WistLibFunction]
    public static void GetLen()
    {
        var value = Interpreter.Pop();
        var res = value.Type switch
        {
            WistType.List => (WistConst)value.GetList().Count,
            WistType.String => (WistConst)value.GetString().Length,
            _ => throw new WistException($"Cannot get len for this type {value.Type}")
        };
        Interpreter.Push(res);
    }

    [WistLibFunction]
    public static void AddElem()
    {
        var value = Interpreter.Pop();
        var list = Interpreter.Pop();

        if (list.Type == WistType.List)
            list.GetList().Add(value);
        else throw new WistException($"Cannot add element for this type {value.Type}");

        Interpreter.Push(value);
    }
}