namespace Backend.Interpreter;

using System.Runtime.CompilerServices;

public partial class WistInterpreter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst Pop() => _stack.Pop();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Push(WistConst c) => _stack.Push(c);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ExitInterpreter() => _index = _ops.Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetEngine(WistEngine engine) => _engine = engine;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst PeekReturnValue() => _stack.Peek();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistImageObject GetImage() => new(_consts.ToList(), _consts2.ToList(), _ops.ToList());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetExecutionIndex(int addr) => _index = addr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PushRet(int addr, int pushedVarsCount)
    {
        _returnStack.Push(addr);
        _pushed.Push(pushedVarsCount);
    }
}