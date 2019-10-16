using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polite
{

    public static class CommonGrammar
    {

        /// <summary>
        /// Creates a prefix expression from the prefix <see cref="Token"/>.
        /// </summary>
        /// <param name="self">The <see cref="Token"/> that is a prefix.</param>
        /// <param name="parser">The current <see cref="Parser"/>.</param>
        /// <returns>The <see cref="Token"/> tree representing the prefix' expression.</returns>
        public static Token PrefixNud(Token self, Parser parser)
        {
            parser.CurrentScope.Reserve(self);
            self.First = parser.Expression(70);
            self.Arity = Token.Arities.Unary;

            return self;
        }
        
        /// <summary>
        /// Creates an infix expression from the infix <see cref="Token"/>.
        /// </summary>
        /// <param name="self">The <see cref="Token"/> that is an infix.</param>
        /// <param name="binding_power">The left-binding-power used for ordering an <see cref="Token.Types.Primary.Operator"/> <see cref="Token"/>'s relationship with the <see cref="Token"/> to its left.</param>
        /// <param name="parser">The current <see cref="Parser"/>.</param>
        /// <returns>The <see cref="Token"/> tree representing the infix' expression.</returns>
        public static Token InfixLed(Token self, Token left, int binding_power, Parser parser)
        {
            self.First = left;
            self.Second = parser.Expression(binding_power);
            self.Arity = Token.Arities.Binary;

            return self;
        }
        
        /// <summary>
        /// Creates an infix expression from the infix <see cref="Token"/>, but reduces the binding_power by 1 before processing.
        /// </summary>
        /// <param name="self">The <see cref="Token"/> that is an infix.</param>
        /// <param name="binding_power">The left-binding-power used for ordering an <see cref="Token.Types.Primary.Operator"/> <see cref="Token"/>'s relationship with the <see cref="Token"/> to its left.</param>
        /// <param name="parser">The current <see cref="Parser"/>.</param>
        /// <returns>The <see cref="Token"/> tree representing the infix' expression.</returns>
        public static Token ReducedInfixLed(Token self, Token left, int binding_power, Parser parser)
        {
            return InfixLed(self, left, binding_power - 1, parser);
        }

        /// <summary>
        /// Creates an addresser expression from the <see cref="Token.Types.Secondary.Addresser"/> <see cref="Token"/>.
        /// </summary>
        /// <param name="self">The <see cref="Token"/> that is an <see cref="Token.Types.Secondary.Addresser"/>.</param>
        /// <param name="left">The <see cref="Token"/> directly to the left of 'self'.</param>
        /// <param name="binding_power">The left-binding-power used for ordering an <see cref="Token.Types.Primary.Operator"/> <see cref="Token"/>'s relationship with the <see cref="Token"/> to its left.</param>
        /// <param name="parser">The current <see cref="Parser"/>.</param>
        /// <returns>The <see cref="Token"/> tree representing the addresser's expression.</returns>
        public static Token AddresserLed(Token self, Token left, int binding_power, Parser parser)
        {
            Token newToken = self;

            newToken.First = left;

            if (parser.CurrentToken.Arity != Token.Types.Primary.Identifier)
                parser.CurrentToken.ThrowException("Expected a property name.");

            parser.CurrentToken.Arity = Token.Types.Primary.Literal;
            newToken.Second = parser.CurrentToken;
            newToken.Arity = Token.Arities.Binary;

            parser.Advance(string.Empty);

            return newToken;
        }

    }

}
