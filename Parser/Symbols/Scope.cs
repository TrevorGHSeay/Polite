using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polite
{

    /// <summary>
    /// The <see cref="Token"/> that a <see cref="Parser"/> uses to organize scoping mechanisms.
    /// </summary>
    public class Scope : Token
    {
        /// <summary>
        /// This <see cref="Scope"/>'s parent <see cref="Scope"/>. If the <see cref="Scope"/> is global, this will be null.
        /// </summary>
        public Scope Parent;

        /// <summary>
        /// The <see cref="TokenTable"/> containing every <see cref="Token"/> in the <see cref="Scope"/>
        /// </summary>
        public TokenTable Definitions;

        /// <summary>
        /// The <see cref="Scope"/>'s identifier. Used by the <see cref="Parser"/>.
        /// </summary>
        public int ID { get; internal set; }

        internal Scope(int lbp, string value, int ID) : base(Token.Types.Primary.Scope, Token.Types.Undefined, value, lbp)
        { this.Definitions = new TokenTable(); }

        /// <summary>
        /// Defines a <see cref="Token"/> found within the scope and saves it to the <see cref="Definitions"/> property.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Token Define(Token token)
        {
            token.Nud = Itself;
            token.Reserved = false;
            this.Definitions[token.Name] = token;

            if (token.PrimaryType == Token.Types.Primary.Statement)
                token.Stad = null;

            token.LBP = 0;
            token.Scope = this;
            return token;
        }

        /// <summary>
        /// Finds the location of a <see cref="Token"/> in the <see cref="Scope"/> hierarchy by traversing <see cref="Parent"/>s.
        /// </summary>
        /// <param name="token">The <see cref="Token"/> to find.</param>
        /// <param name="parser">The <see cref="Parser"/> to use as reference.</param>
        /// <returns>The existing <see cref="Token"/>, or a new <see cref="Token.Types.Primary.Identifier"/> <see cref="Token"/>.</returns>
        public Token Find(Token token, Parser parser)
        {
            //System.Diagnostics.Debug.WriteLine("Looking for token '" + token.Value + "'");

            Scope e = this;
            Token o;
            while (true)
            {
                if (e.Definitions.Contains(token.Name))
                {
                    o = e.Definitions[token.Name];
                    if (o.PrimaryType != Token.Types.Primary.Function)
                        return o;
                }
                e = e.Parent;
                if (e == null)
                {
                    if (parser.Lexer.Lexicon.TokenTable.Contains(token.Name) && token.PrimaryType != Token.Types.Primary.Function)
                        return token;
                    else
                        return parser.Lexer.Lexicon.TokenTable[Token.Types.Primary.ToPUA(Token.Types.Primary.Identifier)];
                }
            }
        }

        /// <summary>
        /// Reserves a <see cref="Token"/>'s name within the scope.
        /// </summary>
        /// <param name="token">The <see cref="Token"/> to reserve.</param>
        public void Reserve(Token token)
        {
            if (token.Arity != Token.Types.Primary.Identifier || token.Reserved)
                return;

            Token t = this.Definitions.Contains(token.Name) ? this.Definitions[token.Name] : null;

            if (t != null)
            {
                if (t.Reserved)
                    return;
                if (t.Arity == Token.Types.Primary.Identifier)
                    token.ThrowException("Already defined.");
            }
            this.Definitions[token.Name] = token;
            token.Reserved = true;
        }

        /// <summary>
        /// Does this <see cref="Scope"/> match identifiers with <see cref="Scope"/> 'obj'?
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var scope = obj as Scope;
            return !(scope is null) && this.ID == scope.ID;
        }

        public override int GetHashCode()
        { return 1213502048 + ID.GetHashCode(); }

        public static bool operator ==(Scope scope1, Scope scope2)
        {
            bool oneIsNull = scope1 is null;
            bool twoIsNull = scope2 is null;

            if (oneIsNull && twoIsNull)
                return true;
            else if (!oneIsNull && !twoIsNull)
                return scope1.ID == scope2.ID;
            else
                return false;
                
        }

        public static bool operator !=(Scope scope1, Scope scope2)
        { return !(scope1 == scope2); }

    }
}
