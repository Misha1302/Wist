namespace WistCompiler;

using System.Reflection.Emit;

public static class WistDynamicMethodFabric
{
    public static DynamicMethod CreateDynamicMethod(string name)
    {
        return new DynamicMethod(
            name,
            typeof(WistConst),
            new[] { typeof(WistConst) },
            true
        );
    }
}