namespace WistDebuggerHelper;

using Antlr4.Runtime;
using Backend;
using Backend.Attributes;
using Backend.Interpreter;
using WisT;
using WisT.WistContent;

[WistLib]
public static class WistDebuggerHelper
{
    [WistLibFunction]
    public static void CreateInterpreter(WistInterpreter i, int paramsCount)
    {
        if (paramsCount != 1)
            throw new WistError("number of parameters must be 1");

        var s = i.Pop().GetString();

        var inputStream = new AntlrInputStream(s);
        var simpleLexer = new WistGrammarLexer(inputStream);
        var commonTokenStream = new CommonTokenStream(simpleLexer);
        var simpleParser = new WistGrammarParser(commonTokenStream);
        var simpleContext = simpleParser.program();
        var visitor = new WistGrammarVisitor();

        var image = visitor.CompileCode(simpleContext, "Content");
        var interpreter = new WistInterpreter(image);
        interpreter.SetEngine(new WistEngine());
        i.Push(new WistConst(interpreter));
    }

    [WistLibFunction]
    public static void GetNumberOfExecutionLine(WistInterpreter i, int paramsCount)
    {
#if DEBUG
        if (paramsCount != 1)
            throw new WistError("number of parameters must be 1");

        var interpreter = i.Pop();
        i.Push(new WistConst(interpreter.GetInterpreter().GetNumberOfExecutionLine()));
#else
        throw new WistError($"Cannot use function {nameof(GetNumberOfExecutionLine)} in release mode");
#endif
    }

    [WistLibFunction]
    public static void GetRuntimeInfo(WistInterpreter i, int paramsCount)
    {
#if DEBUG
        if (paramsCount != 1)
            throw new WistError("number of parameters must be 1");

        var interpreter = i.Pop().GetInterpreter();
        var locals = interpreter.GetLocals();
        var globals = interpreter.GetGlobals();
        i.Push(new WistConst(new List<WistConst>
        {
            new(locals.Select(x => new WistConst(x.s)).ToList()), // strings
            new(locals.Select(x => x.value).ToList()), // values

            new(globals.Select(x => new WistConst(x.s)).ToList()), // strings
            new(globals.Select(x => x.value).ToList()) // values
        }));
#else
        throw new WistError($"Cannot use function {nameof(GetRuntimeInfo)} in release mode");
#endif
    }

    [WistLibFunction]
    public static void DoOneStep(WistInterpreter i, int paramsCount)
    {
        if (paramsCount != 1)
            throw new WistError("number of parameters must be 1");

        var interpreter = i.Pop();
        interpreter.GetInterpreter().RunSteps(1);
        i.Push(WistConst.CreateNull());
    }
}