namespace Backend;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
public readonly struct WistConst
{
    [FieldOffset(0)] private readonly IntPtr _ptr; // 8 bytes
    [FieldOffset(0)] private readonly decimal _valueR; // 16 bytes
    [FieldOffset(0)] private readonly long _valueL; // 8 bytes
    [FieldOffset(0)] private readonly int _valueI; // 4 bytes
    [FieldOffset(0)] private readonly bool _valueB; // 1-4-8 bytes
    // max - 16 bytes
    [FieldOffset(16)] public readonly WistType Type; // 1-4-8 bytes

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst(string v)
    {
        _ptr = Marshal.StringToHGlobalUni(v);
        Type = WistType.String;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst(decimal v)
    {
        _valueR = v;
        Type = WistType.Number;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private WistConst(bool b)
    {
        _valueB = b;
        Type = WistType.Bool;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CreateInternalConst(int i) => new(i);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CreateInternalConst(IntPtr i) => new(i);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private WistConst(int i)
    {
        _valueI = i;
        Type = WistType.InternalInteger;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private WistConst(IntPtr i)
    {
        _ptr = i;
        Type = WistType.Pointer;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public decimal GetNumber() => _valueR;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetInternalInteger() => _valueI;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IntPtr GetInternalPtr() => _ptr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool GetBool() => _valueB;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetString() => Marshal.PtrToStringUni(_ptr)!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is WistConst c && c.GetHashCode() == GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => _valueL.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(WistConst left, WistConst right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(WistConst left, WistConst right) => !(left == right);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator WistConst(decimal d) => new(d);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator WistConst(string d) => new(d);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator WistConst(bool d) => new(d);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString()
    {
        return Type switch
        {
            WistType.Number => $"{GetNumber()}",
            WistType.Bool => $"{GetBool()}",
            WistType.InternalInteger => $"{GetInternalInteger()}",
            WistType.Pointer => $"{GetInternalPtr()}",
            WistType.String => $"{GetString()}",
            WistType.None => "<<None>>",
            _ => throw new NotImplementedException()
        };
    }
}