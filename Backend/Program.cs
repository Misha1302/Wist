namespace Backend;

using System.Diagnostics;

public static class Program
{
    public static void Main(string[] args)
    {
        WistImage image = new();

        // i = 0
        image.CreateVar("i");
        image.PushConst((WistConst)0);
        image.SetVar("i");

        // while i < 100_000_000
        image.SetLabel("while");
        image.LoadVar("i");
        image.PushConst((WistConst)100_000_000);
        image.LessThan();
        image.JmpIfFalse("end");

        // i = i + 1
        image.LoadVar("i");
        image.PushConst((WistConst)1);
        image.Add();
        image.SetVar("i");
        image.Jmp("while");

        // end
        image.SetLabel("end");


        var s = Stopwatch.StartNew();
        Interpreter.Run(image);
        s.Stop();
        // only 3-4% slower than python!
        Console.WriteLine(s.ElapsedMilliseconds / 1000.0);
    }


    // ReSharper disable once UnusedMember.Local
    private static void Print()
    {
        Console.WriteLine($"{Interpreter.Pop()}");
    }
}