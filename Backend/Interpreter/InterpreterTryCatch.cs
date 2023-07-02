namespace Backend.Interpreter;

public partial class WistInterpreter
{
    private readonly (int ind, int rsp)[] _tryStack = new (int ind, int rsp)[128];
    private int _tp;

    private void PushTry(int ind)
    {
        _tryStack[_tp++] = (ind, _rsp);
    }

    private void DropTry()
    {
        _tp--;
    }

    private bool TryPopTry(out int ind, out int rsp)
    {
        if (_tp > 0)
        {
            (ind, rsp) = _tryStack[--_tp];
            return true;
        }

        (ind, rsp) = (-1, -1);
        return false;
    }
}