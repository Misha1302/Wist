#if DEBUG

namespace Backend.Interpreter;

using System.Runtime.CompilerServices;

public partial class WistInterpreter
{
    private readonly List<(string name, int offset)> _locals = new();
    private int _curLine = 1;
    private int _localsCount;

    public int GetNumberOfExecutionLine() => _curLine;

    public List<(string s, WistConst value)> GetLocals() =>
        new(
            _vars[_vp..(_localsCount + _vp)].Select((x, i) =>
                (_locals.Find(y => y.offset == i).name, x))
        );

    public List<(string s, WistConst value)> GetGlobals()
    {
        var lst = new List<(string, WistConst Value)>();
        var e = _engine.Globals.GetEnumerator();
        while (e.MoveNext())
            lst.Add((WistHashCode.Instance.GetSourceString(e.Current.Key), e.Current.Value));
        return lst;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetCurLine()
    {
        _curLine = _consts[_index].GetInternalInteger();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetLocalsCount()
    {
        _localsCount = _consts[_index].GetInternalInteger();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CreateLocal()
    {
        _locals.Add((_consts[_index].GetString(), _consts2[_index].GetInternalInteger()));
    }
}

#endif