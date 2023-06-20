using System.Diagnostics;
using System.Globalization;
using Antlr4.Runtime;
using Backend;
using WisT;
using WisT.WistGrammar;

// only 13.5% slower than python
const string code = """
loop let i = 0; i < 20_100; i = i + 1 {
    IsPrime(i)
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
var wistFixedImage = visitor.CompileCode(simpleContext);


for (var i = 0; i < 10; i++)
{
    var s = Stopwatch.StartNew();
    Interpreter.Run(wistFixedImage);
    Console.Write($"{(s.ElapsedMilliseconds / 1000.0).ToString(CultureInfo.InvariantCulture)} + ");
}