#define DEBUG

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;
using System.Dynamic;

namespace Polite
{
    /// <summary>
    /// Determines if a <see cref="string"/> meets the criteria to be an identifer (<see cref="Container"/>, <see cref="Function"/>, etc).
    /// </summary>
    /// <param name="source_name">The string to be checked.</param>
    /// <returns>Returns true if the name provided is a valid identifier for the custom language to be tokenized.</returns>
    public delegate bool IdentifierChecker(string name);

    /// <summary>
    /// Finds all literals in source, using token_table, and sets the reserved <see cref="BitArray"/>'s indices to true that are the <see cref="char"/> indices contained in the literals found within source.
    /// </summary>
    /// <param name="source">The source code wherein to extract literals from.</param>
    /// <param name="token_table">The <see cref="TokenTable"/> to use as reference for pre-created <see cref="Token"/>s that represent the literals' <see cref="TokenTypes"/></param>
    /// <param name="reserved">The <see cref="BitArray"/> that must have its indices set to true for all corresponding indices that make up the extracted literals within source.</param>
    /// <returns>All extracted <see cref="Token"/> literals.</returns>
    public delegate Token[] LiteralSniffer(string source, TokenTable token_table, ref BitArray reserved);

    /// <summary>
    /// The default class for creating 
    /// </summary>
    public class Lexer
    { 

        /// <summary>
        /// The <see cref="Lexicon"/> containing definitions for every <see cref="Token"/> found in a custom language.
        /// </summary>
        public Lexicon Lexicon;

        /// <summary>
        /// The result destination for the <see cref="Tokenize(string)"/> method.
        /// </summary>
        public List<Token> Tokens;
        
        /// <summary>
        /// The <see cref="Delegate"/> that determines if a particular substring constitutes a valid <see cref="TokenTypes.Primary.Identifier"/>.
        /// </summary>
        public IdentifierChecker IsIdentifier;

        /// <summary>
        /// The <see cref="Delegate"/> that sniffs out, creates, and returns every <see cref="Token"/> found to constitute a valid <see cref="TokenTypes.Primary.Literal"/>.
        /// </summary>
        public LiteralSniffer SniffLiterals;

        /// <summary>
        /// Creates a new instance of <see cref="Lexer"/>.
        /// </summary>
        public Lexer() : 
            this(
                new Lexicon(),
                Lexicon.Defaults.JavaScript.IsIdentifier,
                Lexicon.Defaults.JavaScript.SniffLiterals,
                Array.Empty<short>())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="Lexer"/>.
        /// </summary>
        public Lexer(
            Lexicon lexicon,
            IdentifierChecker identifier_checker,
            LiteralSniffer literal_sniffer,
            short[] illegal_duplicates)
        {
            this.Lexicon = lexicon;
            this.IsIdentifier = identifier_checker;
            this.SniffLiterals = literal_sniffer;
            this.Tokens = new List<Token>();
        }

        /// <summary>
        /// Creates a new instance of <see cref="Lexer"/>.
        /// </summary>
        public Lexer(
            Lexicon lexicon,
            IdentifierChecker identifier_checker,
            LiteralSniffer literal_sniffer,
            string source,
            short[] illegal_duplicates)
            : this(
                  lexicon,
                  identifier_checker,
                  literal_sniffer,
                  illegal_duplicates)
        { this.Tokenize(source); }

        /// <summary>
        /// Tokenizes source into a <see cref="List{T}"/> to destination <see cref="Tokens"/>.
        /// </summary>
        /// <param name="source">The raw source code to tokenize.</param>
        public void Tokenize(string source)
        {
            this.Tokens = this.Tokenized(source);
        }

        /// <summary>
        /// Tokenizes source into a <see cref="List{Token}"/> of <see cref="Type"/> <see cref="Token"/>.
        /// </summary>
        /// <param name="source">The raw source code to tokenize.</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="Type"/> <see cref="Token"/>.</returns>
        public List<Token> Tokenized(string source)
        {

            List<Token> results = new List<Token>();

            List<Token> tokenTable_DescendingLength = this.Lexicon.TokenTable.Tokens.Values.ToList();
            tokenTable_DescendingLength.Sort((t1, t2) => t2.Name.Length.CompareTo(t1.Name.Length));

            BitArray sourceReserved = new BitArray(source.Length, false);

            // Get Literals
            Token[] literals = SniffLiterals(source, this.Lexicon.TokenTable, ref sourceReserved);
            if (literals.Length != 0)
                results.AddRange(literals);

            // Get all known tokens from source
            foreach (Token token in tokenTable_DescendingLength)
            {
                int tokenInd = -1;
                while ((tokenInd += 1) < source.Length && (tokenInd = source.IndexOf(token.Name, tokenInd)) != -1)
                {
                    if (sourceReserved.Get(tokenInd))
                        continue;

                    Token t = token.Clone();
                    t.Start = tokenInd;
                    t.End = tokenInd + t.Name.Length - 1;

                    for (int i = t.Start; i <= t.End; i += 1)
                        sourceReserved.Set(i, true);
                    
                    results.Add(t);
                }
            }

            // Sort in order of appearance
            results.Sort((t1, t2) => t1.Start.CompareTo(t2.Start));

            Token identifier = this.Lexicon.TokenTable[Token.Types.Primary.ToPUA(Token.Types.Primary.Identifier)].Clone();

            // Make sure there isnt an identifier missing at the beginning before we start
            if (results.Count > 0 && results[0].Start != 0)
            {
                Token t2 = 1 < results.Count ? results[0] : null;
                if (t2 != null)
                {
                    int newStart = 0;
                    int newEnd = t2.Start - 1;

                    string value = source.Substring(newStart, newEnd - newStart + 1);
                    
                    if (!IsIdentifier(value))
                        InsufficientIdentifierException(value);

                    Token newToken = identifier.Clone();
                    newToken.Name = value;

                    newToken.Start = newStart;
                    newToken.End = newEnd;
                    
                    results.Insert(0, newToken);
                }
            }

            // Get identifiers
            for (int i = 0; i < results.Count; i += 1)
            {
                Token t1 = results[i];
                Token t2 = i + 1 < results.Count ? results[i + 1] : null;

                if (t2 != null && t2.Start - t1.End > 1)
                {
                    int newStart = t1.End + 1;
                    int newEnd = t2.Start - 1;

                    string value = source.Substring(newStart, newEnd - newStart + 1);
                    
                    if (!IsIdentifier(value))
                        InsufficientIdentifierException(value);

                    Token newToken = identifier.Clone();
                    newToken.Name = value;

                    newToken.Start = newStart;
                    newToken.End = newEnd;
                    
                    i += 1;
                    results.Insert(i, newToken);
                }
            }

            // Parse through results for illegal "Next" entries, then concatenate into single identifier (all until first legal one found)
            for (int i = 0; i < results.Count; i += 1)
            {
                Token t1 = results[i];

                Token t2 = i + 1 < results.Count ? results[i + 1] : null;

                if (t1.Next.Length == 0 || t2 is null)
                    continue;

                if (!t1.Preceeds(t2.PrimaryType))
                {
                    int j = i;
                    string name = string.Empty;
                    for (; j < results.Count; j += 1)
                    {
                        Token t = results[j];
                        string newName = name + t.Name;
                        if (identifier.Preceeds(t.PrimaryType) || !IsIdentifier(newName))
                            break;
                        name = newName;
                    }

                    if (IsIdentifier(name))
                    {
                        Token newToken = identifier.Clone();
                        newToken.Name = name;

                        newToken.Start = t1.End + 1;
                        newToken.End = newToken.Start + name.Length - 1;

                        results.RemoveRange(i, j - i);
                        results.Insert(i, newToken);

                        // Check again now that we have a new identifier type in place
                        i = i > 0 ? i - 2 : -1;
                    }
                    else
                        InsufficientIdentifierException(name);
                }
            }

            // Remove ignored tokens from final product
            for (int i = 0; i < results.Count; i += 1)
            {
                Token t1 = results[i];
                
                if (t1.PrimaryType == Token.Types.Primary.Ignored || t1.PrimaryType == Token.Types.Primary.IgnoredDelimiter)
                {
                    results.RemoveAt(i--);
                }
            }

            return results;
        }

        private void InsufficientIdentifierException(string loaded)
        {
            throw new Exception("Expected identifier, but got '" + loaded + "' instead.");
        }

    }

}
