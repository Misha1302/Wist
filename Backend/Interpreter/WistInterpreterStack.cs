namespace Backend.Interpreter;

using System.Runtime.CompilerServices;

public partial class WistInterpreter
{
    private readonly int[] _returnStack = new int[512];
    private readonly WistConst[] _stack = new WistConst[16384];
    private int _rsp;
    private int _sp;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Push(WistConst c) => _stack[_sp++] = c;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst Pop() => _stack[--_sp];


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PushRet(int c) => _returnStack[_rsp++] = c;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int PopRet() => _returnStack[--_rsp];
}