namespace Backend;

using System.Runtime.CompilerServices;

public class WistError : Exception
{
    public WistError(string s) : base(s)
    {
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowTypesMustBeTheSame() =>
        throw new WistError("Types must be the same");

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static WistConst ThrowInvalidOperationForThisTypes(WistType aType, WistType bType) =>
        throw new WistError($"Invalid operation for types: {aType}, {bType}");

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static WistConst ThrowInvalidOperationForThisType(WistType aType) =>
        throw new WistError($"Invalid operation for type {aType}");

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowTypeMustBe(WistType t) =>
        throw new WistError($"Type must be {t}");
}