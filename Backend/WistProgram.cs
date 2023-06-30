namespace Backend;

using System.Diagnostics;
using System.Reflection;
using Backend.Interpreter;

public static class WistProgram
{
    public static void Main(string[] args)
    {
        WistImageBuilder image = new();

        // i = 0
        image.CreateLocal("i");
        image.PushConst(new WistConst(0));
        image.SetLocal("i");

        // while i < 100_000_000
        image.SetLabel("while");


        image.LoadLocal("i");
        image.PushConst(new WistConst(5));
        image.LessThan();
        image.JmpIfFalse("end");

        image.CallExternalMethod(
            typeof(WistProgram).GetMethod("PrintHelloWorld", BindingFlags.Static | BindingFlags.Public)!, 1
        );

        // i = i + 1
        image.LoadLocal("i");
        image.PushConst(new WistConst(1));
        image.Add();
        image.SetLocal("i");
        image.Jmp("while");

        // end
        image.SetLabel("end");


        var wistFixedImage = image.Compile();
        WistEngine.Instance.AddToTasks(new WistInterpreter(wistFixedImage));

        var s = Stopwatch.StartNew();
        WistEngine.Instance.Run();
        s.Stop();
        Console.WriteLine(s.ElapsedMilliseconds / 1000.0);
    }

    public static void PrintHelloWorld()
    {
        Console.WriteLine("Hello, World!");
    }
}