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
    GoTo,
    CallExternMethod,
    Drop,
    LessThan,
    GoToIfTrue,
    LoadArg,
    IsNotEquals
}