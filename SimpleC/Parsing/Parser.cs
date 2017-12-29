using SimpleC.Types;
using SimpleC.Types.AstNodes;
using SimpleC.Types.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleC.Parsing
{
    /// <summary>
    /// Parser for the SimpleC language.
    /// </summary>
    class Parser
    {
        public Token[] Tokens { get; private set; }

        private int readingPosition;
        private Stack<StatementSequenceNode> scopes;

        private static readonly KeywordType[] typeKeywords = { KeywordType.Int, KeywordType.Void };

        public Parser(Token[] tokens)
        {
            this.Tokens = tokens;

            readingPosition = 0;
            scopes = new Stack<StatementSequenceNode>();
        }

        public ProgramNode ParseToAst()
        {
            scopes.Push(new ProgramNode());

            while (!Eof())
            {
                if (Peek() is KeywordToken)
                {
                    var keyword = (KeywordToken)Next();

                    if (scopes.Count == 1) //we are a top level, the only valid keywords are variable types, starting a variable or function definition
                    {
                        if (keyword.IsTypeKeyword)
                        {
                            var varType = keyword.ToVariableType();
                            //it must be followed by a identifier:
                            var name = ReadToken<IdentifierToken>();
                            //so see what it is (function or variable):
                            Token lookahead = Peek();
                            if (lookahead is OperatorToken && (((OperatorToken)lookahead).OperatorType == OperatorType.Assignment) || lookahead is StatementSperatorToken) //variable declaration
                            {
                                if (lookahead is OperatorToken)
                                    Next(); //skip the "="
                                scopes.Peek().AddStatement(new VariableDeclarationNode(varType, name.Content, ExpressionNode.CreateFromTokens(ReadUntilStatementSeperator())));
                            }
                            else if (lookahead is OpenBraceToken && (((OpenBraceToken)lookahead).BraceType == BraceType.Round)) //function definition
                            {
                                var func = new FunctionDeclarationNode(name.Content);
                                scopes.Peek().AddStatement(func); //add the function to the old (root) scope...
                                scopes.Push(func); //...and set it a the new scope!
                                //Read the argument list
                                Next(); //skip the opening brace
                                while (!(Peek() is CloseBraceToken && ((CloseBraceToken)Peek()).BraceType == BraceType.Round)) //TODO: Refactor using readUntilClosingBrace?
                                {
                                    var argType = ReadToken<KeywordToken>();
                                    if (!argType.IsTypeKeyword)
                                        throw new ParsingException("Expected type keyword!");
                                    var argName = ReadToken<IdentifierToken>();
                                    func.AddParameter(new ParameterDeclarationNode(argType.ToVariableType(), argName.Content));
                                    if (Peek() is ArgSeperatorToken) //TODO: Does this allow (int a int b)-style functions? (No arg-seperator!)
                                        Next(); //skip the sperator
                                }
                                Next(); //skip the closing brace
                                var curlyBrace = ReadToken<OpenBraceToken>();
                                if (curlyBrace.BraceType != BraceType.Curly)
                                    throw new ParsingException("Wrong brace type found!");
                            }
                            else
                                throw new Exception("The parser encountered an unexpected token.");
                        }
                        else
                            throw new ParsingException("Found non-type keyword on top level.");
                    }
                    else //we are in a nested scope
                    {
                        //TODO: Can we avoid the code duplication from above?
                        if (keyword.IsTypeKeyword) //local variable declaration!
                        {
                            var varType = keyword.ToVariableType();
                            //it must be followed by a identifier:
                            var name = ReadToken<IdentifierToken>();
                            //so see what it is (function or variable):
                            Token lookahead = Peek();
                            if (lookahead is OperatorToken && (((OperatorToken)lookahead).OperatorType == OperatorType.Assignment) || lookahead is StatementSperatorToken) //variable declaration
                            {
                                if (lookahead is OperatorToken)
                                    Next(); //skip the "="
                                scopes.Peek().AddStatement(new VariableDeclarationNode(varType, name.Content, ExpressionNode.CreateFromTokens(ReadUntilStatementSeperator())));
                            }
                        }
                        else
                        {
                            switch (keyword.KeywordType)
                            {
                                case KeywordType.Return:
                                    scopes.Peek().AddStatement(new ReturnStatementNode(ExpressionNode.CreateFromTokens(ReadUntilStatementSeperator())));
                                    break;
                                case KeywordType.If:
                                    var @if = new IfStatementNode(ExpressionNode.CreateFromTokens(ReadUntilClosingBrace()));
                                    scopes.Peek().AddStatement(@if);
                                    scopes.Push(@if);
                                    break;
                                case KeywordType.While:
                                    var @while = new WhileLoopNode(ExpressionNode.CreateFromTokens(ReadUntilClosingBrace()));
                                    scopes.Peek().AddStatement(@while);
                                    scopes.Push(@while);
                                    break;
                                default:
                                    throw new ParsingException("Unexpected keyword type.");
                            }
                        }
                    }
                }
                else if(Peek() is IdentifierToken && scopes.Count > 1) //in nested scope
                {
                    var name = ReadToken<IdentifierToken>();
                    if(Peek() is OperatorToken && ((OperatorToken)Peek()).OperatorType == OperatorType.Assignment) //variable assignment
                    {
                        Next(); //skip the "="
                        scopes.Peek().AddStatement(new VariableAssignmentNode(name.Content, ExpressionNode.CreateFromTokens(ReadUntilStatementSeperator())));
                    }
                    else //lone expression (incl. function calls!)
                        scopes.Peek().AddStatement(ExpressionNode.CreateFromTokens(new[]{name}.Concat(ReadUntilStatementSeperator()))); //don't forget the name here!
                }
                else if(Peek() is CloseBraceToken)
                {
                    var brace = ReadToken<CloseBraceToken>();
                    if (brace.BraceType != BraceType.Curly)
                        throw new ParsingException("Wrong brace type found!");
                    scopes.Pop(); //Scope has been closed!
                }
                else
                    throw new ParsingException("The parser ran into an unexpeted token.");
            }

            if (scopes.Count != 1)
                throw new ParsingException("The scopes are not correctly nested.");

            return (ProgramNode)scopes.Pop();
        }

        private IEnumerable<Token> ReadTokenSeqence(params Type[] expectedTypes)
        {
            foreach (var t in expectedTypes)
            {
                if (!t.IsAssignableFrom(Peek().GetType()))
                    throw new ParsingException("Unexpected token");
                yield return Next();
            }
        }

        private IEnumerable<Token> ReadUntilClosingBrace()
        {
            //TODO: Only allow round braces, handle nested braces!
            while (!Eof() && !(Peek() is CloseBraceToken))
                yield return Next();
            Next(); //skip the closing brace
        }

        private IEnumerable<Token> ReadUntilStatementSeperator()
        {
            while (!Eof() && !(Peek() is StatementSperatorToken))
                yield return Next();
            Next(); //skip the semicolon
        }

        private TExpected ReadToken<TExpected>() where TExpected : Token
        {
            if (Peek() is TExpected)
                return (TExpected)Next();
            else
                throw new ParsingException("Unexpected token " + Peek());
        }

        [DebuggerStepThrough]
        private Token Peek()
        {
            //TODO: Check for eof()
            return Tokens[readingPosition];
        }

        [DebuggerStepThrough]
        private Token Next()
        {
            var ret = Peek();
            readingPosition++;
            return ret;
        }

        [DebuggerStepThrough]
        private bool Eof()
        {
            return readingPosition >= Tokens.Length;
        }
    }
}
