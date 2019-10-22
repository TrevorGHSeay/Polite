# Polite
Polite is a Lexing, Parsing, and Runtime framework designed for easy creation and execution of new .NET scripting languages. It includes a default lexer, parser, and runtime that allows users to create any language using the TDOP (top-down operator precedence) methodology. This includes all context-free grammars and, additionally, most other grammars if you manage your static denotation methods correctly.

The lexer, while simple and relatively slow by comparison to other commercial tokenizers, is included in the framework for two reasons: firstly, it's capable of tokenizing a vast number of varied languages and, secondly, because creating a lexer for your own language can be tedious and require an aggregious amount of overhead time. If you wish to create your own lexer, you can just inherit from the Lexer class, override the Tokenize and Tokenized methods, and then plug it into the Parser.

The parser, heavily influenced by Douglas Crockford's implementation of Vaughn Pratts iconic contribution, uses the top-down operator precendence methodology. If you're unfamiliar with TDOP, and would like to get a handle on it before starting your own language, you can refer to [this video](https://youtu.be/Nlqv6NtBXcA) that explains in particular detail as to how it's implemented, how you can use it, and why I chose it for this framework. You can also check out his [github](https://github.com/douglascrockford/TDOP) for his original implementation.

The runtime (PCR - Polite Common Runtime) is capable of running any language created by the Polite framework, if the parser's outputted Token trees match those of its expected inputs. I plan to upload graphics of the expected structures for each operational entity in the near future, but they can all be inferred from the source code within the Grammars.JavaScript and CommonGrammar files - with enough attention to detail. My apologies that this is a temporary burden to those who might like to create their own languages quickly, but it is what it is and won't be for much longer.

If you're ready to get started right away, but need more information, you can always view the [api](https://trevorghseay.github.io/Polite/Documentation/api/Polite.CommonGrammar.html) and [parser/runtime](https://github.com/TrevorGHSeay/Polite/tree/master/Documentation/Parser%20Structure%20Diagrams) documentation for an in-depth look.
