using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Alphicsh.LinqExpressions.Fluent
{
    public static partial class Flex
    {
        /// <summary>
        /// A part of the Fluent Express interface, used when an operand is expected.
        /// It shouldn't be used directly, but rather obtained somewhere in the Flex building chain.
        /// </summary>
        /// <typeparam name="D">The type of the delegate built.</typeparam>
        public class _OperandStarter<D> : BaseFluentExpressBuilder<D>
    where D : class
        {
            /// <summary>
            /// Creates an operand starter with a given underlying expression body builder.
            /// </summary>
            /// <param name="builder">The underlying expression body builder.</param>
            public _OperandStarter(BodyBuilder<D> builder)
                : base(builder)
            { }

            #region Operand starting methods

            /// <summary>
            /// Adds a null value to the built expression.
            /// </summary>
            /// <returns>An expression expander to continue building.</returns>
            public _ExpressionExpander<D> Null() => FluentAction(new _ExpressionExpander<D>(InnerBuilder), InnerBuilder.Null);
            /// <summary>
            /// Adds a typed null value to the built expression.
            /// </summary>
            /// <param name="type">The type of the null value.</param>
            /// <returns>An expression expander to continue building.</returns>
            public _ExpressionExpander<D> Null(Type type) => FluentAction(new _ExpressionExpander<D>(InnerBuilder), InnerBuilder.Null, type);
            /// <summary>
            /// Adds a typed null value to the built expression.
            /// </summary>
            /// <typeparam name="TNull">The type of the null value.</typeparam>
            /// <returns>An expression expander to continue building.</returns>
            public _ExpressionExpander<D> Null<TNull>() => Null(typeof(TNull));
            /// <summary>
            /// Adds a default value for the given type to the built expression.
            /// </summary>
            /// <param name="type">The type to get the default value of.</param>
            /// <returns>An expression expander to continue building.</returns>
            public _ExpressionExpander<D> Default(Type type) => FluentAction(new _ExpressionExpander<D>(InnerBuilder), InnerBuilder.Default, type);
            /// <summary>
            /// Adds a default value for the given type to the built expression.
            /// </summary>
            /// <typeparam name="TDefault">The type to get the default value of.</typeparam>
            /// <returns>An expression expander to continue building.</returns>
            public _ExpressionExpander<D> Default<TDefault>() => Default(typeof(TDefault));
            /// <summary>
            /// Adds a constant value to the built expression.
            /// </summary>
            /// <param name="value">The value to add.</param>
            /// <returns>An expression expander to continue building.</returns>
            public _ExpressionExpander<D> Const(object value) => FluentAction(new _ExpressionExpander<D>(InnerBuilder), InnerBuilder.Const, value);
            /// <summary>
            /// Adds a constant value with a specific type to the built expression.
            /// </summary>
            /// <param name="value">The value to add.</param>
            /// <param name="type">The type of the value.</param>
            /// <returns>An expression expander to continue building.</returns>
            public _ExpressionExpander<D> Const(object value, Type type) => FluentAction(new _ExpressionExpander<D>(InnerBuilder), InnerBuilder.Const, value, type);
            /// <summary>
            /// Adds a constant value with a specific type to the built expression.
            /// </summary>
            /// <typeparam name="TValue">The type of the value.</typeparam>
            /// <param name="value">The value to add.</param>
            /// <returns>An expression expander to continue building.</returns>
            public _ExpressionExpander<D> Const<TValue>(object value) => Const(value, typeof(TValue));
            /// <summary>
            /// Adds a variable value to the built expression.
            /// </summary>
            /// <param name="name">The name of the variable.</param>
            /// <returns>An expression expander to continue building.</returns>
            public _ExpressionExpander<D> Var(string name) => FluentAction(new _ExpressionExpander<D>(InnerBuilder), InnerBuilder.Var, name);
            /// <summary>
            /// Adds a subexpression to the built expression.
            /// </summary>
            /// <param name="exp">The subexpression to add.</param>
            /// <returns>An expression expander to continue building.</returns>
            public _ExpressionExpander<D> Subexpression(Expression exp) => FluentAction(new _ExpressionExpander<D>(InnerBuilder), InnerBuilder.Subexpression, exp);

            /// <summary>
            /// Adds a newly constructed object of specific type to the built expression, with specific parameter types. That action must be followed by a list of arguments.
            /// </summary>
            /// <param name="newType">The type of the constructed object.</param>
            /// <returns>An expression expander to continue building.</returns>
            public _ExpressionExpander<D> New(Type newType) => FluentAction(new _ExpressionExpander<D>(InnerBuilder), InnerBuilder.New, newType);
            /// <summary>
            /// Adds a newly constructed object of specific type to the built expression, with specific parameter types. That action must be followed by a list of arguments.
            /// </summary>
            /// <typeparam name="TNew">The type of the constructed object.</typeparam>
            /// <returns>An expression expander to continue building.</returns>
            public _ExpressionExpander<D> New<TNew>() => FluentAction(new _ExpressionExpander<D>(InnerBuilder), InnerBuilder.New, typeof(TNew));

            /// <summary>
            /// Adds a newly constructed object of specific type to the built expression, with specific parameter types. That action must be followed by a list of arguments.
            /// </summary>
            /// <param name="newType">The type of the constructed object.</param>
            /// <param name="parameterTypes">The types of the constructor parameters.</param>
            /// <returns>An argument list starter to continue building.</returns>
            public _ArgumentListStarter<D> New(Type newType, params Type[] parameterTypes) => FluentAction(new _ArgumentListStarter<D>(InnerBuilder), InnerBuilder.New, newType, parameterTypes);
            /// <summary>
            /// Adds a newly constructed object of specific type to the built expression, with specific parameter types. That action must be followed by a list of arguments.
            /// </summary>
            /// <typeparam name="TNew">The type of the constructed object.</typeparam>
            /// <param name="parameterTypes">The types of the constructor parameters.</param>
            /// <returns>An argument list starter to continue building.</returns>
            public _ArgumentListStarter<D> New<TNew>(params Type[] parameterTypes) => FluentAction(new _ArgumentListStarter<D>(InnerBuilder), InnerBuilder.New, typeof(TNew), parameterTypes);

            #endregion

            #region Unary operations, stacked before operands

            /// <summary>
            /// Imaginarily applies an arithmetic positive plus to the following operand, which essentially does nothing. You might as well multiply it by 1.
            /// </summary>
            /// <returns>This operand starter to continue building.</returns>
            public _OperandStarter<D> Plus() => this;
            /// <summary>
            /// Changes the sign of the following operand to its negative.
            /// </summary>
            /// <returns>This operand starter to continue building.</returns>
            public _OperandStarter<D> Minus() => FluentAction(this, InnerBuilder.Minus);
            /// <summary>
            /// Switches the boolean value of the following operand.
            /// </summary>
            /// <returns>This operand starter to continue building.</returns>
            public _OperandStarter<D> Not() => FluentAction(this, InnerBuilder.Not);
            /// <summary>
            /// Returns a bitwise negation of the following operand.
            /// </summary>
            /// <returns>This operand starter to continue building.</returns>
            public _OperandStarter<D> BitwiseNot() => FluentAction(this, InnerBuilder.BitwiseNot);
            /// <summary>
            /// Performs a pre-incrementation on the following operand.
            /// </summary>
            /// <returns>This operand starter to continue building.</returns>
            public _OperandStarter<D> PreIncrement() => FluentAction(this, InnerBuilder.PreIncrement);
            /// <summary>
            /// Performs a pre-decrementation on the following operand.
            /// </summary>
            /// <returns>This operand starter to continue building.</returns>
            public _OperandStarter<D> PreDecrement() => FluentAction(this, InnerBuilder.PreDecrement);
            /// <summary>
            /// Casts the following operand to the given type. If during execution the cast will turn out to be impossible, an exception will be thrown.
            /// </summary>
            /// <param name="type">The type to cast the operand to.</param>
            /// <returns>This operand starter to continue building.</returns>
            public _OperandStarter<D> Convert(Type type) => FluentAction(this, InnerBuilder.Convert, type);

            #endregion

            /// <summary>
            /// Starts a nested subexpression, which corresponds to placing an opening bracket.
            /// </summary>
            /// <returns>This operand to begin the new subexpression.</returns>
            public _OperandStarter<D> Brace() => FluentAction(this, InnerBuilder.Brace);
        }

        /// <summary>
        /// A part of the Fluent Express interface, used when a statement is started. Also, it can be used to finish building the expression.
        /// It shouldn't be used directly, but rather obtained somewhere in the Flex building chain.
        /// </summary>
        /// <typeparam name="D">The type of the delegate built.</typeparam>
        public class _StatementStarter<D> : _OperandStarter<D>
            where D : class
        {
            #region Constructors and inner builder setup

            /// <summary>
            /// Creates a statement starter with parameterless expression body builder.
            /// </summary>
            public _StatementStarter()
                : base(new BodyBuilder<D>())
            { }

            /// <summary>
            /// Creates a statement starter with expression body builder using parameters specified.
            /// </summary>
            /// <param name="parameters">The parameters to use in the built expression.</param>
            public _StatementStarter(IEnumerable<ParameterExpression> parameters)
                : base(new BodyBuilder<D>(parameters))
            { }

            /// <summary>
            /// Creates a statement starter with a given underlying expression body builder.
            /// </summary>
            /// <param name="builder">The underlying expression body builder.</param>
            public _StatementStarter(BodyBuilder<D> builder)
                : base(builder)
            { }

            #endregion

            #region Statement starting methods

            /// <summary>
            /// Declares a new variable in the current block's scope.
            /// </summary>
            /// <param name="type">The type of the new variable.</param>
            /// <param name="name">The name of the new variable.</param>
            /// <returns>This statement starter to build the next statement.</returns>
            public _StatementStarter<D> Declare(Type type, string name) => FluentAction(this, InnerBuilder.Declare, type, name);
            /// <summary>
            /// Declares a new variable in the current block's scope.
            /// </summary>
            /// <typeparam name="TVar">The type of the new variable.</typeparam>
            /// <param name="name">The name of the new variable.</param>
            /// <returns>This statement starter to build the next statement.</returns>
            public _StatementStarter<D> Declare<TVar>(string name) => FluentAction(this, InnerBuilder.Declare, typeof(TVar), name);
            /// <summary>
            /// Declares a new variable in the current block's scope, and begins its initialization assignment expression.
            /// </summary>
            /// <param name="type">The type of the new variable.</param>
            /// <param name="name">The name of the new variable.</param>
            /// <returns>An expression expander to build the assigned value.</returns>
            public _ExpressionExpander<D> DeclareAndAssign(Type type, string name) => FluentAction(new _ExpressionExpander<D>(InnerBuilder), InnerBuilder.DeclareAndAssign, type, name);
            /// <summary>
            /// Declares a new variable in the current block's scope, and begins its initialization assignment expression.
            /// </summary>
            /// <typeparam name="TVar">The type of the new variable.</typeparam>
            /// <param name="name">The name of the new variable.</param>
            /// <returns>An expression expander to build the assigned value.</returns>
            public _ExpressionExpander<D> DeclareAndAssign<TVar>(string name) => FluentAction(new _ExpressionExpander<D>(InnerBuilder), InnerBuilder.DeclareAndAssign, typeof(TVar), name);
            /// <summary>
            /// Declares and initializes a new variable with a given value.
            /// </summary>
            /// <param name="type">The type of the new variable.</param>
            /// <param name="name">The name of the new variable.</param>
            /// <param name="value">The value of the new variable.</param>
            /// <returns>This statement starter to build the next statement.</returns>
            public _StatementStarter<D> DeclareAndAssign(Type type, string name, object value) => FluentAction(this, InnerBuilder.DeclareAndAssign, type, name, value);
            /// <summary>
            /// Declares and initializes a new variable with a given value.
            /// </summary>
            /// <typeparam name="TVar">The type of the new variable.</typeparam>
            /// <param name="name">The name of the new variable.</param>
            /// <param name="value">The value of the new variable.</param>
            /// <returns>This statement starter to build the next statement.</returns>
            public _StatementStarter<D> DeclareAndAssign<TVar>(string name, object value) => FluentAction(this, InnerBuilder.DeclareAndAssign, typeof(TVar), name, value);

            #endregion

            #region Tree completion methods

            /// <summary>
            /// Passes the built expression.
            /// It provides no context for included parameters. If that context is needed, use CompleteLambda instead.
            /// </summary>
            /// <returns>The expression body built.</returns>
            public Expression Complete() => InnerBuilder.Complete();
            /// <summary>
            /// Passes a lambda expression using the built expression as a body.
            /// </summary>
            /// <returns>The lambda expression built.</returns>
            public Expression<D> CompleteLambda() => InnerBuilder.CompleteLambda();
            /// <summary>
            /// Passes a compiled delegate using the built expression as a body.
            /// </summary>
            /// <returns>The compiled executable delegate.</returns>
            public D CompleteDelegate() => InnerBuilder.CompleteDelegate();

            #endregion
        }

        /// <summary>
        /// A part of the Fluent Express interface, used when an argument list should be started.
        /// It shouldn't be used directly, but rather obtained somewhere in the Flex building chain.
        /// </summary>
        /// <typeparam name="D">The type of the delegate built.</typeparam>
        public class _ArgumentListStarter<D> : BaseFluentExpressBuilder<D>
            where D : class
        {
            /// <summary>
            /// Creates an argument list starter with a given underlying expression body builder.
            /// </summary>
            /// <param name="builder">The underlying expression body builder.</param>
            public _ArgumentListStarter(BodyBuilder<D> builder)
                : base(builder)
            { }

            /// <summary>
            /// Starts a new argument for the currently built parameterized action (method call, object construction or indexation).
            /// </summary>
            /// <returns>An operand starter to start building the argument.</returns>
            public _OperandStarter<D> Argument() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.Argument);
            /// <summary>
            /// Ends a list arguments for the currently built parameterized action (method call, object construction or indexation) and completes an expression for that action.
            /// </summary>
            /// <returns>An expression expander to continue building.</returns>
            public _ExpressionExpander<D> EndArgs() => FluentAction(new _ExpressionExpander<D>(InnerBuilder), InnerBuilder.EndArgs);
        }

        /// <summary>
        /// A part of the Fluent Express interface, used when an operand is started and ready to operate on.
        /// It shouldn't be used directly, but rather obtained somewhere in the Flex building chain.
        /// </summary>
        /// <typeparam name="D">The type of the delegate built.</typeparam>
        public class _ExpressionExpander<D> : BaseFluentExpressBuilder<D>
            where D : class
        {
            /// <summary>
            /// Creates an expression expander with a given underlying expression body builder.
            /// </summary>
            /// <param name="builder">The underlying expression body builder.</param>
            public _ExpressionExpander(BodyBuilder<D> builder)
                : base(builder)
            { }

            #region Member access

            /// <summary>
            /// Accesses a field of the currently built operand.
            /// </summary>
            /// <param name="name">The name of the field to access.</param>
            /// <returns>This expression expander to continue building.</returns>
            public _ExpressionExpander<D> Field(string name) => FluentAction(this, InnerBuilder.Field, name);
            /// <summary>
            /// Accesses a property of the currently built operand.
            /// </summary>
            /// <param name="name">The name of the property to access.</param>
            /// <returns>This expression expander to continue building.</returns>
            public _ExpressionExpander<D> Property(string name) => FluentAction(this, InnerBuilder.Property, name);

            #endregion

            #region Post-incrementation and decrementation

            /// <summary>
            /// Performs a post-incrementation on the current operand.
            /// </summary>
            /// <returns>This expression expander to continue building.</returns>
            public _ExpressionExpander<D> PostIncrement() => FluentAction(this, InnerBuilder.PostIncrement);
            /// <summary>
            /// Performs a post-decrementation on the current operand.
            /// </summary>
            /// <returns>This expression expander to continue building.</returns>
            public _ExpressionExpander<D> PostDecrement() => FluentAction(this, InnerBuilder.PostDecrement);

            #endregion

            #region Parameterised actions handling

            /// <summary>
            /// Performs a method call on the current operand, with a specific method and parameter names. That action must be followed by a list of arguments.
            /// </summary>
            /// <param name="name">The name of the method to call.</param>
            /// <param name="parameterTypes">The types of the method parameters.</param>
            /// <returns>An argument list starter for providing parameters.</returns>
            public _ArgumentListStarter<D> Call(string name, params Type[] parameterTypes) => FluentAction(new _ArgumentListStarter<D>(InnerBuilder), InnerBuilder.Call, name, parameterTypes);
            /// <summary>
            /// Performs a method call on the current operand for a given parameterless method. No list of arguments needs to be provided.
            /// </summary>
            /// <param name="name">The name of the method to call.</param>
            /// <returns>This expression expander to continue building.</returns>
            public _ExpressionExpander<D> CallNoArgs(string name) => FluentAction(this, InnerBuilder.CallNoArgs, name);
            /// <summary>
            /// Performs an indexation on the current operand, with specific index parameter types. That action must be followed by a list of arguments.
            /// </summary>
            /// <param name="indexTypes">The types of the index parameters.</param>
            /// <returns>An argument list starter for providing parameters.</returns>
            public _ArgumentListStarter<D> IndexBy(params Type[] indexTypes) => FluentAction(new _ArgumentListStarter<D>(InnerBuilder), InnerBuilder.IndexBy, indexTypes);
            /// <summary>
            /// Starts a new argument for the currently built parameterized action (method call, object construction or indexation).
            /// </summary>
            /// <returns>An operand starter to start building the argument.</returns>
            public _OperandStarter<D> Argument() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.Argument);
            /// <summary>
            /// Ends a list arguments for the currently built parameterized action (method call, object construction or indexation) and completes an expression for that action.
            /// </summary>
            /// <returns>This expression expander to continue building.</returns>
            public _ExpressionExpander<D> EndArgs() => FluentAction(this, InnerBuilder.EndArgs);

            #endregion

            #region Basic binary operations (+ is/as)

            /// <summary>
            /// Multiplies the previous and the following operand.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> Multiply() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.Multiply);
            /// <summary>
            /// Divides the previous and the following operand.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> Divide() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.Divide);
            /// <summary>
            /// Returns the remainder of a division of the previous operand by the following one.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> Modulo() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.Modulo);

            /// <summary>
            /// Adds the previous and the following operand.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> Add() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.Add);
            /// <summary>
            /// Subtracts the previous and the following operand.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> Subtract() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.Subtract);

            /// <summary>
            /// Shifts bits of the previous operand to the left, by the number equal to the following operand.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> ShiftLeft() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.ShiftLeft);
            /// <summary>
            /// Shifts bits of the previous operand to the right, by the number equal to the following operand.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> ShiftRight() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.ShiftRight);

            /// <summary>
            /// Checks whether the previous operand is greater than the following one.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> GreaterThan() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.GreaterThan);
            /// <summary>
            /// Checks whether the previous operand is greater than or equal to the following one.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> GreaterThanOrEqual() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.GreaterThanOrEqual);
            /// <summary>
            /// Checks whether the previous operand is lower than the following one.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> LessThan() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.LessThan);
            /// <summary>
            /// Checks whether the previous operand is lower than or equal to the following one.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> LessThanOrEqual() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.LessThanOrEqual);
            /// <summary>
            /// Checks whether the current operand is of a given type.
            /// </summary>
            /// <param name="type">The expected type of the current operand.</param>
            /// <returns>This expression expander to continue building.</returns>
            public _ExpressionExpander<D> Is(Type type) => FluentAction(this, InnerBuilder.Is, type);
            /// <summary>
            /// Passes the current operand as an instance of a given type, or null if the operand isn't of that type.
            /// </summary>
            /// <param name="type">The expected type of the current operand.</param>
            /// <returns>This expression expander to continue building.</returns>
            public _ExpressionExpander<D> As(Type type) => FluentAction(this, InnerBuilder.As, type);

            /// <summary>
            /// Checks whether the previous operand is equal to the following one.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> Equal() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.Equal);
            /// <summary>
            /// Checks whether the previous operand is not equal to the following one.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> NotEqual() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.NotEqual);

            /// <summary>
            /// Performs a bitwise AND operation on the previous and the following operand.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> BitwiseAnd() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.BitwiseAnd);
            /// <summary>
            /// Performs a bitwise XOR operation on the previous and the following operand.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> BitwiseXor() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.BitwiseXor);
            /// <summary>
            /// Performs a bitwise OR operation on the previous and the following operand.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> BitwiseOr() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.BitwiseOr);
            /// <summary>
            /// Checks whether both the previous and the following operand have "true" boolean value.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> And() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.And);
            /// <summary>
            /// Checks whether the previous and/or the following operand have "true" boolean value.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> Or() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.Or);

            #endregion

            #region Ternary conditional operator

            /// <summary>
            /// Begins an inline if operation, taking the previous operand as the condition and two following operands as the values.
            /// It's equivalent to placing a questionmark part of the ternary conditional operator ('?:').
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> InlineIf() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.InlineIf);
            /// <summary>
            /// Continues an inline if operation, taking the previous operand as the value to pass if the condition is true, and the following operand as the value to pass otherwise.
            /// It's equivalent to placing a colon part of the ternary conditional operator ('?:').
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> InlineElse() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.InlineElse);

            #endregion

            #region Assignment operations

            /// <summary>
            /// Assigns the variable in the previous operand to the value of the following operand.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> Assign() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.Assign);

            /// <summary>
            /// Multiplies the variable in the previous operand by the value of the following operand.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> MultiplyAssign() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.MultiplyAssign);
            /// <summary>
            /// Divides the variable in the previous operand by the value of the following operand.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> DivideAssign() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.DivideAssign);
            /// <summary>
            /// Performs a modulo operation on the variable in the previous operand using the value of the following operand.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> ModuloAssign() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.ModuloAssign);

            /// <summary>
            /// Increases the variable in the previous operand by the value of the following operand.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> AddAssign() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.AddAssign);
            /// <summary>
            /// Decreases the variable in the previous operand by the value of the following operand.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> SubtractAssign() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.SubtractAssign);

            /// <summary>
            /// Shifts bits of the variable in the previous operand to the left, by the value of the following operand.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> ShiftLeftAssign() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.ShiftLeftAssign);
            /// <summary>
            /// Shifts bits of the variable in the previous operand to the right, by the value of the following operand.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> ShiftRightAssign() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.ShiftRightAssign);

            /// <summary>
            /// Performs a bitwise AND operation on the variable in the previous operand with the value of the following operand.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> BitwiseAndAssign() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.BitwiseAndAssign);
            /// <summary>
            /// Performs a bitwise XOR operation on the variable in the previous operand with the value of the following operand.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> BitwiseXorAssign() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.BitwiseXorAssign);
            /// <summary>
            /// Performs a bitwise OR operation on the variable in the previous operand with the value of the following operand.
            /// </summary>
            /// <returns>An operand starter to build the right-side operand.</returns>
            public _OperandStarter<D> BitwiseOrAssign() => FluentAction(new _OperandStarter<D>(InnerBuilder), InnerBuilder.BitwiseOrAssign);

            #endregion

            /// <summary>
            /// Ends a nested subexpression, which corresponds to placing a closing bracket.
            /// </summary>
            /// <returns>This expression expander to continue building.</returns>
            public _ExpressionExpander<D> Unbrace() => FluentAction(this, InnerBuilder.Unbrace);

            /// <summary>
            /// Ends the currently built statement.
            /// </summary>
            /// <returns>A statement starter to build new statement or finish building.</returns>
            public _StatementStarter<D> EndStatement() => FluentAction(new _StatementStarter<D>(InnerBuilder), InnerBuilder.EndStatement);
            /// <summary>
            /// A shorthand to end the currently built statement. The name stands for "semicolon".
            /// </summary>
            /// <returns>A statement starter to build new statement or finish building.</returns>
            public _StatementStarter<D> sc() => FluentAction(new _StatementStarter<D>(InnerBuilder), InnerBuilder.EndStatement);
        }
    }
}
