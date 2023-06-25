using System.Diagnostics;
using System.Globalization;
using Antlr4.Runtime;
using Backend;
using Backend.Interpreter;
using WisT;
using WisT.WistContent;

const string dir = "WistContent";
const string path = @$"{dir}\Code.wst";
var code = string.Join("\n", File.ReadAllText(path).Split('\n', '\r').Where(x => !string.IsNullOrWhiteSpace(x)));

var inputStream = new AntlrInputStream(code);
var simpleLexer = new WistGrammarLexer(inputStream);
var commonTokenStream = new CommonTokenStream(simpleLexer);
var simpleParser = new WistGrammarParser(commonTokenStream);
var simpleContext = simpleParser.program();
var visitor = new WistGrammarVisitor();
visitor.CompileCode(simpleContext, dir, false);

for (var i = 0; i < 1; i++)
{
    var s = Stopwatch.StartNew();
    WistInterpreter.Run(visitor.GetFixedImage());
    Console.Write($"{(s.ElapsedMilliseconds / 1000.0).ToString(CultureInfo.InvariantCulture)} + ");
}

Console.Write("\b\b \b\b");