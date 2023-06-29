namespace Backend;

using Backend.Interpreter;

public class WistEngine
{
    private readonly List<WistInterpreter> _interpreters = new();

    public static WistEngine CreateEngine(WistImageObject image)
    {
        var engine = new WistEngine();
        engine.AddToTasks(new WistInterpreter(image));
        return engine;
    }

    public void AddToTasks(WistInterpreter interpreter)
    {
        _interpreters.Add(interpreter);
    }

    public WistConst Run()
    {
        for (var i = 0; i < _interpreters.Count; i++)
            _interpreters[i].RunSteps(10);
        return _interpreters[0].Pop();
    }
}