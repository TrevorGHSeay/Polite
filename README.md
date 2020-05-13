# Polite
Polite is a Lexing, Parsing, Runtime, and Compiler framework designed for easy creation and execution of new .NET scripting languages. It includes a default lexer, parser, runtime, and compiler that allows users to create any language using the TDOP (top-down operator precedence) methodology. This includes all context-free grammars and, additionally, most other grammars if you manage your static denotation methods correctly.

The lexer, while simple and relatively slow by comparison to other commercial tokenizers, is included in the framework for two reasons: firstly, it's capable of tokenizing a vast number of varied languages and, secondly, because creating a lexer for your own language can be tedious and require an aggregious amount of overhead time. If you wish to create your own lexer, you can just inherit from the Lexer class, override the Tokenize and Tokenized methods, and then plug it into the Parser.

The parser, heavily influenced by Douglas Crockford's implementation of Vaughn Pratts iconic contribution, uses the top-down operator precendence methodology. If you're unfamiliar with TDOP, and would like to get a handle on it before starting your own language, you can refer to [this video](https://youtu.be/Nlqv6NtBXcA) that explains in particular detail as to how it's implemented, how you can use it, and why I chose it for this framework. You can also check out his [github](https://github.com/douglascrockford/TDOP) for its original implementation.

The runtime (PCR - Polite Common Runtime) is capable of running any language created by the Polite framework, if the parser's outputted Token trees match those of its expected inputs. If you're ready to get started right away, but need more information, you can always view the [api](https://trevorghseay.github.io/Polite/Documentation/_site/api/CommonGrammar.html) and [parser/runtime](https://github.com/TrevorGHSeay/Polite/tree/master/Documentation/Parser%20Structure%20Diagrams) documentation for an in-depth look.

The compiler (utilizing Roslyn Syntax Trees) lets you compile your language into C# structures and emit them as an executable, console application, or whatever you see fit. See the documentation for this API [here](https://trevorghseay.github.io/Polite/Documentation/_site/api/Polite.CompilerServices.html).
