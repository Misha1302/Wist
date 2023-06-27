namespace Backend.Interpreter;

public static unsafe partial class WistInterpreter
{
    private static WistConst[] _consts = null!;
    private static WistConst[] _consts2 = null!;
    private static WistOp[] _ops = null!;
    private static int _index;

    // https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/function-pointers.md
    // methods MUST BE in enum WistOp order
    private static readonly delegate*<void>[] _functions =
    {
        &PushConst, &Add, &Cmp, &JmpIfFalse, &CallExternalMethod, &JmpIfTrue, &Jmp, &SetLocal, &LoadLocal,
        &LessThan, &GreaterThan, &NotCmp, &LessOrEquals, &GreaterOrEquals, &Sub, &Rem, &Mul, &Div, &Ret, &CallFunc,
        &Drop, &Dup, &SetElem, &PushElem, &AddElem, &SetGlobal, &LoadGlobal, &CopyClass, &SetField, &LoadField,
        &CallMethod, &SetFirstRegister, &LoadFirstRegister, &PushNewList, &And, &Or, &Xor, &Not
    };

    private static void Init(WistImageObject imageObject)
    {
        _ops = imageObject.Ops.ToArray();
        _consts = imageObject.Consts.ToArray();
        _consts2 = imageObject.Consts2.ToArray();
    }


    public static WistConst Run(WistImageObject imageObject)
    {
        Init(imageObject);

        var len = _ops.Length;
        var ops = _ops.AsSpan();

        for (_index = 0; _index < len; _index++)
        {
            var wistOp = ops[_index];

            /* var format = $"{wistOp} :: {_consts[_index]}";
            if (_sp > 0) format += $" :: {string.Join(", ", _stack[.._sp])}";
            Console.WriteLine(format); */

            _functions[(int)wistOp]();
        }

        return Pop();
    }

    public static void Exit()
    {
        _index = _ops.Length;
    }
}