namespace WisT;

using Backend;
using Backend.Attributes;
using Backend.Interpreter;

[WistLib]
public static class WistBuildInFunctions
{
    [WistLibFunction]
    public static void Print()
    {
        Console.WriteLine(WistInterpreter.Pop());
        WistInterpreter.Push(WistConst.CreateNull());
    }

    [WistLibFunction]
    public static void Write()
    {
        Console.Write(WistInterpreter.Pop());
        WistInterpreter.Push(WistConst.CreateNull());
    }

    [WistLibFunction]
    public static void Input()
    {
        WistInterpreter.Push(new WistConst(Console.ReadLine() ?? string.Empty));
    }

    [WistLibFunction]
    public static void Exit()
    {
        WistInterpreter.Exit();
        // there is no need for Interpreter.Push(WistConst.CreateNull()) because the program will already exit
    }

    [WistLibFunction]
    public static void GetLen()
    {
        var value = WistInterpreter.Pop();
        var res = value.Type switch
        {
            WistType.List => new WistConst(value.GetList().Count),
            WistType.String => new WistConst(value.GetString().Length),
            _ => throw new WistException($"Cannot get len for this type {value.Type}")
        };
        WistInterpreter.Push(res);
    }

    [WistLibFunction]
    public static void AddElem()
    {
        var value = WistInterpreter.Pop();
        var list = WistInterpreter.Pop();

        if (list.Type == WistType.List)
            list.GetList().Add(value);
        else throw new WistException($"Cannot add element for this type {value.Type}");

        WistInterpreter.Push(value);
    }

    [WistLibFunction]
    public static void UniteLists()
    {
        var list1 = WistInterpreter.Pop();
        var list2 = WistInterpreter.Pop();
        var result = new WistConst(new List<WistConst>());

        if (list1.Type != WistType.List || list1.Type != list2.Type)
            throw new WistException($"Cannot unite lists, 'cause types: {list1.Type} {list2.Type}");

        result.GetList().AddRange(list1.GetList());
        result.GetList().AddRange(list2.GetList());

        WistInterpreter.Push(result);
    }

    [WistLibFunction]
    public static void GetTypeAsNumber()
    {
        WistInterpreter.Push(new WistConst((int)WistInterpreter.Pop().Type));
    }

    [WistLibFunction]
    public static void IsSubclass()
    {
        var b = WistInterpreter.Pop();
        var a = WistInterpreter.Pop();

        if (a.Type != WistType.Class || b.Type != WistType.Class)
            throw new WistException("first and second param must be classes");

        WistInterpreter.Push(new WistConst(a.GetClass().IsSubclass(b.GetClass())));
    }
}