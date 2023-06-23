namespace Backend;

using System.Reflection;

public class WistImageBuilder
{
    private readonly List<WistBuilderClass> _classes = new();
    private readonly List<(int ind, string c)> _classInserts = new();
    private readonly List<WistConst> _constants = new();
    private readonly List<WistConst> _constants2 = new();
    private readonly List<string> _globals = new();
    private readonly List<(int jmpInd, string labelName)> _jumps = new();
    private readonly Dictionary<string, int> _labels = new();
    private readonly List<string> _locals = new();
    private readonly List<WistOp> _ops = new();
    private (string name, int localsCount) _curFunction = (string.Empty, 0);

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
        _constants.Add(c);
        _constants2.Add(default);
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
        _jumps.Add((_constants.Count - 1, labelName));
    }


    public void JmpIfTrue(string labelName)
    {
        _ops.Add(WistOp.JmpIfTrue);
        SetConst(default);
        _jumps.Add((_constants.Count - 1, labelName));
    }


    public void Jmp(string labelName)
    {
        _ops.Add(WistOp.Jmp);
        SetConst(default);
        _jumps.Add((_constants.Count - 1, labelName));
    }

    public void CallExternalMethod(MethodInfo methodInfo)
    {
        _ops.Add(WistOp.CallExternalMethod);
        SetConst(WistConst.CreateInternalConst(methodInfo.MethodHandle.GetFunctionPointer()));
    }

    public void CreateLocal(string name)
    {
        _curFunction.localsCount++;
        _locals.Add($"local<{name}>{_curFunction.name}");
    }

    public void CreateGlobal(string name)
    {
        _globals.Add(name);
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
        _curFunction = (string.Empty, 0);
    }

    public void SetLocal(string s)
    {
        var ind = FindLocal(s);

        _ops.Add(WistOp.SetLocal);
        SetConst(WistConst.CreateInternalConst(ind));
    }

    public void LoadLocal(string s)
    {
        var ind = FindLocal(s);

        _ops.Add(WistOp.LoadLocal);
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
        _constants2[^1] = WistConst.CreateInternalConst(_curFunction.localsCount);
        _jumps.Add((_constants.Count - 1, funcName));
    }

    public void Drop()
    {
        _ops.Add(WistOp.Drop);
        SetConst(default);
    }


    public WistImageObject Compile()
    {
        EndPreviousFunc();

        var constantsCopy = CopyListConstants(_constants);

        foreach (var (ind, labelName) in _jumps)
            constantsCopy[ind] = WistConst.CreateInternalConst(GetLabelOrFuncPtr(labelName));

        foreach (var (ind, className) in _classInserts)
            constantsCopy[ind] = new WistConst(GetClass(className));


        return new WistImageObject(constantsCopy, CopyListConstants(_constants2), _ops.ToList());


        WistClass GetClass(string name) =>
            CreateWistClass(_classes.Find(x => x.Name == name)
                            ?? throw new WistException($"Cannot find the label or function with name: {name}"));


        int GetLabelOrFuncPtr(string labelName) =>
            _labels.TryGetValue(labelName, out var value)
                ? value
                : throw new WistException($"Cannot find the label or function with name: {labelName}");

        
        WistClass CreateWistClass(WistBuilderClass c) =>
            new(c.Fields.Select(x => (x.GetHashCode(), WistConst.CreateNull())).ToArray());

        
        List<WistConst> CopyListConstants(IEnumerable<WistConst> constants)
        {
            return constants.Select(c => c.Type != WistType.List
                ? c
                : new WistConst(c.GetList().ToList())).ToList();
        }
    }

    public void Dup()
    {
        _ops.Add(WistOp.Dup);
        SetConst(default);
    }

    public void SetElem()
    {
        _ops.Add(WistOp.SetElem);
        SetConst(default);
    }

    public void PushElem()
    {
        _ops.Add(WistOp.PushElem);
        SetConst(default);
    }

    public void AddElem()
    {
        _ops.Add(WistOp.AddElem);
        SetConst(default);
    }

    public void SetGlobal(string name)
    {
        var ind = FindGlobal(name);

        _ops.Add(WistOp.SetGlobal);
        SetConst(WistConst.CreateInternalConst(ind));
    }

    public void LoadGlobal(string name)
    {
        var ind = FindGlobal(name);

        _ops.Add(WistOp.LoadGlobal);
        SetConst(WistConst.CreateInternalConst(ind));
    }

    private int FindGlobal(string name)
    {
        var ind = _globals.IndexOf(name);
        return ind == -1 ? throw new WistException($"Cannot find {name} global") : ind;
    }

    private int FindLocal(string name)
    {
        var ind = _locals.IndexOf($"local<{name}>{_curFunction.name}");
        return ind == -1 ? throw new WistException($"Cannot find {name} local in {_curFunction.name} func") : ind;
    }

    public bool IsLocal(string name) => _locals.Contains($"local<{name}>{_curFunction.name}");

    public void LoadGlobalOrLocal(string name)
    {
        if (IsLocal(name)) LoadLocal(name);
        else LoadGlobal(name);
    }

    public void SetGlobalOrLocal(string name)
    {
        if (IsLocal(name)) SetLocal(name);
        else SetGlobal(name);
    }

    public void CreateClass(string name, List<string> fields)
    {
        _classes.Add(new WistBuilderClass(name, fields));
    }

    public void InstantiateClass(string name)
    {
        _ops.Add(WistOp.CopyClass);
        SetConst(default);
        _classInserts.Add((_constants.Count - 1, name));
    }

    private record WistBuilderClass(string Name, List<string> Fields);

    public void SetField(string name)
    {
        _ops.Add(WistOp.SetField);
        SetConst(WistConst.CreateInternalConst(name.GetHashCode()));
    }

    public void LoadField(string name)
    {
        _ops.Add(WistOp.LoadField);
        SetConst(WistConst.CreateInternalConst(name.GetHashCode()));
    }
}