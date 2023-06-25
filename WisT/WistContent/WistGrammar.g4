grammar WistGrammar;

program: line* EOF;
line: statement | ifBlock | whileBlock | loopBlock | funcDecl | methodDecl | classDecl;
statement: simpleStatement (SEMICOLON simpleStatement)* (SEMICOLON)? NEWLINE;
simpleStatement: methodCall | call | gotoLabel | setLabel | return | dllImport | assigment;

assigment: varAssigment | elementOfArrayAssigment | fieldAssigment;
fieldAssigment: expression '.' IDENTIFIER '=' expression;
varAssigment: (TYPE)? IDENTIFIER '=' expression;
elementOfArrayAssigment: expression '[' expression ']' '=' expression;

ifBlock: 'if' expression block ('else' elseIfBlock)?;
elseIfBlock: block | ifBlock;
whileBlock: WHILE expression block;
loopBlock: LOOP assigment? SEMICOLON expression SEMICOLON assigment? block;
call: IDENTIFIER '(' (expression (',' expression)*)? ')';
block: '{' line* '}';
gotoLabel: 'goto' IDENTIFIER;
setLabel: IDENTIFIER ':';
arrayInit: '[' (expression (',' expression)*)? ']';
funcDecl: 'func' IDENTIFIER '(' (IDENTIFIER (',' IDENTIFIER)*)? ')' block;
methodDecl: 'method' IDENTIFIER '(' (IDENTIFIER (',' IDENTIFIER)*)? ')' block;
return: 'return' expression;
dllImport: 'import' STRING;
classDecl: 'class' IDENTIFIER '(' (IDENTIFIER (',' IDENTIFIER)*)? ')' block;
classInit: 'new' IDENTIFIER '(' (expression (',' expression)*)? ')';
methodCall: expression '.' call;

expression
    : constant                              #constantExpression
    | expression '[' expression ']'         #elementOfArrayExpression
    | expression '.' call                   #methodExpression
    | call                                  #functionExpression
    | IDENTIFIER                            #identifierExpression
    | '(' expression ')'                    #parenthesizedExpression
    | '!' expression                        #notExpression
    | expression REM_OP expression          #remExpression
    | expression MUL_OP expression          #mulExpression
    | expression ADD_OP expression          #addExpression
    | expression CMP_OP expression          #cmpExpression
    | expression BOOL_OP expression         #boolExpression
    | arrayInit                             #arrayInitExpression
    | classInit                             #classInitExpression
    | expression '.' IDENTIFIER             #fieldExpression
    ;


constant: NUMBER | STRING | BOOL | NULL;

NEWLINE: ( '\r'? '\n' | '\r' | '\f' );
SEMICOLON: ';';
NUMBER: [-]? [0-9] [0-9_]* ('.' [0-9_]*)? ('e' ('+' | '-')? [0-9_]*)?;
STRING: ('"' ~'"'* '"') | ('\'' ~'\''* '\'');
BOOL: 'true' | 'false';
NULL: 'null' | 'none';
BOOL_OP: 'and' | 'or' | 'xor';
MUL_OP: '*' | '/';
ADD_OP: '+' | '-';
REM_OP: '%';
CMP_OP: '==' | '!=' | '>' | '<' | '<=' | '>=';
WHILE: 'while' | 'until';
LOOP: 'loop';
TYPE: 'let' | 'var';
WS: ('\t' | ' ' | NEWLINE) -> skip;
IDENTIFIER: [a-zA-Z_][a-zA-Z0-9_]*;