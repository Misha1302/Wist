namespace Backend.Interpreter;

using System.Collections.Immutable;
using System.Runtime.CompilerServices;

public partial class WistInterpreter
{
    private WistConst[] _consts = null!;
    private WistConst[] _consts2 = null!;
    private WistEngine _engine = null!;
    private int _index;
    private ImmutableArray<WistOp> _ops;


    public WistInterpreter(WistImageObject imageObject)
    {
        Init(imageObject);
    }

    public bool Halted => _index >= _ops.Length;


    private void Init(WistImageObject imageObject)
    {
        _ops = ImmutableArray.Create(imageObject.Ops.ToArray());
        _consts = imageObject.Consts.ToArray();
        _consts2 = imageObject.Consts2.ToArray();
        _index = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void RunSteps(int count)
    {
        for (var i = 0; i < count && _index < _ops.Length; _index++, i++)
            try
            {
                /* var format = $"{_ops[_index]} :: {_consts[_index]}";
                if (_sp > 0) format += $" :: {string.Join(", ", _stack[.._sp])}";
                Console.WriteLine(format + $" -- {_index} -- {_curLine}"); */
                
                ExecuteOneOp();
            }
            catch (WistError ex)
            {
                if (TryPopTry(out var ind, out var rsp))
                {
                    Push(new WistConst(ex.Message));

                    _rsp = rsp;
                    _index = ind;
                }
                else
                {
                    throw;
                }
            }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    private void ExecuteOneOp()
    {
        switch (_ops[_index])
        {
            case WistOp.PushConst:
                PushConst(this);
                break;
            case WistOp.Add:
                Add(this);
                break;
            case WistOp.Cmp:
                Cmp(this);
                break;
            case WistOp.JmpIfFalse:
                JmpIfFalse(this);
                break;
            case WistOp.CallExternalMethod:
                CallExternalMethod(this);
                break;
            case WistOp.JmpIfTrue:
                JmpIfTrue(this);
                break;
            case WistOp.Jmp:
                Jmp(this);
                break;
            case WistOp.SetLocal:
                SetLocal(this);
                break;
            case WistOp.LoadLocal:
                LoadLocal(this);
                break;
            case WistOp.LessThan:
                LessThan(this);
                break;
            case WistOp.GreaterThan:
                GreaterThan(this);
                break;
            case WistOp.NotCmp:
                NotCmp(this);
                break;
            case WistOp.LessOrEquals:
                LessOrEquals(this);
                break;
            case WistOp.GreaterOrEquals:
                GreaterOrEquals(this);
                break;
            case WistOp.Sub:
                Sub(this);
                break;
            case WistOp.Rem:
                Rem(this);
                break;
            case WistOp.Mul:
                Mul(this);
                break;
            case WistOp.Div:
                Div(this);
                break;
            case WistOp.Ret:
                Ret(this);
                break;
            case WistOp.CallFunc:
                CallFunc(this);
                break;
            case WistOp.Drop:
                Drop(this);
                break;
            case WistOp.Dup:
                Dup(this);
                break;
            case WistOp.SetElem:
                SetElem(this);
                break;
            case WistOp.PushElem:
                PushElem(this);
                break;
            case WistOp.AddElem:
                AddElem(this);
                break;
            case WistOp.SetGlobal:
                SetGlobal(this);
                break;
            case WistOp.LoadGlobal:
                LoadGlobal(this);
                break;
            case WistOp.CopyClass:
                CopyClass(this);
                break;
            case WistOp.SetField:
                SetField(this);
                break;
            case WistOp.LoadField:
                LoadField(this);
                break;
            case WistOp.CallMethod:
                CallMethod(this);
                break;
            case WistOp.PushNewList:
                PushNewList(this);
                break;
            case WistOp.And:
                And(this);
                break;
            case WistOp.Or:
                Or(this);
                break;
            case WistOp.Xor:
                Xor(this);
                break;
            case WistOp.Not:
                Not(this);
                break;
            case WistOp.CreateGlobal:
                CreateGlobal(this);
                break;
#if DEBUG
            case WistOp.SetCurLine:
                SetCurLine(this);
                break;
            case WistOp.SetLocalsCount:
                SetLocalsCount(this);
                break;
            case WistOp.CreateLocal:
                CreateLocal(this);
                break;
#endif
            case WistOp.PushTry:
                PushTry(this);
                break;
            case WistOp.DropTry:
                DropTry(this);
                break;
            default:
                ThrowArgumentOutOfRange();
                break;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowArgumentOutOfRange()
    {
        throw new ArgumentOutOfRangeException();
    }

    public void ExitInterpreter()
    {
        _index = _ops.Length;
    }

    public void SetEngine(WistEngine engine)
    {
        _engine = engine;
    }
}