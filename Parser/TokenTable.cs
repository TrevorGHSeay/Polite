using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polite
{

    /// <summary>
    /// The class that contains definitions for every <see cref="Token"/> within a <see cref="Lexicon"/>.
    /// </summary>
    public class TokenTable : IEnumerable
    {

        internal Dictionary<string, Token> Tokens;

        public Token this[string id]
        {
            get { return Tokens[id]; }
            set { this.Add(value); }
        }
        public Token this[char id]
        {
            get { return Tokens[id.ToString()]; }
            set { this.Add(value); }
        }
        public Token this[Token token]
        {
            get { return Tokens[token.Name]; }
            set { this.Add(token); }
        }

        public IEnumerator GetEnumerator()
        { foreach (KeyValuePair<string, Token> token in Tokens) yield return token.Value; }

        /// <summary>
        /// Creates an empty <see cref="TokenTable"/>.
        /// </summary>
        public TokenTable() : this(new Dictionary<string, Token>())
        { }

        /// <summary>
        /// Creates an empty <see cref="TokenTable"/> with the set of every predefined <see cref="Token"/>.
        /// </summary>
        public TokenTable(Dictionary<string, Token> tokens)
        { Tokens = tokens; }

        /// <summary>
        /// Adds or updates the <see cref="Token"/> internally.
        /// </summary>
        public Token Add(Token token)
        {
            Token currentToken;
            foreach (KeyValuePair<string, Token> tokenPair in Tokens)
            {
                if (tokenPair.Key == token.Name)
                {
                    currentToken = tokenPair.Value;
                    if (token.LBP > currentToken.LBP)
                    {
                        currentToken.LBP = token.LBP;
                        return currentToken;
                    }
                    else
                        return token;
                }
            }
            Tokens.Add(token.Name, token);
            return token;
        }
        
        /// <summary>
        /// Determines if the 'id' is found within <see cref="TokenTable"/>.
        /// </summary>
        /// <param name="id">The <see cref="string"/> to search for.</param>
        /// <returns>True if found.</returns>
        public bool Contains(string id)
        { return Tokens.ContainsKey(id); }

        /// <summary>
        /// Determines if the 'id' is found within <see cref="TokenTable"/>.
        /// </summary>
        /// <param name="id">The <see cref="string"/> to search for.</param>
        /// <param name="destination">The <see cref="Token"/> found internally; null if unfound.</param>
        /// <returns>True if found.</returns>
        public bool TryGet(string id, out Token destination)
        {
            foreach (KeyValuePair<string, Token> tokenPair in this.Tokens)
            {
                if (tokenPair.Key == id)
                {
                    destination = tokenPair.Value;
                    return true;
                }
            }
            destination = null;
            return false;
        }
        
    }
}
