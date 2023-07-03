namespace Backend.Interpreter;

using System.Runtime.CompilerServices;

public partial class WistInterpreter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst Pop() => _stack.Pop();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Push(WistConst c) => _stack.Push(c);
}