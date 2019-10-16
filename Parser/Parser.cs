using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polite
{

    /// <summary>
    /// <para>The main class that performs parsing using the Top-Down Operator Precedence methodology.</para>
    /// <para>Heavily influenced by Douglas Crockford's JavaScript implementation of Vaughn Pratt's iconic contribution.</para>
    /// </summary>
    public class Parser
    {

        internal List<Token> Tokens;

        /// <summary>
        /// The <see cref="Polite.Lexer"/> used to tokenize the source code of a custom language with.
        /// </summary>
        public Lexer Lexer;

        /// <summary>
        /// The location where a resulting <see cref="Token"/> tree is stored, after being created by parsing source code.
        /// </summary>
        public Token Result { get; private set; }

        /// <summary>
        /// The current <see cref="Scope"/> that is the location of the <see cref="Parser"/> during parsing. Used internally and for creating custom methods' <see cref="Grammars"/>.
        /// </summary>
        public Scope CurrentScope;

        private int NewScopeIndex;

        /// <summary>
        /// The current <see cref="Token"/> that is the location of the <see cref="Parser"/> during parsing. Used internally and for creating custom methods' <see cref="Grammars"/>.
        /// </summary>
        public Token CurrentToken;

        private int Index;

        /// <summary>
        /// Creates a new instance of <see cref="Parser"/> with a chosen <see cref="Polite.Lexer"/> for tokenizing.
        /// </summary>
        /// <param name="lexer">The lexer to tokenize a given source code before parsing it into a <see cref="Token"/> tree.</param>
        public Parser(Lexer lexer)
        {
            this.Lexer = lexer;
            Init();
        }

        /// <summary>
        /// Creates a new instance of <see cref="Parser"/> with a chosen <see cref="Polite.Lexer"/> for tokenizing.
        /// </summary>
        /// <param name="lexer">The lexer to tokenize a given source code before parsing it into a <see cref="Token"/> tree.</param>
        /// <param name="tokens">The <see cref="List"/> of <see cref="Tokens"/> provided by a <see cref="Polite.Lexer"/> to immediately <see cref="Parse"/>.</param>
        public Parser(Lexer lexer, List<Token> tokens) : this(lexer)
        { this.Parse(tokens); }

        private void Init()
        {
            if (this.Tokens == null)
                this.Tokens = new List<Token>();
            else
                this.Tokens.Clear();

            ResetResults();
        }

        private void ResetResults()
        {
            if (this.Result == null)
                this.Result = new Token(Token.Types.Undefined, 0, string.Empty);
            else
                this.Result.Children.Clear();

            NewScopeIndex = 0;
            CurrentScope = null;
        }

        /// <summary>
        /// Attempts to <see cref="Parse"/> source and returns a <see cref="bool"/> denoting success or failure.
        /// </summary>
        /// <param name="source">The source code to try to parse.</param>
        /// <returns>True if parsing was successful.</returns>
        public bool TryParse(string source)
        {
            try
            {
                Parse(source);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to <see cref="Parse"/> source and returns a <see cref="bool"/> denoting success or failure.
        /// </summary>
        /// <param name="tokens">The tokenized source code to try to parse.</param>
        /// <returns>True if parsing was successful.</returns>
        public bool TryParse(List<Token> tokens)
        {
            try
            {
                Parse(tokens);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Parses the source code of a custom language and caches the <see cref="Result"/>.
        /// </summary>
        /// <param name="source">The source code to parse.</param>
        public void Parse(string source)
        {
            this.Lexer.Tokenize(source);
            this.Parse(Lexer.Tokens);
        }

        /// <summary>
        /// Parses a <see cref="Token"/> tree (that is the result of a <see cref="Lexer"/>) of a custom language and caches the <see cref="Result"/>.
        /// </summary>
        /// <param name="source">The source code to parse.</param>
        public void Parse(List<Token> tokens)
        {
            this.Tokens = tokens;
            if (this.Lexer != null && this.Tokens.Count > 0)
                this.Parse();
        }

        private void Parse()
        {
            ResetResults();

            this.Index = 0;
            this.CurrentToken = this.Tokens[this.Index];
            this.CurrentScope = null;
            this.NewScope();
            this.Advance(string.Empty);
            Token result = Grammars.JavaScript.StatementsFunction(this);
            this.Advance(Token.Types.Primary.ToPUA(Token.Types.Primary.End).ToString());
            this.PopScope();
            this.Result = result;
        }

        /// <summary>
        /// Constructs an expression using the <see cref="CurrentToken"/> <see cref="NullDenotion"/> or <see cref="LeftDenotation"/>.
        /// </summary>
        /// <param name="rbp">The right-binding-power that denotes where to halt construction if the <see cref="CurrentToken"/> has a greater than, or equal to, left-binding-power than the rbp provided.</param>
        /// <returns>A <see cref="Token"/> tree describing the expression that was constructed.</returns>
        public Token Expression(int rbp)
        {
            Token left;
            Token t = this.CurrentToken.Clone();

            Advance(string.Empty);
            left = t.Nud(t, this);
            while (rbp < this.CurrentToken.LBP)
            {
                t = this.CurrentToken.Clone();

                Advance(string.Empty);

                left = t.Led(t, left, t.LBP, this);
                //System.Diagnostics.Debug.WriteLine("Expression Loop: " + (left.First == null ? "" : "left.First.Value = " + left.First.Value + ", ") + "left.Value = " + left.Value + (left.Second == null ? "" : ", left.Second.Value = " + left.Second.Value));

            }
            //System.Diagnostics.Debug.WriteLine("Expression: " + (left.First == null ? "" : "left.First.Value = " + left.First.Value + ", ") + "left.Value = " + left.Value + (left.Second == null ? "" : ", left.Second.Value = " + left.Second.Value));
            return left;
        }

        /// <summary>
        /// Advances the <see cref="CurrentToken"/> forward one increment.
        /// </summary>
        /// <param name="name">The expected name for the <see cref="CurrentToken"/> before incrementing. Used primarily for error-handling.</param>
        /// <returns>The next <see cref="Token"/> in sequence.</returns>
        public Token Advance(string name)
        {
            if (name != string.Empty && CurrentToken.Name != name)
                CurrentToken.ThrowException("Expected ID '" + name + "'");

            else if (Index >= Tokens.Count)
            {
                CurrentToken = Lexer.Lexicon.TokenTable[Token.Types.Primary.ToPUA(Token.Types.Primary.End)];
                return CurrentToken;
            }

            Token token = Tokens[Index];
            Index += 1;
            string newName = token.Name;
            short newType = token.PrimaryType;

            switch (newType)
            {

                case (Token.Types.Primary.Constant):
                case (Token.Types.Primary.Identifier):
                    {
                        CurrentToken = this.CurrentScope.Find(token, this).Clone();
                        break;
                    }
                    
                case (Token.Types.Primary.Function):
                case (Token.Types.Primary.Statement):
                case (Token.Types.Primary.Operator):
                case (Token.Types.Primary.EndLine):
                    {
                        bool tokenExists = Lexer.Lexicon.TokenTable.TryGet(newName, out Token newToken);
                        if (tokenExists)
                            CurrentToken = newToken.Clone();
                        else
                            CurrentToken.ThrowException("Unknown Operator or Statement");
                        break;
                    }

                case (Token.Types.Primary.Literal):
                    {
                        bool tokenExists = Lexer.Lexicon.TokenTable.TryGet(Token.Types.Primary.ToPUA(newType).ToString(), out Token newToken);
                        if (tokenExists)
                            CurrentToken = newToken.Clone();
                        else
                            CurrentToken.ThrowException("Unknown Literal");
                        break;
                    }

                default:
                    token.ThrowException("Unexpected Token.");
                    break;
            }

            CurrentToken.SecondaryType = token.SecondaryType;
            CurrentToken.Value = token.Value;
            CurrentToken.Name = newName;
            CurrentToken.Arity = newType;

            CurrentToken.Start = token.Start;
            CurrentToken.End = token.End;

            return CurrentToken;

        }
        
        /// <summary>
        /// Creates a new <see cref="Scope"/>, sets its parent to the <see cref="CurrentScope"/>, then sets the <see cref="CurrentScope"/> to the new <see cref="Scope"/>.
        /// </summary>
        /// <returns>The new <see cref="CurrentScope"/>.</returns>
        public Scope NewScope()
        {
            this.CurrentScope = new Scope(0, string.Empty, NewScopeIndex) { Parent = this.CurrentScope };
            NewScopeIndex += 1;
            return this.CurrentScope;
        }
        
        /// <summary>
        /// Destroys the <see cref="CurrentScope"/> and sets it to its previous parent.
        /// </summary>
        public void PopScope()
        {
            this.CurrentScope = this.CurrentScope.Parent;
        }

        /// <summary>
        /// Prints a <see cref="Token"/> tree to the <see cref="Console"/>.
        /// </summary>
        /// <param name="tree">The tree to be printed.</param>
        /// <param name="parent">The parent of the tree.</param>
        /// <param name="depth">The depth into the current tree we have delved. Used for recursing through <see cref="Token"/> nodes of tree.</param>
        public static void PrintResults(Token tree, Token parent, int depth)
        {
            if (tree.HasChildren || tree.HasMembers)
            {
                System.Diagnostics.Trace.WriteLine("'" + (string.IsNullOrEmpty(tree.Name) ? "Statements" : tree.Name) + "'" + (parent is null ? "" : (tree == parent.First ? "(First)" : tree == parent.Second ? "(Second)" : tree == parent.Third ? "(Third)" : "(Child)")) + "(" + tree.PrimaryType + "," + tree.SecondaryType + ")   1. [" + (tree.First is null ? "_" : string.IsNullOrEmpty(tree.First.Name) ? "'Statements'" : tree.First.Name) + "],   2. [" + (tree.Second is null ? "_" : string.IsNullOrEmpty(tree.Second.Name) ? "'Statements'" : tree.Second.Name) + "],   3. [" + (tree.Third is null ? "_" : string.IsNullOrEmpty(tree.Third.Name) ? "'Statements'" : tree.Third.Name) + "],   Children.Count = " + tree.Children.Count);
                if (tree.First != null || tree.Second != null || tree.Third != null)
                    System.Diagnostics.Trace.WriteLine((tree.First is null ? "_" : "'" + (string.IsNullOrEmpty(tree.First.Name) ? "Statements" : tree.First.Name) + "'") + " " + tree.Name + " " + (tree.Second is null ? "_" : "'" + (string.IsNullOrEmpty(tree.Second.Name) ? "Statements" : tree.Second.Name) + "'"));
                System.Diagnostics.Trace.WriteLine(string.Empty);
            }

            if (tree.First != null || tree.Second != null || tree.Third != null)
            {
                if (tree.First != null)
                    PrintResults(tree.First, tree, depth + 1);

                if (tree.Second != null)
                    PrintResults(tree.Second, tree, depth + 1);

                if (tree.Third != null)
                    PrintResults(tree.Third, tree, depth + 1);
            }

            if (tree.HasChildren)
                for (int i = 0; i < tree.Children.Count; i += 1)
                    PrintResults(tree.Children[i], tree, depth + 1);
            
        }

    }

}
