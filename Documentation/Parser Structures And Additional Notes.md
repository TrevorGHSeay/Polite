# Parser Structure

Here I will lay out the fundamentals for understanding the inner logic of the parser structures, how they're interpreted by the PCR, and hopefully provide the reader with a concrete reference to which they can return in times of ambiguity. The following is written under the assumption that the reader has already become loosely familiar with the provided diagrams, and wishes to grasp the interpretation stage perhaps a little better.


## Order of Operations

Operations in the Polite Framework follow a very simple strategy, with two definitive steps.

### Members
The runtime begins with the first member of the program, and then continues to that token's first member, and so on, until a bottom is reached. It then performs the operation/expression attributed to that token, its parent, and its sibling members. Once completed, it pops up to the parent's layer, moving onto its siblings in the same way (finding bottom, caching operations/expressions, etc.).

### Children 
Once all members have been cached in this fashion, an identical approach is then taken with all children of the program (moving through the first child of the first child, until a bottom is reached). These searches are in fact done simultaneously, but functionally require the knowledge that children are a lower priority than members, and as such will always be performed last. If a token has both members and children then the reader must note that members - in this way - will be treated preferentially, and that if (for example) a return statement is indicated within a token's member, and is reached before its child operations/expressions are called, its child operations/expessions will not be performed.

## Liberty
Unlike many runtimes, which are often quite strict with their inputs, the Polite Framework allows for some leeway when creating token structures - as expected for the nature of the project. Below is a list of ways in which this can be seen.

### Substitution
There are some cases where tokens may be substituted with others for the sake of alternative behaviour. For example, take the conditional statement as seen in the provided [diagram](https://github.com/TrevorGHSeay/Polite/blob/master/Documentation/Parser%20Structure%20Diagrams/Conditions.jpg): because the 'if' clause has a generic second member, and that member has only one child, you may prefer to remove it so as to make the operator token the second member of the conditional token itself. The runtime will see this as functionally identical, because the original member was no more than a container to denote a collection of operations/expressions which, since there was only one in its scope, makes it contextually redundant. This knowledge is particularly useful in the case of custom languages that allow only a single operation/expression within a specific kind of scope.

### Dynamic Typing
Even though the Polite Framework is written in C#, it still defaults to using dynamic types at runtime. This means that the reader may ignore parsing classes' fields' expressions when parsing them, and that they (if planning to created a statically typed language using this framework) will need to both perform parser-time type-checking and parser-time member creation in the form of injected operations/expressions for a class' properties or methods. For example, in a statically typed language, a new object that might make use of a type-specific Get() method will need the operation that assigns this structure to it to be injected into the parser structure behind the scenes. Since Polite and its languages do not have polymorphism or inheritance, due to its open-ended structure, the parser stage (or any additional stage before the PCR is executed) becomes invaluable for these particular use cases.

# Additional Notes

## Quirks
Because Polite was designed with all possible languages in mind, there a few quirks to note that the reader should keep in mind as to the limitations and services provided by the PCR.

### Dynamic Member Declaration
As a dynamically typed language runtime, the PCR must be capable of performing duties found commonly among pre-existing dynamic languages; this includes dynamic member declaration.
For example, bearing in mind our version of JavaScript, consider the following:

```
var x = new System.Object();
x.Name = "Obby";
```

Even though the `System.Object` class does not define a member 'Name', this operation is still permitted - due to the nature of the interpretation process - and will perform exactly as expected. The reader may find this useful when creating new languages as it adds an entirely new layer to the .NET environment, though it can require a more attentive programmer to manage. For example, if handing off the above variable `x` to another .NET language, such as a C# program, only the inner `System.Object` will be passed so as to prevent type errors.

### Properties
Properties in Polite languages utilize a priority system that the reader should be aware of before creating a new language; take the following case:

```
var x = "Hello, World!";
var y = x.Length;
```
The above will set `y`'s inner `System.Object` to the `System.Int32` that represents the `String.Length` property of `x`. This means that, in the case where...
```
var x = "Hello, World!";
x.Length = 0;
```
... the user will receive a runtime error. This is because .NET `System.Object`s take precedence over Polite members, and so - instead of creating a new member named `Length`, it will attempt to access the inner `System.String`'s field or property first. This will fail because `System.String`'s `Length` property, being a part of an immutable struct, does not have a public `set` accessor. One workaround to be considered is to create a custom string class with a public `set` accessor, reference it in the `Runtime`'s `ReferenceTypes` field, and have all string literal tokens (created during the lexing phase) utilize this new custom class instead.

## A Note to the Reader

If yourself, or anyone you know, finds confusion in the instructions written here - or anywhere within the Polite Framework's documentation - please do not hesitate to let me know. I created this project with the goal in mind to eventually allow anyone to create their own .NET language, and so will naturally mend any ambiguities wherever I see them.

Thanks for reading and supporting the Polite Framework!
