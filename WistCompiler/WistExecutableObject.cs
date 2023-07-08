namespace WistCompiler;

using System.Reflection;

public record WistExecutableObject(List<WistClass> Classes)
{
    public const string MainMethodName = "Main";
    private readonly int _mainMethodHash = "Main".GenerateHashCode();

    public WistConst Execute()
    {
        FindMainMethodAndClass(out var main, out var mainClass);
        return (WistConst)main.Invoke(null, new object[] { new WistConst(mainClass) })!;
    }

    private void FindMainMethodAndClass(out MethodInfo main, out WistClass mainClass)
    {
        main = default!;
        mainClass = default!;

        foreach (var c in Classes)
        {
            var method = c.GetAllMethods().Find(x => x.Item1 == _mainMethodHash).Item2;
            if (method?.Name != MainMethodName) continue;

            main = method;
            mainClass = c;
            break;
        }

        if (main is null)
            throw new WistError("'Main' method was not found");
    }
}