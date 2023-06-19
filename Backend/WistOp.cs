namespace Backend;

public enum WistOp
{
    PushConst,
    Add,
    Cmp,
    JmpIfFalse,
    Dup,
    CallExternalMethod,
    JmpIfTrue,
    Jmp,
    FreeVars,
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
    Div
}