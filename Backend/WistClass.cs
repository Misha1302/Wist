namespace Backend;

using System.Runtime.CompilerServices;
using Backend.Glossary;

public class WistClass
{
    private readonly WistGlossary<WistConst> _fields = new(3);
    private readonly WistGlossary<int> _methods = new(3);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistClass(IEnumerable<(int id, WistConst value)> fields, IEnumerable<(int id, int pos)> methods)
    {
        foreach (var (id, value) in fields)
            _fields.Add(id, value);

        foreach (var (id, pos) in methods)
            _methods.Add(id, pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private WistClass(WistGlossary<WistConst> fields, WistGlossary<int> methods)
    {
        foreach (var entry in fields)
            _fields.Add(entry.Key, entry.Value);

        foreach (var entry in methods)
            _methods.Add(entry.Key, entry.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistClass Copy() => new(_fields, _methods);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => WistToStringManager.ToStr(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetField(int id, WistConst value) => _fields.GetValue(id) = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst GetField(int id) => _fields.GetValue(id);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetMethodPtr(int id) => _methods.GetValue(id);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSubclass(WistClass other)
    {
        foreach (var entry in other._fields)
            if (!_fields.ContainsKey(entry.Key))
                return false;

        foreach (var entry in other._methods)
            if (!_methods.ContainsKey(entry.Key))
                return false;

        return true;
    }

    public List<(int, WistConst)> GetAllFields()
    {
        var list = new List<(int, WistConst)>();
        var e = _fields.GetEnumerator();
        while (e.MoveNext())
            list.Add((e.Current.Key, e.Current.Value));
        return list;
    }

    public List<(int Key, int Value)> GetAllMethods()
    {
        var list = new List<(int Key, int Value)>();
        var e = _methods.GetEnumerator();
        while (e.MoveNext())
            list.Add((e.Current.Key, e.Current.Value));
        return list;
    }
}