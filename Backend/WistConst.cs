namespace Backend;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
public readonly struct WistConst
{
    [FieldOffset(0)] private readonly nint _ptr; // 4 or 8 bytes
    [FieldOffset(0)] private readonly double _valueR; // 8 bytes
    [FieldOffset(0)] private readonly long _valueL; // 8 bytes
    [FieldOffset(0)] private readonly int _valueI; // 4 bytes
    [FieldOffset(0)] private readonly bool _valueB; // 1 byte

    // max - 8 bytes
    [FieldOffset(8)] public readonly WistType Type; // 1 byte

    [FieldOffset(16)] private readonly WistGcHandleProvider? _handle; // 8 bytes

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst(string v)
    {
        _handle = new WistGcHandleProvider(v);
        Type = WistType.String;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst(WistClass c)
    {
        _handle = new WistGcHandleProvider(c);
        Type = WistType.Class;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst(double v)
    {
        _valueR = v;
        Type = WistType.Number;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CreateInternalConst(int i) => new(i);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CreateInternalConst(nint i) => new(i);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private WistConst(int i)
    {
        _valueI = i;
        Type = WistType.InternalInteger;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private WistConst(nint i)
    {
        _ptr = i;
        Type = WistType.Pointer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst(bool b)
    {
        _valueB = b;
        Type = WistType.Bool;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private WistConst(WistType t)
    {
        Type = t;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst(List<WistConst> wistConsts)
    {
        _handle = new WistGcHandleProvider(wistConsts);
        Type = WistType.List;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double GetNumber() => _valueR;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetInternalInteger() => _valueI;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public nint GetInternalPtr() => _ptr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool GetBool() => _valueB;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public List<WistConst> GetList() => (List<WistConst>)((GCHandle)_handle!.Pointer).Target!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistClass GetClass() =>
        (WistClass)(((GCHandle)_handle!.Pointer).Target ?? throw new InvalidOperationException());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetString() => (string)((GCHandle)_handle!.Pointer).Target!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is WistConst c && c.GetHashCode() == GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => _valueL.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(WistConst left, WistConst right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(WistConst left, WistConst right) => !(left == right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => WistToStringManager.ToStr(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CreateNull() => new(WistType.Null);
}