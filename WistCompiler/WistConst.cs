namespace WistCompiler;

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
    public WistConst(double v)
    {
        _valueR = v;
        Type = WistType.Number;
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

    public WistConst(object createInstance)
    {
        _handle = new WistGcHandleProvider(createInstance);
        Type = WistType.Class;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double GetNumber() => Type == WistType.Number ? _valueR : WistThrowHelper.ThrowGetNumber(Type);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool GetBool() => Type == WistType.Bool ? _valueB : WistThrowHelper.ThrowGetBool(Type);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public List<WistConst> GetList() => Type == WistType.List
        ? (List<WistConst>)((GCHandle)_handle!.Pointer).Target!
        : WistThrowHelper.ThrowGetList(Type);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object GetClass() => Type == WistType.Class
        ? ((GCHandle)_handle!.Pointer).Target!
        : WistThrowHelper.ThrowGetClass(Type);

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
    public static WistConst CreateNull() => new(WistType.Null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Sum(WistConst a, WistConst b)
    {
        return a.Type switch
        {
            WistType.Number => new WistConst(a.GetNumber() + b.GetNumber()),
            WistType.String => new WistConst(a.GetString() + b.GetString()),
            _ => throw new WistError($"Cannot sum types: {a.Type} and {b.Type}")
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Sub(WistConst a, WistConst b)
    {
        return a.Type switch
        {
            WistType.Number => new WistConst(a.GetNumber() - b.GetNumber()),
            _ => throw new WistError($"Cannot sum types: {a.Type} and {b.Type}")
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Mul(WistConst a, WistConst b)
    {
        return a.Type switch
        {
            WistType.Number => new WistConst(a.GetNumber() * b.GetNumber()),
            _ => throw new WistError($"Cannot sum types: {a.Type} and {b.Type}")
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Div(WistConst a, WistConst b)
    {
        return a.Type switch
        {
            WistType.Number => new WistConst(a.GetNumber() / b.GetNumber()),
            _ => throw new WistError($"Cannot sum types: {a.Type} and {b.Type}")
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst LessThan(WistConst a, WistConst b)
    {
        return a.Type switch
        {
            WistType.Number => new WistConst(a.GetNumber() < b.GetNumber()),
            _ => throw new WistError($"Cannot sum types: {a.Type} and {b.Type}")
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst GreaterThan(WistConst a, WistConst b)
    {
        return a.Type switch
        {
            WistType.Number => new WistConst(a.GetNumber() > b.GetNumber()),
            _ => throw new WistError($"Cannot sum types: {a.Type} and {b.Type}")
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst LessThanOrEquals(WistConst a, WistConst b)
    {
        return a.Type switch
        {
            WistType.Number => new WistConst(a.GetNumber() <= b.GetNumber()),
            _ => throw new WistError($"Cannot sum types: {a.Type} and {b.Type}")
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst GreaterThanOrEquals(WistConst a, WistConst b)
    {
        return a.Type switch
        {
            WistType.Number => new WistConst(a.GetNumber() >= b.GetNumber()),
            _ => throw new WistError($"Cannot sum types: {a.Type} and {b.Type}")
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Equals(WistConst a, WistConst b)
    {
        return a.Type switch
        {
            WistType.Number => new WistConst(Math.Abs(a.GetNumber() - b.GetNumber()) < 0.001),
            WistType.String => new WistConst(a.GetString() == b.GetString()),
            WistType.Bool => new WistConst(a.GetBool() == b.GetBool()),
            WistType.List => new WistConst(a.GetList().SequenceEqual(b.GetList())),
            _ => throw new WistError($"Cannot sum types: {a.Type} and {b.Type}")
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst NotEquals(WistConst a, WistConst b) => new(!Equals(a, b).GetBool());
}