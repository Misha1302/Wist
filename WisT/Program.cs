﻿using Antlr4.Runtime;
using Backend;
using Backend.Interpreter;
using WisT;
using WisT.WistContent;

const string dir = "WistContent";
const string path = @$"{dir}\Code.wst";

var code = WistPreprocessor.Preprocess(File.ReadAllText(path));

var inputStream = new AntlrInputStream(code);
var simpleLexer = new WistGrammarLexer(inputStream);
var commonTokenStream = new CommonTokenStream(simpleLexer);
var simpleParser = new WistGrammarParser(commonTokenStream);
var simpleContext = simpleParser.program();
var visitor = new WistGrammarVisitor();

var image = visitor.CompileCode(simpleContext, dir);
WistEngine.Instance.AddToTasks(new WistInterpreter(image));
WistEngine.Instance.Run();
Console.WriteLine($"main func returned: {WistEngine.Instance.ExitValue}");