﻿using System.Diagnostics;
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
var wistFixedImage = visitor.CompileCode(simpleContext, "WistGrammar");


for (var i = 0; i < 1; i++)
{
    var s = Stopwatch.StartNew();
    Interpreter.Run(wistFixedImage);
    Console.Write($"{(s.ElapsedMilliseconds / 1000.0).ToString(CultureInfo.InvariantCulture)} + ");
}