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

            new(WistOp.LoadLocal, "loc"), // class
            new(WistOp.LoadLocal, "loc"), // class
            new(WistOp.CallDynamicMethod, "method2"), // method call / value
            new(WistOp.SetField, "info"), // set

            new(WistOp.LoadLocal, "loc"),
            new(WistOp.LoadField, "info"),
            new(WistOp.CallExternMethod, typeof(WistProgram).GetMethod(nameof(PrintNumber))),
            new(WistOp.Drop),

            new(WistOp.Push, 0.0),
            new(WistOp.Ret)
        };

        var method2Instrs = new List<WistInstruction>
        {
            new(WistOp.Push, 2.0),
            new(WistOp.SetLocal, "a"),
            new(WistOp.Push, 3.0),
            new(WistOp.SetLocal, "b"),
            
            new(WistOp.LoadLocal, "a"),
            new(WistOp.LoadLocal, "b"),

            new(WistOp.Add),
            new(WistOp.Ret)
        };

        var mainMethod = new WistImageMethod(WistExecutableObject.MainMethodName, new[] { "this" }, instr);
        var otherMethod = new WistImageMethod("method2", new[] { "this" }, method2Instrs);

        var mainClass = new WistImageClass("Main", new[] { "field" }, new[] { mainMethod });
        var class2 = new WistImageClass("SomeInfo", new[] { "info" }, new[] { otherMethod });

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