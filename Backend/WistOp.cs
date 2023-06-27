﻿namespace Backend;

public enum WistOp : byte
{
    PushConst,
    Add,
    Cmp,
    JmpIfFalse,
    CallExternalMethod,
    JmpIfTrue,
    Jmp,
    SetLocal,
    LoadLocal,
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
    Drop,
    Dup,
    SetElem,
    PushElem,
    AddElem,
    SetGlobal,
    LoadGlobal,
    CopyClass,
    SetField,
    LoadField,
    CallMethod,
    SetFirstRegister,
    LoadFirstRegister,
    PushNewList,
    And,
    Or,
    Xor,
    Not
}