using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polite
{

    public class Lexicon
    {

        /// <summary>
        /// The <see cref="Polite.TokenTable"/> that contains definitions about, and how to treat, each <see cref="Token"/> within the parsing and interpretation stages.
        /// </summary>
        public TokenTable TokenTable;

        /// <summary>
        /// Every <see cref="Token.Types.Primary.Ignored"/> <see cref="Token"/> found in the <see cref="TokenTable"/>.
        /// </summary>
        public List<Token> Ignored
        { get { return TokensOfType(Token.Types.Primary.Ignored); } }

        /// <summary>
        /// Every <see cref="Token.Types.Primary.IgnoredDelimiter"/> <see cref="Token"/> found in the <see cref="TokenTable"/>.
        /// </summary>
        public List<Token> IgnoredDelimiters
        { get { return TokensOfType(Token.Types.Primary.IgnoredDelimiter); } }

        /// <summary>
        /// Every <see cref="Token.Types.Primary.Operator"/> <see cref="Token"/> found in the <see cref="TokenTable"/>.
        /// </summary>
        public List<Token> Operators
        { get { return TokensOfType(Token.Types.Primary.Operator); } }

        /// <summary>
        /// Creates an empty instance of <see cref="Lexicon"/>.
        /// </summary>
        public Lexicon() : this(new TokenTable())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="Lexicon"/> with token_table.
        /// </summary>
        /// <param name="token_table">The <see cref="Polite.TokenTable"/> to store and extract <see cref="Token.Types"/> from.</param>
        public Lexicon(TokenTable token_table)
        { this.TokenTable = token_table; }

        public List<Token> TokensOfType(short token_type)
        {
            List<Token> result = new List<Token>();
            foreach (Token t in TokenTable)
            {
                if (Token.IsTypeOf(t, token_type))
                    result.Add(t);
            }
            return result;
        }

        /// <summary>
        /// Contains methods used as defaults in various pre-implemented languages.
        /// </summary>
        public static class Defaults
        {
            /// <summary>
            /// Contains methods used as defaults for the JavaScript language.
            /// </summary>
            public static class JavaScript
            {

                /// <summary>
                /// Finds, converts, and returns all literals found in the provided JavaScript source code.
                /// </summary>
                /// <param name="source">The source code to extract literals from.</param>
                /// <param name="token_table">The <see cref="Polite.TokenTable"/> to use as reference for the creation of a returned <see cref="Token"/></param>
                /// <param name="reserved">The <see cref="BitArray"/> to set true for every index that the extracted literals occupy within source.</param>
                /// <returns>Every extracted literal <see cref="Token"/> found in source.</returns>
                public static Token[] SniffLiterals(string source, TokenTable token_table, ref BitArray reserved)
                {
                    List<Token> result = new List<Token>();

                    // String sniffer
                    int start = 0;
                    int start1 = 0;
                    int start2 = 0;
                    while 
                    (
                        start < source.Length && start > -1 &&
                        (
                            (start1 = source.IndexOf("\"", start)) != -1) ||
                            (start2 = source.IndexOf("\'", start)) != -1
                        )
                    {
                        start = start1 == -1 ? start2 : start1;

                        char opener = source[start];

                        int end = -1;
                        while // Find next valid closer
                        (
                            (end =
                                (start + 1 < source.Length - 1 ?
                                    source.IndexOf(opener, start + 1) :
                                    throw new Exception("Opening encapsulator at index " + start + " has no closer.")
                                )
                            )
                            != -1 &&
                            (end < source.Length - 1 ? true : throw new Exception("Opening encapsulator at index " + start + " has no closer.")) &&
                            source[end - 1] == '\\'
                        ) { }

                        Token newToken = token_table[Token.Types.Primary.ToPUA(Token.Types.Primary.Literal)].Clone();
                        newToken.SecondaryType = Token.Types.Secondary.Literals.String;
                        newToken.Name = source.Substring(start, (end == -1 ? source.Length - 1 : end) - start + 1);
                        newToken.Value = newToken.Name.Substring(1, newToken.Name.Length - 2);
                        newToken.Start = start;
                        newToken.End = end;
                        
                        for (int i = start; i <= end; i += 1)
                            reserved.Set(i, true);

                        result.Add(newToken);
                        start = end + 1;
                    }

                    // Array sniffer
                    start = 0;
                    start1 = 0;
                    start2 = 0;
                    while
                    (
                        start < source.Length &&
                        (start = source.IndexOf("[]", start)) != -1
                    )
                    {
                        if (reserved.Get(start))
                        {
                            start += 1;
                            continue;
                        }

                        Token newToken = token_table[Token.Types.Primary.ToPUA(Token.Types.Primary.Literal)].Clone();
                        newToken.SecondaryType = Token.Types.Secondary.Literals.Array;

                        newToken.Value = new List<object>();

                        newToken.Start = start;
                        newToken.End = start + 1;

                        for (int i = start; i <= newToken.End; i += 1)
                            reserved.Set(i, true);

                        result.Add(newToken);

                        start = newToken.End + 1;
                    }

                    // Boolean sniffer
                    start = 0;
                    start1 = 0;
                    start2 = 0;
                    string trueL = "true";
                    string falseL = "false";
                    while
                    (
                        (start1 = source.IndexOf(trueL, start)) != -1 ||
                        (start2 = source.IndexOf(falseL, start)) != -1                     
                    )
                    {
                        start = start1 == -1 ? start2 : start1;

                        if (reserved.Get(start))
                        {
                            start += 1;
                            continue;
                        }

                        Token newToken = token_table[Token.Types.Primary.ToPUA(Token.Types.Primary.Literal)].Clone();
                        newToken.SecondaryType = Token.Types.Secondary.Literals.Boolean;
                        
                        if (source[start] == 't')
                            newToken.Name = trueL;

                        if (source[start] == 'f')
                            newToken.Name = falseL;

                        newToken.Value = Convert.ToBoolean(newToken.Name);

                        newToken.Start = start;
                        newToken.End = start + newToken.Name.Length - 1;

                        for (int i = start; i <= newToken.End; i += 1)
                            reserved.Set(i, true);

                        result.Add(newToken);

                        start = newToken.End + 1;
                    }

                    // Number sniffer (integer and double)
                    for (int i = 0; i < source.Length; i += 1)
                    {
                        char character = source[i];

                        if
                        (
                            !reserved.Get(i) &&
                            char.IsDigit(character)
                        )
                        {
                            start = i;

                            bool foundDecimal = false;
                            while // Find end of digit string
                            (
                                char.IsDigit(source[i]) ||
                                (!foundDecimal && (foundDecimal = source[i] == '.')) // Thinks integers are doubles, need to figure that out
                            )
                            { i += 1; }

                            i -= 1;

                            Token newToken = token_table[Token.Types.Primary.ToPUA(Token.Types.Primary.Literal)].Clone();
                            newToken.Name = source.Substring(start, i - start + 1);
                            if (foundDecimal)
                            {
                                newToken.SecondaryType = Token.Types.Secondary.Literals.Double;
                                newToken.Value = Convert.ToDouble(newToken.Name);
                            }
                            else
                            {
                                newToken.SecondaryType = Token.Types.Secondary.Literals.Integer;
                                newToken.Value = Convert.ToInt32(newToken.Name);
                            }

                            newToken.Start = start;
                            newToken.End = start + newToken.Name.Length - 1;

                            for (i = start; i <= newToken.End; i += 1)
                                reserved.Set(i, true);

                            result.Add(newToken);
                        }
                    }

                    return result.ToArray();
                }

                /// <summary>
                /// Determines if source' entirety constitutes a valid <see cref="Token.Types.Primary.Literal"/> within the <see cref="JavaScriptRuntime"/> framework.
                /// </summary>
                /// <param name="snippet">The source code to determine.</param>
                /// <returns>True if source' entirety constitutes a valid <see cref="Token.Types.Primary.Literal"/>.</returns>
                public static bool IsLiteral(string snippet)
                {
                    return !string.IsNullOrEmpty(snippet) && (IsBooleanLiteral(snippet) || IsStringLiteral(snippet, "\"") || IsStringLiteral(snippet, "\'") || IsDoubleLiteral(snippet));
                }

                /// <summary>
                /// Determines if source' entirety constitutes a valid <see cref="Token.Types.Primary.Identifier"/> within the <see cref="JavaScriptRuntime"/> framework.
                /// </summary>
                /// <param name="snippet">The source code to determine.</param>
                /// <returns>True if source' entirety constitutes a valid <see cref="Token.Types.Primary.Identifier"/>.</returns>
                public static bool IsIdentifier(string snippet)
                {
                    return !string.IsNullOrEmpty(snippet) && !char.IsDigit(snippet[0]) && snippet.All(char.IsLetterOrDigit);
                }

                /// <summary>
                /// Determines if source' entirety constitutes a valid <see cref="Token.Types.Secondary.Literals.Boolean"/> within the <see cref="JavaScriptRuntime"/> framework.
                /// </summary>
                /// <param name="snippet">The source code to determine.</param>
                /// <returns>True if source' entirety constitutes a valid <see cref="Token.Types.Secondary.Literals.Boolean"/>.</returns>
                public static bool IsBooleanLiteral(string snippet)
                {
                    return snippet == "true" || snippet == "false";
                }

                /// <summary>
                /// Determines if source' entirety constitutes a valid <see cref="Token.Types.Secondary.Literals.Integer"/> within the <see cref="JavaScriptRuntime"/> framework.
                /// </summary>
                /// <param name="snippet">The source code to determine.</param>
                /// <returns>True if source' entirety constitutes a valid <see cref="Token.Types.Secondary.Literals.Integer"/>.</returns>
                public static bool IsIntegerLiteral(string snippet)
                {
                    return snippet.All(char.IsDigit);
                }

                /// <summary>
                /// Determines if source' entirety constitutes a valid <see cref="Token.Types.Secondary.Literals.String"/> within the <see cref="JavaScriptRuntime"/> framework.
                /// </summary>
                /// <param name="snippet">The source code to determine.</param>
                /// <returns>True if source' entirety constitutes a valid <see cref="Token.Types.Secondary.Literals.String"/>.</returns>
                public static bool IsStringLiteral(string snippet)
                {
                    return IsStringLiteral(snippet, "\'") || IsStringLiteral(snippet, "\"");
                }

                /// <summary>
                /// Determines if source' entirety constitutes a valid <see cref="Token.Types.Secondary.Literals.Boolean"/> within the <see cref="JavaScriptRuntime"/> framework.
                /// </summary>
                /// <param name="snippet">The source code to determine.</param>
                /// <param name="encapsulator">The matching quotes found at the beginning and end of a string literal.</param>
                /// <returns>True if source' entirety constitutes a valid <see cref="Token.Types.Secondary.Literals.Boolean"/>.</returns>
                public static bool IsStringLiteral(string snippet, string encapsulator)
                {
                    int end = snippet.Length - 1;

                    string first = snippet[0].ToString();
                    string last = snippet[end].ToString();

                    int secondIndex = (end == 0 ? -1 : snippet.IndexOf(encapsulator, 1));

                    return first == encapsulator && (secondIndex == -1 || secondIndex == end);
                }

                /// <summary>
                /// Determines if source' entirety constitutes a valid <see cref="Token.Types.Secondary.Literals.Double"/> within the <see cref="JavaScriptRuntime"/> framework.
                /// </summary>
                /// <param name="snippet">The source code to determine.</param>
                /// <returns>True if source' entirety constitutes a valid <see cref="Token.Types.Secondary.Literals.Double"/>.</returns>
                public static bool IsDoubleLiteral(string snippet)
                {
                    bool foundDecimal = false;
                    foreach (char digit in snippet)
                    {
                        if (!char.IsDigit(digit))
                        {
                            if (!foundDecimal && digit == '.')
                                foundDecimal = true;
                            else
                                return false;
                        }
                    }
                    return true;
                }

            }

        }

    }

}
