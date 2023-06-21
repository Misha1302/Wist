namespace Backend;

using System.Reflection;

public class WistImageBuilder
{
    private readonly List<WistConst> _consts = new();
    private readonly List<WistConst> _consts2 = new();
    private readonly List<(int jmpInd, string labelName)> _jumps = new();
    private readonly Dictionary<string, int> _labels = new();
    private readonly List<string> _localVars = new();
    private readonly List<WistOp> _ops = new();
    private (string name, int varsCount) _curFunction = (string.Empty, 0);

    public void PushConst(WistConst c)
    {
        _ops.Add(WistOp.PushConst);
        SetConst(c);
    }


    public void SetLabel(string labelName)
    {
        // Set a label in place of the next instruction
        _labels.Add(labelName, _ops.Count - 1);
    }

    private void SetConst(WistConst c)
    {
        _consts.Add(c);
        _consts2.Add(default);
    }

    public void Add()
    {
        _ops.Add(WistOp.Add);
        SetConst(default);
    }

    public void Sub()
    {
        _ops.Add(WistOp.Sub);
        SetConst(default);
    }

    public void Mul()
    {
        _ops.Add(WistOp.Mul);
        SetConst(default);
    }

    public void Div()
    {
        _ops.Add(WistOp.Div);
        SetConst(default);
    }

    public void Cmp()
    {
        _ops.Add(WistOp.Cmp);
        SetConst(default);
    }

    public void JmpIfFalse(string labelName)
    {
        _ops.Add(WistOp.JmpIfFalse);
        SetConst(default);
        _jumps.Add((_consts.Count - 1, labelName));
    }


    public void JmpIfTrue(string labelName)
    {
        _ops.Add(WistOp.JmpIfTrue);
        SetConst(default);
        _jumps.Add((_consts.Count - 1, labelName));
    }


    public void Jmp(string labelName)
    {
        _ops.Add(WistOp.Jmp);
        SetConst(default);
        _jumps.Add((_consts.Count - 1, labelName));
    }

    public void CallExternalMethod(MethodInfo methodInfo)
    {
        _ops.Add(WistOp.CallExternalMethod);
        SetConst(WistConst.CreateInternalConst(methodInfo.MethodHandle.GetFunctionPointer()));
    }

    public void CreateVar(string name)
    {
        _curFunction.varsCount++;
        _localVars.Add($"var<{name}>{_curFunction.name}");
    }

    public void CreateFunction(string name)
    {
        EndPreviousFunc();

        Jmp($"{name}_end");
        SetLabel(name);
        _curFunction = (name, 0);
    }

    private void EndPreviousFunc()
    {
        if (_curFunction.name == string.Empty) return;

        SetLabel($"{_curFunction.name}_end");
    }

    public void SetVar(string s)
    {
        var ind = _localVars.IndexOf($"var<{s}>{_curFunction.name}");
        _ops.Add(WistOp.SetVar);
        SetConst(WistConst.CreateInternalConst(ind));
    }

    public void LoadVar(string s)
    {
        var ind = _localVars.IndexOf($"var<{s}>{_curFunction.name}");
        _ops.Add(WistOp.LoadVar);
        SetConst(WistConst.CreateInternalConst(ind));
    }

    public void LessThan()
    {
        _ops.Add(WistOp.LessThan);
        SetConst(default);
    }

    public void GreaterThan()
    {
        _ops.Add(WistOp.GreaterThan);
        SetConst(default);
    }

    public void NotCmp()
    {
        _ops.Add(WistOp.NotCmp);
        SetConst(default);
    }

    public void LessOrEquals()
    {
        _ops.Add(WistOp.LessOrEquals);
        SetConst(default);
    }

    public void GreaterOrEquals()
    {
        _ops.Add(WistOp.GreaterOrEquals);
        SetConst(default);
    }

    public void Rem()
    {
        _ops.Add(WistOp.Rem);
        SetConst(default);
    }

    public void Ret()
    {
        _ops.Add(WistOp.Ret);
        SetConst(default);
    }

    public void CallFunc(string funcName)
    {
        _ops.Add(WistOp.CallFunc);
        SetConst(default);
        _consts2[^1] = WistConst.CreateInternalConst(_curFunction.varsCount);
        _jumps.Add((_consts.Count - 1, funcName));
    }

    public void Drop()
    {
        _ops.Add(WistOp.Drop);
        SetConst(default);
    }


    public WistImageObject Compile()
    {
        EndPreviousFunc();

        var constsCopy = _consts.ToList();

        foreach (var (ind, labelName) in _jumps)
            constsCopy[ind] = WistConst.CreateInternalConst(GetLabelOrFuncPtr(labelName));


        return new WistImageObject(constsCopy, _consts2.ToList(), _ops.ToList());


        int GetLabelOrFuncPtr(string labelName) =>
            _labels.TryGetValue(labelName, out var value)
                ? value
                : throw new WistException($"Cannot find the label or function with name: {labelName}");
    }
}