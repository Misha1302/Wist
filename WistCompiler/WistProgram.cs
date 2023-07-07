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
        var mainMethodInstructions = new List<WistInstruction>
        {
            new(WistOp.LoadArg, "this"),
            new(WistOp.Push, 5.0),
            new(WistOp.CallDynamicMethod, "IsPrime"), // true
            new(WistOp.CallExternMethod, typeof(WistProgram).GetMethod(nameof(Print))),
            new(WistOp.Drop),

            new(WistOp.LoadArg, "this"),
            new(WistOp.Push, 13.0),
            new(WistOp.CallDynamicMethod, "IsPrime"), // true
            new(WistOp.CallExternMethod, typeof(WistProgram).GetMethod(nameof(Print))),
            new(WistOp.Drop),

            new(WistOp.LoadArg, "this"),
            new(WistOp.Push, 123.0),
            new(WistOp.CallDynamicMethod, "IsPrime"), // false
            new(WistOp.CallExternMethod, typeof(WistProgram).GetMethod(nameof(Print))),
            new(WistOp.Drop),

            new(WistOp.Push, 0.0),
            new(WistOp.Ret)
        };

        var isPrimeMethodInstructions = new List<WistInstruction>
        {
            // if n <= 1 then return False
            new(WistOp.LoadArg, "n"),
            new(WistOp.Push, 1.0),
            new(WistOp.LessThanOrEquals),
            new(WistOp.GotoIfFalse, "ifEnd"),

            new(WistOp.Push, false),
            new(WistOp.Ret),

            new(WistOp.SetLabel, "ifEnd"),

            // else ... i = 0
            new(WistOp.Push, 2.0),
            new(WistOp.SetLocal, "i"),

            // top = sqrt(n) + 1
            new(WistOp.LoadArg, "n"),
            new(WistOp.CallExternMethod, typeof(WistProgram).GetMethod(nameof(Sqrt))),
            new(WistOp.Push, 1.0),
            new(WistOp.Add),
            new(WistOp.SetLocal, "top"),

            // while i < top
            new(WistOp.SetLabel, "whileStart"),
            new(WistOp.LoadLocal, "i"),
            new(WistOp.LoadLocal, "top"),
            new(WistOp.LessThan),
            new(WistOp.GotoIfFalse, "whileEnd"),

            // if n % i == 0 then return False
            new(WistOp.LoadArg, "n"),
            new(WistOp.LoadLocal, "i"),
            new(WistOp.Rem),
            new(WistOp.Push, 0.0),
            new(WistOp.IsEquals),
            new(WistOp.GotoIfFalse, "if2End"),

            new(WistOp.Push, false),
            new(WistOp.Ret),

            new(WistOp.SetLabel, "if2End"),

            new(WistOp.LoadLocal, "i"),
            new(WistOp.Push, 1.0),
            new(WistOp.Add),
            new(WistOp.SetLocal, "i"),

            new(WistOp.Goto, "whileStart"),


            // return false
            new(WistOp.SetLabel, "whileEnd"),
            new(WistOp.Push, true),
            new(WistOp.Ret)
        };

        var mainMethod =
            new WistImageMethod(WistExecutableObject.MainMethodName, new[] { "this" }, mainMethodInstructions);
        var isPrimeMethod = new WistImageMethod("IsPrime", new[] { "this", "n" }, isPrimeMethodInstructions);

        var mainClass = new WistImageClass("Main", Array.Empty<string>(), new[] { mainMethod, isPrimeMethod });

        var obj = WistCompiler.Compile(
            new WistImageObject(new[] { mainClass })
        );


        var invoke = obj.Execute();
        Console.WriteLine(invoke.GetNumber());
    }

    public static WistConst Print(WistConst a)
    {
        Console.WriteLine(a);
        return new WistConst();
    }

    public static WistConst Sqrt(WistConst a) => new(Math.Sqrt(a.GetNumber()));
}