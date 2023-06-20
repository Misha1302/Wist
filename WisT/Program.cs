using System.Diagnostics;
using Antlr4.Runtime;
using Backend;
using WisT;
using WisT.WistGrammar;

const string code = """
loop let i = 0; i < 20_100; i = i + 1 {
    let q = IsPrime(i) 
}

func IsPrime(n) {
    loop let i = 2; i <= (n / 2); i = i + 1 {
        if n % i == 0 {
            return false
        }
    }
    return true
}
""";

var inputStream = new AntlrInputStream(code);
var simpleLexer = new WistGrammarLexer(inputStream);
var commonTokenStream = new CommonTokenStream(simpleLexer);
var simpleParser = new WistGrammarParser(commonTokenStream);
var simpleContext = simpleParser.program();
var visitor = new WistGrammarVisitor();
visitor.Visit(simpleContext);
var image = visitor.GetImage();

var s = Stopwatch.StartNew();
Interpreter.Run(image);
Console.WriteLine($"ms: {s.ElapsedMilliseconds / 1000.0}");