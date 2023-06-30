namespace Backend.Interpreter;

using System.Runtime.CompilerServices;

public partial class WistInterpreter
{
    private readonly int[] _pushed = new int[16384];
    private readonly WistConst[] _vars = new WistConst[16384];
    private int _pvp;
    private int _vp;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst GetCurVar(int offset) => _vars[_vp + offset];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetCurVar(int offset, WistConst value) => _vars[_vp + offset] = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst GetGlobalVar(int hash) => _engine.Globals.GetValue(hash);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetGlobalVar(int hash, WistConst value) => _engine.Globals.GetValue(hash) = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CreateGlobalVar(int hash)
    {
        var q = _engine.Globals;
        q.Add(hash);
        _engine.Globals = q;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PushVariables(int count)
    {
        _vp += count;
        _pushed[_pvp++] = count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PopVariables() => _vp -= _pushed[--_pvp];
}