namespace WistCompiler;

using System.Reflection;

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
            // Main.field1 = 5.0
            new(WistOp.Push, 5.0),
            new(WistOp.SetField, "field1"),

            // start:
            new(WistOp.SetLabel, "start"),

            // PrintHi(Main.field1)
            new(WistOp.LoadField, "field1"),
            new(WistOp.CallExternMethod, GetType().GetMethod("PrintHi", BindingFlags.Public | BindingFlags.Static)),
            new(WistOp.Drop),

            // loc1 = 5.2
            new(WistOp.Push, 1.0),
            new(WistOp.SetLocal, "loc1"),

            // loc2 = 2.0
            new(WistOp.Push, 0.99),
            new(WistOp.SetLocal, "loc2"),

            // Main.field1 = loc1 / loc2 * Main.field1
            new(WistOp.LoadLocal, "loc1"),
            new(WistOp.LoadLocal, "loc2"),
            new(WistOp.Div),
            new(WistOp.LoadField, "field1"),
            new(WistOp.Mul),
            new(WistOp.SetField, "field1"),

            // if Main.Field1 < inf, then goto start
            new(WistOp.LoadField, "field1"),
            new(WistOp.Push, double.PositiveInfinity),
            new(WistOp.LessThan),
            new(WistOp.GoToIfTrue, "start"),
            
            new(WistOp.LoadField, "field1"),
        };

        var mainMethod = new WistImageMethod(WistExecutableObject.MainMethodName, instr);

        var mainClass = new WistImageClass("Main", new[] { "field1" }, new[] { mainMethod });

        var obj = WistCompiler.Instance.Compile(
            new WistImageObject(
                new[]
                {
                    mainClass
                }
            )
        );


        var invoke = obj.Execute();
        Console.WriteLine(invoke.GetNumber());
    }

    public static WistConst PrintHi(WistConst a)
    {
        Console.WriteLine(a.GetNumber());
        return new WistConst();
    }
}