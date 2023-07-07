namespace WistCompiler;

using System.Reflection.Emit;

public record WistCompilerClass(string Name, WistClass Class)
{
    private readonly Dictionary<int, List<WistInstruction>> _methods = new();

    public void AddMethod(int id, DynamicMethod dyn, List<WistInstruction> instructions)
    {
        Class.AddMethod(id, dyn);
        _methods.Add(id, instructions);
    }

    public List<WistInstruction> GetInstructions(int id) => _methods[id];
}