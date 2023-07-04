namespace Backend.Interpreter;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public partial class WistInterpreter
{
    private WistConst[] _consts = null!;
    private WistConst[] _consts2 = null!;
    private WistEngine _engine = null!;
    private int _index;
    private WistOp[] _ops = null!;


    public WistInterpreter(WistImageObject imageObject)
    {
        Init(imageObject);
    }

    public bool Halted
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _index >= _ops.Length;
    }


    private void Init(WistImageObject imageObject)
    {
        _ops = imageObject.Ops.ToArray();

        _consts = imageObject.Consts.ToArray();
        _consts2 = imageObject.Consts2.ToArray();
        _index = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public void RunSteps(int count)
    {
        ref var start = ref MemoryMarshal.GetArrayDataReference(_ops);
        ref var end = ref Unsafe.Add(ref start, _ops.Length);

        var i = 0;
        while (i < count)
        {
            ref var curElem = ref Unsafe.Add(ref start, _index);
            if (!Unsafe.IsAddressLessThan(ref curElem, ref end))
                break;

            try
            {
                /* var format = $"{_ops[_index]} :: {_consts[_index]}";
                if (_stack.ToString() != "") format += $" :: {_stack}";
                Console.WriteLine(format + $" -- {_index}"); */

                ExecuteOneOp();
            }
            catch (WistError ex)
            {
                if (TryPopTry(out var ind, out var rsp))
                {
                    _stack.Push(new WistConst(ex.Message));

                    _returnStack.ResetPointer(rsp);
                    _index = ind;
                }
                else
                {
                    throw;
                }
            }

            _index++;
            i++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    private void ExecuteOneOp()
    {
        switch (_ops[_index])
        {
            case WistOp.PushConst:
                PushConst();
                break;
            case WistOp.Add:
                Add();
                break;
            case WistOp.Cmp:
                Cmp();
                break;
            case WistOp.JmpIfFalse:
                JmpIfFalse();
                break;
            case WistOp.CallExternalMethod:
                CallExternalMethod();
                break;
            case WistOp.JmpIfTrue:
                JmpIfTrue();
                break;
            case WistOp.Jmp:
                Jmp();
                break;
            case WistOp.SetLocal:
                SetLocal();
                break;
            case WistOp.LoadLocal:
                LoadLocal();
                break;
            case WistOp.LessThan:
                LessThan();
                break;
            case WistOp.GreaterThan:
                GreaterThan();
                break;
            case WistOp.NotCmp:
                NotCmp();
                break;
            case WistOp.LessOrEquals:
                LessOrEquals();
                break;
            case WistOp.GreaterOrEquals:
                GreaterOrEquals();
                break;
            case WistOp.Sub:
                Sub();
                break;
            case WistOp.Rem:
                Rem();
                break;
            case WistOp.Mul:
                Mul();
                break;
            case WistOp.Div:
                Div();
                break;
            case WistOp.Ret:
                Ret();
                break;
            case WistOp.CallFunc:
                CallFunc();
                break;
            case WistOp.Drop:
                Drop();
                break;
            case WistOp.Dup:
                Dup();
                break;
            case WistOp.SetElem:
                SetElem();
                break;
            case WistOp.PushElem:
                PushElem();
                break;
            case WistOp.AddElem:
                AddElem();
                break;
            case WistOp.SetGlobal:
                SetGlobal();
                break;
            case WistOp.LoadGlobal:
                LoadGlobal();
                break;
            case WistOp.CopyClass:
                CopyClass();
                break;
            case WistOp.SetField:
                SetField();
                break;
            case WistOp.LoadField:
                LoadField();
                break;
            case WistOp.CallMethod:
                CallMethod();
                break;
            case WistOp.PushNewList:
                PushNewList();
                break;
            case WistOp.And:
                And();
                break;
            case WistOp.Or:
                Or();
                break;
            case WistOp.Xor:
                Xor();
                break;
            case WistOp.Not:
                Not();
                break;
            case WistOp.CreateGlobal:
                CreateGlobal();
                break;
#if DEBUG
            case WistOp.SetCurLine:
                SetCurLine();
                break;
            case WistOp.SetLocalsCount:
                SetLocalsCount();
                break;
            case WistOp.CreateLocal:
                CreateLocal();
                break;
#endif
            case WistOp.PushTry:
                PushTry();
                break;
            case WistOp.DropTry:
                DropTry();
                break;
            default:
                ThrowArgumentOutOfRange();
                break;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowArgumentOutOfRange() => throw new ArgumentOutOfRangeException();
}