namespace Backend;

using System.Diagnostics;

public static class Program
{
    public static void Main(string[] args)
    {
        WistImageBuilder image = new();

        // i = 0
        image.CreateLocal("i");
        image.PushConst((WistConst)0);
        image.SetLocal("i");

        // while i < 100_000_000
        image.SetLabel("while");


        image.LoadLocal("i");
        image.PushConst((WistConst)100_000_000);
        image.LessThan();
        image.JmpIfFalse("end");

        // i = i + 1
        image.LoadLocal("i");
        image.PushConst((WistConst)1);
        image.Add();
        image.SetLocal("i");
        image.Jmp("while");

        // end
        image.SetLabel("end");


        var wistFixedImage = image.Compile();
        var s = Stopwatch.StartNew();
        Interpreter.Run(wistFixedImage);
        s.Stop();
        Console.WriteLine(s.ElapsedMilliseconds / 1000.0);
    }
}