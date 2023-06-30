namespace Backend;

using System.Reflection;

public class WistImageBuilder
{
    private readonly List<WistBuilderClass> _classes = new();
    private readonly List<(int ind, string c)> _classInserts = new();
    private readonly List<WistConst> _constants = new();
    private readonly List<WistConst> _constants2 = new();
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
        _constants2[^1] = WistConst.CreateInternalConst(_curFunction.localsCount);
    }

    public void Sub()
    {
        _ops.Add(WistOp.Sub);
        SetConst(default);
        _constants2[^1] = WistConst.CreateInternalConst(_curFunction.localsCount);
    }

    public void Mul()
    {
        _ops.Add(WistOp.Mul);
        SetConst(default);
        _constants2[^1] = WistConst.CreateInternalConst(_curFunction.localsCount);
    }

    public void Div()
    {
        _ops.Add(WistOp.Div);
        SetConst(default);
        _constants2[^1] = WistConst.CreateInternalConst(_curFunction.localsCount);
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
        _locals.Add(GenerateLocalName(name));
    }

    public void CreateGlobal(string name)
    {
        _ops.Add(WistOp.CreateGlobal);
        SetConst(WistConst.CreateInternalConst(name.GetHashCode()));
    }

    public void CreateFunction(string name, int paramsCount)
    {
        EndPreviousFunc();

        var generated = GenerateFuncName(name, paramsCount);
        Jmp(generated + "_end");
        SetLabel(generated);
        _curFunction = (generated, 0);
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

    public void CallFunc(string funcName, int paramsCount)
    {
        _ops.Add(WistOp.CallFunc);
        SetConst(default);
        _constants2[^1] = WistConst.CreateInternalConst(_curFunction.localsCount);
        _jumps.Add((_constants.Count - 1, GenerateFuncName(funcName, paramsCount)));
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
            new(
                c.Fields.Select(x => (x.GetHashCode(), WistConst.CreateNull())),
                c.Methods.Select(x => (x.GetHashCode(), GetLabelOrFuncPtr(GenerateNewMethodName(x, c.Name))))
            );


        List<WistConst> CopyListConstants(IEnumerable<WistConst> constants) =>
            constants.Select(c => c.Type != WistType.List
                ? c
                : new WistConst(c.GetList().ToList())).ToList();
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

    private void SetGlobal(string name)
    {
        _ops.Add(WistOp.SetGlobal);
        SetConst(WistConst.CreateInternalConst(name.GetHashCode()));
    }

    private void LoadGlobal(string name)
    {
        _ops.Add(WistOp.LoadGlobal);
        SetConst(WistConst.CreateInternalConst(name.GetHashCode()));
    }

    private int FindLocal(string name)
    {
        var ind = _locals.IndexOf(GenerateLocalName(name));
        return ind == -1 ? throw new WistException($"Cannot find {name} local in {_curFunction.name} func") : ind;
    }

    public bool IsLocal(string name) => _locals.Contains(GenerateLocalName(name));


    private string GenerateLocalName(string name) => $"local<{name}>{_curFunction.name}";

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

    public void CreateClass(string? name, List<string> fields, List<string> methods)
    {
        _classes.Add(new WistBuilderClass(name, fields, methods));
    }

    public void InstantiateClass(string name)
    {
        _ops.Add(WistOp.CopyClass);
        SetConst(default);
        _classInserts.Add((_constants.Count - 1, name));
    }

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

    public void CreateMethod(string name, int paramsCount)
    {
        EndPreviousFunc();

        var generated = GenerateNewMethodName(name + paramsCount);
        Jmp(generated + "_end");
        SetLabel(generated);
        _curFunction = (generated, 0);

        _classes[^1].Methods.Add(GenerateMethodNameToCall(name + paramsCount));
    }

    private string GenerateNewMethodName(string name, string? className = null) =>
        $"{className ?? _classes[^1].Name}_<method>_{name}";

    private static string GenerateMethodNameToCall(string name) => $"{name}";
    private static string GenerateFuncName(string name, int paramsCount) => $"<func>_{name}_<{paramsCount}>";

    public void CallMethod(string methName, int paramsCount)
    {
        _ops.Add(WistOp.CallMethod);
        var generateMethodName = GenerateMethodNameToCall(methName + paramsCount);
        SetConst(WistConst.CreateInternalConst(generateMethodName.GetHashCode()));
        _constants2[^1] = WistConst.CreateInternalConst(_curFunction.localsCount);
    }

    public void SetFirstRegister()
    {
        _ops.Add(WistOp.SetFirstRegister);
        SetConst(default);
    }

    public void LoadFirstRegister()
    {
        _ops.Add(WistOp.LoadFirstRegister);
        SetConst(default);
    }

    public void PushList()
    {
        _ops.Add(WistOp.PushNewList);
        SetConst(default);
    }

    public void And()
    {
        _ops.Add(WistOp.And);
        SetConst(default);
    }

    public void Or()
    {
        _ops.Add(WistOp.Or);
        SetConst(default);
    }

    public void Xor()
    {
        _ops.Add(WistOp.Xor);
        SetConst(default);
    }

    public void Not()
    {
        _ops.Add(WistOp.Not);
        SetConst(default);
    }

    public void EndFunc()
    {
        EndPreviousFunc();
    }

    private record WistBuilderClass(string? Name, List<string> Fields, List<string> Methods);
}