namespace WistCompiler;

using System.Reflection;

public record WistExecutableObject(List<WistClass> Classes)
{
    public const string MainMethodName = "Main";
    public readonly int MainMethodHash = "Main".GenerateHashCode();

    public WistConst Execute()
    {
        // var ptr = (delegate*<void>)Methods.Find(x => x.Name == MainMethodName)!.MethodHandle.GetFunctionPointer();
        // ptr();
        MethodInfo? main = null;
        WistClass? mainClass = null;
        foreach (var c in Classes)
        {
            var methods = c.GetAllMethods();

            main = methods.Find(x => x.Item1 == MainMethodHash).Item2;
            if (main?.Name != MainMethodName) continue;

            mainClass = c;
            break;
        }

        if (main is null)
            throw new WistError("'Main' method not found");

        return (WistConst)main.Invoke(null, new object[] { new WistConst(mainClass!) })!;
    }
}