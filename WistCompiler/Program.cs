using System.Reflection;
using System.Reflection.Emit;
using WistCompiler;

var instr = new List<WistInstruction>
{
    new(WistOp.CreateLocal, "loc1"),
    new(WistOp.Push, 5.2),
    new(WistOp.SetLocal, "loc1"),

    new(WistOp.CreateLocal, "loc2"),
    new(WistOp.Push, 2.0),
    new(WistOp.SetLocal, "loc2"),

    new(WistOp.LoadLocal, "loc1"),
    new(WistOp.LoadLocal, "loc2"),
    new(WistOp.Div),
};

var mainMethod = new WistImageMethod("Main", instr);

var mainClass = new WistImageClass("Programmm", new[] { "field1" }, new[] { mainMethod });

var asm = WistCompiler.WistCompiler.Instance.Compile(
    new WistImageObject(
        new[] { mainClass }
    )
);

var type = asm.GetType("Programmm")!;
var method = type.GetMethod("Main")!;

var baseCtor = type.GetConstructor(
    BindingFlags.Public | BindingFlags.Instance, null,
    Array.Empty<Type>(), null)!;

var invoke = method.Invoke(null, new object[]
{
    new WistConst(
        baseCtor.Invoke(Activator.CreateInstance(type), null)!
    )
});
Console.WriteLine(((WistConst)(invoke ?? throw new InvalidOperationException())).GetNumber());