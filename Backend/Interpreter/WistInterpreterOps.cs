namespace Backend.Interpreter;

using System.Runtime.CompilerServices;
using static WistMagicMethodsNames;

public partial class WistInterpreter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PushConst(WistInterpreter i)
    {
        i.Push(i._consts[i._index]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Drop(WistInterpreter i)
    {
        i._sp--;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CallFunc(WistInterpreter i)
    {
        i.PushVariables(i._consts2[i._index].GetInternalInteger());
        i.PushRet(i._index);
        Jmp(i);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CallMethod(WistInterpreter i)
    {
        CallMethodInternal(i,
            i._consts2[i._index].GetInternalInteger(),
            i._consts[i._index].GetInternalInteger(),
            i.Pop().GetClass()
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CallMethodInternal(WistInterpreter i, int varsCountToPush, int methodId, WistClass @class)
    {
        i.PushVariables(varsCountToPush);
        i.PushRet(i._index);
        i._index = @class.GetMethodPtr(methodId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PushNewList(WistInterpreter i)
    {
        i.Push(new WistConst(new List<WistConst>()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Ret(WistInterpreter i)
    {
        i.PopVariables();
        i._index = i.PopRet();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Add(WistInterpreter i)
    {
        var b = i.Pop();
        var a = i.Pop();

        switch (a.Type)
        {
            case WistType.Number:
                if (a.Type != b.Type)
                    WistException.ThrowTypesMustBeTheSame();

                var res = new WistConst(a.GetNumber() + b.GetNumber());
                i.Push(res);
                break;
            case WistType.String:
                if (a.Type != b.Type)
                    WistException.ThrowTypesMustBeTheSame();

                res = new WistConst(a.GetString() + b.GetString());
                i.Push(res);
                break;
            case WistType.Class:
                var @class = a.GetClass();
                i._sp += 2;
                Dup(i);
                CallMethodInternal(i, i._consts2[i._index].GetInternalInteger(), __add__, @class);
                break;
            default:
                WistException.ThrowInvalidOperationForThisTypes(a.Type, b.Type);
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Sub(WistInterpreter i)
    {
        var b = i.Pop();
        var a = i.Pop();

        if (a.Type != b.Type)
            WistException.ThrowTypesMustBeTheSame();

        switch (a.Type)
        {
            case WistType.Number:
                if (a.Type != b.Type)
                    WistException.ThrowTypesMustBeTheSame();

                var res = new WistConst(a.GetNumber() - b.GetNumber());
                i.Push(res);
                break;
            case WistType.Class:
                var @class = a.GetClass();
                i._sp += 2;
                Dup(i);
                CallMethodInternal(i, i._consts2[i._index].GetInternalInteger(), __sub__, @class);
                break;
            default:
                WistException.ThrowInvalidOperationForThisTypes(a.Type, b.Type);
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Mul(WistInterpreter i)
    {
        var b = i.Pop();
        var a = i.Pop();

        if (a.Type != b.Type)
            WistException.ThrowTypesMustBeTheSame();

        switch (a.Type)
        {
            case WistType.Number:
                if (a.Type != b.Type)
                    WistException.ThrowTypesMustBeTheSame();

                var res = new WistConst(a.GetNumber() * b.GetNumber());
                i.Push(res);
                break;
            case WistType.Class:
                var @class = a.GetClass();
                i._sp += 2;
                Dup(i);
                CallMethodInternal(i, i._consts2[i._index].GetInternalInteger(), __mul__, @class);
                break;
            default:
                WistException.ThrowInvalidOperationForThisTypes(a.Type, b.Type);
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Div(WistInterpreter i)
    {
        var b = i.Pop();
        var a = i.Pop();

        if (a.Type != b.Type)
            WistException.ThrowTypesMustBeTheSame();

        switch (a.Type)
        {
            case WistType.Number:
                if (a.Type != b.Type)
                    WistException.ThrowTypesMustBeTheSame();

                var res = new WistConst(a.GetNumber() / b.GetNumber());
                i.Push(res);
                break;
            case WistType.Class:
                var @class = a.GetClass();
                i._sp += 2;
                Dup(i);
                CallMethodInternal(i, i._consts2[i._index].GetInternalInteger(), __div__, @class);
                break;
            default:
                WistException.ThrowInvalidOperationForThisTypes(a.Type, b.Type);
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void And(WistInterpreter i)
    {
        var b = i.Pop();
        var a = i.Pop();

        if (a.Type != b.Type)
            WistException.ThrowTypesMustBeTheSame();

        var res = a.Type switch
        {
            WistType.Bool => new WistConst(a.GetBool() && b.GetBool()),
            _ => WistException.ThrowInvalidOperationForThisTypes(a.Type, b.Type)
        };

        i.Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Or(WistInterpreter i)
    {
        var b = i.Pop();
        var a = i.Pop();

        if (a.Type != b.Type)
            WistException.ThrowTypesMustBeTheSame();

        var res = a.Type switch
        {
            WistType.Bool => new WistConst(a.GetBool() || b.GetBool()),
            _ => WistException.ThrowInvalidOperationForThisTypes(a.Type, b.Type)
        };

        i.Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Xor(WistInterpreter i)
    {
        var b = i.Pop();
        var a = i.Pop();

        if (a.Type != b.Type)
            WistException.ThrowTypesMustBeTheSame();

        var res = a.Type switch
        {
            WistType.Bool => new WistConst(a.GetBool() ^ b.GetBool()),
            _ => WistException.ThrowInvalidOperationForThisTypes(a.Type, b.Type)
        };

        i.Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Not(WistInterpreter i)
    {
        var a = i.Pop();

        var res = a.Type switch
        {
            WistType.Bool => new WistConst(!a.GetBool()),
            _ => WistException.ThrowInvalidOperationForThisType(a.Type)
        };

        i.Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Rem(WistInterpreter i)
    {
        var b = i.Pop();
        var a = i.Pop();

        if (a.Type != b.Type)
            WistException.ThrowTypesMustBeTheSame();

        var res = a.Type switch
        {
            WistType.Number => new WistConst(a.GetNumber() % b.GetNumber()),
            _ => WistException.ThrowInvalidOperationForThisTypes(a.Type, b.Type)
        };

        i.Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Cmp(WistInterpreter i)
    {
        var res = CmpInternal(i);

        i.Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void NotCmp(WistInterpreter i)
    {
        var res = !CmpInternal(i).GetBool();

        i.Push(new WistConst(res));
    }


    private static WistConst CmpInternal(WistInterpreter i)
    {
        var b = i.Pop();
        var a = i.Pop();

        if (a.Type != b.Type)
            WistException.ThrowTypesMustBeTheSame();

        switch (a.Type)
        {
            case WistType.Number:
                if (a.Type != b.Type)
                    WistException.ThrowTypesMustBeTheSame();

                var res = new WistConst(double.Abs(a.GetNumber() - b.GetNumber()) < 0.000_01);
                i.Push(res);
                break;
            case WistType.Class:
                var @class = a.GetClass();
                i._sp += 2;
                Dup(i);
                CallMethodInternal(i, i._consts2[i._index].GetInternalInteger(), __cmp__, @class);
                break;
            default:
                WistException.ThrowInvalidOperationForThisTypes(a.Type, b.Type);
                break;
        }

        return i.Pop();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void JmpIfFalse(WistInterpreter i)
    {
        if (i._stack[i._sp - 1].Type != WistType.Bool)
            WistException.ThrowTypeMustBe(WistType.Bool);

        if (!i.Pop().GetBool()) Jmp(i);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void JmpIfTrue(WistInterpreter i)
    {
        if (i._stack[i._sp - 1].Type != WistType.Bool)
            WistException.ThrowTypeMustBe(WistType.Bool);

        if (i.Pop().GetBool()) Jmp(i);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Jmp(WistInterpreter i)
    {
        i._index = i._consts[i._index].GetInternalInteger();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetLocal(WistInterpreter i)
    {
        i.SetCurVar(i._consts[i._index].GetInternalInteger(), i.Pop());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe void CallExternalMethod(WistInterpreter i)
    {
        ((delegate*<WistInterpreter, int, void>)i._consts[i._index].GetInternalPtr())(i,
            i._consts2[i._index].GetInternalInteger());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LoadLocal(WistInterpreter i)
    {
        i.Push(i.GetCurVar(i._consts[i._index].GetInternalInteger()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LoadGlobal(WistInterpreter i)
    {
        i.Push(i.GetGlobalVar(i._consts[i._index].GetInternalInteger()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CopyClass(WistInterpreter i)
    {
        i.Push(new WistConst(i._consts[i._index].GetClass().Copy()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetField(WistInterpreter i)
    {
        var c = i.Pop().GetClass();
        var value = i.Pop();
        c.SetField(i._consts[i._index].GetInternalInteger(), value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LoadField(WistInterpreter i)
    {
        i.Push(i.Pop().GetClass().GetField(i._consts[i._index].GetInternalInteger()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetGlobal(WistInterpreter i)
    {
        i.SetGlobalVar(i._consts[i._index].GetInternalInteger(), i.Pop());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CreateGlobal(WistInterpreter i)
    {
        i.CreateGlobalVar(i._consts[i._index].GetInternalInteger());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LessThan(WistInterpreter i)
    {
        var b = i.Pop();
        var a = i.Pop();

        if (a.Type != b.Type)
            WistException.ThrowTypesMustBeTheSame();

        var res = a.Type switch
        {
            WistType.Number => new WistConst(a.GetNumber() < b.GetNumber()),
            _ => WistException.ThrowInvalidOperationForThisTypes(a.Type, b.Type)
        };

        i.Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void GreaterThan(WistInterpreter i)
    {
        var b = i.Pop();
        var a = i.Pop();

        if (a.Type != b.Type)
            WistException.ThrowTypesMustBeTheSame();

        var res = a.Type switch
        {
            WistType.Number => new WistConst(a.GetNumber() > b.GetNumber()),
            _ => WistException.ThrowInvalidOperationForThisTypes(a.Type, b.Type)
        };

        i.Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void GreaterOrEquals(WistInterpreter i)
    {
        var b = i.Pop();
        var a = i.Pop();

        if (a.Type != b.Type)
            WistException.ThrowTypesMustBeTheSame();

        var res = a.Type switch
        {
            WistType.Number => new WistConst(a.GetNumber() >= b.GetNumber()),
            _ => WistException.ThrowInvalidOperationForThisTypes(a.Type, b.Type)
        };

        i.Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LessOrEquals(WistInterpreter i)
    {
        var b = i.Pop();
        var a = i.Pop();

        if (a.Type != b.Type)
            WistException.ThrowTypesMustBeTheSame();

        var res = a.Type switch
        {
            WistType.Number => new WistConst(a.GetNumber() <= b.GetNumber()),
            _ => WistException.ThrowInvalidOperationForThisTypes(a.Type, b.Type)
        };

        i.Push(res);
    }

    private static void Dup(WistInterpreter i)
    {
        i._stack[i._sp] = i._stack[i._sp - 1];
        i._sp++;
    }

    private static void SetElem(WistInterpreter i)
    {
        var index = i.Pop().GetNumber();
        var elem = i.Pop();
        var list = i.Pop();

        list.GetList()[(int)(index + 0.1) - 1] = elem;
    }

    private static void PushElem(WistInterpreter i)
    {
        var index = i.Pop().GetNumber();
        var list = i.Pop();

        i.Push(list.GetList()[(int)(index + 0.1) - 1]);
    }

    private static void AddElem(WistInterpreter i)
    {
        var elem = i.Pop();
        var list = i.Pop();

        list.GetList().Add(elem);
    }
}