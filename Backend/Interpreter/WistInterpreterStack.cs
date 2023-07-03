namespace Backend.Interpreter;

using System.Runtime.CompilerServices;

public partial class WistInterpreter
{
    private readonly WistStack<int> _returnStack = new(512);
    private readonly WistStack<WistConst> _stack = new(2048);
}