namespace Backend.Interpreter;

using System.Collections.Immutable;
using System.Runtime.CompilerServices;

public unsafe partial class WistInterpreter
{
    // https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/function-pointers.md
    // methods MUST BE in enum WistOp order
    private readonly delegate*<WistInterpreter, void>[] _functions =
    {
        &PushConst, &Add, &Cmp, &JmpIfFalse, &CallExternalMethod, &JmpIfTrue, &Jmp, &SetLocal, &LoadLocal,
        &LessThan, &GreaterThan, &NotCmp, &LessOrEquals, &GreaterOrEquals, &Sub, &Rem, &Mul, &Div, &Ret, &CallFunc,
        &Drop, &Dup, &SetElem, &PushElem, &AddElem, &SetGlobal, &LoadGlobal, &CopyClass, &SetField, &LoadField,
        &CallMethod, &PushNewList, &And, &Or, &Xor, &Not, &CreateGlobal
    };

    private WistConst[] _consts = null!;
    private WistConst[] _consts2 = null!;
    private WistEngine _engine = null!;
    private int _index;
    private ImmutableArray<WistOp> _ops;


    public WistInterpreter(WistImageObject imageObject)
    {
        Init(imageObject);
    }

    public bool Halted => _index >= _ops.Length;


    private void Init(WistImageObject imageObject)
    {
        _ops = ImmutableArray.Create(imageObject.Ops.ToArray());
        _consts = imageObject.Consts.ToArray();
        _consts2 = imageObject.Consts2.ToArray();
        _index = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void RunSteps(int count)
    {
        for (var i = 0; i < count && _index < _ops.Length; _index++, i++)
            /* var format = $"{_ops[_index]} :: {_consts[_index]}";
            if (_sp > 0) format += $" :: {string.Join(", ", _stack[.._sp])}";
            Console.WriteLine(format); */
            _functions[(int)_ops[_index]](this);
    }

    public void ExitInterpreter()
    {
        _index = _ops.Length;
    }

    public void SetEngine(WistEngine engine)
    {
        _engine = engine;
    }
}