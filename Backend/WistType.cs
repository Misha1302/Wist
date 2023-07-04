namespace Backend;

public enum WistType : long
{
    // ReSharper disable once UnusedMember.Global
    // None should never be used
    None,
    Number,
    String,
    InternalInteger,
    Bool,
    Pointer,
    Null,
    List,
    Class,
    Interpreter
}