using System;
using System.Collections.Generic;
using System.Runtime;

using Settings = Polite.RuntimeSettings;

namespace Polite
{

    public partial class Runtime
    {

        /// <summary>
        /// Defines an operation that always returns an internal value (that is, not a Polite <see cref="Variable"/> but rather a .NET <see cref="object"/>), except in the case of assignments.
        /// </summary>
        /// <returns>A .NET <see cref="object"/> that is the result of the operation.</returns> //TODO: Make a different delegate type for assignments?
        public delegate dynamic Operation();

        /// <summary>
        /// The master class that creates an <see cref="Operation"/> from a defined expression.
        /// </summary>
        public static class OperationFactory
        {

            /// <summary>
            /// Determines if a <see cref="Token.PrimaryType"/> and <see cref="Token.SecondaryType"/> require and operation to process its instruction. See <see cref="Token.Types"/>
            /// </summary>
            /// <param name="primary_type">The <see cref="Token.Types.Primary"/> to analyze.</param>
            /// <param name="secondary_type">The <see cref="Token.Types.Secondary"/> to analyze.</param>
            /// <returns>Returns <see cref="true"/> if the <see cref="Token"/> that contains these <see cref="Token.Types"/> requires an operation to process it instruction.</returns>
            public static bool RequiresOperation(short primary_type, short secondary_type)
            {
                return primary_type == Token.Types.Primary.Operator &&
                    Token.Types.Secondary.IsAssignmentType(secondary_type) ||
                    Token.Types.Secondary.IsArithmeticType(secondary_type) ||
                    Token.Types.Secondary.IsConditionType(secondary_type) ||
                    Token.Types.Secondary.IsComparisonType(secondary_type) ||
                    Token.Types.Secondary.IsBitwiseType(secondary_type);
            }

            /// <summary>
            /// Attempts to create an <see cref="Operation"/> using the given parameters.
            /// </summary>
            /// <param name="x">The <see cref="Variable"/> on the left side of the operator.</param>
            /// <param name="operator_type">The <see cref="Token.Types.Secondary"/> of the operator that the operation is being made on behalf of.</param>
            /// <param name="y">The <see cref="Variable"/> on the right side of the operator.</param>
            /// <param name="runtime_settings">The <see cref="Runtime.RuntimeSettings"/> used to properly create the intended <see cref="Operation"/>.</param>
            /// <param name="result">The resulting <see cref="Operation"/>, if creation was successful.</param>
            /// <returns>Returns <see cref="true"/> if creation was successful, and otherwise false.</returns>
            public static bool TryCreate(Variable x, short operator_type, Variable y, int runtime_settings, out Operation result)
            {

                if (Token.Types.Secondary.IsAssignmentType(operator_type))
                    result = Assignment(x, operator_type, y, runtime_settings);
                else if (Token.Types.Secondary.IsArithmeticType(operator_type))
                    result = Arithmetic(x, operator_type, y);
                else if (Token.Types.Secondary.IsConditionType(operator_type))
                    result = Condition(x, operator_type, y);
                else if (Token.Types.Secondary.IsComparisonType(operator_type))
                    result = Comparison(x, operator_type, y);
                else if (Token.Types.Secondary.IsBitwiseType(operator_type))
                    result = Bitwise(x, operator_type, y);
                else
                    result = null;

                return result is null;
            }

            /// <summary>
            /// Attempts to create an <see cref="Operation"/> using the given parameters.
            /// </summary>
            /// <param name="x">The <see cref="Variable"/> on the left side of the operator.</param>
            /// <param name="operator_type">The <see cref="Token.Types.Secondary"/> of the operator that the operation is being made on behalf of.</param>
            /// <param name="y">The <see cref="Operation"/> on the right side of the operator.</param>
            /// <param name="runtime_settings">The <see cref="Runtime.RuntimeSettings"/> used to properly create the intended <see cref="Operation"/>.</param>
            /// <param name="result">The resulting <see cref="Operation"/>, if creation was successful.</param>
            /// <returns>Returns <see cref="true"/> if creation was successful, and otherwise false.</returns>
            public static bool TryCreate(Variable x, short operator_type, Operation y, int runtime_settings, out Operation result)
            {

                if (Token.Types.Secondary.IsAssignmentType(operator_type))
                    result = Assignment(x, operator_type, y, runtime_settings);
                else if (Token.Types.Secondary.IsArithmeticType(operator_type))
                    result = Arithmetic(x, operator_type, y);
                else if (Token.Types.Secondary.IsConditionType(operator_type))
                    result = Condition(x, operator_type, y);
                else if (Token.Types.Secondary.IsComparisonType(operator_type))
                    result = Comparison(x, operator_type, y);
                else if (Token.Types.Secondary.IsBitwiseType(operator_type))
                    result = Bitwise(x, operator_type, y);
                else
                    result = null;

                return result is null;
            }

            /// <summary>
            /// Attempts to create an <see cref="Operation"/> using the given parameters.
            /// </summary>
            /// <param name="x">The <see cref="Operation"/> on the left side of the operator.</param>
            /// <param name="operator_type">The <see cref="Token.Types.Secondary"/> of the operator that the operation is being made on behalf of.</param>
            /// <param name="y">The <see cref="Variable"/> on the right side of the operator.</param>
            /// <param name="runtime_settings">The <see cref="Runtime.RuntimeSettings"/> used to properly create the intended <see cref="Operation"/>.</param>
            /// <param name="result">The resulting <see cref="Operation"/>, if creation was successful.</param>
            /// <returns>Returns <see cref="true"/> if creation was successful, and otherwise false.</returns>
            public static bool TryCreate(Operation x, short operator_type, Variable y, int runtime_settings, out Operation result)
            {

                if (Token.Types.Secondary.IsArithmeticType(operator_type))
                    result = Arithmetic(x, operator_type, y);
                else if (Token.Types.Secondary.IsConditionType(operator_type))
                    result = Condition(x, operator_type, y);
                else if (Token.Types.Secondary.IsComparisonType(operator_type))
                    result = Comparison(x, operator_type, y);
                else if (Token.Types.Secondary.IsBitwiseType(operator_type))
                    result = Bitwise(x, operator_type, y);
                else
                    result = null;

                return result is null;
            }

            /// <summary>
            /// Attempts to create an <see cref="Operation"/> using the given parameters.
            /// </summary>
            /// <param name="x">The <see cref="Operation"/> on the left side of the operator.</param>
            /// <param name="operator_type">The <see cref="Token.Types.Secondary"/> of the operator that the operation is being made on behalf of.</param>
            /// <param name="y">The <see cref="Operation"/> on the right side of the operator.</param>
            /// <param name="runtime_settings">The <see cref="Runtime.RuntimeSettings"/> used to properly create the intended <see cref="Operation"/>.</param>
            /// <param name="result">The resulting <see cref="Operation"/>, if creation was successful.</param>
            /// <returns>Returns <see cref="true"/> if creation was successful, and otherwise false.</returns>
            public static bool TryCreate(Operation x, short operator_type, Operation y, int runtime_settings, out Operation result)
            {

                if (Token.Types.Secondary.IsArithmeticType(operator_type))
                    result = Arithmetic(x, operator_type, y);
                else if (Token.Types.Secondary.IsConditionType(operator_type))
                    result = Condition(x, operator_type, y);
                else if (Token.Types.Secondary.IsComparisonType(operator_type))
                    result = Comparison(x, operator_type, y);
                else if (Token.Types.Secondary.IsBitwiseType(operator_type))
                    result = Bitwise(x, operator_type, y);
                else
                    result = null;

                return result is null;
            }

            /// <summary>
            /// Creates an arithmetic <see cref="Operation"/> using x and y, given a <see cref="Token.Types.Secondary.Operations"/> operator_type.
            /// </summary>
            /// <param name="x">The <see cref="Variable"/> on the left side of the <see cref="Operation"/>.</param>
            /// <param name="operator_type">The <see cref="Token.SecondaryType"/> defining which of the <see cref="Token.Types.Secondary.Operations"/> to create.</param>
            /// <param name="y">The <see cref="Variable"/> on the right side of the <see cref="Operation"/>.</param>
            /// <returns>An <see cref="Operation"/> representing that which was passed as arguments.</returns>
            public static Operation Arithmetic(Variable x, short operator_type, Variable y)
            {
                switch (operator_type)
                {
                    case Token.Types.Secondary.Operations.Addition: return () => x.Container.Value + y.Container.Value;
                    case Token.Types.Secondary.Operations.Subtraction: return () => x.Container.Value - y.Container.Value;
                    case Token.Types.Secondary.Operations.Multiplication: return () => x.Container.Value * y.Container.Value;
                    case Token.Types.Secondary.Operations.Division: return () => x.Container.Value / y.Container.Value;
                }

                throw new Exception("Could not create arithmetic operation. Invalid operator_type " + operator_type + ".");
            }

            /// <summary>
            /// Creates an arithmetic <see cref="Operation"/> using x and y, given a <see cref="Token.Types.Secondary.Operations"/> operator_type.
            /// </summary>
            /// <param name="x">The <see cref="Variable"/> on the left side of the <see cref="Operation"/>.</param>
            /// <param name="operator_type">The <see cref="Token.SecondaryType"/> defining which of the <see cref="Token.Types.Secondary.Operations"/> to create.</param>
            /// <param name="y">The <see cref="Operation"/> on the right side of the <see cref="Operation"/>.</param>
            /// <returns>An <see cref="Operation"/> representing that which was passed as arguments.</returns>
            public static Operation Arithmetic(Variable x, short operator_type, Operation y)
            {
                switch (operator_type)
                {
                    case Token.Types.Secondary.Operations.Addition: return () => x.Container.Value + y.Invoke();
                    case Token.Types.Secondary.Operations.Subtraction: return () => x.Container.Value - y.Invoke();
                    case Token.Types.Secondary.Operations.Multiplication: return () => x.Container.Value * y.Invoke();
                    case Token.Types.Secondary.Operations.Division: return () => x.Container.Value / y.Invoke();
                }

                throw new Exception("Could not create arithmetic operation. Invalid operator_type " + operator_type + ".");
            }

            /// <summary>
            /// Creates an arithmetic <see cref="Operation"/> using x and y, given a <see cref="Token.Types.Secondary.Operations"/> operator_type.
            /// </summary>
            /// <param name="x">The <see cref="Operation"/> on the left side of the <see cref="Operation"/>.</param>
            /// <param name="operator_type">The <see cref="Token.SecondaryType"/> defining which of the <see cref="Token.Types.Secondary.Operations"/> to create.</param>
            /// <param name="y">The <see cref="Variable"/> on the right side of the <see cref="Operation"/>.</param>
            /// <returns>An <see cref="Operation"/> representing that which was passed as arguments.</returns>
            public static Operation Arithmetic(Operation x, short operator_type, Variable y)
            {
                switch (operator_type)
                {
                    case Token.Types.Secondary.Operations.Addition: return () => x.Invoke() + y.Container.Value;
                    case Token.Types.Secondary.Operations.Subtraction: return () => x.Invoke() - y.Container.Value;
                    case Token.Types.Secondary.Operations.Multiplication: return () => x.Invoke() * y.Container.Value;
                    case Token.Types.Secondary.Operations.Division: return () => x.Invoke() / y.Container.Value;
                }

                throw new Exception("Could not create arithmetic operation. Invalid operator_type " + operator_type + ".");
            }

            /// <summary>
            /// Creates an arithmetic <see cref="Operation"/> using x and y, given a <see cref="Token.Types.Secondary.Operations"/> operator_type.
            /// </summary>
            /// <param name="x">The <see cref="Operation"/> on the left side of the <see cref="Operation"/>.</param>
            /// <param name="operator_type">The <see cref="Token.SecondaryType"/> defining which of the <see cref="Token.Types.Secondary.Operations"/> to create.</param>
            /// <param name="y">The <see cref="Operation"/> on the right side of the <see cref="Operation"/>.</param>
            /// <returns>An <see cref="Operation"/> representing that which was passed as arguments.</returns>
            public static Operation Arithmetic(Operation x, short operator_type, Operation y)
            {
                switch (operator_type)
                {
                    case Token.Types.Secondary.Operations.Addition: return () => x.Invoke() + y.Invoke();
                    case Token.Types.Secondary.Operations.Subtraction: return () => x.Invoke() - y.Invoke();
                    case Token.Types.Secondary.Operations.Multiplication: return () => x.Invoke() * y.Invoke();
                    case Token.Types.Secondary.Operations.Division: return () => x.Invoke() / y.Invoke();
                }

                throw new Exception("Could not create arithmetic operation. Invalid operator_type " + operator_type + ".");
            }


            /// <summary>
            /// Creates a bitwise <see cref="Operation"/> using x and y, given a <see cref="Token.Types.Secondary.Bitwise"/> operator_type.
            /// </summary>
            /// <param name="x">The <see cref="Variable"/> on the left side of the <see cref="Operation"/>.</param>
            /// <param name="operator_type">The <see cref="Token.SecondaryType"/> defining which of the <see cref="Token.Types.Secondary.Operations"/> to create.</param>
            /// <param name="y">The <see cref="Variable"/> on the right side of the <see cref="Operation"/>.</param>
            /// <returns>An <see cref="Operation"/> representing that which was passed as arguments.</returns>
            public static Operation Bitwise(Variable x, short operator_type, Variable y)
            {
                switch (operator_type)
                {
                    case Token.Types.Secondary.Bitwise.Or: return () => x.Container.Value | y.Container.Value;
                    case Token.Types.Secondary.Bitwise.XOr: return () => x.Container.Value ^ y.Container.Value;
                    case Token.Types.Secondary.Bitwise.And: return () => x.Container.Value & y.Container.Value;
                }

                throw new Exception("Could not create bitwise operation. Invalid operator_type " + operator_type + ".");
            }

            /// <summary>
            /// Creates a bitwise <see cref="Operation"/> using x and y, given a <see cref="Token.Types.Secondary.Bitwise"/> operator_type.
            /// </summary>
            /// <param name="x">The <see cref="Variable"/> on the left side of the <see cref="Operation"/>.</param>
            /// <param name="operator_type">The <see cref="Token.SecondaryType"/> defining which of the <see cref="Token.Types.Secondary.Operations"/> to create.</param>
            /// <param name="y">The <see cref="Operation"/> on the right side of the <see cref="Operation"/>.</param>
            /// <returns>An <see cref="Operation"/> representing that which was passed as arguments.</returns>
            public static Operation Bitwise(Variable x, short operator_type, Operation y)
            {
                switch (operator_type)
                {
                    case Token.Types.Secondary.Bitwise.Or: return () => x.Container.Value | y.Invoke();
                    case Token.Types.Secondary.Bitwise.XOr: return () => x.Container.Value ^ y.Invoke();
                    case Token.Types.Secondary.Bitwise.And: return () => x.Container.Value & y.Invoke();
                }

                throw new Exception("Could not create bitwise operation. Invalid operator_type " + operator_type + ".");
            }

            /// <summary>
            /// Creates a bitwise <see cref="Operation"/> using x and y, given a <see cref="Token.Types.Secondary.Bitwise"/> operator_type.
            /// </summary>
            /// <param name="x">The <see cref="Operation"/> on the left side of the <see cref="Operation"/>.</param>
            /// <param name="operator_type">The <see cref="Token.SecondaryType"/> defining which of the <see cref="Token.Types.Secondary.Operations"/> to create.</param>
            /// <param name="y">The <see cref="Variable"/> on the right side of the <see cref="Operation"/>.</param>
            /// <returns>An <see cref="Operation"/> representing that which was passed as arguments.</returns>
            public static Operation Bitwise(Operation x, short operator_type, Variable y)
            {
                switch (operator_type)
                {
                    case Token.Types.Secondary.Bitwise.Or: return () => x.Invoke() | y.Container.Value;
                    case Token.Types.Secondary.Bitwise.XOr: return () => x.Invoke() ^ y.Container.Value;
                    case Token.Types.Secondary.Bitwise.And: return () => x.Invoke() & y.Container.Value;
                }

                throw new Exception("Could not create bitwise operation. Invalid operator_type " + operator_type + ".");
            }

            /// <summary>
            /// Creates a bitwise <see cref="Operation"/> using x and y, given a <see cref="Token.Types.Secondary.Bitwise"/> operator_type.
            /// </summary>
            /// <param name="x">The <see cref="Operation"/> on the left side of the <see cref="Operation"/>.</param>
            /// <param name="operator_type">The <see cref="Token.SecondaryType"/> defining which of the <see cref="Token.Types.Secondary.Operations"/> to create.</param>
            /// <param name="y">The <see cref="Operation"/> on the right side of the <see cref="Operation"/>.</param>
            /// <returns>An <see cref="Operation"/> representing that which was passed as arguments.</returns>
            public static Operation Bitwise(Operation x, short operator_type, Operation y)
            {
                switch (operator_type)
                {
                    case Token.Types.Secondary.Bitwise.Or: return () => x.Invoke() | y.Invoke();
                    case Token.Types.Secondary.Bitwise.XOr: return () => x.Invoke() ^ y.Invoke();
                    case Token.Types.Secondary.Bitwise.And: return () => x.Invoke() & y.Invoke();
                }

                throw new Exception("Could not create bitwise operation. Invalid operator_type " + operator_type + ".");
            }
            
            /// <summary>
            /// Creates a comparison <see cref="Operation"/> using x and y, given a <see cref="Token.Types.Secondary.Comparers"/> operator_type.
            /// </summary>
            /// <param name="x">The <see cref="Variable"/> on the left side of the <see cref="Operation"/>.</param>
            /// <param name="operator_type">The <see cref="Token.SecondaryType"/> defining which of the <see cref="Token.Types.Secondary.Operations"/> to create.</param>
            /// <param name="y">The <see cref="Variable"/> on the right side of the <see cref="Operation"/>.</param>
            /// <returns>An <see cref="Operation"/> representing that which was passed as arguments.</returns>
            public static Operation Comparison(Variable x, short operator_type, Variable y)
            {
                switch (operator_type)
                {
                    case Token.Types.Secondary.Comparers.EqualTo: return () => x.Container.Value == y.Container.Value;
                    case Token.Types.Secondary.Comparers.GreaterThan: return () => x.Container.Value > y.Container.Value;
                    case Token.Types.Secondary.Comparers.GreaterThanOrEqualTo: return () => x.Container.Value >= y.Container.Value;
                    case Token.Types.Secondary.Comparers.LessThan: return () => x.Container.Value < y.Container.Value;
                    case Token.Types.Secondary.Comparers.LessThanOrEqualTo: return () => x.Container.Value <= y.Container.Value;
                    case Token.Types.Secondary.Comparers.UnequalTo: return () => x.Container.Value != y.Container.Value;
                }

                throw new Exception("Could not create comparison operation. Invalid operator_type " + operator_type + ".");
            }

            /// <summary>
            /// Creates a comparison <see cref="Operation"/> using x and y, given a <see cref="Token.Types.Secondary.Comparers"/> operator_type.
            /// </summary>
            /// <param name="x">The <see cref="Variable"/> on the left side of the <see cref="Operation"/>.</param>
            /// <param name="operator_type">The <see cref="Token.SecondaryType"/> defining which of the <see cref="Token.Types.Secondary.Operations"/> to create.</param>
            /// <param name="y">The <see cref="Operation"/> on the right side of the <see cref="Operation"/>.</param>
            /// <returns>An <see cref="Operation"/> representing that which was passed as arguments.</returns>
            public static Operation Comparison(Variable x, short operator_type, Operation y)
            {
                switch (operator_type)
                {
                    case Token.Types.Secondary.Comparers.EqualTo: return () => x.Container.Value == y.Invoke();
                    case Token.Types.Secondary.Comparers.GreaterThan: return () => x.Container.Value > y.Invoke();
                    case Token.Types.Secondary.Comparers.GreaterThanOrEqualTo: return () => x.Container.Value >= y.Invoke();
                    case Token.Types.Secondary.Comparers.LessThan: return () => x.Container.Value < y.Invoke();
                    case Token.Types.Secondary.Comparers.LessThanOrEqualTo: return () => x.Container.Value <= y.Invoke();
                    case Token.Types.Secondary.Comparers.UnequalTo: return () => x.Container.Value != y.Invoke();
                }

                throw new Exception("Could not create comparison operation. Invalid operator_type " + operator_type + ".");
            }

            /// <summary>
            /// Creates a comparison <see cref="Operation"/> using x and y, given a <see cref="Token.Types.Secondary.Comparers"/> operator_type.
            /// </summary>
            /// <param name="x">The <see cref="Operation"/> on the left side of the <see cref="Operation"/>.</param>
            /// <param name="operator_type">The <see cref="Token.SecondaryType"/> defining which of the <see cref="Token.Types.Secondary.Operations"/> to create.</param>
            /// <param name="y">The <see cref="Variable"/> on the right side of the <see cref="Operation"/>.</param>
            /// <returns>An <see cref="Operation"/> representing that which was passed as arguments.</returns>
            public static Operation Comparison(Operation x, short operator_type, Variable y)
            {
                switch (operator_type)
                {
                    case Token.Types.Secondary.Comparers.EqualTo: return () => x.Invoke() == y.Container.Value;
                    case Token.Types.Secondary.Comparers.GreaterThan: return () => x.Invoke() > y.Container.Value;
                    case Token.Types.Secondary.Comparers.GreaterThanOrEqualTo: return () => x.Invoke() >= y.Container.Value;
                    case Token.Types.Secondary.Comparers.LessThan: return () => x.Invoke() < y.Container.Value;
                    case Token.Types.Secondary.Comparers.LessThanOrEqualTo: return () => x.Invoke() <= y.Container.Value;
                    case Token.Types.Secondary.Comparers.UnequalTo: return () => x.Invoke() != y.Container.Value;
                }

                throw new Exception("Could not create comparison operation. Invalid operator_type " + operator_type + ".");
            }

            /// <summary>
            /// Creates a comparison <see cref="Operation"/> using x and y, given a <see cref="Token.Types.Secondary.Comparers"/> operator_type.
            /// </summary>
            /// <param name="x">The <see cref="Operation"/> on the left side of the <see cref="Operation"/>.</param>
            /// <param name="operator_type">The <see cref="Token.SecondaryType"/> defining which of the <see cref="Token.Types.Secondary.Operations"/> to create.</param>
            /// <param name="y">The <see cref="Operation"/> on the right side of the <see cref="Operation"/>.</param>
            /// <returns>An <see cref="Operation"/> representing that which was passed as arguments.</returns>
            public static Operation Comparison(Operation x, short operator_type, Operation y)
            {
                switch (operator_type)
                {
                    case Token.Types.Secondary.Comparers.EqualTo: return () => x.Invoke() == y.Invoke();
                    case Token.Types.Secondary.Comparers.GreaterThan: return () => x.Invoke() > y.Invoke();
                    case Token.Types.Secondary.Comparers.GreaterThanOrEqualTo: return () => x.Invoke() >= y.Invoke();
                    case Token.Types.Secondary.Comparers.LessThan: return () => x.Invoke() < y.Invoke();
                    case Token.Types.Secondary.Comparers.LessThanOrEqualTo: return () => x.Invoke() <= y.Invoke();
                    case Token.Types.Secondary.Comparers.UnequalTo: return () => x.Invoke() != y.Invoke();
                }

                throw new Exception("Could not create comparison operation. Invalid operator_type " + operator_type + ".");
            }

            /// <summary>
            /// Creates a conditional <see cref="Operation"/> using x and y, given a <see cref="Token.Types.Secondary.Conditionals"/> operator_type.
            /// </summary>
            /// <param name="x">The <see cref="Variable"/> on the left side of the <see cref="Operation"/>.</param>
            /// <param name="operator_type">The <see cref="Token.SecondaryType"/> defining which of the <see cref="Token.Types.Secondary.Operations"/> to create.</param>
            /// <param name="y">The <see cref="Variable"/> on the right side of the <see cref="Operation"/>.</param>
            /// <returns>An <see cref="Operation"/> representing that which was passed as arguments.</returns>
            public static Operation Condition(Variable x, short operator_type, Variable y)
            {
                switch (operator_type)
                {
                    case Token.Types.Secondary.Conditionals.And: return () => x.Container.Value && y.Container.Value;
                    case Token.Types.Secondary.Conditionals.Or: return () => x.Container.Value || y.Container.Value;
                }

                throw new Exception("Could not create arithmetic operation. Invalid operator_type " + operator_type + ".");
            }

            /// <summary>
            /// Creates a conditional <see cref="Operation"/> using x and y, given a <see cref="Token.Types.Secondary.Conditionals"/> operator_type.
            /// </summary>
            /// <param name="x">The <see cref="Variable"/> on the left side of the <see cref="Operation"/>.</param>
            /// <param name="operator_type">The <see cref="Token.SecondaryType"/> defining which of the <see cref="Token.Types.Secondary.Operations"/> to create.</param>
            /// <param name="y">The <see cref="Operation"/> on the right side of the <see cref="Operation"/>.</param>
            /// <returns>An <see cref="Operation"/> representing that which was passed as arguments.</returns>
            public static Operation Condition(Variable x, short operator_type, Operation y)
            {
                switch (operator_type)
                {
                    case Token.Types.Secondary.Conditionals.And: return () => x.Container.Value && y.Invoke();
                    case Token.Types.Secondary.Conditionals.Or: return () => x.Container.Value || y.Invoke();
                }

                throw new Exception("Could not create arithmetic operation. Invalid operator_type " + operator_type + ".");
            }

            /// <summary>
            /// Creates a conditional <see cref="Operation"/> using x and y, given a <see cref="Token.Types.Secondary.Conditionals"/> operator_type.
            /// </summary>
            /// <param name="x">The <see cref="Operation"/> on the left side of the <see cref="Operation"/>.</param>
            /// <param name="operator_type">The <see cref="Token.SecondaryType"/> defining which of the <see cref="Token.Types.Secondary.Operations"/> to create.</param>
            /// <param name="y">The <see cref="Variable"/> on the right side of the <see cref="Operation"/>.</param>
            /// <returns>An <see cref="Operation"/> representing that which was passed as arguments.</returns>
            public static Operation Condition(Operation x, short operator_type, Variable y)
            {
                switch (operator_type)
                {
                    case Token.Types.Secondary.Conditionals.And: return () => x.Invoke() && y.Container.Value;
                    case Token.Types.Secondary.Conditionals.Or: return () => x.Invoke() || y.Container.Value;
                }

                throw new Exception("Could not create arithmetic operation. Invalid operator_type " + operator_type + ".");
            }

            /// <summary>
            /// Creates a conditional <see cref="Operation"/> using x and y, given a <see cref="Token.Types.Secondary.Conditionals"/> operator_type.
            /// </summary>
            /// <param name="x">The <see cref="Operation"/> on the left side of the <see cref="Operation"/>.</param>
            /// <param name="operator_type">The <see cref="Token.SecondaryType"/> defining which of the <see cref="Token.Types.Secondary.Operations"/> to create.</param>
            /// <param name="y">The <see cref="Operation"/> on the right side of the <see cref="Operation"/>.</param>
            /// <returns>An <see cref="Operation"/> representing that which was passed as arguments.</returns>
            public static Operation Condition(Operation x, short operator_type, Operation y)
            {
                switch (operator_type)
                {
                    case Token.Types.Secondary.Conditionals.And: return () => x.Invoke() && y.Invoke();
                    case Token.Types.Secondary.Conditionals.Or: return () => x.Invoke() || y.Invoke();
                }

                throw new Exception("Could not create arithmetic operation. Invalid operator_type " + operator_type + ".");
            }
            
            /// <summary>
            /// Creates an assignment <see cref="Operation"/> using x and y, given a <see cref="Token.Types.Secondary.Assignments"/> operator_type.
            /// </summary>
            /// <param name="x">The <see cref="Variable"/> on the left side of the <see cref="Operation"/>.</param>
            /// <param name="operator_type">The <see cref="Token.SecondaryType"/> defining which of the <see cref="Token.Types.Secondary.Operations"/> to create.</param>
            /// <param name="y">The <see cref="Variable"/> on the right side of the <see cref="Operation"/>.</param>
            /// <returns>An <see cref="Operation"/> representing that which was passed as arguments.</returns>
            public static Operation Assignment(Variable x, short operator_type, Variable y, int settings)
            {
                switch (operator_type)
                {
                    case Token.Types.Secondary.Assignments.Basic:
                        {
                            if (Settings.Contains(settings, Settings.ValueTypes.AssignByRef))
                            {
                                if (Settings.Contains(settings, Settings.ObjectTypes.AssignByRef))
                                    return Assignments.RefAll.Basic(x, y);
                                else
                                    return Assignments.RefValue.Basic(x, y);
                            }
                            else
                            {
                                if (Settings.Contains(settings, Settings.ObjectTypes.AssignByRef))
                                    return Assignments.RefObject.Basic(x, y);
                                else
                                    return Assignments.RefNone.Basic(x, y);
                            }
                        }

                    case Token.Types.Secondary.Assignments.Addition:
                        {
                            if (Settings.Contains(settings, Settings.ValueTypes.AssignByRef))
                            {
                                if (Settings.Contains(settings, Settings.ObjectTypes.AssignByRef))
                                    return Assignments.RefAll.Addition(x, y);
                                else
                                    return Assignments.RefValue.Addition(x, y);
                            }
                            else
                            {
                                if (Settings.Contains(settings, Settings.ObjectTypes.AssignByRef))
                                    return Assignments.RefObject.Addition(x, y);
                                else
                                    return Assignments.RefNone.Addition(x, y);
                            }
                        }

                    case Token.Types.Secondary.Assignments.Division:
                        {
                            if (Settings.Contains(settings, Settings.ValueTypes.AssignByRef))
                            {
                                if (Settings.Contains(settings, Settings.ObjectTypes.AssignByRef))
                                    return Assignments.RefAll.Division(x, y);
                                else
                                    return Assignments.RefValue.Division(x, y);
                            }
                            else
                            {
                                if (Settings.Contains(settings, Settings.ObjectTypes.AssignByRef))
                                    return Assignments.RefObject.Division(x, y);
                                else
                                    return Assignments.RefNone.Division(x, y);
                            }
                        }
                    case Token.Types.Secondary.Assignments.Multiplication:
                        {
                            if (Settings.Contains(settings, Settings.ValueTypes.AssignByRef))
                            {
                                if (Settings.Contains(settings, Settings.ObjectTypes.AssignByRef))
                                    return Assignments.RefAll.Multiplication(x, y);
                                else
                                    return Assignments.RefValue.Multiplication(x, y);
                            }
                            else
                            {
                                if (Settings.Contains(settings, Settings.ObjectTypes.AssignByRef))
                                    return Assignments.RefObject.Multiplication(x, y);
                                else
                                    return Assignments.RefNone.Multiplication(x, y);
                            }
                        }
                    case Token.Types.Secondary.Assignments.Subtraction:
                        {
                            if (Settings.Contains(settings, Settings.ValueTypes.AssignByRef))
                            {
                                if (Settings.Contains(settings, Settings.ObjectTypes.AssignByRef))
                                    return Assignments.RefAll.Subtraction(x, y);
                                else
                                    return Assignments.RefValue.Subtraction(x, y);
                            }
                            else
                            {
                                if (Settings.Contains(settings, Settings.ObjectTypes.AssignByRef))
                                    return Assignments.RefObject.Subtraction(x, y);
                                else
                                    return Assignments.RefNone.Subtraction(x, y);
                            }
                        }
                }

                throw new Exception("Could not create arithmetic operation. Invalid operator_type " + operator_type + ".");
            }

            /// <summary>
            /// Creates an assignment <see cref="Operation"/> using x and y, given a <see cref="Token.Types.Secondary.Assignments"/> operator_type.
            /// </summary>
            /// <param name="x">The <see cref="Variable"/> on the left side of the <see cref="Operation"/>.</param>
            /// <param name="operator_type">The <see cref="Token.SecondaryType"/> defining which of the <see cref="Token.Types.Secondary.Operations"/> to create.</param>
            /// <param name="y">The <see cref="Operation"/> on the right side of the <see cref="Operation"/>.</param>
            /// <returns>An <see cref="Operation"/> representing that which was passed as arguments.</returns>
            public static Operation Assignment(Variable x, short operator_type, Operation y, int settings)
            {
                switch (operator_type)
                {
                    case Token.Types.Secondary.Assignments.Basic:
                        {
                            if (Settings.Contains(settings, Settings.ValueTypes.AssignByRef))
                            {
                                if (Settings.Contains(settings, Settings.ObjectTypes.AssignByRef))
                                    return Assignments.RefAll.Basic(x, y);
                                else
                                    return Assignments.RefValue.Basic(x, y);
                            }
                            else
                            {
                                if (Settings.Contains(settings, Settings.ObjectTypes.AssignByRef))
                                    return Assignments.RefObject.Basic(x, y);
                                else
                                    return Assignments.RefNone.Basic(x, y);
                            }
                        }

                    case Token.Types.Secondary.Assignments.Addition:
                        {
                            if (Settings.Contains(settings, Settings.ValueTypes.AssignByRef))
                            {
                                if (Settings.Contains(settings, Settings.ObjectTypes.AssignByRef))
                                    return Assignments.RefAll.Addition(x, y);
                                else
                                    return Assignments.RefValue.Addition(x, y);
                            }
                            else
                            {
                                if (Settings.Contains(settings, Settings.ObjectTypes.AssignByRef))
                                    return Assignments.RefObject.Addition(x, y);
                                else
                                    return Assignments.RefNone.Addition(x, y);
                            }
                        }

                    case Token.Types.Secondary.Assignments.Division:
                        {
                            if (Settings.Contains(settings, Settings.ValueTypes.AssignByRef))
                            {
                                if (Settings.Contains(settings, Settings.ObjectTypes.AssignByRef))
                                    return Assignments.RefAll.Division(x, y);
                                else
                                    return Assignments.RefValue.Division(x, y);
                            }
                            else
                            {
                                if (Settings.Contains(settings, Settings.ObjectTypes.AssignByRef))
                                    return Assignments.RefObject.Division(x, y);
                                else
                                    return Assignments.RefNone.Division(x, y);
                            }
                        }
                    case Token.Types.Secondary.Assignments.Multiplication:
                        {
                            if (Settings.Contains(settings, Settings.ValueTypes.AssignByRef))
                            {
                                if (Settings.Contains(settings, Settings.ObjectTypes.AssignByRef))
                                    return Assignments.RefAll.Multiplication(x, y);
                                else
                                    return Assignments.RefValue.Multiplication(x, y);
                            }
                            else
                            {
                                if (Settings.Contains(settings, Settings.ObjectTypes.AssignByRef))
                                    return Assignments.RefObject.Multiplication(x, y);
                                else
                                    return Assignments.RefNone.Multiplication(x, y);
                            }
                        }
                    case Token.Types.Secondary.Assignments.Subtraction:
                        {
                            if (Settings.Contains(settings, Settings.ValueTypes.AssignByRef))
                            {
                                if (Settings.Contains(settings, Settings.ObjectTypes.AssignByRef))
                                    return Assignments.RefAll.Subtraction(x, y);
                                else
                                    return Assignments.RefValue.Subtraction(x, y);
                            }
                            else
                            {
                                if (Settings.Contains(settings, Settings.ObjectTypes.AssignByRef))
                                    return Assignments.RefObject.Subtraction(x, y);
                                else
                                    return Assignments.RefNone.Subtraction(x, y);
                            }
                        }
                }

                throw new Exception("Could not create arithmetic operation. Invalid operator_type " + operator_type + ".");
            }

            /// <summary>
            /// The static class defining all ways in which the Polite Framework lets you to assign variables.
            /// </summary>
            public static class Assignments
            {
                public static class RefAll
                {
                    public static Operation Basic(Variable x, Variable y)
                    {
                        if (y.Container is MethodInvoker)
                        {
                            return () =>
                            {
                                x.Container.Value = y.Container.Value;
                                x.Container.Members.Clear();
                                return x;
                            };
                        }
                        else if (y.Container is FunctionInvoker fi && fi.ReturnsNull)
                        {
                            return () =>
                            {
                                x.Container = new Container();
                                return x;
                            };
                        }
                        else if (y.Container is Function)
                        {
                            
                            if (y.Container as FunctionInvoker is null)
                            {
                                return () =>
                                {
                                    x.Container = y.Container;
                                    return x;
                                };
                            }
                            else
                            {
                                return () =>
                                {
                                    FunctionInvoker f = ((FunctionInvoker)y.Container);
                                    f.Operation.Invoke();
                                    x.Container = f.Result.Container;
                                    return x;
                                };
                            }
                        }
                        else
                        {
                            return () =>
                            {
                                x.Container = y.Container;
                                return x;
                            };
                        }
                    }
                    public static Operation Addition(Variable x, Variable y)
                    {
                        if (y.Container is MethodInvoker)
                        {
                            return () =>
                            {
                                x.Container.Value += y.Container.Value;
                                return x;
                            };
                        }
                        else if (y.Container is FunctionInvoker fi && fi.ReturnsNull)
                        {
                            return () =>
                            {
                                x.Container = new Container();
                                return x;
                            };
                        }
                        else if (y.Container is Function)
                        {

                            if (y.Container as FunctionInvoker is null)
                            {
                                return () =>
                                {
                                    x.Container = y.Container;
                                    return x;
                                };
                            }
                            else
                            {
                                return () =>
                                {
                                    FunctionInvoker f = ((FunctionInvoker)y.Container);
                                    f.Operation.Invoke();
                                    x.Container.Value += f.Result.Container.Value;
                                    f.Result.Container = x.Container;
                                    return x;
                                };
                            }
                        }
                        else
                        {
                            return () =>
                            {
                                x.Container.Value += y.Container.Value;
                                y.Container = x.Container;
                                return x;
                            };
                        }
                    }
                    public static Operation Subtraction(Variable x, Variable y)
                    {
                        if (y.Container is MethodInvoker)
                        {
                            return () =>
                            {
                                x.Container.Value -= y.Container.Value;
                                return x;
                            };
                        }
                        else if (y.Container is FunctionInvoker fi && fi.ReturnsNull)
                        {
                            return () =>
                            {
                                x.Container = new Container();
                                return x;
                            };
                        }
                        else if (y.Container is Function)
                        {

                            if (y.Container as FunctionInvoker is null)
                            {
                                return () =>
                                {
                                    x.Container = y.Container;
                                    return x;
                                };
                            }
                            else
                            {
                                return () =>
                                {
                                    FunctionInvoker f = ((FunctionInvoker)y.Container);
                                    f.Operation.Invoke();
                                    x.Container.Value -= f.Result.Container.Value;
                                    f.Result.Container = x.Container;
                                    return x;
                                };
                            }
                        }
                        else
                        {
                            return () =>
                            {
                                x.Container.Value -= y.Container.Value;
                                y.Container = x.Container;
                                return x;
                            };
                        }
                    }
                    public static Operation Multiplication(Variable x, Variable y)
                    {
                        if (y.Container is MethodInvoker)
                        {
                            return () =>
                            {
                                x.Container.Value *= y.Container.Value;
                                return x;
                            };
                        }
                        else if (y.Container is FunctionInvoker fi && fi.ReturnsNull)
                        {
                            return () =>
                            {
                                x.Container = new Container();
                                return x;
                            };
                        }
                        else if (y.Container is Function)
                        {

                            if (y.Container as FunctionInvoker is null)
                            {
                                return () =>
                                {
                                    x.Container = y.Container;
                                    return x;
                                };
                            }
                            else
                            {
                                return () =>
                                {
                                    FunctionInvoker f = ((FunctionInvoker)y.Container);
                                    f.Operation.Invoke();
                                    x.Container.Value *= f.Result.Container.Value;
                                    f.Result.Container = x.Container;
                                    return x;
                                };
                            }
                        }
                        else
                        {
                            return () =>
                            {
                                x.Container.Value *= y.Container.Value;
                                y.Container = x.Container;
                                return x;
                            };
                        }
                    }
                    public static Operation Division(Variable x, Variable y)
                    {
                        if (y.Container is MethodInvoker)
                        {
                            return () =>
                            {
                                x.Container.Value /= y.Container.Value;
                                return x;
                            };
                        }
                        else if (y.Container is FunctionInvoker fi && fi.ReturnsNull)
                        {
                            return () =>
                            {
                                x.Container = new Container();
                                return x;
                            };
                        }
                        else if (y.Container is Function)
                        {

                            if (y.Container as FunctionInvoker is null)
                            {
                                return () =>
                                {
                                    x.Container = y.Container;
                                    return x;
                                };
                            }
                            else
                            {
                                return () =>
                                {
                                    FunctionInvoker f = ((FunctionInvoker)y.Container);
                                    f.Operation.Invoke();
                                    x.Container.Value /= f.Result.Container.Value;
                                    f.Result.Container = x.Container;
                                    return x;
                                };
                            }
                        }
                        else
                        {
                            return () =>
                            {
                                x.Container.Value /= y.Container.Value;
                                y.Container = x.Container;
                                return x;
                            };
                        }
                    }

                    public static Operation Basic(Variable x, Operation y)
                    {
                        return () =>
                        {
                            x.Container.Value = y.Invoke();
                            x.Container.Members.Clear();
                            return x;
                        };
                    }
                    public static Operation Addition(Variable x, Operation y)
                    {
                        return () =>
                        {
                            x.Container.Value = x.Container.Value + y.Invoke();
                            return x;
                        };
                    }
                    public static Operation Subtraction(Variable x, Operation y)
                    {
                        return () =>
                        {
                            x.Container.Value = x.Container.Value - y.Invoke();
                            return x;
                        };
                    }
                    public static Operation Multiplication(Variable x, Operation y)
                    {
                        return () =>
                        {
                            x.Container.Value = x.Container.Value * y.Invoke();
                            return x;
                        };
                    }
                    public static Operation Division(Variable x, Operation y)
                    {
                        return () =>
                        {
                            x.Container.Value = x.Container.Value / y.Invoke();
                            return x;
                        };
                    }
                }
                public static class RefValue
                {
                    public static Operation Basic(Variable x, Variable y)
                    {
                        if (y.Container is MethodInvoker mi)
                        {
                            return () =>
                            {
                                x.Container.Value = mi.Result.Container.Value;
                                x.Container.Members.Clear();
                                return x;
                            };
                        }
                        else if (y.Container is Function func)
                        { 
                            if (y.Container is FunctionInvoker fi)
                            {
                                if (fi.ReturnsNull)
                                {
                                    return () =>
                                    {
                                        x.Container = new Container();
                                        return x;
                                    };
                                }
                                else
                                {
                                    return () =>
                                    {
                                        x.Container = fi.Result.Container;
                                        return x;
                                    };
                                }
                            }
                            else
                            {
                                return () =>
                                {
                                    x.Container = y.Container;
                                    return x;
                                };
                            }
                        }
                        else
                        {
                            return () =>
                            {
                                if (y.Container.Value is ValueType)
                                    x.Container = y.Container;
                                else
                                {
                                    x.Container.Value = y.Container.Value.MemberwiseClone();
                                    x.Container.Members.Clear();
                                }
                                return x;
                            };
                        }
                    }
                    public static Operation Addition(Variable x, Variable y)
                    {
                        if (y.Container is MethodInvoker mi)
                        {
                            return () =>
                            {
                                x.Container.Value += mi.Result.Container.Value;
                                return x;
                            };
                        }
                        else if (y.Container is FunctionInvoker fi && fi.ReturnsNull)
                        {
                            return () =>
                            {
                                x.Container = new Container();
                                return x;
                            };
                        }
                        else
                        {
                            return () =>
                            {
                                x.Container.Value += y.Container.Value;
                                return x;
                            };
                        }
                    }
                    public static Operation Subtraction(Variable x, Variable y)
                    {
                        if (y.Container is MethodInvoker mi)
                        {
                            return () =>
                            {
                                x.Container.Value -= mi.Result.Container.Value;
                                return x;
                            };
                        }
                        else if (y.Container is FunctionInvoker fi && fi.ReturnsNull)
                        {
                            return () =>
                            {
                                x.Container = new Container();
                                return x;
                            };
                        }
                        else
                        {
                            return () =>
                            {
                                x.Container.Value -= y.Container.Value;
                                return x;
                            };
                        }
                    }
                    public static Operation Multiplication(Variable x, Variable y)
                    {
                        if (y.Container is MethodInvoker mi)
                        {
                            return () =>
                            {
                                x.Container.Value *= mi.Result.Container.Value;
                                return x;
                            };
                        }
                        else if (y.Container is FunctionInvoker fi && fi.ReturnsNull)
                        {
                            return () =>
                            {
                                x.Container = new Container();
                                return x;
                            };
                        }
                        else
                        {
                            return () =>
                            {
                                x.Container.Value *= y.Container.Value;
                                return x;
                            };
                        }
                    }
                    public static Operation Division(Variable x, Variable y)
                    {
                        if (y.Container is MethodInvoker mi)
                        {
                            return () =>
                            {
                                x.Container.Value /= mi.Result.Container.Value;
                                return x;
                            };
                        }
                        else if (y.Container is FunctionInvoker fi && fi.ReturnsNull)
                        {
                            return () =>
                            {
                                x.Container = new Container();
                                return x;
                            };
                        }
                        else
                        {
                            return () =>
                            {
                                x.Container.Value /= y.Container.Value;
                                return x;
                            };
                        }
                    }

                    public static Operation Basic(Variable x, Operation y)
                    {
                        return () =>
                        {
                            x.Container.Value = y.Invoke();
                            x.Container.Members.Clear();
                            return x;
                        };
                    }
                    public static Operation Addition(Variable x, Operation y)
                    {
                        return () =>
                        {
                            x.Container.Value += y.Invoke();
                            return x;
                        };
                    }
                    public static Operation Subtraction(Variable x, Operation y)
                    {
                        return () =>
                        {
                            x.Container.Value -= y.Invoke();
                            return x;
                        };
                    }
                    public static Operation Multiplication(Variable x, Operation y)
                    {
                        return () =>
                        {
                            x.Container.Value *= y.Invoke();
                            return x;
                        };
                    }
                    public static Operation Division(Variable x, Operation y)
                    {
                        return () =>
                        {
                            x.Container.Value /= y.Invoke();
                            return x;
                        };
                    }
                }
                public static class RefObject
                {
                    public static Operation Basic(Variable x, Variable y)
                    {
                        if (y.Container is MethodInvoker)
                        {
                            return () =>
                            {
                                x.Container.Value = y.Container.Value;
                                x.Container.Members.Clear();
                                return x;
                            };
                        }
                        else if (y.Container is FunctionInvoker fi && fi.ReturnsNull)
                        {
                            return () =>
                            {
                                x.Container = new Container();
                                return x;
                            };
                        }
                        else if (y.Container is Function && y.Container as FunctionInvoker is null)
                        {
                            return () =>
                            {
                                x.Container = y.Container;
                                return x;
                            };
                        }
                        else
                        {
                            return () =>
                            {
                                x.Container.Value = y.Container.Value;
                                x.Container.Members = y.Container.Members;
                                return x;
                            };
                        }
                    }
                    public static Operation Addition(Variable x, Variable y)
                    {
                        if (y.Container is MethodInvoker)
                        {
                            return () =>
                            {
                                x.Container.Value += y.Container.Value;
                                x.Container.Members.Clear();
                                return x;
                            };
                        }
                        else if (y.Container is FunctionInvoker fi && fi.ReturnsNull)
                        {
                            return () =>
                            {
                                x.Container = new Container();
                                return x;
                            };
                        }
                        else if (y.Container is Function && y.Container as FunctionInvoker is null)
                        {
                            return () =>
                            {
                                x.Container = y.Container;
                                return x;
                            };
                        }
                        else
                        {
                            return () =>
                            {
                                x.Container.Value += y.Container.Value;
                                return x;
                            };
                        }
                    }
                    public static Operation Subtraction(Variable x, Variable y)
                    {
                        if (y.Container is MethodInvoker)
                        {
                            return () =>
                            {
                                x.Container.Value -= y.Container.Value;
                                x.Container.Members.Clear();
                                return x;
                            };
                        }
                        else if (y.Container is FunctionInvoker fi && fi.ReturnsNull)
                        {
                            return () =>
                            {
                                x.Container = new Container();
                                return x;
                            };
                        }
                        else if (y.Container is Function && y.Container as FunctionInvoker is null)
                        {
                            return () =>
                            {
                                x.Container = y.Container;
                                return x;
                            };
                        }
                        else
                        {
                            return () =>
                            {
                                x.Container.Value -= y.Container.Value;
                                return x;
                            };
                        }
                    }
                    public static Operation Multiplication(Variable x, Variable y)
                    {
                        if (y.Container is MethodInvoker)
                        {
                            return () =>
                            {
                                x.Container.Value *= y.Container.Value;
                                x.Container.Members.Clear();
                                return x;
                            };
                        }
                        else if (y.Container is FunctionInvoker fi && fi.ReturnsNull)
                        {
                            return () =>
                            {
                                x.Container = new Container();
                                return x;
                            };
                        }
                        else if (y.Container is Function && y.Container as FunctionInvoker is null)
                        {
                            return () =>
                            {
                                x.Container = y.Container;
                                return x;
                            };
                        }
                        else
                        {
                            return () =>
                            {
                                x.Container.Value *= y.Container.Value;
                                return x;
                            };
                        }
                    }
                    public static Operation Division(Variable x, Variable y)
                    {
                        if (y.Container is MethodInvoker)
                        {
                            return () =>
                            {
                                x.Container.Value /= y.Container.Value;
                                x.Container.Members.Clear();
                                return x;
                            };
                        }
                        else if (y.Container is FunctionInvoker fi && fi.ReturnsNull)
                        {
                            return () =>
                            {
                                x.Container = new Container();
                                return x;
                            };
                        }
                        else if (y.Container is Function && y.Container as FunctionInvoker is null)
                        {
                            return () =>
                            {
                                x.Container = y.Container;
                                return x;
                            };
                        }
                        else
                        {
                            return () =>
                            {
                                x.Container.Value /= y.Container.Value;
                                return x;
                            };
                        }
                    }

                    public static Operation Basic(Variable x, Operation y)
                    {
                        return () =>
                        {
                            x.Container.Value = y.Invoke();
                            x.Container.Members.Clear();
                            return x;
                        };
                    }
                    public static Operation Addition(Variable x, Operation y)
                    {
                        return () =>
                        {
                            x.Container.Value += y.Invoke();
                            return x;
                        };
                    }
                    public static Operation Subtraction(Variable x, Operation y)
                    {
                        return () =>
                        {
                            x.Container.Value -= y.Invoke();
                            return x;
                        };
                    }
                    public static Operation Multiplication(Variable x, Operation y)
                    {
                        return () =>
                        {
                            x.Container.Value *= y.Invoke();
                            return x;
                        };
                    }
                    public static Operation Division(Variable x, Operation y)
                    {
                        return () =>
                        {
                            x.Container.Value /= y.Invoke();
                            return x;
                        };
                    }
                }
                public static class RefNone
                {
                    public static Operation Basic(Variable x, Variable y)
                    {
                        if (y.Container is MethodInvoker mi)
                        {
                            return () =>
                            {
                                x.Container.Value = mi.Result.Container.Value;
                                x.Container.Members.Clear();
                                return x;
                            };
                        }
                        else if (y.Container is Function func)
                        {
                            if (y.Container is FunctionInvoker fi)
                            {
                                if (fi.ReturnsNull)
                                {
                                    return () =>
                                    {
                                        x.Container = new Container();
                                        return x;
                                    };
                                }
                                else
                                {
                                    return () =>
                                    {
                                        x.Container.Members = new Dictionary<int, Variable>(fi.Result.Container.Members);
                                        if (fi.LastResult.Container.Value is ValueType)
                                            x.Container.Value = fi.LastResult.Container.Value;
                                        else
                                        {
                                            x.Container.Value = fi.LastResult.Container.Value.MemberwiseClone();
                                            x.Container.Members = new Dictionary<int, Variable>(fi.LastResult.Container.Members);
                                        }
                                        return x;
                                    };
                                }
                            }
                            else
                            {
                                return () =>
                                {
                                    x.Container = y.Container;
                                    return x;
                                };
                            }
                        }
                        else
                        {
                            return () =>
                            {
                                if (y.Container.Value is ValueType)
                                    x.Container.Value = y.Container.Value;
                                else
                                {
                                    x.Container.Value = y.Container.Value.MemberwiseClone();
                                    x.Container.Members = new Dictionary<int, Variable>(y.Container.Members);
                                }
                                return x;
                            };
                        }
                    }
                    public static Operation Addition(Variable x, Variable y)
                    {
                        if (y.Container is MethodInvoker)
                        {
                            return () =>
                            {
                                x.Container.Value += y.Container.Value;
                                x.Container.Members.Clear();
                                return x;
                            };
                        }
                        else if (y.Container is FunctionInvoker fi && fi.ReturnsNull)
                        {
                            return () =>
                            {
                                x.Container = new Container();
                                return x;
                            };
                        }
                        else if (y.Container is Function && y.Container as FunctionInvoker is null)
                        {
                            return () =>
                            {
                                x.Container = y.Container;
                                return x;
                            };
                        }
                        else
                        {
                            return () =>
                            {
                                x.Container.Value += y.Container.Value;
                                return x;
                            };
                        }
                    }
                    public static Operation Subtraction(Variable x, Variable y)
                    {
                        if (y.Container is MethodInvoker)
                        {
                            return () =>
                            {
                                x.Container.Value -= y.Container.Value;
                                x.Container.Members.Clear();
                                return x;
                            };
                        }
                        else if (y.Container is FunctionInvoker fi && fi.ReturnsNull)
                        {
                            return () =>
                            {
                                x.Container = new Container();
                                return x;
                            };
                        }
                        else if (y.Container is Function && y.Container as FunctionInvoker is null)
                        {
                            return () =>
                            {
                                x.Container = y.Container;
                                return x;
                            };
                        }
                        else
                        {
                            return () =>
                            {
                                x.Container.Value -= y.Container.Value;
                                return x;
                            };
                        }
                    }
                    public static Operation Multiplication(Variable x, Variable y)
                    {
                        if (y.Container is MethodInvoker)
                        {
                            return () =>
                            {
                                x.Container.Value *= y.Container.Value;
                                x.Container.Members.Clear();
                                return x;
                            };
                        }
                        else if (y.Container is FunctionInvoker fi && fi.ReturnsNull)
                        {
                            return () =>
                            {
                                x.Container = new Container();
                                return x;
                            };
                        }
                        else if (y.Container is Function && y.Container as FunctionInvoker is null)
                        {
                            return () =>
                            {
                                x.Container = y.Container;
                                return x;
                            };
                        }
                        else
                        {
                            return () =>
                            {
                                x.Container.Value *= y.Container.Value;
                                return x;
                            };
                        }
                    }
                    public static Operation Division(Variable x, Variable y)
                    {
                        if (y.Container is MethodInvoker)
                        {
                            return () =>
                            {
                                x.Container.Value /= y.Container.Value;
                                x.Container.Members.Clear();
                                return x;
                            };
                        }
                        else if (y.Container is FunctionInvoker fi && fi.ReturnsNull)
                        {
                            return () =>
                            {
                                x.Container = new Container();
                                return x;
                            };
                        }
                        else if (y.Container is Function && y.Container as FunctionInvoker is null)
                        {
                            return () =>
                            {
                                x.Container = y.Container;
                                return x;
                            };
                        }
                        else
                        {
                            return () =>
                            {
                                x.Container.Value /= y.Container.Value;
                                return x;
                            };
                        }
                    }

                    public static Operation Basic(Variable x, Operation y)
                    {
                        return () =>
                        {
                            dynamic result = y.Invoke();
                            if (result is ValueType)
                                x.Container.Value = result;
                            else
                                x.Container.Value = result.MemberwiseClone();
                            x.Container.Members.Clear();
                            return x;
                        };
                    }
                    public static Operation Addition(Variable x, Operation y)
                    {
                        return () =>
                        {
                            x.Container.Value += y.Invoke();
                            return x;
                        };
                    }
                    public static Operation Subtraction(Variable x, Operation y)
                    {
                        return () =>
                        {
                            x.Container.Value -= y.Invoke();
                            return x;
                        };
                    }
                    public static Operation Multiplication(Variable x, Operation y)
                    {
                        return () =>
                        {
                            x.Container.Value *= y.Invoke();
                            return x;
                        };
                    }
                    public static Operation Division(Variable x, Operation y)
                    {
                        return () =>
                        {
                            x.Container.Value /= y.Invoke();
                            return x;
                        };
                    }
                }
            }

        }

    }

}
