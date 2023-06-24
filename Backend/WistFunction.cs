namespace Backend;

using System.Runtime.CompilerServices;

public readonly struct WistFunction
{
    public readonly List<WistConst> Locals = new();
    public readonly int OpStartIndex;

    private WistFunction(List<WistConst> locals, int opStartIndex)
    {
        Locals = locals;
        OpStartIndex = opStartIndex;
    }

    private WistFunction Copy() => new(CopyList(Locals), OpStartIndex);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private List<T> CopyList<T>(IReadOnlyList<T> list)
    {
        var localsCopy = new List<T>(Locals.Count);
        for (var i = 0; i < list.Count; i++)
            localsCopy[i] = list[i];
        return localsCopy;
    }
}