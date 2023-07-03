namespace Backend.Interpreter;

using System.Runtime.CompilerServices;
using static WistMagicMethodsNames;

public partial class WistInterpreter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PushConst()
    {
        _stack.Push(_consts[_index]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Drop()
    {
        _stack.Drop();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CallFunc()
    {
        CallByAddr(_consts[_index].GetInternalInteger(), _consts2[_index].GetInternalInteger());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CallMethod()
    {
        CallMethodInternal(
            _consts2[_index].GetInternalInteger(),
            _consts[_index].GetInternalInteger(),
            _stack.Pop().GetClass()
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CallMethodInternal(int varsCountToPush, int methodId, WistClass @class)
    {
        PushVariables(varsCountToPush);
        _returnStack.Push(_index);
        _index = @class.GetMethodPtr(methodId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PushNewList()
    {
        _stack.Push(new WistConst(new List<WistConst>()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Ret()
    {
        PopVariables();
        _index = _returnStack.Pop();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Add()
    {
        var b = _stack.Pop();
        var a = _stack.Pop();

        switch (a.Type)
        {
            case WistType.Number:
                var res = new WistConst(a.GetNumber() + b.GetNumber());
                _stack.Push(res);
                break;
            case WistType.String:
                res = new WistConst(a.GetString() + b.GetString());
                _stack.Push(res);
                break;
            case WistType.Class:
                var @class = a.GetClass();
                _stack.Restore(2);
                Dup();
                CallMethodInternal(_consts2[_index].GetInternalInteger(), __add__, @class);
                break;
            default:
                WistError.ThrowInvalidOperationForThisTypes(a.Type, b.Type);
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Sub()
    {
        var b = _stack.Pop();
        var a = _stack.Pop();


        switch (a.Type)
        {
            case WistType.Number:
                var res = new WistConst(a.GetNumber() - b.GetNumber());
                _stack.Push(res);
                break;
            case WistType.Class:
                var @class = a.GetClass();
                _stack.Restore(2);
                Dup();
                CallMethodInternal(_consts2[_index].GetInternalInteger(), __sub__, @class);
                break;
            default:
                WistError.ThrowInvalidOperationForThisTypes(a.Type, b.Type);
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Mul()
    {
        var b = _stack.Pop();
        var a = _stack.Pop();


        switch (a.Type)
        {
            case WistType.Number:
                var res = new WistConst(a.GetNumber() * b.GetNumber());
                _stack.Push(res);
                break;
            case WistType.Class:
                var @class = a.GetClass();
                _stack.Restore(2);
                Dup();
                CallMethodInternal(_consts2[_index].GetInternalInteger(), __mul__, @class);
                break;
            default:
                WistError.ThrowInvalidOperationForThisTypes(a.Type, b.Type);
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Div()
    {
        var b = _stack.Pop();
        var a = _stack.Pop();


        switch (a.Type)
        {
            case WistType.Number:
                var res = new WistConst(a.GetNumber() / b.GetNumber());
                _stack.Push(res);
                break;
            case WistType.Class:
                var @class = a.GetClass();
                _stack.Restore(2);
                Dup();
                CallMethodInternal(_consts2[_index].GetInternalInteger(), __div__, @class);
                break;
            default:
                WistError.ThrowInvalidOperationForThisTypes(a.Type, b.Type);
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void And()
    {
        var b = _stack.Pop();
        var a = _stack.Pop();


        var res = a.Type switch
        {
            WistType.Bool => new WistConst(a.GetBool() && b.GetBool()),
            _ => WistError.ThrowInvalidOperationForThisTypes(a.Type, b.Type)
        };

        _stack.Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Or()
    {
        var b = _stack.Pop();
        var a = _stack.Pop();


        var res = a.Type switch
        {
            WistType.Bool => new WistConst(a.GetBool() || b.GetBool()),
            _ => WistError.ThrowInvalidOperationForThisTypes(a.Type, b.Type)
        };

        _stack.Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Xor()
    {
        var b = _stack.Pop();
        var a = _stack.Pop();


        var res = a.Type switch
        {
            WistType.Bool => new WistConst(a.GetBool() ^ b.GetBool()),
            _ => WistError.ThrowInvalidOperationForThisTypes(a.Type, b.Type)
        };

        _stack.Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Not()
    {
        var a = _stack.Pop();

        var res = a.Type switch
        {
            WistType.Bool => new WistConst(!a.GetBool()),
            _ => WistError.ThrowInvalidOperationForThisType(a.Type)
        };

        _stack.Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Rem()
    {
        var b = _stack.Pop();
        var a = _stack.Pop();


        var res = a.Type switch
        {
            WistType.Number => new WistConst(a.GetNumber() % b.GetNumber()),
            _ => WistError.ThrowInvalidOperationForThisTypes(a.Type, b.Type)
        };

        _stack.Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Cmp()
    {
        var res = CmpInternal();

        _stack.Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void NotCmp()
    {
        var res = !CmpInternal().GetBool();

        _stack.Push(new WistConst(res));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private WistConst CmpInternal()
    {
        var b = _stack.Pop();
        var a = _stack.Pop();


        switch (a.Type)
        {
            case WistType.Number:
                var res = new WistConst(double.Abs(a.GetNumber() - b.GetNumber()) < 0.000_01);
                _stack.Push(res);
                break;
            case WistType.Class:
                var @class = a.GetClass();
                _stack.Restore(2);
                Dup();
                CallMethodInternal(_consts2[_index].GetInternalInteger(), __cmp__, @class);
                break;
            default:
                WistError.ThrowInvalidOperationForThisTypes(a.Type, b.Type);
                break;
        }

        return _stack.Pop();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void JmpIfFalse()
    {
        if (!_stack.Pop().GetBool()) Jmp();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void JmpIfTrue()
    {
        if (_stack.Pop().GetBool()) Jmp();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Jmp()
    {
        _index = _consts[_index].GetInternalInteger();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetLocal()
    {
        SetCurVar(_consts[_index].GetInternalInteger(), _stack.Pop());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe void CallExternalMethod()
    {
        ((delegate*<WistInterpreter, int, void>)_consts[_index].GetInternalPtr())
            (this, _consts2[_index].GetInternalInteger());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void LoadLocal()
    {
        _stack.Push(GetCurVar(_consts[_index].GetInternalInteger()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void LoadGlobal()
    {
        _stack.Push(GetGlobalVar(_consts[_index].GetInternalInteger()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CopyClass()
    {
        _stack.Push(new WistConst(_consts[_index].GetClass().Copy()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetField()
    {
        var c = _stack.Pop().GetClass();
        var value = _stack.Pop();
        c.SetField(_consts[_index].GetInternalInteger(), value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void LoadField()
    {
        _stack.Push(_stack.Pop().GetClass().GetField(_consts[_index].GetInternalInteger()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetGlobal()
    {
        SetGlobalVar(_consts[_index].GetInternalInteger(), _stack.Pop());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CreateGlobal()
    {
        CreateGlobalVar(_consts[_index].GetInternalInteger());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void LessThan()
    {
        var b = _stack.Pop();
        var a = _stack.Pop();


        var res = a.Type switch
        {
            WistType.Number => new WistConst(a.GetNumber() < b.GetNumber()),
            _ => WistError.ThrowInvalidOperationForThisTypes(a.Type, b.Type)
        };

        _stack.Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GreaterThan()
    {
        var b = _stack.Pop();
        var a = _stack.Pop();


        var res = a.Type switch
        {
            WistType.Number => new WistConst(a.GetNumber() > b.GetNumber()),
            _ => WistError.ThrowInvalidOperationForThisTypes(a.Type, b.Type)
        };

        _stack.Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GreaterOrEquals()
    {
        var b = _stack.Pop();
        var a = _stack.Pop();


        var res = a.Type switch
        {
            WistType.Number => new WistConst(a.GetNumber() >= b.GetNumber()),
            _ => WistError.ThrowInvalidOperationForThisTypes(a.Type, b.Type)
        };

        _stack.Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void LessOrEquals()
    {
        var b = _stack.Pop();
        var a = _stack.Pop();


        var res = a.Type switch
        {
            WistType.Number => new WistConst(a.GetNumber() <= b.GetNumber()),
            _ => WistError.ThrowInvalidOperationForThisTypes(a.Type, b.Type)
        };

        _stack.Push(res);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Dup()
    {
        _stack.Dup();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetElem()
    {
        var index = _stack.Pop().GetNumber();
        var elem = _stack.Pop();
        var list = _stack.Pop();

        list.GetList()[(int)(index + 0.1) - 1] = elem;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PushElem()
    {
        var index = _stack.Pop().GetNumber();
        var list = _stack.Pop();

        _stack.Push(list.GetList()[(int)(index + 0.1) - 1]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddElem()
    {
        var elem = _stack.Pop();
        var list = _stack.Pop();

        list.GetList().Add(elem);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PushTry()
    {
        PushTry(_consts[_index].GetInternalInteger());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DropTryOp()
    {
        DropTry();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CallByAddr(int addr, int varsCountToPush)
    {
        PushVariables(varsCountToPush);
        _returnStack.Push(_index);
        _index = addr;
    }
}