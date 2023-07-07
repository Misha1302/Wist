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
        /*
         this.field = 1
         local = 1.01
         
    start:
         print(this.field)
         this.field = this.field * local
         if this.field != +inf then goto start
        */
        var instr = new List<WistInstruction>
        {
            // this.field = 1
            new(WistOp.LoadArg, "this"),
            new(WistOp.Push, 1.0),
            new(WistOp.SetField, "field"),

            // local = 1.01
            new(WistOp.Push, 1.01),
            new(WistOp.SetLocal, "local"),

            // start:
            new(WistOp.SetLabel, "start"),

            // print(this.field)
            new(WistOp.LoadArg, "this"),
            new(WistOp.LoadField, "field"),
            new(WistOp.CallExternMethod, GetType().GetMethod("PrintNumber", BindingFlags.Public | BindingFlags.Static)),
            new(WistOp.Drop),

            // this.field = this.field * local
            new(WistOp.LoadArg, "this"),

            // --- this.field
            new(WistOp.LoadArg, "this"),
            new(WistOp.LoadField, "field"),
            // --- local
            new(WistOp.LoadLocal, "local"),
            // --- *
            new(WistOp.Mul),

            // --- this.field = 
            new(WistOp.SetField, "field"),


            // this.field != +inf
            new(WistOp.LoadArg, "this"),
            new(WistOp.LoadField, "field"),

            new(WistOp.Push, double.PositiveInfinity),
            new(WistOp.LessThan),

            // goto start
            new(WistOp.GoToIfTrue, "start"),

            new(WistOp.LoadArg, "this"),
            new(WistOp.LoadField, "field")
        };

        var mainMethod = new WistImageMethod(WistExecutableObject.MainMethodName, new[] { "this" }, instr);

        var mainClass = new WistImageClass("Main", new[] { "field" }, new[] { mainMethod });

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

    public static WistConst PrintNumber(WistConst a)
    {
        Console.WriteLine(a.GetNumber());
        return new WistConst();
    }
}