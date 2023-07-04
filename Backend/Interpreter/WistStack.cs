namespace Backend.Interpreter;

using System.Runtime.CompilerServices;

public class WistStack<T>
{
    private readonly T[] _arr;
    private int _sp;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistStack(int stackSize)
    {
        _arr = new T[stackSize];
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public WistStack()
    {
        throw new WistError("You must set capacity for stack");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Pop() => _arr[--_sp];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Push(T v) => _arr[_sp++] = v;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanPop() => _sp > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Drop() => _sp--;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Restore(int i) => _sp += i;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ResetPointer(int sp) => _sp = sp;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dup()
    {
        _arr[_sp] = _arr[_sp - 1];
        _sp++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Peek() => _arr[_sp - 1];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetPointer() => _sp;

    public override string ToString() => string.Join(", ", _arr[.._sp]);
}