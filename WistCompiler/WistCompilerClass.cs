namespace WistCompiler;

using System.Reflection.Emit;

public record WistCompilerClass(string Name, WistClass Class)
{
    private readonly Dictionary<int, (List<WistInstruction> instructions, List<string> paramsList)> _methods = new();

    public void AddMethod(int id, DynamicMethod dyn, List<WistInstruction> instructions, List<string> paramsList)
    {
        Class.AddMethod(id, dyn);
        _methods.Add(id, (instructions, paramsList));
    }

    public List<WistInstruction> GetInstructions(int id) => _methods[id].instructions;

    public List<string> GetParams(int id) => _methods[id].paramsList;
}