grammar WistGrammar;

program: line* EOF;
line: statement | ifBlock | whileBlock | loopBlock | funcDecl;
statement: (assigment | functionCall | gotoLabel | setLabel | return | dllImport) ('\n' | ';');

assigment: varAssigment | elementOfArrayAssigment;
varAssigment: (TYPE)? IDENTIFIER '=' expression;
elementOfArrayAssigment: IDENTIFIER '[' expression ']' '=' expression;

ifBlock: 'if' expression block ('else' elseIfBlock)?;
elseIfBlock: block | ifBlock;
whileBlock: WHILE expression block;
loopBlock: LOOP assigment? expression assigment? block;
functionCall: IDENTIFIER '(' (expression (',' expression)*)? ')';
block: '{' line* '}';
gotoLabel: 'goto' IDENTIFIER;
setLabel: IDENTIFIER ':';
elementOfArray: IDENTIFIER '[' expression ']'; 
arrayInit: '[' (expression (',' expression)*)? ']';
funcDecl: 'func' IDENTIFIER '(' (IDENTIFIER (',' IDENTIFIER)*)? ')' block;
return: 'return' expression;
dllImport: 'import' STRING;

expression
    : constant                              #constantExpression
    | functionCall                          #functionalExpression
    | IDENTIFIER                            #identifierExpression
    | elementOfArray                        #elementOfArrayExpression
    | '(' expression ')'                    #parenthesizedExpression
    | '!' expression                        #notExpression
    | expression REM_OP expression          #remExpression
    | expression MUL_OP expression          #mulExpression
    | expression ADD_OP expression          #addExpression
    | expression CMP_OP expression          #cmpExpression
    | expression BOOL_OP expression         #boolExpression
    | arrayInit                             #arrayInitExpression
    ;


constant: NUMBER | STRING | BOOL | NULL;

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
TYPE: 'let' | 'global';
WS: [ \t\r\n]+ -> skip;
IDENTIFIER: [a-zA-Z_][a-zA-Z0-9_]*;