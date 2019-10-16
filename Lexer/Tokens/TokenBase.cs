using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polite
{

    /// <summary>
    /// Defines the actions taken by the <see cref="Parser"/> to integrate a <see cref="Token"/> into a <see cref="Token"/> tree.
    /// </summary>
    public delegate Token NullDenotion(Token self, Parser parser);

    /// <summary>
    /// Defines the actions taken by the <see cref="Parser"/> with the <see cref="Token"/> directly to the left of the <see cref="Token"/> that uses this method group.
    /// </summary>
    public delegate Token LeftDenotation(Token self, Token left, int binding_power, Parser parser);

    /// <summary>
    /// Defines the actions taken by the <see cref="Parser"/> to create a statement from a <see cref="Token"/>.
    /// </summary>
    public delegate Token StatementDenotation(Token self, Parser parser);

    public abstract class TokenBase
    {

        /// <summary>
        /// Defines the actions taken by the <see cref="Parser"/> to integrate the <see cref="Token"/> into a <see cref="Token"/> tree.
        /// </summary>
        public NullDenotion Nud;

        /// <summary>
        /// Defines the actions taken by the <see cref="Parser"/> with the <see cref="Token"/> directly to the left of this <see cref="Token"/>.
        /// </summary>
        public LeftDenotation Led;

        /// <summary>
        /// Defines the actions taken by the <see cref="Parser"/> to create a statement from this <see cref="Token"/>.
        /// </summary>
        public StatementDenotation Stad;

        /// <summary>
        /// The left binding-power. This is how tightly the <see cref="Token"/> is bound with the <see cref="Token"/> to its left.
        /// </summary>
        public int LBP;

        internal TokenBase(int lbp)
        {
            this.LBP = lbp;

            this.Nud = NudError;
            this.Led = LedError;
        }

        /// <summary>
        /// The default method thrown when a <see cref="Token"/>'s <see cref="NullDenotion"/> has been called, but remains unpopulated.
        /// </summary>
        /// <returns>A thrown <see cref="Exception"/>.</returns>
        public Token NudError(Token self, Parser parser)
        { ThrowException("Undefined"); return null; }

        /// <summary>
        /// The default method thrown when a <see cref="Token"/>'s <see cref="LeftDenotation"/> has been called, but remains unpopulated.
        /// </summary>
        /// <returns>A thrown <see cref="Exception"/>.</returns>
        public Token LedError(Token self, Token left, int binding_power, Parser parser)
        { ThrowException("Missing Operator"); return null; }

        public void ThrowException(string message)
        { throw new Exception(message); }

    }

}
