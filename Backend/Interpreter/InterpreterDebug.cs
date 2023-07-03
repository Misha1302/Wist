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
    private static void SetCurLine(WistInterpreter i)
    {
        i._curLine = i._consts[i._index].GetInternalInteger();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetLocalsCount(WistInterpreter i)
    {
        i._localsCount = i._consts[i._index].GetInternalInteger();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CreateLocal(WistInterpreter i)
    {
        i._locals.Add((i._consts[i._index].GetString(), i._consts2[i._index].GetInternalInteger()));
    }
}

#endif