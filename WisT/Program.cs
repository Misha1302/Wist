using Antlr4.Runtime;
using Backend;
using Backend.Interpreter;
using WisT;
using WisT.WistContent;

const string dir = "WistContent";
const string path = @$"{dir}\Code.wst";

var code = File.ReadAllText(path);

var inputStream = new AntlrInputStream(code);
var simpleLexer = new WistGrammarLexer(inputStream);
simpleLexer.AddErrorListener(new WistThrowingErrorListener());

var commonTokenStream = new CommonTokenStream(simpleLexer);
var simpleParser = new WistGrammarParser(commonTokenStream);
simpleParser.AddErrorListener(new WistThrowingErrorListener());

var simpleContext = simpleParser.program();
var visitor = new WistGrammarVisitor();

var image = visitor.CompileCode(simpleContext, dir);
WistEngine.Instance.AddToTasks(new WistInterpreter(image));
WistEngine.Instance.Run();
Console.WriteLine($"main func returned: {WistEngine.Instance.ExitValue}");