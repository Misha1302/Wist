namespace WisT;

using System.Text;

internal static class WistGenerator
{
    private const string Bottom = """

        this._state = -1
        return false
    }
    
    method Current() {
        return this._current
    }
}
""";

    private static string Top(string name) => $@"
class {name}(_state, _current) {{
    method Ctor() {{
        this._state = 0
    }}
    
    method MoveNext() {{
        this._state += 1
";

    public static string CreateGenerator(string s, string name)
    {
        StringBuilder sb = new(s.Length * 2);

        var i = 1;
        while (true)
        {
            var ind = s.IndexOf("give", StringComparison.Ordinal);
            if (ind == -1) break;

            sb.Append($"if this._state == {i} {{");

            sb.Append(s[..ind]);
            s = s.Remove(0, ind + 4);

            var indexOf = s.IndexOf('\n');
            sb.AppendLine($"this._current = {s[..indexOf]}");
            sb.AppendLine("return true");
            sb.Append('}');

            s = s.Remove(0, indexOf);
            i++;
        }

        sb.AppendLine(s);

        return sb.Insert(0, Top(name)).Append(Bottom).ToString();
    }
}