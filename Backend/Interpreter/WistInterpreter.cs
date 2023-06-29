namespace Backend.Interpreter;

using System.Collections.Immutable;

public unsafe partial class WistInterpreter
{
    // https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/function-pointers.md
    // methods MUST BE in enum WistOp order
    private readonly delegate*<WistInterpreter, void>[] _functions =
    {
        &PushConst, &Add, &Cmp, &JmpIfFalse, &CallExternalMethod, &JmpIfTrue, &Jmp, &SetLocal, &LoadLocal,
        &LessThan, &GreaterThan, &NotCmp, &LessOrEquals, &GreaterOrEquals, &Sub, &Rem, &Mul, &Div, &Ret, &CallFunc,
        &Drop, &Dup, &SetElem, &PushElem, &AddElem, &SetGlobal, &LoadGlobal, &CopyClass, &SetField, &LoadField,
        &CallMethod, &SetFirstRegister, &LoadFirstRegister, &PushNewList, &And, &Or, &Xor, &Not
    };

    private WistConst[] _consts = null!;
    private WistConst[] _consts2 = null!;
    private int _index;
    private ImmutableArray<WistOp> _ops;

    public WistInterpreter(WistImageObject imageObject)
    {
        Init(imageObject);
    }

    private void Init(WistImageObject imageObject)
    {
        _ops = ImmutableArray.Create(imageObject.Ops.ToArray());
        _consts = imageObject.Consts.ToArray();
        _consts2 = imageObject.Consts2.ToArray();
        _index = 0;
    }

    public void RunSteps(int count)
    {
        for (var i = 0; i < count && _index < _ops.Length; _index++, count++)
        {
            var wistOp = _ops[_index];

            /* var format = $"{wistOp} :: {_consts[_index]}";
            if (_sp > 0) format += $" :: {string.Join(", ", _stack[.._sp])}";
            Console.WriteLine(format); */

            _functions[(int)wistOp](this);
        }
    }

    public void ExitInterpreter()
    {
        _index = _ops.Length;
    }
}