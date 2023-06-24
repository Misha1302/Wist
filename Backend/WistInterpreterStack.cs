namespace Backend;

using System.Runtime.CompilerServices;

public static partial class WistInterpreter
{
    private static readonly WistConst[] _stack = new WistConst[32768];
    private static int _sp;

    private static readonly int[] _returnStack = new int[32768];
    private static int _rsp;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Push(WistConst c) => _stack[_sp++] = c;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Pop() => _stack[--_sp];


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PushRet(int c) => _returnStack[_rsp++] = c;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int PopRet() => _returnStack[--_rsp];
}