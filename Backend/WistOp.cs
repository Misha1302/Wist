namespace Backend;

public enum WistOp : byte
{
    PushConst,
    Add,
    Cmp,
    JmpIfFalse,
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
    CallFunc,
    Drop
}