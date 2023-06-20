namespace Backend;

public enum WistOp : byte
{
    PushConst,
    Add,
    Cmp,
    JmpIfFalse,
    Dup,
    CallExternalMethod,
    JmpIfTrue,
    Jmp,
    SetVar,
    LoadVar,
    LessThan,
    GreaterThan,
    NotCmp,
    LessOrEquals,
    GreaterOrEquals,
    Sub,
    Rem,
    Mul,
    Div,
    Ret,
    CallFunc
}