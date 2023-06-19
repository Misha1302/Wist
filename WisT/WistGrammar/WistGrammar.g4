grammar WistGrammar;

program: line* EOF;
line: statement | ifBlock | whileBlock;
statement: (assigment | functionCall | gotoLabel | setLabel) ('\n' | ';');

ifBlock: 'if' expression block ('else' elseIfBlock)?;
elseIfBlock: block | ifBlock;
whileBlock: WHILE expression block;
assigment: (TYPE)? (IDENTIFIER | elementOfArray) '=' expression;
functionCall: IDENTIFIER '(' (expression (',' expression)*)? ')';
block: '{' line* '}';
gotoLabel: 'goto' IDENTIFIER;
setLabel: IDENTIFIER ':';
elementOfArray: IDENTIFIER '[' expression ']'; 
arrayInit: '[' (expression)* ']';

expression
    : constant                              #constantExpression
    | IDENTIFIER                            #identifierExpression
    | functionCall                          #functionalExpression
    | elementOfArray                        #elementOfArrayExpression
    | arrayInit                             #arrayInitExpression
    | '(' expression ')'                    #parenthesizedExpression
    | '!' expression                        #notExpression
    | expression REM_OP expression          #remExpression
    | expression MUL_OP expression          #mulExpression
    | expression ADD_OP expression          #addExpression
    | expression CMP_OP expression          #cmpExpression
    | expression BOOL_OP expression         #boolExpression
    ;


constant: NUMBER | STRING | NULL;

NUMBER: [-]? [0-9] [0-9_]* ('.' [0-9_]*)?;
STRING: ('"' ~'"'* '"') | ('\'' ~'\''* '\'');
NULL: 'null' | 'none';
BOOL_OP: 'and' | 'or' | 'xor';
MUL_OP: '*' | '/';
ADD_OP: '+' | '-';
REM_OP: '%';
CMP_OP: '==' | '!=' | '>' | '<' | '<=' | '>=';
WHILE: 'while' | 'until';
TYPE: 'let' | 'global';
ARRAY_IDENTIFIER: '[' ']';
WS: [ \t\r\n]+ -> skip;
SINGLE_COMMENT: ('#' [.]* '\n') -> skip;
IDENTIFIER: [a-zA-Z_][a-zA-Z0-9_]*;