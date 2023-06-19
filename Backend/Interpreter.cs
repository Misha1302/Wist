namespace Backend;

public static unsafe partial class Interpreter
{
    private static WistConst[] _consts = null!;
    private static WistOp[] _ops = null!;
    private static int _index;

    private static void Init(WistImage image)
    {
        image.Compile();
        _ops = image.GetOps().ToArray();
        _consts = image.GetConsts().ToArray();
    }

    public static void Run(WistImage image)
    {
        Init(image);

        // https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/function-pointers.md
        // methods MUST BE in enum WistOp order
        delegate*<void>[] functions =
        {
            &PushConst,
            &Add,
            &Cmp,
            &JmpIfFalse,
            &Dup,
            &CallExternalMethod,
            &JmpIfTrue,
            &Jmp,
            &FreeVars,
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
            &Div
        };

        // TODO: also try stackalloc - int* p = stackalloc int[size];
        var opsSpan = new ReadOnlySpan<WistOp>(_ops);
        var len = opsSpan.Length;
        for (_index = 0; _index < len; _index++)
        {
            var wistOp = opsSpan[_index];
            functions[(int)wistOp]();

            
            continue;
            var format = $"{wistOp} :: {_consts[_index]}";
            if (_sp > 0) format += $" :: {_stack[_sp]}";

            Console.WriteLine(format);
        }
    }
}