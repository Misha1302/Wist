﻿namespace Backend;

using Backend.Glossary;
using Backend.Interpreter;

public class WistEngine
{
    private static WistEngine? _instance;
    private readonly List<WistInterpreter> _interpreters = new();
    public WistGlossary<WistConst> Globals = new(512);

    public static WistEngine Instance => _instance ??= new WistEngine();
    public WistConst ExitValue { get; private set; }

    public void AddToTasks(WistInterpreter interpreter)
    {
        interpreter.SetEngine(this);
        _interpreters.Add(interpreter);
    }

    public void Run()
    {
        while (_interpreters.Count != 0)
        {
            for (var i = _interpreters.Count - 1; i >= 0; i--)
                _interpreters[i].RunSteps(1_000);

            for (var i = _interpreters.Count - 1; i >= 0; i--)
                if (_interpreters[i].Halted)
                {
                    if (_interpreters.Count == 1)
                        ExitValue = _interpreters[0].PeekReturnValue();
                    _interpreters.RemoveAt(i--);
                }
        }
    }
}