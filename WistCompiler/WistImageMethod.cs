namespace WistCompiler;

public record WistImageMethod(string Name, string[] Args, List<WistInstruction> Instructions);