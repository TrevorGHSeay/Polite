using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polite
{

    /// <summary>
    /// Contains every default <see cref="Lexicon"/> included in the <see cref="Polite"/> framework.
    /// </summary>
    public static class Lexicons
    {

        /// <summary>
        /// The default <see cref="Lexicon"/> that <see cref="Polite"/> uses to process JavaScript source code.
        /// </summary>
        public class JavaScript : Lexicon
        {

            /// <summary>
            /// Creates a new instance of <see cref="Polite"/>'s JavaScript <see cref="Lexicon"/>.
            /// </summary>
            public JavaScript() : base(DefaultTokens)
            { }

            /// <summary>
            /// A 'short'-hand container for an array that contains the <see cref="Token.Types"/> for identifiers, literals, and ignored delimiters.
            /// </summary>
            public static short[] IdentifierLiteralOrIgnoredDelimiter { get => new short[] { Token.Types.Primary.Identifier, Token.Types.Primary.Literal, Token.Types.Primary.IgnoredDelimiter }; }

            /// <summary>
            /// A 'short'-hand container for an array that contains the <see cref="Token.Types"/> for operators, endlines, and ignored delimiters.
            /// </summary>
            public static short[] OperatorEndlineOrIgnoredDelimiter { get => new short[] { Token.Types.Primary.Operator, Token.Types.Primary.EndLine, Token.Types.Primary.IgnoredDelimiter }; }

            /// <summary>
            /// The master <see cref="TokenTable"/> that contains every default <see cref="Token"/> used within the <see cref="JavaScriptRuntime"/> framework.
            /// </summary>
            public static TokenTable DefaultTokens
            {
                get
                {
                    return new TokenTable()
                    {

                        new Token(Token.Types.Primary.Statement, Token.Types.Secondary.Variable.Declaration, 0, "var", null, null, Grammars.JavaScript.VariableDeclarationStad, new short[]{ Token.Types.Primary.IgnoredDelimiter }),
                        new Token(Token.Types.Primary.Statement, Token.Types.Secondary.Variable.Constructor, 80, "new", Grammars.JavaScript.ConstructorNud, null, null, new short[] { Token.Types.Primary.IgnoredDelimiter }),

                        new Token(Token.Types.Primary.Identifier, Token.Types.Undefined, 0, Token.Types.Primary.ToPUA(Token.Types.Primary.Identifier).ToString(), Token.Itself, null, null, JavaScript.OperatorEndlineOrIgnoredDelimiter),
                        new Token(Token.Types.Primary.Operator, Token.Types.Secondary.Variable.Addresser, 80, ".", null, CommonGrammar.AddresserLed, null, new short[] { Token.Types.Primary.Identifier }),
                        new Token(Token.Types.Primary.Literal, Token.Types.Undefined, 0, Token.Types.Primary.ToPUA(Token.Types.Primary.Literal).ToString(), Token.Itself, null, null, JavaScript.OperatorEndlineOrIgnoredDelimiter),

                        new Token(Token.Types.Primary.Operator, Token.Types.Secondary.Variable.Indexer, 80, "[", Grammars.JavaScript.IndexerNud, Grammars.JavaScript.IndexerLed, null),

                        new Token(Token.Types.Primary.Operator, Token.Types.Secondary.Operations.Addition, 60, "+", null, CommonGrammar.InfixLed, null, JavaScript.IdentifierLiteralOrIgnoredDelimiter),
                        new Token(Token.Types.Primary.Operator, Token.Types.Secondary.Operations.Subtraction, 60, "-", null, CommonGrammar.InfixLed, null, JavaScript.IdentifierLiteralOrIgnoredDelimiter),
                        new Token(Token.Types.Primary.Operator, Token.Types.Secondary.Operations.Multiplication, 70, "*", null, CommonGrammar.InfixLed, null, JavaScript.IdentifierLiteralOrIgnoredDelimiter),
                        new Token(Token.Types.Primary.Operator, Token.Types.Secondary.Operations.Division, 70, "/", null, CommonGrammar.InfixLed, null, JavaScript.IdentifierLiteralOrIgnoredDelimiter),
                        
                        new Token(Token.Types.Primary.Operator, Token.Types.Secondary.Assignments.Basic, 10, "=", null, Grammars.JavaScript.AssignmentLed, null, JavaScript.IdentifierLiteralOrIgnoredDelimiter),
                        new Token(Token.Types.Primary.Operator, Token.Types.Secondary.Assignments.Addition, 10, "+=", null, Grammars.JavaScript.AssignmentLed, null, JavaScript.IdentifierLiteralOrIgnoredDelimiter),
                        new Token(Token.Types.Primary.Operator, Token.Types.Secondary.Assignments.Subtraction, 10, "-=", null, Grammars.JavaScript.AssignmentLed, null, JavaScript.IdentifierLiteralOrIgnoredDelimiter),
                        new Token(Token.Types.Primary.Operator, Token.Types.Secondary.Assignments.Multiplication, 10, "*=", null, Grammars.JavaScript.AssignmentLed, null, JavaScript.IdentifierLiteralOrIgnoredDelimiter),
                        new Token(Token.Types.Primary.Operator, Token.Types.Secondary.Assignments.Division, 10, "/=", null, Grammars.JavaScript.AssignmentLed, null, JavaScript.IdentifierLiteralOrIgnoredDelimiter),

                        new Token(Token.Types.Primary.Operator, Token.Types.Secondary.Comparers.EqualTo, 40, "===", null, CommonGrammar.ReducedInfixLed, null, JavaScript.IdentifierLiteralOrIgnoredDelimiter),
                        new Token(Token.Types.Primary.Operator, Token.Types.Secondary.Comparers.UnequalTo, 40, "!==", null, CommonGrammar.ReducedInfixLed, null, JavaScript.IdentifierLiteralOrIgnoredDelimiter),
                        new Token(Token.Types.Primary.Operator, Token.Types.Secondary.Comparers.LessThan, 40, "<", null, CommonGrammar.ReducedInfixLed, null, JavaScript.IdentifierLiteralOrIgnoredDelimiter),
                        new Token(Token.Types.Primary.Operator, Token.Types.Secondary.Comparers.LessThanOrEqualTo, 40, "<=", null, CommonGrammar.ReducedInfixLed, null, JavaScript.IdentifierLiteralOrIgnoredDelimiter),
                        new Token(Token.Types.Primary.Operator, Token.Types.Secondary.Comparers.GreaterThan, 40, ">", null, CommonGrammar.ReducedInfixLed, null, JavaScript.IdentifierLiteralOrIgnoredDelimiter),
                        new Token(Token.Types.Primary.Operator, Token.Types.Secondary.Comparers.GreaterThanOrEqualTo, 40, ">=", null, CommonGrammar.ReducedInfixLed, null, JavaScript.IdentifierLiteralOrIgnoredDelimiter),

                        new Token(Token.Types.Primary.Operator, Token.Types.Secondary.Conditionals.And, 30, "&&", null, CommonGrammar.ReducedInfixLed, null, JavaScript.IdentifierLiteralOrIgnoredDelimiter),
                        new Token(Token.Types.Primary.Operator, Token.Types.Secondary.Conditionals.Or, 30, "||", null, CommonGrammar.ReducedInfixLed, null, JavaScript.IdentifierLiteralOrIgnoredDelimiter),
                        
                        new Token(Token.Types.Primary.Operator, Token.Types.Secondary.Function.Invoker, 80, "(", null, Grammars.JavaScript.OpenParenLed, null),
                        new Token(Token.Types.Primary.Statement, Token.Types.Undefined, 0, "{", null, null, Grammars.JavaScript.OpenCurlyStad, null),

                        new Token(Token.Types.Primary.EndLine, Token.Types.Undefined, 0, ";", null, null, null, null),
                        new Token(Token.Types.Primary.Operator, Token.Types.Undefined, 0, ":", null, null, null, null),
                        new Token(Token.Types.Primary.Operator, Token.Types.Undefined, 0, ")", null, null, null, null),
                        new Token(Token.Types.Primary.Operator, Token.Types.Undefined, 0, "]", null, null, null, null),
                        new Token(Token.Types.Primary.Operator, Token.Types.Undefined, 0, "}", null, null, null, null),
                        new Token(Token.Types.Primary.Operator, Token.Types.Undefined, 0, ",", null, null, null, null),

                        new Token(Token.Types.Primary.Statement, Token.Types.Secondary.Variable.Referencer, 0, "import from", null, null, Grammars.JavaScript.ReferenceStad),
                        new Token(Token.Types.Primary.Function, Token.Types.Undefined, 0, "function", Grammars.JavaScript.FunctionNud, null, null),
                        new Token(Token.Types.Primary.Statement, Token.Types.Secondary.Orders.Condition, 0, "if", null, null, Grammars.JavaScript.IfStad),
                        new Token(Token.Types.Primary.Operator, Token.Types.Undefined, 0, "else", null, null, null, null),
                        new Token(Token.Types.Primary.Statement, Token.Types.Secondary.Orders.Loop, 0, "while", null, null, Grammars.JavaScript.WhileStad),

                        new Token(Token.Types.Primary.Constant, Token.Types.Secondary.Variable.ParentReference, 0, "this", Grammars.JavaScript.ThisNud, null, null, null),
                        new Token(Token.Types.Primary.Statement, Token.Types.Secondary.Function.Result, 0, "return", null, null, Grammars.JavaScript.ReturnStad, null),

                        new Token(Token.Types.Primary.IgnoredDelimiter, Token.Types.Undefined, 0, Environment.NewLine, null, null, null, null),
                        new Token(Token.Types.Primary.IgnoredDelimiter, Token.Types.Undefined, 0, "\n", null, null, null, null),
                        new Token(Token.Types.Primary.IgnoredDelimiter, Token.Types.Undefined, 0, "\t", null, null, null, null),
                        new Token(Token.Types.Primary.IgnoredDelimiter, Token.Types.Undefined, 0, " ", null, null, null, null),

                        new Token(Token.Types.Primary.EndLine, Token.Types.Undefined, 0, Token.Types.Primary.ToPUA(Token.Types.Primary.EndLine).ToString(), null, null, null, null),
                        new Token(Token.Types.Primary.End, Token.Types.Undefined, 0, Token.Types.Primary.ToPUA(Token.Types.Primary.End).ToString(), null, null, null, null),
                    };
                }
            }
        }

    }
}
