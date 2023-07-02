namespace Backend;

using System.Runtime.CompilerServices;

internal static class WistThrowHelper
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static WistClass ThrowGetClass(WistType s) =>
        throw new WistError($"Cannot get class 'cause type of this const is {s}");

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static List<WistConst> ThrowGetList(WistType s) =>
        throw new WistError($"Cannot get list 'cause type of this const is {s}");

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static double ThrowGetNumber(WistType s) =>
        throw new WistError($"Cannot get number 'cause type of this const is {s}");

    /*
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static int ThrowGetInteger(WistType s) =>
        throw new WistException($"Cannot get integer 'cause type of this const is {s}");

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static nint ThrowGetPointer(WistType s) =>
        throw new WistException($"Cannot get pointer 'cause type of this const is {s}");
    */

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool ThrowGetBool(WistType s) =>
        throw new WistError($"Cannot get bool 'cause type of this const is {s}");

    public static WistError ThrowGetWistError(WistType type) =>
        throw new WistError($"Cannot get error 'cause type of this const is {type}");
}