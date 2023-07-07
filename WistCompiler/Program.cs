namespace WistCompiler;

public class WistProgram
{
    public static void Main(string[] args)
    {
        var p = new WistProgram();
        p.Start(args);
    }

    // ReSharper disable once UnusedParameter.Local
    private void Start(string[] args)
    {
        var instr = new List<WistInstruction>
        {
            new(WistOp.CreateClass, "SomeInfo"),
            new(WistOp.SetLocal, "loc"),

            new(WistOp.LoadLocal, "loc"),
            new(WistOp.Push, 123456789.0),
            new(WistOp.SetField, "info"),

            new(WistOp.LoadLocal, "loc"),
            new(WistOp.LoadField, "info")
        };

        var mainMethod = new WistImageMethod(WistExecutableObject.MainMethodName, new[] { "this" }, instr);

        var mainClass = new WistImageClass("Main", new[] { "field" }, new[] { mainMethod });
        var class2 = new WistImageClass("SomeInfo", new[] { "info" }, Array.Empty<WistImageMethod>());

        var obj = WistCompiler.Compile(
            new WistImageObject(new[] { mainClass, class2 })
        );


        var invoke = obj.Execute();
        Console.WriteLine(invoke.GetNumber());
    }

    public static WistConst PrintNumber(WistConst a)
    {
        Console.WriteLine(a.GetNumber());
        return new WistConst();
    }
}