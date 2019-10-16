using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polite
{

    public partial class Token : TokenBase
    {
        /// <summary>
        /// The start index of this <see cref="Token"/> within source.
        /// </summary>
        public int Start;

        /// <summary>
        /// The end index of this <see cref="Token"/> within source.
        /// </summary>
        public int End;

        /// <summary>
        /// The <see cref="Token.Types.Primary"/> type of this <see cref="Token"/>.
        /// </summary>
        public short PrimaryType;

        /// <summary>
        /// The <see cref="Token"/>'s literal representation in source.
        /// </summary>
        public string Name;

        /// <summary>
        /// The value from all possible <see cref="Token.Arities"/> that best represents this <see cref="Token"/>.
        /// </summary>
        public short Arity;

        /// <summary>
        /// The <see cref="Token.Types.Secondary"/> type of this <see cref="Token"/>.
        /// </summary>
        public short SecondaryType;

        private Token[] Members;

        /// <summary>
        /// The <see cref="First"/> member.
        /// </summary>
        public Token First { get => Members[0]; set => Members[0] = value; }
        /// <summary>
        /// The <see cref="Second"/> member.
        /// </summary>
        public Token Second { get => Members[1]; set => Members[1] = value; }
        /// <summary>
        /// The <see cref="Third"/> member.
        /// </summary>
        public Token Third { get => Members[2]; set => Members[2] = value; }

        /// <summary>
        /// Does the <see cref="Token"/> have children?
        /// </summary>
        public bool HasChildren => Children.Count != 0;

        /// <summary>
        /// Does the <see cref="Token"/> have members?
        /// </summary>
        public bool HasMembers => Members[0] != null || Members[1] != null || Members[2] != null;

        /// <summary>
        /// The children nodes/leaves of this <see cref="Token"/>.
        /// </summary>
        public List<Token> Children;

        /// <summary>
        /// Has this token been reserved by a <see cref="Polite.Scope"/>?
        /// </summary>
        public bool Reserved = false;

        /// <summary>
        /// The <see cref="Polite.Scope"/> that this <see cref="Token"/> belongs to; <see cref="null"/> if not applicable or yet unprocessed.
        /// </summary>
        public Scope Scope;

        /// <summary>
        /// The <see cref="object"/> value that this <see cref="Token"/> represents at the point of <see cref="Runtime"/> initialization. Used only in the case of a <see cref="Token.Types.Primary.Literal"/>
        /// </summary>
        public object Value;

        /// <summary>
        /// A collection of known <see cref="Token.Types.Primary"/> types that may follow this <see cref="Token"/> syntactically.
        /// <para>If populated, this and every illegally trailing <see cref="Token"/> (until found in <see cref="Next"/>) will be emalgamated into one (during the lexing phase) and passed to the <see cref="Parser"/> as a single identifier, so that keywords may be included inside variable names.</para>
        /// For example, in <see cref="Lexicons.JavaScript"/>, "var vars = 2;" processes the phrase "vars" as an identifier, since "var" only contains whitespace as a <see cref="Token.Types.Primary.IgnoredDelimiter"/> in it's <see cref="Next"/> collection; the <see cref="Lexer"/> then knows you mean "vars", and not a consecutive "var" and "s" (since "var" would, in the second case, be missing its necessary whitespace).
        /// </summary>
        public short[] Next;

        /// <summary>
        /// Creates a new instance of <see cref="Token"/>.
        /// </summary>
        /// <param name="primary_type">The <see cref="Token.Types.Primary"/> type that the new <see cref="Token"/> will have.</param>
        /// <param name="lbp">The left-binding-power used in the creation of <see cref="Token"/> trees.</param>
        /// <param name="name">The name of the new <see cref="Token"/>.</param>
        public Token(short primary_type, int lbp, string name) :
            this(primary_type, Token.Types.Undefined, lbp, name, null, null, null, Array.Empty<short>())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="Token"/>.
        /// </summary>
        /// <param name="primary_type">The <see cref="Token.Types.Primary"/> type that the new <see cref="Token"/> will have.</param>
        /// <param name="secondary_type">The <see cref="Token.Types.Secondary"/> type that the new <see cref="Token"/> will have.</param>
        /// <param name="name">The name of the new <see cref="Token"/>.</param>
        /// <param name="lbp">The left-binding-power used in the creation of <see cref="Token"/> trees.</param>
        public Token(short primary_type, short secondary_type, string name, int lbp) :
            this(primary_type, secondary_type, lbp, name, null, null, null, Array.Empty<short>())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="Token"/>.
        /// </summary>
        /// <param name="primary_type">The <see cref="Token.Types.Primary"/> type that the new <see cref="Token"/> will have.</param>
        /// <param name="secondary_type">The <see cref="Token.Types.Secondary"/> type that the new <see cref="Token"/> will have.</param>
        /// <param name="lbp">The left-binding-power used in the creation of <see cref="Token"/> trees.</param>
        /// <param name="name">The name of the new <see cref="Token"/>.</param>
        /// <param name="nud">The <see cref="NullDenotion"/> that instructs the <see cref="Parser"/> on how to handle the <see cref="Token"/>.</param>
        /// <param name="led">The <see cref="LeftDenotation"/> that instructs the <see cref="Parser"/> on how to handle the <see cref="Token"/>.</param>
        /// <param name="stad">The <see cref="StatementDenotation"/> that instructs the <see cref="Parser"/> on how to handle the <see cref="Token"/>.</param>
        /// <param name="op">The <see cref="Operation"/> that instructs the <see cref="Runtime"/> on how to handle the <see cref="Token"/>.</param>
        public Token(short primary_type, short secondary_type, int lbp, string name, NullDenotion nud, LeftDenotation led, StatementDenotation stad) :
            this(primary_type, secondary_type, lbp, name, nud, led, stad, Array.Empty<short>())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="Token"/>.
        /// </summary>
        /// <param name="primary_type">The <see cref="Token.Types.Primary"/> type that the new <see cref="Token"/> will have.</param>
        /// <param name="secondary_type">The <see cref="Token.Types.Secondary"/> type that the new <see cref="Token"/> will have.</param>
        /// <param name="lbp">The left-binding-power used in the creation of <see cref="Token"/> trees.</param>
        /// <param name="name">The name of the new <see cref="Token"/>.</param>
        /// <param name="nud">The <see cref="NullDenotion"/> that instructs the <see cref="Parser"/> on how to handle the <see cref="Token"/>.</param>
        /// <param name="led">The <see cref="LeftDenotation"/> that instructs the <see cref="Parser"/> on how to handle the <see cref="Token"/>.</param>
        /// <param name="stad">The <see cref="StatementDenotation"/> that instructs the <see cref="Parser"/> on how to handle the <see cref="Token"/>.</param>
        /// <param name="op">The <see cref="Operation"/> that instructs the <see cref="Runtime"/> on how to handle the <see cref="Token"/>.</param>
        /// <param name="next">The <see cref="Token.Types"/> permitted to follow this <see cref="Token"/> syntactically. Used in the tokenizing process.</param>
        public Token(short primary_type, short secondary_type, int lbp, string name, NullDenotion nud, LeftDenotation led, StatementDenotation stad, short[] next) : base(lbp)
        {
            this.PrimaryType = primary_type;
            this.SecondaryType = secondary_type;
            this.Name = name;
            this.Next = next is null ? Array.Empty<short>() : next;

            if (nud != null) this.Nud = nud;
            if (led != null) this.Led = led;
            if (stad != null) this.Stad = stad;

            this.Members = new Token[3];
            this.Children = new List<Token>();
        }

        /// <summary>
        /// The <see cref="NullDenotion"/> that refers to itself only.
        /// </summary>
        /// <param name="token">The return value.</param>
        /// <param name="parser">The current <see cref="Parser"/></param>
        /// <returns>Returns 'token'.</returns>
        public static Token Itself(Token token, Parser parser)
        { return token; }

        /// <summary>
        /// Creates and returns a deep-copy. Does not copy children - do this manually.
        /// </summary>
        /// <returns>A deep copy, wth an empty collection of <see cref="Children"/>.</returns>
        public virtual Token Clone()
        {
            var clone = new Token(this.PrimaryType, this.SecondaryType, this.LBP, this.Name, this.Nud, this.Led, this.Stad);

            clone.Scope = this.Scope;

            clone.Value = this.Value;

            clone.Start = this.Start;
            clone.End = this.End;

            clone.Arity = this.Arity;
            clone.Reserved = this.Reserved;
            clone.SecondaryType = this.SecondaryType;

            clone.Next = this.Next.Clone() as short[];

            if (this.First != null)
                clone.First = First.Clone();
            if (this.Second != null)
                clone.Second = Second.Clone();
            if (this.Third != null)
                clone.Third = Third.Clone();

            return clone;
        }

        /// <summary>
        /// Does the primary_type come before this <see cref="Token"/> (is it found in its <see cref="Next"/> field)?
        /// </summary>
        /// <param name="primary_type">The <see cref="Token.Types.Primary"/> type of <see cref="Token"/> to compare.</param>
        /// <returns>Returns <see cref="true"/> if it may come before primary_type.</returns>
        public bool Preceeds(short primary_type)
        {
            if (Next.Length == 0)
                return true;

            for (int i = 0; i < Next.Length; i += 1)
                if (Next[i] == primary_type)
                    return true;

            return false;
        }

        /// <summary>
        /// Is the <see cref="Token"/>'s <see cref="Token.Types.Primary"/> type a match with 'type'?
        /// </summary>
        /// <param name="token">The <see cref="Token"/> to analyze.</param>
        /// <param name="type">The <see cref="Token.Types.Primary"/> type to compare.</param>
        /// <returns>True if <see cref="Token"/> is of type.</returns>
        public static bool IsTypeOf(Token token, short type)
        { return token.PrimaryType == type; }

        public override string ToString()
        {
            return "Name: '" + this.Name + "', Type: " + this.PrimaryType + ", Start: " + this.Start + ", End: " + this.End;
        }

        internal static string PrintAddress(Token token, string seperator)
        {
            if (token.SecondaryType != Token.Types.Secondary.Variable.Addresser)
                return token.Name;

            return new StringBuilder(Token.PrintAddress(token.First, seperator)).Append(seperator).Append(Token.PrintAddress(token.Second, seperator)).ToString();
        }

    }

}
