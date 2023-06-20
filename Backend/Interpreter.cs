﻿namespace Backend;

public static unsafe partial class Interpreter
{
    private static WistConst[] _consts = null!;
    private static WistConst[] _consts2 = null!;
    private static WistOp[] _ops = null!;
    private static int _index;

    // https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/function-pointers.md
    // methods MUST BE in enum WistOp order
    private static readonly delegate*<void>[] _functions =
    {
        &PushConst,
        &Add,
        &Cmp,
        &JmpIfFalse,
        &Dup,
        &CallExternalMethod,
        &JmpIfTrue,
        &Jmp,
        &SetVar,
        &LoadVar,
        &LessThan,
        &GreaterThan,
        &NotCmp,
        &LessOrEquals,
        &GreaterOrEquals,
        &Sub,
        &Rem,
        &Mul,
        &Div,
        &Ret,
        &CallFunc
    };

    private static void Init(WistImage image)
    {
        image.Compile();
        _ops = image.GetOps().ToArray();
        _consts = image.GetConsts().ToArray();
        _consts2 = image.GetConsts2().ToArray();
    }


    public static void Run(WistImage image)
    {
        Init(image);

        var len = _ops.Length;
        var ops = _ops.AsSpan();

        for (_index = 0; _index < len; _index++)
        {
            var wistOp = ops[_index];
            _functions[(int)wistOp]();


            continue;
            var format = $"{wistOp} :: {_consts[_index]}";
            if (_sp > 0) format += $" :: {string.Join(", ", _stack[.._sp])}";

            Console.WriteLine(format);
        }
    }
}