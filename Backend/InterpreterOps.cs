namespace Backend;

using System.Runtime.CompilerServices;

public static partial class Interpreter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PushConst()
    {
        Push(_consts[_index]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Drop()
    {
        _sp--;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CallFunc()
    {
        PushVariables(_consts2[_index].GetInternalInteger());
        PushRet(_index);
        Jmp();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Ret()
    {
        PopVariables();
        _index = PopRet();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Add()
    {
        var b = Pop();
        var a = Pop();
        // ReSharper disable once JoinDeclarationAndInitializer
        WistConst res;

#if DEBUG
        if (a.Type != b.Type)
            throw new InvalidOperationException();


        res = a.Type switch
        {
            WistType.Number => (WistConst)(a.GetNumber() + b.GetNumber()),
            WistType.String => (WistConst)(a.GetString() + b.GetString()),
            _ => throw new NotImplementedException()
        };
#else
        res = a.Type switch
        {
            WistType.Number => (WistConst)(a.GetNumber() + b.GetNumber()),
            _ => (WistConst)(a.GetString() + b.GetString())
        };
#endif

        Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Sub()
    {
        var b = Pop();
        var a = Pop();
        // ReSharper disable once JoinDeclarationAndInitializer
        WistConst res;

#if DEBUG
        if (a.Type != b.Type)
            throw new InvalidOperationException();


        res = a.Type switch
        {
            WistType.Number => (WistConst)(a.GetNumber() - b.GetNumber()),
            _ => throw new NotImplementedException()
        };
#else
        res = (WistConst)(a.GetNumber() - b.GetNumber());
#endif

        Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Mul()
    {
        var b = Pop();
        var a = Pop();
        // ReSharper disable once JoinDeclarationAndInitializer
        WistConst res;

#if DEBUG
        if (a.Type != b.Type)
            throw new InvalidOperationException();


        res = a.Type switch
        {
            WistType.Number => (WistConst)(a.GetNumber() * b.GetNumber()),
            _ => throw new NotImplementedException()
        };
#else
        res = (WistConst)(a.GetNumber() * b.GetNumber());
#endif

        Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Div()
    {
        var b = Pop();
        var a = Pop();
        // ReSharper disable once JoinDeclarationAndInitializer
        WistConst res;

#if DEBUG
        if (a.Type != b.Type)
            throw new InvalidOperationException();


        res = a.Type switch
        {
            WistType.Number => (WistConst)(a.GetNumber() / b.GetNumber()),
            _ => throw new NotImplementedException()
        };
#else
        res = (WistConst)(a.GetNumber() / b.GetNumber());
#endif

        Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Rem()
    {
        var b = Pop();
        var a = Pop();
        // ReSharper disable once JoinDeclarationAndInitializer
        WistConst res;

#if DEBUG
        if (a.Type != b.Type)
            throw new InvalidOperationException();


        res = a.Type switch
        {
            WistType.Number => (WistConst)(a.GetNumber() % b.GetNumber()),
            _ => throw new NotImplementedException()
        };
#else
        res = (WistConst)(a.GetNumber() % b.GetNumber());
#endif

        Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Cmp()
    {
        var res = CmpInternal();

        Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void NotCmp()
    {
        var res = !CmpInternal().GetBool();

        Push((WistConst)res);
    }


    private static WistConst CmpInternal()
    {
        var b = Pop();
        var a = Pop();
        // ReSharper disable once JoinDeclarationAndInitializer
        WistConst res;

#if DEBUG
        if (a.Type != b.Type)
            throw new InvalidOperationException();


        res = a.Type switch
        {
            WistType.Number => (WistConst)(double.Abs(a.GetNumber() - b.GetNumber()) < 0.000_01),
            WistType.String => (WistConst)(a.GetString() == b.GetString()),
            WistType.Null => (WistConst)(b.Type == WistType.Null),
            _ => throw new NotImplementedException()
        };
#else
        res = a.Type switch
        {
            WistType.Number => (WistConst)(double.Abs(a.GetNumber() - b.GetNumber()) < 0.000_01),
            WistType.Null => (WistConst)(b.Type == WistType.Null),
            _ => (WistConst)(a.GetString() == b.GetString())
        };
#endif
        return res;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void JmpIfFalse()
    {
#if DEBUG
        if (_stack[_sp - 1].Type != WistType.Bool)
            throw new InvalidOperationException();
#endif

        if (!Pop().GetBool()) Jmp();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void JmpIfTrue()
    {
#if DEBUG
        if (_stack[_sp - 1].Type != WistType.Bool)
            throw new InvalidOperationException();
#endif

        if (Pop().GetBool()) Jmp();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Jmp()
    {
        _index = _consts[_index].GetInternalInteger();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetVar()
    {
        SetCurVar(_consts[_index].GetInternalInteger(), Pop());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Dup()
    {
        DupTop();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe void CallExternalMethod()
    {
        ((delegate*<void>)_consts[_index].GetInternalPtr())();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LoadVar()
    {
        Push(GetCurVar(_consts[_index].GetInternalInteger()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LessThan()
    {
        var b = Pop();
        var a = Pop();
        // ReSharper disable once JoinDeclarationAndInitializer
        WistConst res;

#if DEBUG
        if (a.Type != b.Type)
            throw new InvalidOperationException();

        res = a.Type switch
        {
            WistType.Number => (WistConst)(a.GetNumber() < b.GetNumber()),
            _ => throw new NotImplementedException()
        };
#else
        res = (WistConst)(a.GetNumber() < b.GetNumber());
#endif

        Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void GreaterThan()
    {
        var b = Pop();
        var a = Pop();
        // ReSharper disable once JoinDeclarationAndInitializer
        WistConst res;

#if DEBUG
        if (a.Type != b.Type)
            throw new InvalidOperationException();

        res = a.Type switch
        {
            WistType.Number => (WistConst)(a.GetNumber() > b.GetNumber()),
            _ => throw new NotImplementedException()
        };
#else
        res = (WistConst)(a.GetNumber() > b.GetNumber());
#endif

        Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void GreaterOrEquals()
    {
        var b = Pop();
        var a = Pop();
        // ReSharper disable once JoinDeclarationAndInitializer
        WistConst res;

#if DEBUG
        if (a.Type != b.Type)
            throw new InvalidOperationException();

        res = a.Type switch
        {
            WistType.Number => (WistConst)(a.GetNumber() >= b.GetNumber()),
            _ => throw new NotImplementedException()
        };
#else
        res = (WistConst)(a.GetNumber() >= b.GetNumber());
#endif

        Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LessOrEquals()
    {
        var b = Pop();
        var a = Pop();
        // ReSharper disable once JoinDeclarationAndInitializer
        WistConst res;

#if DEBUG
        if (a.Type != b.Type)
            throw new InvalidOperationException();

        res = a.Type switch
        {
            WistType.Number => (WistConst)(a.GetNumber() <= b.GetNumber()),
            _ => throw new NotImplementedException()
        };
#else
        res = (WistConst)(a.GetNumber() <= b.GetNumber());
#endif

        Push(res);
    }
}