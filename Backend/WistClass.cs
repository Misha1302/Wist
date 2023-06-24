namespace Backend;

using System.Runtime.CompilerServices;
using System.Text;

public class WistClass
{
    private readonly WistGlossary<WistConst> _fields = new(3);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistClass(IEnumerable<(int id, WistConst value)> fields)
    {
        foreach (var (id, value) in fields)
            _fields.Add(id, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private WistClass(WistGlossary<WistConst> fields)
    {
        foreach (var entry in fields)
            _fields.Add(entry.Key, entry.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistClass Copy() => new(_fields);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString()
    {
        var sb = new StringBuilder();

        // possible rearrangement of fields due to SortedList
        foreach (var field in _fields)
            sb.Append($"{field.Value}, ");

        return sb.ToString(0, sb.Length - 2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetField(int id, WistConst value) => _fields.GetValue(id) = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst GetField(int id) => _fields.GetValue(id);
}