namespace WistCompiler;

public static class WistCompilerHelper
{
    public static readonly SortedDictionary<int, WistClass> Classes = new();

    public static WistClass PushClass(int id) => Classes[id];
}