namespace Backend;

using System.Runtime.CompilerServices;

public static partial class Interpreter
{
    private static readonly WistConst[] _stack = new WistConst[512];
    private static int _sp;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Push(WistConst c) => _stack[_sp++] = c;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Pop() => _stack[--_sp];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DupTop()
    {
        _stack[_sp] = _stack[_sp - 1];
        _sp++;
    }
}