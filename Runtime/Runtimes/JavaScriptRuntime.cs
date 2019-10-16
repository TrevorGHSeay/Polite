using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polite
{

    public class JavaScriptRuntime : Runtime
    {
        
        public static readonly short[] IllegalDuplicates = new short[]
        {
            Token.Types.Primary.Statement,
            Token.Types.Primary.Identifier,
            Token.Types.Primary.Operator,
            Token.Types.Primary.Literal
        };

        public static Parser DefaultParser
        { get { return new Parser(new Lexer(new Lexicons.JavaScript(), Lexicon.Defaults.JavaScript.IsIdentifier, Lexicon.Defaults.JavaScript.SniffLiterals, IllegalDuplicates)); } }

        public JavaScriptRuntime(Type[] reference_types) : base(DefaultParser, reference_types)
        { }

    }

}
