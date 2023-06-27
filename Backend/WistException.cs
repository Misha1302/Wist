namespace Backend;

using System.Runtime.CompilerServices;

public class WistException : Exception
{
    public WistException(string s) : base(s)
    {
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowTypesMustBeTheSame() =>
        throw new WistException("Types must be the same");

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static WistConst ThrowInvalidOperationForThisTypes(WistType aType, WistType bType) =>
        throw new WistException($"Invalid operation for types: {aType}, {bType}");

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static WistConst ThrowInvalidOperationForThisType(WistType aType) =>
        throw new WistException($"Invalid operation for type {aType}");

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowTypeMustBe(WistType t) =>
        throw new WistException($"Type must be {t}");
}