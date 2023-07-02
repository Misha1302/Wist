namespace Backend.Interpreter;

using System.Runtime.CompilerServices;

public partial class WistInterpreter
{
    private readonly int[] _pushed = new int[16384];
    private readonly WistConst[] _vars = new WistConst[16384];
#if DEBUG
    private int _localsCount;
#endif
    private int _pvp;
    private int _vp;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private WistConst GetCurVar(int offset) => _vars[_vp + offset];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetCurVar(int offset, WistConst value) => _vars[_vp + offset] = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private WistConst GetGlobalVar(int hash) => _engine.Globals.GetValue(hash);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetGlobalVar(int hash, WistConst value) => _engine.Globals.GetValue(hash) = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CreateGlobalVar(int hash)
    {
        var q = _engine.Globals;
        q.Add(hash);
        _engine.Globals = q;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PushVariables(int count)
    {
        _vp += count;
        _pushed[_pvp++] = count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PopVariables() => _vp -= _pushed[--_pvp];
}