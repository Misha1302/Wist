namespace Backend.Interpreter;

using System.Runtime.CompilerServices;

public partial class WistInterpreter
{
    private readonly WistConst[] _globals = new WistConst[512];
    private readonly int[] _pushed = new int[16384];
    private readonly WistConst[] _vars = new WistConst[16384];
    private int _pvp;
    private int _vp;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst GetCurVar(int offset) => _vars[_vp + offset];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetCurVar(int offset, WistConst value) => _vars[_vp + offset] = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst GetGlobalVar(int offset) => _globals[offset];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetGlobalVar(int offset, WistConst value) => _globals[offset] = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PushVariables(int count)
    {
        _vp += count;
        _pushed[_pvp++] = count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PopVariables() => _vp -= _pushed[--_pvp];
}