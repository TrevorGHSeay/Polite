using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Polite
{

    /// <summary>
    /// The PCR (<see cref="Polite"/> Common Runtime) used for caching and executing instructions in any <see cref="Polite"/> language.
    /// </summary>
    public partial class Runtime
    {
        /// <summary>
        /// The <see cref="Parser"/> to parse source code / <see cref="Token"/> trees with.
        /// </summary>
        public Parser Parser { get; private set; }

        /// <summary>
        /// The <see cref="Polite.Stack"/> containing every currently utilized <see cref="Variable"/>.
        /// </summary>
        public Stack VariableStack { get; private set; }

        /// <summary>
        /// The collection of all functions declared within a parsed source code or <see cref="Token"/> tree.
        /// </summary>
        internal Dictionary<int, Function> FunctionHeap { get; private set; }

        /// <summary>
        /// Every .NET <see cref="Type"/> that the <see cref="Runtime"/> may allow source code to reference.
        /// </summary>
        public Type[] GlobalReferences;

        /// <summary>
        /// The destination field for <see cref="Populate(string)"/> and <see cref="Populate(Token)"/> methods.
        /// </summary>
        public Dictionary<int, Operation> Instructions;

        /// <summary>
        /// The maximum <see cref="Frame"/> depth that a global <see cref="Variable"/> may exist at and still be stored on the Heap (<see cref="Stack.Bottom"/>).
        /// </summary>
        public int GlobalDepth { get; set; }

        /// <summary>
        /// The bitfield used to reference the behaviour of a <see cref="Variable"/> at the time of interpretation. If this field is changed, you must repopulate the <see cref="Runtime.Instructions"/> for settings to take effect. If no settings are provided, they default to mimic C#. See <see cref="Polite.RuntimeSettings"/>.
        /// </summary>
        public int RuntimeSettings
        {
            get
            {
                return Polite.RuntimeSettings.ToBitMask(this.PassValuesByRef, this.AssignValuesByRef, this.PassObjectsByRef, this.AssignObjectsByRef);
            }
            set
            {
                PassValuesByRef = Polite.RuntimeSettings.Contains(this.RuntimeSettings, Polite.RuntimeSettings.ValueTypes.PassByRef);
                AssignValuesByRef = Polite.RuntimeSettings.Contains(this.RuntimeSettings, Polite.RuntimeSettings.ValueTypes.PassByRef);
                PassObjectsByRef = Polite.RuntimeSettings.Contains(this.RuntimeSettings, Polite.RuntimeSettings.ValueTypes.PassByRef);
                AssignObjectsByRef = Polite.RuntimeSettings.Contains(this.RuntimeSettings, Polite.RuntimeSettings.ValueTypes.PassByRef);
            }
        }

        private bool PassValuesByRef = false;
        private bool AssignValuesByRef = false;
        private bool PassObjectsByRef = true;
        private bool AssignObjectsByRef = true;

        /// <summary>
        /// Creates a new instance of the default <see cref="Runtime"/>.
        /// </summary>
        /// <param name="parser">The <see cref="Parser"/> to use for constructing <see cref="Token"/> trees.</param>
        /// <param name="global_references">Every <see cref="Type"/> that source code you <see cref="Execute()"/> may utilize at <see cref="Runtime"/>.</param>
        public Runtime(Parser parser, Type[] global_references)
        {
            this.Parser = parser;
            this.GlobalReferences = global_references;
            this.FunctionHeap = new Dictionary<int, Function>();
            this.VariableStack = new Stack();
            this.Instructions = new Dictionary<int, Operation>();
        }

        /// <summary>
        /// Resets the <see cref="Runtime"/>'s cached memory banks.
        /// </summary>
        public void Reset()
        {
            //Instructions.Clear();
            FunctionHeap.Clear();
            VariableStack.Reset();
        }

        /// <summary>
        /// Executes the provided source code.
        /// </summary>
        /// <param name="source">The custom language's source code to be tokenized, parsed, and then executed.</param>
        public void Execute(string source)
        {
            this.Populate(source);
            Execute();
        }

        /// <summary>
        /// Executes the provided instruction(s).
        /// </summary>
        /// <param name="instructions">The <see cref="Token"/> tree that expresses <see cref="Runtime"/> expressions to be executed.</param>
        public void Execute(Token instructions)
        {
            Populate(instructions);
            this.Execute();
        }

        /// <summary>
        /// Executes the instructions already pre-processed by the <see cref="Populate(string)"/> or <see cref="Populate(Token)"/> methods.
        /// </summary>
        public void Execute()
        {
            Dictionary<int, Operation> instructions = this.Instructions;
            for (int i = 0; i < instructions.Count; i += 1)
                instructions[i].Invoke();
        }

        /// <summary>
        /// Processes and caches the provided source code.
        /// </summary>
        /// <param name="source">The source code to be processed and cached, which you can later <see cref="Execute()"/>.</param>
        public void Populate(string source)
        {
            this.Parser.Parse(source);
            this.Populate(this.Parser.Result);
        }
        
        /// <summary>
        /// Processes and caches the provided source code.
        /// </summary>
        /// <param name="source">The <see cref="Token"/> tree to be processed and cached, which you can later <see cref="Execute()"/>.</param>
        public void Populate(Token source)
        {
            this.Reset();
            this.Instructions.Clear();

            VariableCollection globalsCache = new VariableCollection();
            VariableCollection localsCache = globalsCache;
            Operation instr = GetInstructions(this.Parser.Result, globalsCache, localsCache, out Variable variable, out Operation last_operation);
            Delegate[] instrArray = instr.GetInvocationList();

            for (int i = 0; i < instrArray.Length; i += 1)
                this.Instructions.Add(i, (Operation)instrArray[i]);
        }

        private Operation GetInstructions(Token token, VariableCollection globals_cache, VariableCollection locals_cache, out Variable last_variable, out Operation last_instruction)
        {

            Token first = token?.First;
            Token second = token?.Second;
            Token third = token?.Third;

            Operation result = null;

            if (token.PrimaryType == Token.Types.Primary.Identifier ||
                    token.SecondaryType == Token.Types.Secondary.Variable.Addresser ||
                    token.PrimaryType == Token.Types.Primary.Literal ||
                    token.PrimaryType == Token.Types.Primary.Constant ||
                    token.SecondaryType == Token.Types.Secondary.Variable.ParentReference)
            {
                Variable variable = null;

                List<Variable> address = new List<Variable>();
                bool isNew = TryCreateNewVariable(token, address, globals_cache, locals_cache, out variable);

                last_variable = variable;
                last_instruction = null;
                if (isNew)
                    result = () => { variable.Reinitialize(); return variable; };
                else
                    result = () => { return variable; };
                return result;
            }

            else if (token.SecondaryType == Token.Types.Secondary.Orders.Condition)
            {
                Operation condition = GetInstructions(first, globals_cache, locals_cache, out last_variable, out last_instruction);
                Operation ifSuccessful = GetInstructions(second, globals_cache, locals_cache, out last_variable, out last_instruction);
                Operation otherwise = third is null ? null : GetInstructions(third, globals_cache, locals_cache, out last_variable, out last_instruction);

                if (third is null)
                {
                    return () =>
                    {
                        if ((bool)condition.Invoke())
                            ifSuccessful.Invoke();
                        return null;
                    };
                }
                else
                {
                    return () =>
                    {
                        if ((bool)condition.Invoke())
                            ifSuccessful.Invoke();
                        else
                            otherwise.Invoke();
                        return null;
                    };
                }
            }

            else if (token.SecondaryType == Token.Types.Secondary.Orders.Loop)
            {
                Operation condition = GetInstructions(first, globals_cache, locals_cache, out last_variable, out last_instruction);
                Operation ifSuccessful = GetInstructions(second, globals_cache, locals_cache, out last_variable, out last_instruction);

                return () =>
                {
                    while ((bool)condition.Invoke())
                        ifSuccessful.Invoke();
                    return null;
                };
            }

            else if (token.SecondaryType == Token.Types.Secondary.Function.Invoker)
            {
                last_instruction = null;
                return NewInvocation(token, first, third, globals_cache, locals_cache, out last_variable);
            }

            else if (token.SecondaryType == Token.Types.Secondary.Variable.Indexer)
            {
                last_instruction = null;
                return NewIndexer(token, globals_cache, locals_cache, out last_variable);
            }

            else if(token.PrimaryType == Token.Types.Primary.Function)
            {
                last_instruction = null;
                return NewCustomFunction(first, second, globals_cache, locals_cache, out last_variable);
            }

            else if(token.SecondaryType == Token.Types.Secondary.Function.Result)
            {
                Operation functionResult;
                if (first is null)
                {
                    functionResult = null;
                    last_variable = null;
                    last_instruction = null;

                    return () => null;
                }
                else
                {
                    functionResult = GetInstructions(first, globals_cache, locals_cache, out last_variable, out last_instruction);

                    string functionResultName = Token.Types.Secondary.ToPUA(Token.Types.Secondary.Function.Result).ToString();

                    if (!locals_cache.TryGetVariable(functionResultName, out Variable functionResultVar))
                    {
                        functionResultVar = new Variable(functionResultName);
                        locals_cache.AddVariable(functionResultVar);
                    }
                    
                    return () =>
                    {
                        Function funcInv = ((Function)functionResultVar.Container);
                        object rslt = functionResult();
                        if (rslt is Function rsltFunc)
                            funcInv.Result = rsltFunc.Result;
                        else if (rslt is Variable rsltVar)
                        {
                            funcInv.Result = rsltVar;
                        }
                        else
                        {
                            Variable v = new Variable(string.Empty); v.Container.Value = rslt;
                            funcInv.Result = v;
                        }
                        return functionResultVar;
                    };
                }
            }

            else if (token.SecondaryType == Token.Types.Secondary.Variable.Referencer)
            {
                last_variable = null;
                last_instruction = null;
                return null;
            }

            else
                last_variable = null;

            Operation    firstInstruction = null,   secondInstruction = null,   thirdInstruction = null;
            Operation    firstOperation = null,     secondOperation = null,     thirdOperation = null;
            Variable     firstVariable = null,      secondVariable = null,      thirdVariable = null;

            if (!(first is null))
                firstInstruction = GetInstructions(first, globals_cache, locals_cache, out firstVariable, out firstOperation);
            if (!(second is null))
                secondInstruction = GetInstructions(second, globals_cache, locals_cache, out secondVariable, out secondOperation);
            if (!(third is null))
                thirdInstruction = GetInstructions(third, globals_cache, locals_cache, out thirdVariable, out thirdOperation);

            if (OperationFactory.RequiresOperation(token.PrimaryType, token.SecondaryType))
            {
                Operation operation = null;
                bool created = false;
                if (firstVariable is null)
                {
                    if (secondVariable is null)
                        created = OperationFactory.TryCreate(firstInstruction, token.SecondaryType, secondInstruction, this.RuntimeSettings, out operation);
                    else
                    {
                        created = OperationFactory.TryCreate(firstInstruction, token.SecondaryType, secondVariable, this.RuntimeSettings, out operation);
                        if (!(secondInstruction is null))
                            result += secondInstruction;
                    }
                }
                else
                {
                    if (secondVariable is null)
                    {
                        created = OperationFactory.TryCreate(firstVariable, token.SecondaryType, secondInstruction, this.RuntimeSettings, out operation);
                        if (!(firstInstruction is null))
                            result += firstInstruction;
                    }
                    else
                    {
                        created = OperationFactory.TryCreate(firstVariable, token.SecondaryType, secondVariable, this.RuntimeSettings, out operation);
                        if (!(firstInstruction is null))
                            result += firstInstruction;
                        if (!(secondInstruction is null))
                            result += secondInstruction;
                    }
                }

                result += last_instruction = operation;
                return result;
            }

            if (token.HasChildren)
            {
                for (int i = 0; i < token.Children.Count; i += 1)
                {
                    var toAdd = GetInstructions(token.Children[i], globals_cache, locals_cache, out last_variable, out last_instruction);
                    if (!(toAdd is null))
                        result += toAdd;
                }
            }

            last_instruction = null;
            return result;
        }

        private Operation NewIndexer(Token token, VariableCollection globals_cache, VariableCollection locals_cache, out Variable last_variable)
        {
            Token left = token.First;
            List<Token> tokenIndices = token.Second.Children;

            List<Variable> address = new List<Variable>();
            
            TryCreateNewVariable(left, address, globals_cache, locals_cache, out Variable variable);

            Variable[] indices = new Variable[tokenIndices.Count];
            for (int i = 0; i < indices.Length; i += 1)
            {
                Token index = tokenIndices[i];

                address.Clear();
                
                TryCreateNewVariable(index, address, globals_cache, locals_cache, out Variable variableIndexer);

                indices[i] = variableIndexer;
            }

            bool foundIndexers = this.TryFindIndexers(tokenIndices.Count, out Dictionary<Type, PropertyInfo> typeIndexers);

            //if (!foundIndexers) // Assume the native object has created an indexer in source and so look for that

            object[] args = new object[indices.Length];

            RefContainer indexContainer = new RefContainer();
            Variable indexerVariable = new Variable(address + "->[i]");
            indexerVariable.Container = indexContainer;

            indexContainer.Get = () =>
            {
                Type type = variable.Container.Value.GetType();

                if (!typeIndexers.TryGetValue(type, out PropertyInfo typeIndexer))
                    throw new Exception("Externally referenced indexer has some invalid arguments:\n" + variable.Container.Value.GetType().Name + "[" + indices.Aggregate(string.Empty, (o, v) => o + (string.IsNullOrEmpty(o) ? string.Empty : ", ") + v.Container.Value.GetType().ToString()) + "]");

                for (int i = 0; i < args.Length; i += 1)
                    args[i] = indices[i].Container.Value;

                return typeIndexer.GetValue(variable.Container.Value, args);
            };

            indexContainer.Set = (val) =>
            {
                Type type = variable.Container.Value.GetType();

                if (!typeIndexers.TryGetValue(type, out PropertyInfo typeIndexer))
                    throw new Exception("Externally referenced indexer has some invalid arguments:\n" + variable.Container.Value.GetType().Name + "[" + indices.Aggregate(string.Empty, (o, v) => o + (string.IsNullOrEmpty(o) ? string.Empty : ", ") + v.Container.Value.GetType().ToString()) + "]");

                for (int i = 0; i < args.Length; i += 1)
                    args[i] = indices[i].Container.Value;

                typeIndexer.SetValue(variable.Container.Value, val, args);
            };
            
            locals_cache.AddVariable(indexerVariable);

            last_variable = indexerVariable;

            return () => indexerVariable;
        }

        private Operation NewCustomFunction(Token left, Token second, VariableCollection globals_cache, VariableCollection locals_cache, out Variable function_variable)
        {
            VariableCollection innerCache = new VariableCollection();

            Variable[] parameters = new Variable[left.Children.Count];
            for (int i = 0; i < left.Children.Count; i += 1)
            {
                List<Variable> adr = new List<Variable>();
                TryCreateNewVariable(left.Children[i], adr, globals_cache, innerCache, out Variable param);
                parameters[i] = param;
            }

            Operation functionBody = GetInstructions(second, globals_cache, innerCache, out function_variable, out Operation instruction);
            
            function_variable = new Variable(string.Empty);
            function_variable.Container = new Function() { Parameters = parameters };
            Function function = function_variable.Container as Function;

            string parentName = Token.Types.Secondary.ToPUA(Token.Types.Secondary.Variable.ParentReference).ToString();
            string resultName = Token.Types.Secondary.ToPUA(Token.Types.Secondary.Function.Result).ToString();

            int parentHashCode = parentName.GetHashCode();
            int resultHashCode = resultName.GetHashCode();

            Dictionary<int, Operation> functionBodyInstructions = new Dictionary<int, Operation>();
            Delegate[] instructions = functionBody.GetInvocationList();

            for (int i = 0; i < instructions.Length; i += 1)
                functionBodyInstructions.Add(i, (Operation)instructions[i]);

            if (innerCache.TryGetVariable(resultName, out Variable resultVariable))
            {
                function.ReturnsNull = false;
                resultVariable.Container = function_variable.Container;
            }
            else
                function.ReturnsNull = true;

            function.Running = true;
            function.Operation = () =>
            {
                this.VariableStack.Push();

                for (int j = 0; j < function.Parameters.Length; j += 1)
                    function.Parameters[j].Reinitialize();

                if (!(function.Parent is null))
                    this.VariableStack.Top.Variables.Add(parentHashCode, function.Parent);

                int i = 0;
                function.Running = true; // For recursion
                while (function.Running && i < instructions.Length)
                    functionBodyInstructions[i++].Invoke();

                function.Running = true; // For recursion
                this.VariableStack.Pop();
                return function.LastResult;
            };

            function_variable.Name = "function (" + (function.Parameters.Length > 0 ? function.Parameters.Aggregate(function.Parameters[0].Name, (o, v) => (string.IsNullOrEmpty(o) ? string.Empty : ", ") + v.Name) : string.Empty);
            function_variable.Container = function;
            locals_cache.AddVariable(function_variable);

            return null;
        }

        private Operation NewInvocation(Token invoker, Token left, Token right, VariableCollection globals_cache, VariableCollection locals_cache, out Variable last_variable)
        {

            string addrPUA = Token.Types.Secondary.ToPUA(Token.Types.Secondary.Variable.Addresser).ToString();
            string leftAddressPrinted = Token.PrintAddress(left, addrPUA);
            
            string[] splitAddress = left.SecondaryType == Token.Types.Secondary.Variable.Addresser ? leftAddressPrinted.Split(new string[] { addrPUA }, StringSplitOptions.None) : new string[] { leftAddressPrinted };

            int childCount = right.Children.Count;
            Variable[] args = new Variable[childCount];

            Variable last_reference;
            Operation last_instruction;
            
            for (int i = 0; i < childCount; i += 1)
            {
                Token child = right.Children[i];

                Operation instr = GetInstructions(child, globals_cache, locals_cache, out last_reference, out last_instruction);

                if (!(last_reference is null) && child.SecondaryType != Token.Types.Secondary.Function.Invoker)
                    args[i] = last_reference;
                else
                {
                    Variable arg = new Variable(string.Empty) { Container = new Function() { Operation = () => { return new Variable(string.Empty) { Container = new Container() { Value = instr() } }; } } };
                    args[i] = arg;
                }

                last_reference = null;
                last_instruction = () => null;
            }

            string frAddress = Token.Types.Secondary.ToPUA(Token.Types.Secondary.Function.Result).ToString();
            int frAlias = frAddress.GetHashCode();

            List<Variable> address = new List<Variable>();

            bool isStaticMethod = IsStaticReference(splitAddress);

            bool foundMethod = isStaticMethod ? TryFindStaticReferences(splitAddress, args.Length, out List<MethodBase> methods) : TryFindMethods(splitAddress[splitAddress.Length - 1], args.Length, out methods);
            
            if (foundMethod)
            {
                object[] a = new object[args.Length];

                string invokerName = string.Join(".", splitAddress);
                
                bool argsEmpty = a.Length == 0;

                Variable variable = null;

                if (!isStaticMethod)
                {
                    left = left.First;

                    address.Clear();

                    TryCreateNewVariable(left, address, globals_cache, locals_cache, out variable);
                }

                Variable resultVariable = new Variable(frAddress);

                MethodInvoker internalMethod = new MethodInvoker
                {
                    Operation = () =>
                    {
                        for (int i = 0; i < a.Length; i += 1)
                        {
                            object arg = args[i];
                            if (arg is Variable varArg)
                            {
                                if (varArg.Container is Function funcArg)
                                {
                                    funcArg.Running = true;
                                    arg = funcArg.Value;
                                }
                                else
                                    arg = varArg.Container.Value;
                            }
                            else if (arg is Operation opArg)
                                arg = opArg.Invoke();

                            a[i] = arg;
                        }

                        foundMethod = argsEmpty ? true : false;
                        MethodBase method = argsEmpty ? methods[0] : null;
                        for (int i = 0; i < methods.Count; i += 1) // TODO: Replace with Dictionary< Type, KeyValuePair<string, Func<object[], object> >>
                        {
                            method = methods[i];
                            ParameterInfo[] pars = method.GetParameters();

                            int j = 0;
                            for (; j < pars.Length; j += 1)
                            {
                                Type t1 = a[j].GetType();
                                Type t2 = pars[j].ParameterType;
                                if (t1 != t2 && !t1.IsSubclassOf(t2))
                                    break;
                            }
                            if (j == pars.Length)
                            {
                                foundMethod = true;
                                break;
                            }
                        }

                        if (!foundMethod)
                            throw new Exception("Externally referenced method has some invalid arguments:\n" + methods[0].Name + "(" + a.Aggregate(string.Empty, (o, v) => o + (string.IsNullOrEmpty(o) ? string.Empty : ", ") + v.GetType().ToString()) + ");");
                        
                        return method.IsConstructor ? ((ConstructorInfo)method).Invoke(a) : method.Invoke(isStaticMethod ? null : variable.Container.Value, a);
                    }
                };

                last_variable = new Variable(splitAddress[splitAddress.Length - 1]) { Container = internalMethod };

                return () => internalMethod.Operation.Invoke();
            }

            if (!locals_cache.TryGetVariable(splitAddress[0], out Variable parentVariable))
                throw new Exception("Parent variable named '" + splitAddress[0] + "' could not be found.");

            string varName = Token.PrintAddress(left, Token.Types.Secondary.ToPUA(Token.Types.Secondary.Variable.Addresser).ToString());
            
            if (!locals_cache.TryGetVariable(varName, out last_variable))
            {
                GetInstructions(left, globals_cache, locals_cache, out last_variable, out last_instruction);
            }

            last_variable.Container = new FunctionInvoker();
            Variable functionVariable = last_variable;
            parentVariable = new Variable(Token.Types.Secondary.ToPUA(Token.Types.Secondary.Variable.ParentReference).ToString()) { Container = parentVariable.Container };
            
            return () => 
            {
                Function function = (Function)functionVariable.Container;
                
                int min = function.Parameters.Length < args.Length ? function.Parameters.Length : args.Length;
                for (int i = 0; i < min; i += 1)
                {
                    Variable p = function.Parameters[i];
                    Variable arg = args[i];
                    if (p.Container.Value is ValueType)
                    {
                        if (this.PassValuesByRef)
                            p.Container = arg.Container;
                        else
                            p.Container.Value = arg.Container.Value;
                    }
                    else
                    {
                        object o = 0;
                        if (this.PassObjectsByRef)
                            p.Container = arg.Container;
                        else
                            p.Container.Value = arg.Container.Value.MemberwiseClone();
                    }
                }
                function.Parent = parentVariable;

                if (function.ReturnsNull)
                    function.Operation.Invoke();
                
                return null;
            };
        }

        /// <summary>
        /// Creates a new instance of <see cref="Variable"/>. Returns true if new, or false if it is known to already exist.
        /// </summary>
        /// <param name="token">The <see cref="Token"/> to use as a template.</param>
        /// <param name="address">The raw address of the new <see cref="Variable"/>.</param>
        /// <param name="globals_cache">The <see cref="VariableCollection"/> to use for creation of every global <see cref="Variable"/>.</param>
        /// <param name="locals_cache">The <see cref="VariableCollection"/> to use for creation of every local <see cref="Variable"/>.</param>
        /// <param name="result">The resulting <see cref="Variable"/>.</param>
        /// <returns>True if <see cref="Variable"/> result was newly created. Otherwise, false.</returns>
        private bool TryCreateNewVariable(Token token, List<Variable> address, VariableCollection globals_cache, VariableCollection locals_cache, out Variable result)
        {
            if (token.PrimaryType == Token.Types.Primary.Identifier ||
                token.PrimaryType == Token.Types.Primary.Literal ||
                token.PrimaryType == Token.Types.Primary.Constant ||
                token.SecondaryType == Token.Types.Secondary.Variable.ParentReference)
            {

                bool isParent = address.Count == 0;
                string declaredName = token.Name;

                Variable variable = null;
                bool isDeclaration = token.SecondaryType == Token.Types.Secondary.Variable.Declaration;
                if (!isDeclaration)
                {
                    if (locals_cache.TryGetVariable(declaredName, out variable))
                    {
                        result = variable;
                        address.Add(variable);
                        return false;
                    }
                    else if (globals_cache.TryGetVariable(declaredName, out variable))
                    {
                        result = variable;
                        address.Add(variable);
                        return false;
                    }
                }

                bool isNew = false;

                variable = new Variable(declaredName);
                int varHashCode = declaredName.GetHashCode();

                string parentReferenceName = Token.Types.Secondary.ToPUA(Token.Types.Secondary.Variable.ParentReference).ToString();
                int parentReferenceHashCode = parentReferenceName.GetHashCode();

                if (token.SecondaryType == Token.Types.Secondary.Variable.ParentReference)
                {
                    declaredName = parentReferenceName;
                    varHashCode = declaredName.GetHashCode();

                    variable = NewVariable(declaredName, locals_cache);
                    locals_cache.AddParameter(variable);

                    variable.Reinitialize = () => VariableStack.Top.Variables.Add(variable.Alias, variable);
                }
                else if (token.PrimaryType == Token.Types.Primary.Literal || token.PrimaryType == Token.Types.Primary.Constant)
                {
                    if (!isParent)
                        goto Error;

                    variable.Container.Value = token.Value;
                    locals_cache.AddVariable(variable);
                    result = variable;
                    return false;
                }
                else
                    variable = NewVariable(declaredName, locals_cache);

                if (isParent)
                {
                    isNew = !locals_cache.TryFind(declaredName, out int index);
                    if (isNew)
                    {
                        variable.Reinitialize = () => VariableStack.Top.Variables.Add(variable.Alias, variable);
                        locals_cache.AddVariable(variable);
                    }
                }
                else
                {
                    int[] aliasPath = new int[address.Count];

                    aliasPath[0] = address[0].Alias;

                    Variable parent = address[0];
                    string varName = string.Empty;
                    for (int i = 1; i < aliasPath.Length; i += 1)
                    {
                        parent = address[i];
                        aliasPath[i] = parent.Alias;
                    }
                    
                    isNew = !parent.Container.Members.TryGetValue(varHashCode, out result);

                    // Add parent to locals_cache if not there already
                    if (isNew)
                    {
                        variable.Reinitialize = () =>
                        {
                            Variable p = this.VariableStack.Top.Variables[aliasPath[0]];
                            
                            for (int i = 1; i < aliasPath.Length; i += 1)
                            {
                                p = p.Container.Members[aliasPath[i]];
                            }

                            bool pContainsVariable = p.Container.Members.TryGetValue(variable.Alias, out Variable currentVariable);
                            if (!(p.Container.Value is null))
                            {
                                Type valType = p.Container.Value.GetType();
                                MemberInfo[] mis = valType.GetMember(variable.Name);

                                if (mis.Length > 0)
                                {
                                    bool found = false;
                                    Operation getter = () => null;
                                    Action<object> setter = (arg) => { };

                                    for (int i = 0; i < mis.Length; i += 1)
                                    {
                                        MemberInfo mi = mis[i];
                                        if (mi is FieldInfo fi)
                                        {
                                            if (fi.IsPublic)
                                            {
                                                found = true;
                                                getter = () => fi.GetValue(p.Container.Value);

                                                setter = (arg) => fi.SetValue(p.Container.Value, arg);
                                            }
                                        }
                                        else if (mi is PropertyInfo pi)
                                        {
                                            MethodInfo metIg = pi.GetMethod;
                                            if (!(metIg is null) && pi.CanRead)
                                            {
                                                found = true;
                                                getter = () => metIg.Invoke(p.Container.Value, Array.Empty<object>());
                                            }

                                            MethodInfo metIs = pi.SetMethod;
                                            if (!(metIs is null) && pi.CanWrite)
                                            {
                                                found = true;
                                                setter = (arg) => metIs.Invoke(p.Container.Value, new object[] { arg });
                                            }
                                        }
                                    }

                                    if (found)
                                    {
                                        p.Container.HasInternalMembers = true;
                                        RefContainer rc = new RefContainer(getter, setter);
                                        variable.Container = rc;
                                    }
                                    else
                                        variable.Container = new Container();

                                    if (pContainsVariable)
                                        p.Container.Members[variable.Alias] = variable;
                                    else
                                        p.Container.Members.Add(variable.Alias, variable);

                                    return;
                                }
                            }

                            if (variable.Container is RefContainer)
                                variable.Container = new Container();

                            if (pContainsVariable)
                                variable.Container = p.Container.Members[variable.Alias].Container;
                            else
                                p.Container.Members.Add(variable.Alias, variable);
                        };

                        parent.Container.Members.Add(varHashCode, variable);
                    }
                    else
                        variable = result;
                }
                address.Add(variable);
                result = variable;
                return isNew;
            }
            else if (token.SecondaryType == Token.Types.Secondary.Variable.Addresser)
            {
                TryCreateNewVariable(token.First, address, globals_cache, locals_cache, out result);
                TryCreateNewVariable(token.Second, address, globals_cache, locals_cache, out result);
                return !(result is null);
            }

            Error: // TODO: Allow method return value pathing like: response.ToLower().IndexOf();
            throw new Exception("Insufficient variable construction attempted: '" + Token.PrintAddress(token, "->") + "' of Token.Types.Primary " + token.PrimaryType + " and Token.Types.Secondary '" + token.SecondaryType + " does not match any of the accepted TokenTypes that form a variable.\nYou may only use Literals, Constants, or a combination of alternating Identifiers / Addressers.");
        }

        private Variable NewVariable(string name, VariableCollection locals_cache)
        {
            if (locals_cache.TryGetVariable(name, out Variable variable))
                return variable;

            return new Variable(name);
        }
        
        private bool TryFindType(string full_name, out Type type)
        {
            type = null;

            for (int i = 0; i < this.GlobalReferences.Length; i += 1)
            {
                Type currType = this.GlobalReferences[i];
                if (currType.FullName.CompareTo(full_name) == 0)
                {
                    type = currType;
                    return true;
                }
            }
            return false;
        }
        

        // TODO: Validate types against this.GlobalReferences
        private bool IsStaticReference(string[] address)
        {
            int end = address.Length - 1;
            string curAddr = string.Empty;

            bool foundType = false;
            Type curType = null;
            for (int i = 0; i < address.Length; i += 1)
            {
                string addrFrag = address[i];
                curAddr += addrFrag;
                for (int j = 0; j < this.GlobalReferences.Length; j += 1)
                {
                    curType = this.GlobalReferences[j];
                    if (curType.FullName.CompareTo(curAddr) == 0 || (i == 0 && curType.Name.CompareTo(addrFrag) == 0))
                    {
                        if (i < end)
                        {
                            foundType = true;
                            break;
                        }
                        else
                            return true;
                    }
                }
                if (foundType && i == end - 1)
                {
                    addrFrag = address[end];
                    ConstructorInfo[] ci = curType.GetConstructors();
                    MethodInfo[] mi = curType.GetMethods();
                    MethodBase[] curMets = new MethodBase[ci.Length + mi.Length];
                    if (curMets.Length > 0)
                    {
                        ci.CopyTo(curMets, 0);
                        mi.CopyTo(curMets, ci.Length);
                        for (int j = 0; j < curMets.Length; j += 1)
                        {
                            MethodBase method = curMets[j];
                            if (method.Name.CompareTo(addrFrag) == 0)
                            {
                                if (method is MethodInfo methodInfo)
                                {
                                    if (this.GlobalReferences.Contains(methodInfo.ReturnType))
                                        return true;
                                }
                                else if (method is ConstructorInfo constructorInfo)
                                {
                                    if (this.GlobalReferences.Contains(constructorInfo.DeclaringType))
                                        return true;
                                }
                            }
                        }
                    }
                }

                curAddr += ".";
            }
            return false;
        }

        private bool TryFindMethods(string method_name, int args, out List<MethodBase> methods)
        {
            Type[] types = this.GlobalReferences;

            methods = new List<MethodBase>();
            for (int i = 0; i < types.Length; i += 1)
            {
                Type curType = types[i];
                MethodInfo[] typeMethods = curType.GetMethods();
                for (int j = 0; j < typeMethods.Length; j += 1)
                {
                    MethodInfo method = typeMethods[j];
                    if (method.IsPublic && method.Name.CompareTo(method_name) == 0 && method.GetParameters().Length == args && this.GlobalReferences.Contains(method.ReturnType))
                        methods.Add(method);
                }
            }
            return methods.Count != 0;
        }

        private bool TryFindStaticReferences(string[] address, int args, out List<MethodBase> methods)
        {
            methods = new List<MethodBase>();

            int end = address.Length - 1;
            if (end < 0)
                return false;

            bool foundName = false;
            Type typeFound = null;
            
            string name = address[end];
            string path = string.Join(".", address, 0, end);

            for (int i = 0; i < this.GlobalReferences.Length; i += 1)
            {
                Type curType = this.GlobalReferences[i];

                if (curType.Name.CompareTo(name) == 0 || curType.Name.CompareTo(path) == 0)
                {
                    if (foundName)
                        throw new Exception("Identifier is ambiguous between '" + curType.FullName + "' and '" + typeFound.FullName + "'.");

                    foundName = true;
                    typeFound = curType;
                }

                if (foundName || (!string.IsNullOrEmpty(path) && curType.FullName.IndexOf(path) == 0))
                {

                    ConstructorInfo[] ci = curType.GetConstructors();
                    MethodInfo[] mi = curType.GetMethods();
                    MethodBase[] curMets = new MethodBase[ci.Length + mi.Length];
                    if (curMets.Length > 0)
                    {
                        ci.CopyTo(curMets, 0);
                        mi.CopyTo(curMets, ci.Length);
                        for (int j = 0; j < curMets.Length; j += 1)
                        {
                            MethodBase curMet = curMets[j];
                            if (
                                curMet.GetParameters().Length == args &&
                                    (curMet.Name.CompareTo(name) == 0 ||
                                    (curMet.IsConstructor && name.CompareTo(curType.Name) == 0))
                                )
                            {
                                if (curMet is MethodInfo methodInfo)
                                {
                                    if (this.GlobalReferences.Contains(methodInfo.ReturnType))
                                        methods.Add(curMet);
                                }
                                else if (curMet is ConstructorInfo constructorInfo)
                                {
                                    if (this.GlobalReferences.Contains(constructorInfo.DeclaringType))
                                        methods.Add(curMet);
                                }
                            }
                        }
                    }
                    return methods.Count != 0;
                }
            }

            return false;
        }




        private bool TryFindIndexers(int args, out Dictionary<Type, PropertyInfo> indexers)
        {
            indexers = new Dictionary<Type, PropertyInfo>();
            for (int i = 0; i < this.GlobalReferences.Length; i += 1)
            {
                Type currType = this.GlobalReferences[i];
                PropertyInfo propInfo = currType.GetProperty("Item");
                if (!(propInfo is null))
                {
                    if (this.GlobalReferences.Contains(propInfo.PropertyType))
                        indexers.Add(currType, propInfo);
                }
            }
            return indexers.Count > 0;
        }

    }

}
