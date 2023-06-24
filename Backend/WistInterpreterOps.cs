namespace Backend;

using System.Runtime.CompilerServices;

public static partial class WistInterpreter
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
    private static void CallMethod()
    {
        PushVariables(_consts2[_index].GetInternalInteger());
        PushRet(_index);
        _index = Pop().GetClass().GetMethodPtr(_consts[_index].GetInternalInteger());
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
            WistException.ThrowTypesMustBeTheSame();


        res = a.Type switch
        {
            WistType.Number => (WistConst)(a.GetNumber() + b.GetNumber()),
            WistType.String => (WistConst)(a.GetString() + b.GetString()),
            _ => WistException.ThrowInvalidOperationForThisType(a.Type, b.Type)
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
            _ => WistException.ThrowInvalidOperationForThisType(a.Type, b.Type)
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
            _ => WistException.ThrowInvalidOperationForThisType(a.Type, b.Type)
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
            _ => WistException.ThrowInvalidOperationForThisType(a.Type, b.Type)
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
            _ => WistException.ThrowInvalidOperationForThisType(a.Type, b.Type)
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
            _ => WistException.ThrowInvalidOperationForThisType(a.Type, b.Type)
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
    private static void SetLocal()
    {
        SetCurVar(_consts[_index].GetInternalInteger(), Pop());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe void CallExternalMethod()
    {
        ((delegate*<void>)_consts[_index].GetInternalPtr())();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LoadLocal()
    {
        Push(GetCurVar(_consts[_index].GetInternalInteger()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LoadGlobal()
    {
        Push(GetGlobalVar(_consts[_index].GetInternalInteger()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CopyClass()
    {
        Push(new WistConst(_consts[_index].GetClass().Copy()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetField()
    {
        var value = Pop();
        var c = Pop().GetClass();
        c.SetField(_consts[_index].GetInternalInteger(), value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LoadField()
    {
        Push(Pop().GetClass().GetField(_consts[_index].GetInternalInteger()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetGlobal()
    {
        SetGlobalVar(_consts[_index].GetInternalInteger(), Pop());
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
            _ => WistException.ThrowInvalidOperationForThisType(a.Type, b.Type)
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
            _ => WistException.ThrowInvalidOperationForThisType(a.Type, b.Type)
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
            _ => WistException.ThrowInvalidOperationForThisType(a.Type, b.Type)
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
            _ => WistException.ThrowInvalidOperationForThisType(a.Type, b.Type)
        };
#else
        res = (WistConst)(a.GetNumber() <= b.GetNumber());
#endif

        Push(res);
    }

    private static void Dup()
    {
        _stack[_sp] = _stack[_sp - 1];
        _sp++;
    }

    private static void SetElem()
    {
        var index = Pop().GetNumber();
        var elem = Pop();
        var list = Pop();

        list.GetList()[(int)(index + 0.1) - 1] = elem;
    }

    private static void PushElem()
    {
        var index = Pop().GetNumber();
        var list = Pop();

        Push(list.GetList()[(int)(index + 0.1) - 1]);
    }

    private static void AddElem()
    {
        var elem = Pop();
        var list = Pop();

        list.GetList().Add(elem);
    }
}