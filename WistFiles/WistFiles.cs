namespace WistFiles;

using Backend;
using Backend.Attributes;
using Backend.Interpreter;

[WistLib]
public static class WistFiles
{
    [WistLibFunction]
    public static void ReadTextFromFile(WistInterpreter i, int paramsCount)
    {
        if (paramsCount != 1)
            throw new WistException("number of parameters must be 1");

        var s = i.Pop().GetString();
        i.Push(new WistConst(File.ReadAllText(s)));
    }
}