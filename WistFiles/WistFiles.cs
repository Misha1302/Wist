namespace WistFiles;

using Backend;
using Backend.Attributes;
using Backend.Interpreter;

[WistLib]
public static class WistFiles
{
    [WistLibFunction]
    public static void ReadTextFromFile(WistInterpreter i)
    {
        var s = i.Pop().GetString();
        i.Push(new WistConst(File.ReadAllText(s)));
    }
}