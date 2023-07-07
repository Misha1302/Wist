namespace WistCompiler;

using System.Reflection.Emit;

public static class WistDynamicMethodFabric
{
    public static DynamicMethod CreateDynamicMethod(string name, int paramsCount) =>
        new(
            name,
            typeof(WistConst),
            Enumerable.Repeat(typeof(WistConst), paramsCount).ToArray(),
            true);
}