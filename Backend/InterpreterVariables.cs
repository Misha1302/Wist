namespace Backend;

using System.Runtime.CompilerServices;

public static partial class Interpreter
{
    private static readonly WistConst[] _vars = new WistConst[32768];
    private static readonly WistConst[] _globals = new WistConst[512];
    private static readonly int[] _pushed = new int[32768];
    private static int _pvp;
    private static int _vp;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst GetCurVar(int offset) => _vars[_vp + offset];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetCurVar(int offset, WistConst value) => _vars[_vp + offset] = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst GetGlobalVar(int offset) => _globals[offset];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetGlobalVar(int offset, WistConst value) => _globals[offset] = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PushVariables(int count)
    {
        _vp += count;
        _pushed[_pvp++] = count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PopVariables() => _vp -= _pushed[--_pvp];
}