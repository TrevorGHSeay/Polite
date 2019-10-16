using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polite
{

    /// <summary>
    /// The master class containing directives corresponding to every built-in language's <see cref="NullDenotion"/>, <see cref="LeftDenotation"/>, and <see cref="StatementDenotation"/>.
    /// </summary>
    public static class Grammars
    {

        /// <summary>
        /// The master class containing directives corresponding to <see cref="Polite"/>'s implementation of JavaScript's <see cref="NullDenotion"/>, <see cref="LeftDenotation"/>, and <see cref="StatementDenotation"/>.
        /// </summary>
        /// <remarks>Translated from Douglas Crockford's implementation of TDOP, originally in JavaScript.</remarks>
        public class JavaScript
        {

            /// <summary>
            /// Creates an expression for the 'var' statement.
            /// </summary>
            /// <param name="self">A 'var' <see cref="Token"/>.</param>
            /// <param name="parser">The current <see cref="Parser"/>.</param>
            /// <returns>The <see cref="Token"/> tree representing the variable's declaration expression.</returns>
            public static Token VariableDeclarationStad(Token self, Parser parser)
            {
                Token result = new Token(Token.Types.Undefined, Token.Types.Undefined, 0, string.Empty, null, null, null, null);
                Token lastToken;
                Token currentToken;

                while (true)
                {
                    lastToken = parser.CurrentToken;
                    if (lastToken.Arity != Token.Types.Primary.Identifier)
                        lastToken.ThrowException("Expected a new variable name.");
                    lastToken.SecondaryType = Token.Types.Secondary.Variable.Declaration;
                    parser.CurrentScope.Define(lastToken);
                    parser.Advance(string.Empty);

                    if (parser.CurrentToken.Name == "=")
                    {
                        currentToken = parser.CurrentToken;

                        parser.Advance("=");

                        currentToken.First = lastToken;
                        currentToken.Second = parser.Expression(0);
                        currentToken.Arity = Token.Arities.Binary;

                        result.Children.Add(currentToken);
                    }
                    else
                        result.Children.Add(lastToken);

                    if (parser.CurrentToken.Name != ",")
                        break;
                    parser.Advance(",");
                }

                parser.Advance(";");

                return result;

            }

            /// <summary>
            /// Creates a new statement using the <see cref="Parser.CurrentToken"/>.
            /// </summary>
            /// <param name="parser">The current <see cref="Parser"/> for reference.</param>
            /// <returns>The <see cref="Token"/> tree representing the new statement.</returns>
            public static Token StatementFunction(Parser parser)
            {
                Token n = parser.CurrentToken.Clone();

                Token v = new Token(Token.Types.Undefined, Token.Types.Undefined, 0, string.Empty, null, null, null, null);

                if (n.PrimaryType == Token.Types.Primary.Statement)
                {
                    parser.Advance(string.Empty);
                    parser.CurrentScope.Reserve(n);

                    return n.Stad(n, parser);
                }

                v.Children.Add(parser.Expression(0));

                if (!Token.Types.Secondary.IsAssignmentOperator(v.Children[0]) && v.Children[0].Name != "(")
                    v.Children[0].ThrowException("Bad expression statement.");

                parser.Advance(";");

                return v;
            }

            /// <summary>
            /// Creates a new block of statements using the <see cref="Parser.CurrentToken"/>.
            /// </summary>
            /// <param name="parser">The current <see cref="Parser"/> for reference.</param>
            /// <returns>The <see cref="Token"/> tree representing the new block.</returns>
            public static Token BlockFunction(Parser parser)
            {
                parser.Advance("{");
                Token result = JavaScript.StatementsFunction(parser);
                result.PrimaryType = Token.Types.Primary.Block;
                parser.Advance("}");
                return result;
            }

            /// <summary>
            /// Creates a new constant using the <see cref="Parser.CurrentToken"/>.
            /// </summary>
            /// <param name="parser">The current <see cref="Parser"/> for reference.</param>
            /// <returns>The <see cref="Token"/> tree representing the new constant.</returns>
            public static Token ConstantFunction(Token token, Parser parser)
            {
                parser.CurrentScope.Reserve(token);
                token.Name = parser.Lexer.Lexicon.TokenTable[token.Name].Name;
                token.Arity = Token.Types.Primary.Literal;
                return token;
            }

            /// <summary>
            /// Creates a new statement tree using the <see cref="Parser.CurrentToken"/>.
            /// </summary>
            /// <param name="parser">The current <see cref="Parser"/> for reference.</param>
            /// <returns>The <see cref="Token"/> tree representing the new statement tree.</returns>
            public static Token StatementsFunction(Parser parser)
            {
                Token result = new Token(Token.Types.Undefined, Token.Types.Undefined, 0, string.Empty, null, null, null, null);

                Token s;

                while (true)
                {
                    if (parser.CurrentToken.Name == "}" || parser.CurrentToken.PrimaryType == Token.Types.Primary.End)
                        break;

                    s = StatementFunction(parser);

                    if (s.HasChildren || s.HasMembers)
                        result.Children.Add(s);
                }
                return result;
            }

            /// <summary>
            /// Creates an assignment expression from the <see cref="Token.Types.Secondary.Assignments"/> <see cref="Token"/>.
            /// </summary>
            /// <param name="self">The <see cref="Token"/> that is an <see cref="Token.Types.Secondary.Assignments"/> <see cref="Token"/>.</param>
            /// <param name="binding_power">The left-binding-power used for ordering the <see cref="Token.Types.Primary.Operator"/> <see cref="Token"/>'s relationship with the <see cref="Token"/> to its left.</param>
            /// <param name="parser">The current <see cref="Parser"/>.</param>
            /// <returns>The <see cref="Token"/> tree representing the assignment's expression.</returns>
            public static Token AssignmentLed(Token token, Token left, int binding_power, Parser parser)
            {
                if (left.Name != "." && left.Name != "[" && left.Arity != Token.Types.Primary.Identifier)
                    left.ThrowException("Bad lvalue");

                token.First = left;
                token.Second = parser.Expression(9);

                token.Arity = Token.Arities.Binary;
                return token;
            }

            /// <summary>
            /// Creates an assignment expression from the open parenethesis <see cref="Token"/>.
            /// </summary>
            /// <param name="self">The <see cref="Token"/> that is an open parenthesis <see cref="Token"/>.</param>
            /// <param name="binding_power">The left-binding-power used for ordering the <see cref="Token.Types.Primary.Operator"/> <see cref="Token"/>'s relationship with the <see cref="Token"/> to its left.</param>
            /// <param name="parser">The current <see cref="Parser"/>.</param>
            /// <returns>The <see cref="Token"/> tree representing the open parenthesis' expression.</returns>
            public static Token OpenParenLed(Token token, Token left, int binding_power, Parser parser)
            {
                Token anonymousStatement = new Token(Token.Types.Undefined, Token.Types.Undefined, 0, string.Empty, null, null, null, null);

                token.First = left;
                token.Third = anonymousStatement;
                if (left.Name == "." || left.Name == "[")
                {
                    token.Arity = Token.Arities.Ternary;
                    token.Second = left.Second;
                }
                else
                {
                    token.Arity = Token.Arities.Binary;

                    if ((left.Arity != Token.Arities.Unary || left.PrimaryType != Token.Types.Primary.Function) &&
                        left.Arity != Token.Types.Primary.Identifier && left.Name != "(" &&
                        left.Name != "&&" && left.Name != "||" && left.Name != "?")
                        left.ThrowException("Expected a variable name.");
                }

                if (parser.CurrentToken.Name != ")")
                {
                    while (true)
                    {
                        anonymousStatement.Children.Add(parser.Expression(0));
                        if (parser.CurrentToken.Name != ",")
                            break;
                        parser.Advance(",");
                    }
                }
                parser.Advance(")");
                return token;
            }

            /// <summary>
            /// Creates a prefix expression from the open parenthesis <see cref="Token"/>.
            /// </summary>
            /// <param name="self">The <see cref="Token"/> that is a prefix.</param>
            /// <param name="parser">The current <see cref="Parser"/>.</param>
            /// <returns>The <see cref="Token"/> tree representing the open parentesis' expression.</returns>
            public static Token OpenParenNud(Token token, Token left, int binding_power, Parser parser)
            {
                Token e = parser.Expression(0);
                parser.Advance(")");
                return e;
            }

            /// <summary>
            /// Creates a statement expression from the 'if' <see cref="Token"/>.
            /// </summary>
            /// <param name="self">The <see cref="Token"/> that represents the 'if' statement.</param>
            /// <param name="parser">The current <see cref="Parser"/>.</param>
            /// <returns>The <see cref="Token"/> tree representing the 'if' expression.</returns>
            public static Token IfStad(Token self, Parser parser)
            {
                parser.Advance("(");
                self.First = parser.Expression(0);
                parser.Advance(")");
                
                self.Second = JavaScript.BlockFunction(parser);

                if (parser.CurrentToken.Name == "else")
                {
                    parser.CurrentScope.Reserve(parser.CurrentToken);
                    parser.Advance("else");
                    
                    if (parser.CurrentToken.Name == "if")
                        self.Third = JavaScript.StatementFunction(parser);
                    else
                        self.Third = JavaScript.BlockFunction(parser);
                }
                else
                    self.Third = null;
                self.Arity = Token.Types.Primary.Statement;

                return self;
            }

            /// <summary>
            /// Creates a statement expression from the 'while' <see cref="Token"/>.
            /// </summary>
            /// <param name="self">The <see cref="Token"/> that represents the 'while' statement.</param>
            /// <param name="parser">The current <see cref="Parser"/>.</param>
            /// <returns>The <see cref="Token"/> tree representing the 'while' expression.</returns>
            public static Token WhileStad(Token self, Parser parser)
            {
                parser.Advance("(");
                self.First = parser.Expression(0);
                parser.Advance(")");
                
                self.Second = JavaScript.BlockFunction(parser);

                self.Arity = Token.Types.Primary.Statement;
                return self;
            }

            /// <summary>
            /// Creates a statement expression from the '{' <see cref="Token"/>.
            /// </summary>
            /// <param name="self">The <see cref="Token"/> that represents the '{' statement.</param>
            /// <param name="parser">The current <see cref="Parser"/>.</param>
            /// <returns>The <see cref="Token"/> tree representing the '{' expression.</returns>
            public static Token OpenCurlyStad(Token self, Parser parser)
            {
                parser.NewScope();
                Token block = JavaScript.StatementsFunction(parser);

                List<Token> blockChildren = block.Children;
                for (int i = 0; i < block.Children.Count; i += 1)
                {
                    List<Token> grandChildren = blockChildren[i].Children;
                    List<Token> statementLine = grandChildren[i].Children;
                    self.Children.Add(new Token(Token.Types.Primary.Block, 0, string.Empty));
                    List<Token> memberAsList = self.Children[i].Children;
                    for (int j = 0; j < statementLine.Count; j += 1)
                    {
                        memberAsList.Add(statementLine[j]);
                    }
                }

                parser.PopScope();
                parser.Advance("}");
                return self;
            }

            /// <summary>
            /// Creates a prefix expression from the function declaration <see cref="Token"/>.
            /// </summary>
            /// <param name="self">The <see cref="Token"/> that is a function declaration.</param>
            /// <param name="parser">The current <see cref="Parser"/>.</param>
            /// <returns>The <see cref="Token"/> tree representing the function's expression.</returns>
            public static Token FunctionNud(Token self, Parser parser)
            {
                Token parameters = new Token(Token.Types.Primary.Block, 0, string.Empty);
                parser.NewScope();
                if (parser.CurrentToken.Arity == Token.Types.Primary.Identifier)
                {
                    parser.CurrentScope.Define(parser.CurrentToken);
                    self.Name = parser.CurrentToken.Name;
                    parser.Advance(string.Empty);
                }

                parser.Advance("(");
                if (parser.CurrentToken.Name != ")")
                {
                    while (true)
                    {
                        if (parser.CurrentToken.Arity != Token.Types.Primary.Identifier)
                        {
                            parser.CurrentToken.ThrowException("Expected a parameter name.");
                        }
                        parser.CurrentScope.Define(parser.CurrentToken);
                        parameters.Children.Add(parser.CurrentToken);

                        parser.Advance(string.Empty);
                        if (parser.CurrentToken.Name != ",")
                        {
                            break;
                        }
                        parser.Advance(",");
                    }
                }
                self.First = parameters;
                
                parser.Advance(")");
                parser.Advance("{");
                self.Second = JavaScript.StatementsFunction(parser);
                parser.Advance("}");
                self.Arity = Token.Types.Primary.Function;
                parser.PopScope();
                return self;
            }

            /// <summary>
            /// Creates a prefix expression from the 'function' <see cref="Token"/>.
            /// </summary>
            /// <param name="self">The <see cref="Token"/> that is a function.</param>
            /// <param name="parser">The current <see cref="Parser"/>.</param>
            /// <returns>The <see cref="Token"/> tree representing the function's expression.</returns>
            public static Token ThisNud(Token self, Parser parser)
            {
                parser.CurrentScope.Reserve(self);
                self.Arity = Token.Arities.Unary;
                return self;
            }

            /// <summary>
            /// Creates a statement expression from the 'return' <see cref="Token"/>.
            /// </summary>
            /// <param name="self">The <see cref="Token"/> that represents the 'return' statement.</param>
            /// <param name="parser">The current <see cref="Parser"/>.</param>
            /// <returns>The <see cref="Token"/> tree representing the 'return' expression.</returns>
            public static Token ReturnStad(Token self, Parser parser)
            {
                if (self.Name != ";")
                    self.First = parser.Expression(0);

                parser.Advance(";");
                if (parser.CurrentToken.Name != "}")
                    parser.CurrentToken.ThrowException("Unreachable code detected.");

                self.Arity = Token.Types.Primary.Statement;
                return self;
            }

            /// <summary>
            /// Creates a statement expression from the 'import' <see cref="Token"/>.
            /// </summary>
            /// <param name="self">The <see cref="Token"/> that represents the 'import' statement.</param>
            /// <param name="parser">The current <see cref="Parser"/>.</param>
            /// <returns>The <see cref="Token"/> tree representing the 'import' expression.</returns>
            public static Token ReferenceStad(Token self, Parser parser)
            {
                self.Second = parser.Expression(0);
                parser.Advance(";");

                return self;
            }

            public static Token IndexerNud(Token self, Parser parser)
            {
                List<Token> children = new List<Token>();

                while (true)
                {
                    children.Add(parser.Expression(0));
                    if (parser.CurrentToken.Name != ",")
                        break;
                    parser.Advance(",");
                }
                parser.Advance("]");

                self.First = new Token(Token.Types.Undefined, 0, string.Empty) { Children = children };
                self.Arity = Token.Arities.Unary;
                return self;
            }

            public static Token IndexerLed(Token token, Token left, int binding_power, Parser parser)
            {
                token.First = left;

                List<Token> children = new List<Token>();

                while (true)
                {
                    children.Add(parser.Expression(0));
                    if (parser.CurrentToken.Name != ",")
                        break;
                    parser.Advance(",");
                }
                parser.Advance("]");

                token.Second = new Token(Token.Types.Undefined, 0, string.Empty) { Children = children };

                token.Arity = Token.Arities.Binary;
                return token;
            }

            public static Token ConstructorNud(Token self, Parser parser)
            {
                self = parser.Expression(0);

                return self;

                /*
                parser.Advance(string.Empty);

                if (parser.CurrentToken.PrimaryType != Token.Types.Primary.Identifier)
                    parser.CurrentToken.ThrowException("Expected identifier, but got '" + parser.CurrentToken.Name + "' instead.");

                // TODO: Use Expression instead of factory-made single token as 'First' to prevent '->' from being prohibited in identifier names
                string constructorAddr = string.Empty;
                while (true)
                {
                    if (parser.CurrentToken.PrimaryType == Token.Types.Primary.Identifier)
                        constructorAddr += parser.CurrentToken.Name;
                    else if (parser.CurrentToken.SecondaryType == Token.Types.Secondary.Addresser)
                        constructorAddr += "->";
                    else
                        break;
                    parser.Advance(string.Empty);
                }

                self.First = new Token(Token.Types.Primary.Identifier, 0, constructorAddr);

                if (parser.CurrentToken.Name != "(" && parser.CurrentToken.Name != "[")
                    parser.CurrentToken.ThrowException("Expected encapsulator ('(', '['), but got '" + parser.CurrentToken.Name + "' instead.");

                string opener = parser.CurrentToken.Name;
                string closer = opener == "(" ? ")" : "]";

                parser.Advance(string.Empty);

                List <Token> children = new List<Token>();

                if (parser.CurrentToken.Name != closer)
                {
                    while (true)
                    {
                        children.Add(parser.Expression(0));
                        if (parser.CurrentToken.Name != ",")
                            break;
                        parser.Advance(",");
                    }
                }
                parser.Advance(closer);
                self.Second = new Token(Token.Types.Undefined, 0, string.Empty) { Children = children };
                self.Arity = Token.Arities.Binary;
                return self;
                */
            }

        }

    }

}
