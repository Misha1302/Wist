namespace WistCompiler;

using System.Reflection.Emit;
using System.Runtime.CompilerServices;

public class WistClass
{
    private readonly SortedDictionary<int, DynamicMethod> _methods = new();
    private WistGlossary<WistConst> _fields = new(3);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistClass(IEnumerable<(int id, WistConst value)> fields, IEnumerable<(int id, DynamicMethod m)> methods)
    {
        foreach (var (id, value) in fields)
            _fields.Add(id, value);

        foreach (var (id, m) in methods)
            _methods.Add(id, m);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private WistClass(WistGlossary<WistConst> fields, SortedDictionary<int, DynamicMethod> methods)
    {
        foreach (var entry in fields)
            _fields.Add(entry.Key, entry.Value);

        foreach (var entry in methods)
            _methods.Add(entry.Key, entry.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistClass Copy() => new(_fields, _methods);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetField(int id, WistConst value) => _fields.GetValue(id) = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst GetField(int id) => _fields.GetValue(id);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DynamicMethod GetMethod(int id) => _methods[id];

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

    public List<(int Key, DynamicMethod? Value)> GetAllMethods()
    {
        return _methods.Select(x => (x.Key, x.Value)).ToList()!;
    }

    public void AddField(int id)
    {
        var fields = _fields;
        fields.Add(id);
        _fields = fields;
    }

    public void AddMethod(int id, DynamicMethod dynamicMethod)
    {
        _methods.Add(id, dynamicMethod);
    }
}