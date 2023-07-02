namespace Backend.Interpreter;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class WistMagicMethodsNames
{
    public static readonly int __add__ = "__add__2".GetWistHashCode();
    public static readonly int __sub__ = "__sub__2".GetWistHashCode();
    public static readonly int __mul__ = "__mul__2".GetWistHashCode();
    public static readonly int __div__ = "__div__2".GetWistHashCode();
    public static readonly int __cmp__ = "__cmp__2".GetWistHashCode();
}