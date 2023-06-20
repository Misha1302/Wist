namespace Backend;

public enum WistType : byte
{
    // ReSharper disable once UnusedMember.Global
    // None should never be used
    None,
    Number,
    String,
    InternalInteger,
    Bool,
    Pointer,
    Null
}