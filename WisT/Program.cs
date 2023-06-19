using System.Diagnostics;
using Antlr4.Runtime;
using Backend;
using WisT;
using WisT.WistGrammar;

// only 6.94% slower than python!
const string code = """
let i = 0.1
while i < 100_000_000.1 {
    i = i + 1.1
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
Console.WriteLine(s.ElapsedMilliseconds / 1000.0);