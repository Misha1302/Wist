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