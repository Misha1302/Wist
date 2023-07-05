namespace WistCompiler;

public readonly struct WistInstruction
{
    public readonly WistOp Op;
    public readonly object? Arg;

    public WistInstruction(WistOp op, object? arg = null)
    {
        Op = op;
        Arg = arg;
    }
}