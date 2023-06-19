namespace Backend;

public static partial class Interpreter
{
    private static readonly WistConst[] _vars = new WistConst[512];
    private static int _vp;

    public static WistConst GetCurVar(int offset) => _vars[_vp + offset];
    public static void SetCurVar(int offset, WistConst value) => _vars[_vp + offset] = value;

    public static void PushVariables(int count) => _vp += count;
    public static void PopVariables(int count) => _vp -= count;
}