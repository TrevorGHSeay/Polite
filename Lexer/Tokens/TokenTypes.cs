using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Polite
{

    public partial class Token
    {
        /// <summary>
        /// All <see cref="Token"/> types available to a custom <see cref="Lexer"/>, <see cref="Parser"/>, or <see cref="Runtime"/>.
        /// </summary>
        public static class Types
        {

            public const short Undefined = -1;

            public static class Primary
            {

                public const short Ignored = -1;
                public const short IgnoredDelimiter = -2;

                public const short Identifier = 4;
                public const short Operator = 5;
                public const short Literal = 6;
                public const short Function = 7;
                public const short Scope = 8;
                public const short Block = 9;
                public const short Statement = 10;
                public const short Constant = 11;
                public const short EndLine = 12;
                public const short End = 13;

                public static char ToPUA(short type)
                {
                    switch (type)
                    {
                        case Undefined: return '\uE100';
                        case Identifier: return '\uE101';
                        case Operator: return '\uE102';
                        case Literal: return '\uE103';
                        case Function: return '\uE104';
                        case Scope: return '\uE105';
                        case Block: return '\uE106';
                        case Statement: return '\uE107';
                        case Constant: return '\uE108';
                        case EndLine: return '\uE109';
                        case End: return '\uE110';
                        default: throw new Exception("Unfound type.");
                    }
                }

            }

            public static class Secondary
            {

                public static class Assignments
                {
                    public const short Basic = 14;
                    public const short Addition = 15;
                    public const short Subtraction = 16;
                    public const short Multiplication = 17;
                    public const short Division = 18;
                }

                public static class Operations
                {
                    public const short Addition = 19;
                    public const short Subtraction = 20;
                    public const short Multiplication = 21;
                    public const short Division = 22;
                }

                public static class Conditionals
                {
                    public const short Or = 23;
                    public const short And = 24;
                }

                public static class Comparers
                {
                    public const short EqualTo = 25;
                    public const short UnequalTo = 26;
                    public const short LessThan = 27;
                    public const short LessThanOrEqualTo = 28;
                    public const short GreaterThan = 29;
                    public const short GreaterThanOrEqualTo = 30;
                }

                public static class Bitwise
                {
                    public const short Or = 31;
                    public const short And = 32;
                    public const short XOr = 33;
                    public const short Complement = 34;
                    public const short ShiftLeft = 35;
                    public const short ShiftRight = 36;
                }

                public static class Orders
                {
                    public const short Condition = 37;
                    public const short Loop = 38;
                }

                public static class Literals
                {
                    public const short Integer = 39;
                    public const short Boolean = 40;
                    public const short Double = 41;
                    public const short String = 42;
                    public const short Array = 43;
                }

                public static class Function
                {
                    public const short Invoker = 44;
                    internal const short Result = 45;
                }

                public static class Variable
                {
                    public const short Addresser = 46;
                    public const short Referencer = 47;
                    public const short Indexer = 48;
                    public const short Constructor = 49;
                    public const short Declaration = 50;

                    internal const short ParentReference = short.MaxValue - 1;
                }

                public static char ToPUA(short type)
                {
                    switch (type)
                    {
                        case Variable.Addresser: return '\uE111';
                        case Variable.ParentReference: return '\uE112';
                        case Function.Result: return '\uE113';
                        default: throw new Exception("Unfound type.");
                    }
                }

                public static bool IsArithmeticType(short type)
                { return type >= Operations.Addition && type <= Operations.Division; }
                public static bool IsArithmeticOperator(Token token)
                { return token.PrimaryType == Token.Types.Primary.Operator && IsArithmeticType(token.SecondaryType); }
                
                public static bool IsAssignmentType(short type)
                { return type >= Assignments.Basic && type <= Assignments.Division; }
                public static bool IsAssignmentOperator(Token token)
                { return token.PrimaryType == Token.Types.Primary.Operator && IsAssignmentType(token.SecondaryType); }

                public static bool IsComparisonType(short type)
                { return type >= Comparers.EqualTo && type <= Comparers.GreaterThanOrEqualTo; }
                public static bool IsComparisonOperator(Token token)
                { return token.PrimaryType == Token.Types.Primary.Operator && IsComparisonType(token.SecondaryType); }

                public static bool IsBitwiseType(short type)
                { return type >= Bitwise.Or && type <= Bitwise.ShiftRight; }
                public static bool IsBitwiseOperator(Token token)
                { return token.PrimaryType == Token.Types.Primary.Operator && IsBitwiseType(token.SecondaryType); }

                public static bool IsConditionType(short type)
                { return (type == Conditionals.Or || type == Conditionals.And); }
                public static bool IsConditionOperator(Token token)
                { return token.PrimaryType == Token.Types.Primary.Operator && IsConditionType(token.SecondaryType); }


            }

        }

    }

}
