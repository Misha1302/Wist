namespace WistCompiler;

using System.Reflection.Emit;

public record WistExecutableObject(List<DynamicMethod> Methods, List<Type> Classes)
{
    public const string MainMethodName = "Main";
    public const string MainClassName = "Main";

    public WistConst Execute()
    {
        // var ptr = (delegate*<void>)Methods.Find(x => x.Name == MainMethodName)!.MethodHandle.GetFunctionPointer();
        // ptr();
        var arg = new WistConst(Activator.CreateInstance(Classes.Find(x=>x.Name == MainClassName)!)!);
        return (WistConst)Methods.Find(x => x.Name == MainMethodName)!.Invoke(null, new object[] { arg })!;
    }
}