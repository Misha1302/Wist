namespace WisT;

using System.Reflection;

public class WistLibsManager
{
    private readonly Dictionary<string, MethodInfo> _functions = new();

    public void AddLib(string s)
    {
        foreach (var t in Assembly.LoadFrom(s).GetTypes().Where(t1 => t1.IsDefined(typeof(WistLibAttribute))))
            AddLibByType(t);
    }

    public void AddLibByType(Type t)
    {
        foreach (var m in t.GetMethods().Where(x => x.IsDefined(typeof(WistLibFunctionAttribute))))
            _functions.TryAdd(m.Name, m);
    }

    public MethodInfo? TryGetFunction(string name) => _functions.TryGetValue(name, out var value) ? value : null;
}