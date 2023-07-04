namespace Backend.Interpreter;

using System.Runtime.CompilerServices;

public partial class WistInterpreter
{
    private readonly WistStack<(int ind, int rsp)> _tryStack = new(128);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PushTry(int ind)
    {
        _tryStack.Push((ind, _returnStack.GetPointer()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DropTry()
    {
        _tryStack.Drop();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryPopTry(out int ind, out int rsp)
    {
        if (_tryStack.CanPop())
        {
            (ind, rsp) = _tryStack.Pop();
            return true;
        }

        (ind, rsp) = (-1, -1);
        return false;
    }
}