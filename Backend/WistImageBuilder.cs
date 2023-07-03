namespace Backend;

using System.Reflection;
using Backend.Interpreter;

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
        SetOp(WistOp.PushConst, c, default);
    }


    public void SetLabel(string labelName)
    {
        // Set a label in place of the next instruction
        _labels.Add(labelName, _ops.Count - 1);
    }

    private void SetConst(WistConst c1, WistConst c2)
    {
        _constants.Add(c1);
        _constants2.Add(c2);
    }

    public void Add()
    {
        SetOp(WistOp.Add, default, WistConst.CreateInternalConst(_curFunction.localsCount));
    }

    public void Sub()
    {
        SetOp(WistOp.Sub, default, WistConst.CreateInternalConst(_curFunction.localsCount));
    }

    public void Mul()
    {
        SetOp(WistOp.Mul, default, WistConst.CreateInternalConst(_curFunction.localsCount));
    }

    public void Div()
    {
        SetOp(WistOp.Div, default, WistConst.CreateInternalConst(_curFunction.localsCount));
    }

    public void Cmp()
    {
        SetOp(WistOp.Cmp, default, default);
    }

    public void JmpIfFalse(string labelName)
    {
        SetOp(WistOp.JmpIfFalse, default, default);
        _jumps.Add((_constants.Count - 1, labelName));
    }


    public void JmpIfTrue(string labelName)
    {
        SetOp(WistOp.JmpIfTrue, default, default);
        _jumps.Add((_constants.Count - 1, labelName));
    }


    public void Jmp(string labelName)
    {
        SetOp(WistOp.Jmp, default, default);
        _jumps.Add((_constants.Count - 1, labelName));
    }

    public void CallExternalMethod(MethodInfo methodInfo, int paramsCount)
    {
        SetOp(WistOp.CallExternalMethod, WistConst.CreateInternalConst(methodInfo.MethodHandle.GetFunctionPointer()),
            WistConst.CreateInternalConst(paramsCount));
    }

    public void CreateLocal(string name)
    {
        _curFunction.localsCount++;
        _locals.Add(GenerateLocalName(name));

        SetOp(WistOp.CreateLocal, new WistConst(name), new WistConst(_locals.Count - 1));
    }

    public void CreateGlobal(string name)
    {
        SetOp(WistOp.CreateGlobal, WistConst.CreateInternalConst(name.GetWistHashCode()), default);
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

        SetOp(WistOp.SetLocal, WistConst.CreateInternalConst(ind), default);
    }

    public void LoadLocal(string s)
    {
        var ind = FindLocal(s);

        SetOp(WistOp.LoadLocal, WistConst.CreateInternalConst(ind), default);
    }

    public void LessThan()
    {
        SetOp(WistOp.LessThan, default, default);
    }

    public void GreaterThan()
    {
        SetOp(WistOp.GreaterThan, default, default);
    }

    public void NotCmp()
    {
        SetOp(WistOp.NotCmp, default, default);
    }

    public void LessOrEquals()
    {
        SetOp(WistOp.LessOrEquals, default, default);
    }

    public void GreaterOrEquals()
    {
        SetOp(WistOp.GreaterOrEquals, default, default);
    }

    public void Rem()
    {
        SetOp(WistOp.Rem, default, default);
    }

    public void Ret()
    {
        SetOp(WistOp.Ret, default, default);
    }

    public void CallFunc(string funcName, int paramsCount)
    {
        SetOp(WistOp.CallFunc, default, WistConst.CreateInternalConst(_curFunction.localsCount));
        _jumps.Add((_constants.Count - 1, GenerateFuncName(funcName, paramsCount)));
    }

    public void Drop()
    {
        SetOp(WistOp.Drop, default, default);
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
                            ?? throw new WistError($"Cannot find the label or function with name: {name}"));


        int GetLabelOrFuncPtr(string labelName) =>
            _labels.TryGetValue(labelName, out var value)
                ? value
                : throw new WistError($"Cannot find the label or function with name: {labelName}");


        WistClass CreateWistClass(WistBuilderClass c) =>
            new(
                c.Fields.Select(x => (x.GetWistHashCode(), WistConst.CreateNull())),
                c.Methods.Select(x => (x.GetWistHashCode(), GetLabelOrFuncPtr(GenerateNewMethodName(x, c.Name))))
            );


        List<WistConst> CopyListConstants(IEnumerable<WistConst> constants) =>
            constants.Select(c => c.Type != WistType.List
                ? c
                : new WistConst(c.GetList().ToList())).ToList();
    }

    public void Dup()
    {
        SetOp(WistOp.Dup, default, default);
    }

    public void SetElem()
    {
        SetOp(WistOp.SetElem, default, default);
    }

    public void PushElem()
    {
        SetOp(WistOp.PushElem, default, default);
    }

    public void AddElem()
    {
        SetOp(WistOp.AddElem, default, default);
    }

    private void SetGlobal(string name)
    {
        SetOp(WistOp.SetGlobal, WistConst.CreateInternalConst(name.GetWistHashCode()), default);
    }

    private void LoadGlobal(string name)
    {
        SetOp(WistOp.LoadGlobal, WistConst.CreateInternalConst(name.GetWistHashCode()), default);
    }

    private int FindLocal(string name)
    {
        var ind = _locals.IndexOf(GenerateLocalName(name));
        return ind == -1 ? throw new WistError($"Cannot find {name} local in {_curFunction.name} func") : ind;
    }

    private bool IsLocal(string name) => _locals.Contains(GenerateLocalName(name));


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
        SetOp(WistOp.CopyClass, default, default);
        _classInserts.Add((_constants.Count - 1, name));
    }

    public void SetField(string name)
    {
        SetOp(WistOp.SetField, WistConst.CreateInternalConst(name.GetWistHashCode()), default);
    }

    public void LoadField(string name)
    {
        SetOp(WistOp.LoadField, WistConst.CreateInternalConst(name.GetWistHashCode()), default);
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
        var generateMethodName = GenerateMethodNameToCall(methName + paramsCount);

        SetOp(WistOp.CallMethod,
            WistConst.CreateInternalConst(generateMethodName.GetWistHashCode()),
            WistConst.CreateInternalConst(_curFunction.localsCount)
        );
    }

    public void PushList()
    {
        SetOp(WistOp.PushNewList, default, default);
    }

    public void And()
    {
        SetOp(WistOp.And, default, default);
    }

    public void Or()
    {
        SetOp(WistOp.Or, default, default);
    }

    public void Xor()
    {
        SetOp(WistOp.Xor, default, default);
    }

    public void Not()
    {
        SetOp(WistOp.Not, default, default);
    }

    public void EndFunc()
    {
        EndPreviousFunc();
    }

    public void PushTry(string catchStartName)
    {
        SetOp(WistOp.PushTry, default, default);
        _jumps.Add((_constants.Count - 1, catchStartName));
    }

    public void DropTry()
    {
        SetOp(WistOp.DropTry, default, default);
    }


    private void SetOp(WistOp op, WistConst c1, WistConst c2)
    {
        _ops.Add(op);
        SetConst(c1, c2);
    }

    public void FuncRef(string s, int paramsCount)
    {
        SetOp(WistOp.PushConst, default, default);
        _jumps.Add((_constants.Count - 1, GenerateFuncName(s, paramsCount)));
    }

    private record WistBuilderClass(string? Name, List<string> Fields, List<string> Methods);

#if DEBUG
    public void SetCurLine(int lineNumber)
    {
        SetOp(WistOp.SetCurLine, WistConst.CreateInternalConst(lineNumber), default);
    }


    public void SetLocalsCount()
    {
        SetOp(WistOp.SetLocalsCount, WistConst.CreateInternalConst(_curFunction.localsCount), default);
    }
#endif
}