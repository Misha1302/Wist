namespace WistCompiler;

public enum WistOp
{
    Add,
    Push,
    Sub,
    Mul,
    Div,
    LoadLocal,
    SetLocal,
    SetField,
    LoadField,
    SetLabel,
    Goto,
    CallExternMethod,
    Drop,
    LessThan,
    GotoIfTrue,
    LoadArg,
    IsNotEquals,
    CreateClass,
    CallDynamicMethod,
    Ret,
    LessThanOrEquals,
    GotoIfFalse,
    Rem,
    IsEquals
}