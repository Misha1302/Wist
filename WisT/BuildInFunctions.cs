namespace WisT;

using Antlr4.Runtime;
using Backend;
using Backend.Attributes;
using Backend.Interpreter;
using WisT.WistContent;

[WistLib]
public static class WistBuildInFunctions
{
    [WistLibFunction]
    public static void Print(WistInterpreter i, int paramsCount)
    {
        if (paramsCount > 1)
            throw new WistException("number of parameters must be 1 or 0");
        
        Console.WriteLine(paramsCount == 1 ? i.Pop() : "");
        i.Push(WistConst.CreateNull());
    }

    [WistLibFunction]
    public static void Write(WistInterpreter i, int paramsCount)
    {
        if (paramsCount != 1)
            throw new WistException("number of parameters must be 1");
        
        Console.Write(i.Pop());
        i.Push(WistConst.CreateNull());
    }

    [WistLibFunction]
    public static void Input(WistInterpreter i, int paramsCount)
    {
        if (paramsCount != 0)
            throw new WistException("number of parameters must be 0");

        i.Push(new WistConst(Console.ReadLine() ?? string.Empty));
    }

    [WistLibFunction]
    public static void Exit(WistInterpreter i, int paramsCount)
    {
        if (paramsCount != 0)
            throw new WistException("number of parameters must be 0");

        i.ExitInterpreter();
        // there is no need for Interpreter.Push(WistConst.CreateNull()) because the program will already exit
    }

    [WistLibFunction]
    public static void GetLen(WistInterpreter i, int paramsCount)
    {
        if (paramsCount != 1)
            throw new WistException("number of parameters must be 1");

        var value = i.Pop();
        var res = value.Type switch
        {
            WistType.List => new WistConst(value.GetList().Count),
            WistType.String => new WistConst(value.GetString().Length),
            _ => throw new WistException($"Cannot get len for this type {value.Type}")
        };
        i.Push(res);
    }

    [WistLibFunction]
    public static void AddElem(WistInterpreter i, int paramsCount)
    {
        if (paramsCount != 2)
            throw new WistException("number of parameters must be 2");

        var value = i.Pop();
        var list = i.Pop();

        if (list.Type == WistType.List)
            list.GetList().Add(value);
        else throw new WistException($"Cannot add element for this type {value.Type}");

        i.Push(value);
    }

    [WistLibFunction]
    public static void UniteLists(WistInterpreter i, int paramsCount)
    {
        if (paramsCount != 2)
            throw new WistException("number of parameters must be 2");

        var list1 = i.Pop();
        var list2 = i.Pop();
        var result = new WistConst(new List<WistConst>());

        if (list1.Type != WistType.List || list1.Type != list2.Type)
            throw new WistException($"Cannot unite lists, 'cause types: {list1.Type} {list2.Type}");

        result.GetList().AddRange(list1.GetList());
        result.GetList().AddRange(list2.GetList());

        i.Push(result);
    }

    [WistLibFunction]
    public static void GetTypeAsNumber(WistInterpreter i, int paramsCount)
    {
        if (paramsCount != 1)
            throw new WistException("number of parameters must be 1");

        i.Push(new WistConst((int)i.Pop().Type));
    }

    [WistLibFunction]
    public static void StartAsync(WistInterpreter i, int paramsCount)
    {
        if (paramsCount != 1)
            throw new WistException("number of parameters must be 1");

        var s = i.Pop().GetString();

        var code = WistPreprocessor.Preprocess(s);

        var inputStream = new AntlrInputStream(code);
        var simpleLexer = new WistGrammarLexer(inputStream);
        var commonTokenStream = new CommonTokenStream(simpleLexer);
        var simpleParser = new WistGrammarParser(commonTokenStream);
        var simpleContext = simpleParser.program();
        var visitor = new WistGrammarVisitor();

        var image = visitor.CompileCode(simpleContext, "Content");
        WistEngine.Instance.AddToTasks(new WistInterpreter(image));
        i.Push(WistConst.CreateNull());
    }

    [WistLibFunction]
    public static void IsSubclass(WistInterpreter i, int paramsCount)
    {
        if (paramsCount != 2)
            throw new WistException("number of parameters must be 2");

        var b = i.Pop();
        var a = i.Pop();

        if (a.Type != WistType.Class || b.Type != WistType.Class)
            throw new WistException("first and second param must be classes");

        i.Push(new WistConst(a.GetClass().IsSubclass(b.GetClass())));
    }
}