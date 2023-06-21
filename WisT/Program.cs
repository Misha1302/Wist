using System.Diagnostics;
using System.Globalization;
using Antlr4.Runtime;
using Backend;
using WisT;
using WisT.WistGrammar;

// only 13.5% slower than python
const string code = """
import "WistRandomLib.dll"
import "WistTypeConverter.dll"

loop let i = 0; i < 10; i = i + 1 {
    Print('random int: ' + ToStr(RandomInteger(-10, 10)))
    Print('random real: '  + ToStr(RandomReal(-10, 10)))
    Print('')
}
""";

var inputStream = new AntlrInputStream(code);
var simpleLexer = new WistGrammarLexer(inputStream);
var commonTokenStream = new CommonTokenStream(simpleLexer);
var simpleParser = new WistGrammarParser(commonTokenStream);
var simpleContext = simpleParser.program();
var visitor = new WistGrammarVisitor();
var wistFixedImage = visitor.CompileCode(simpleContext);


for (var i = 0; i < 1; i++)
{
    var s = Stopwatch.StartNew();
    Interpreter.Run(wistFixedImage);
    Console.Write($"{(s.ElapsedMilliseconds / 1000.0).ToString(CultureInfo.InvariantCulture)} + ");
}