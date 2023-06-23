namespace Backend;

using System.Runtime.CompilerServices;
using System.Text;

public class WistClass
{
    private readonly SortedDictionary<int, WistConst> _fields;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistClass(IEnumerable<(int id, WistConst value)> fields)
    {
        _fields = new SortedDictionary<int, WistConst>();

        foreach (var (id, value) in fields)
            _fields.Add(id, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private WistClass(SortedDictionary<int, WistConst> fields)
    {
        _fields = new SortedDictionary<int, WistConst>();

        foreach (var (id, value) in fields)
            _fields.Add(id, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistClass Copy() => new(_fields);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (var field in _fields)
            sb.Append($"{field.Value}, ");

        return sb.ToString(0, sb.Length - 2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetField(int id, WistConst value) => _fields[id] = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst GetField(int id) => _fields[id];
}