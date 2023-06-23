using System.Diagnostics;
using System.Globalization;
using Antlr4.Runtime;
using Backend;
using WisT;
using WisT.WistGrammar;

const string path = @"WistGrammar\Code.wst";
var code = File.ReadAllText(path);

var inputStream = new AntlrInputStream(code);
var simpleLexer = new WistGrammarLexer(inputStream);
var commonTokenStream = new CommonTokenStream(simpleLexer);
var simpleParser = new WistGrammarParser(commonTokenStream);
var simpleContext = simpleParser.program();
var visitor = new WistGrammarVisitor();
visitor.CompileCode(simpleContext, "WistGrammar", false);


for (var i = 0; i < 1; i++)
{
    var wistImageObject = visitor.GetFixedImage();
    Console.WriteLine(wistImageObject);

    var s = Stopwatch.StartNew();
    Interpreter.Run(wistImageObject);
    Console.Write($"{(s.ElapsedMilliseconds / 1000.0).ToString(CultureInfo.InvariantCulture)} + ");
}

Console.Write("\b\b \b\b");