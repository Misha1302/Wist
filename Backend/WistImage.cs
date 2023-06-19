namespace Backend;

using System.Reflection;

public class WistImage
{
    private readonly List<WistConst> _consts = new();
    private readonly List<(int jmpInd, string labelName)> _jumps = new();
    private readonly Dictionary<string, int> _labels = new();
    private readonly List<string> _localVars = new();
    private readonly List<WistOp> _ops = new();
    private (string name, int varsCount) _curMethodName = (string.Empty, 0);

    public void PushConst(WistConst c)
    {
        _ops.Add(WistOp.PushConst);
        _consts.Add(c);
    }

    public List<WistOp> GetOps() => _ops;
    public List<WistConst> GetConsts() => _consts;

    public void SetLabel(string labelName)
    {
        // Set a label in place of the next instruction
        _labels.Add(labelName, _ops.Count - 1);
    }

    public void Add()
    {
        _ops.Add(WistOp.Add);
        _consts.Add(default);
    }

    public void Sub()
    {
        _ops.Add(WistOp.Sub);
        _consts.Add(default);
    }

    public void Mul()
    {
        _ops.Add(WistOp.Mul);
        _consts.Add(default);
    }

    public void Div()
    {
        _ops.Add(WistOp.Div);
        _consts.Add(default);
    }

    public void Cmp()
    {
        _ops.Add(WistOp.Cmp);
        _consts.Add(default);
    }

    public void JmpIfFalse(string labelName)
    {
        _ops.Add(WistOp.JmpIfFalse);
        _consts.Add(default);
        _jumps.Add((_consts.Count - 1, labelName));
    }


    public void JmpIfTrue(string labelName)
    {
        _ops.Add(WistOp.JmpIfTrue);
        _consts.Add(default);
        _jumps.Add((_consts.Count - 1, labelName));
    }


    public void Jmp(string labelName)
    {
        _ops.Add(WistOp.Jmp);
        _consts.Add(default);
        _jumps.Add((_consts.Count - 1, labelName));
    }

    public void Dup()
    {
        _ops.Add(WistOp.Dup);
        _consts.Add(default);
    }

    public void CallExternalMethod(MethodInfo methodInfo)
    {
        _ops.Add(WistOp.CallExternalMethod);
        _consts.Add(WistConst.CreateInternalConst(methodInfo.MethodHandle.GetFunctionPointer()));
    }

    public void CreateVar(string name)
    {
        _curMethodName.varsCount++;
        _localVars.Add(name);
    }

    public void CreateFunction(string name)
    {
        EndPreviousFunc();

        Jmp($"{name}_end");
        SetLabel(name);
        _curMethodName = (name, 0);
    }

    private void EndPreviousFunc()
    {
        if (_curMethodName.name == string.Empty) return;

        _ops.Add(WistOp.FreeVars);
        _consts.Add(WistConst.CreateInternalConst(_curMethodName.varsCount));
        SetLabel($"{_curMethodName.name}_end");
    }

    public void Compile()
    {
        EndPreviousFunc();

        foreach (var (ind, labelName) in _jumps)
            _consts[ind] = WistConst.CreateInternalConst(_labels[labelName]);
    }

    public void SetVar(string s)
    {
        var ind = _localVars.IndexOf(s);
        _ops.Add(WistOp.SetVar);
        _consts.Add(WistConst.CreateInternalConst(ind));
    }

    public void LoadVar(string s)
    {
        var ind = _localVars.IndexOf(s);
        _ops.Add(WistOp.LoadVar);
        _consts.Add(WistConst.CreateInternalConst(ind));
    }

    public void LessThan()
    {
        _ops.Add(WistOp.LessThan);
        _consts.Add(default);
    }

    public void GreaterThan()
    {
        _ops.Add(WistOp.GreaterThan);
        _consts.Add(default);
    }

    public void NotCmp()
    {
        _ops.Add(WistOp.NotCmp);
        _consts.Add(default);
    }

    public void LessOrEquals()
    {
        _ops.Add(WistOp.LessOrEquals);
        _consts.Add(default);
    }

    public void GreaterOrEquals()
    {
        _ops.Add(WistOp.GreaterOrEquals);
        _consts.Add(default);
    }

    public void Rem()
    {
        _ops.Add(WistOp.Rem);
        _consts.Add(default);
    }
}