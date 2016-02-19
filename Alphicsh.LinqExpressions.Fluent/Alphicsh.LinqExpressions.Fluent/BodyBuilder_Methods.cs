using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Alphicsh.LinqExpressions.Fluent
{
    public sealed partial class BodyBuilder<TDelegate>
        where TDelegate : class
    {
        #region Statement start only

        /// <summary>
        /// Declares a new variable in the current block's scope.
        /// </summary>
        /// <param name="type">The type of the new variable.</param>
        /// <param name="name">The name of the new variable.</param>
        public void Declare(Type type, string name) => CurrentBlock.DeclareVar(type, name);

        /// <summary>
        /// Declares a new variable in the current block's scope, and begins its initialization assignment expression.
        /// </summary>
        /// <param name="type">The type of the new variable.</param>
        /// <param name="name">The name of the new variable.</param>
        public void DeclareAndAssign(Type type, string name)
        {
            Declare(type, name);
            Var(name);
            Assign();
        }

        /// <summary>
        /// Declares and initializes a new variable with a given value.
        /// </summary>
        /// <param name="type">The type of the new variable.</param>
        /// <param name="name">The name of the new variable.</param>
        /// <param name="value">The value of the new variable.</param>
        public void DeclareAndAssign(Type type, string name, object value)
        {
            Declare(type, name);
            Var(name);
            Assign();
            Const(value, type);
            EndStatement();
        }

        /// <summary>
        /// Adds a blank statement to the current block.
        /// </summary>
        public void DoNothing() => CurrentBlock.AddStatement(Expression.Empty());

        #endregion

        #region Starting the operand

        /// <summary>
        /// Adds a null value to the built expression.
        /// </summary>
        public void Null() => CurrentFragment = Expression.Constant(null);
        /// <summary>
        /// Adds a typed null value to the built expression.
        /// </summary>
        /// <param name="type">The type of the null value.</param>
        public void Null(Type type) => CurrentFragment = Expression.Constant(null, type);
        /// <summary>
        /// Adds a default value for the given type to the built expression.
        /// </summary>
        /// <param name="type">The type to get the default value of.</param>
        public void Default(Type type) => CurrentFragment = Expression.Default(type);

        /// <summary>
        /// Adds a constant value to the built expression.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public void Const(object value) => CurrentFragment = Expression.Constant(value);
        /// <summary>
        /// Adds a constant value with a specific type to the built expression.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <param name="type">The type of the value.</param>
        public void Const(object value, Type type) => CurrentFragment = Expression.Constant(value, type);

        /// <summary>
        /// Adds a variable value to the built expression.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        public void Var(string name) => CurrentFragment = CurrentBlock.GetVar(name);

        /// <summary>
        /// Adds a subexpression to the built expression.
        /// </summary>
        /// <param name="exp">The subexpression to add.</param>
        public void Subexpression(Expression exp) => CurrentFragment = exp;

        // "New" operation appears in the "Parameterised operations" region, even though it can begin an operand

        #endregion

        #region Accessing members

        /// <summary>
        /// Accesses a field of the currently built operand.
        /// </summary>
        /// <param name="name">The name of the field to access.</param>
        public void Field(string name) => CurrentFragment = Expression.Field(CurrentFragment, name);
        /// <summary>
        /// Accesses a property of the currently built operand.
        /// </summary>
        /// <param name="name">The name of the property to access.</param>
        public void Property(string name) => CurrentFragment = Expression.Property(CurrentFragment, name);

        #endregion

        #region Post-incrementation and decrementation

        /// <summary>
        /// Performs a post-incrementation on the current operand.
        /// </summary>
        public void PostIncrement() => CurrentFragment = Expression.PostIncrementAssign(CurrentFragment);
        /// <summary>
        /// Performs a post-decrementation on the current operand.
        /// </summary>
        public void PostDecrement() => CurrentFragment = Expression.PostDecrementAssign(CurrentFragment);

        #endregion

        #region Parameterized operations

        // a helper variable to recognize parameterless calls
        private bool _JustStartedCall;

        /// <summary>
        /// Adds a newly constructed object of specific type to the built expression, with specific parameter types. That action must be followed by a list of arguments.
        /// </summary>
        /// <param name="newType">The type of the constructed object.</param>
        /// <param name="parameterTypes">The types of the constructor parameters.</param>
        public void New(Type newType, params Type[] parameterTypes)
        {
            ConstructorInfo constructor = newType.GetConstructor(parameterTypes);
            CurrentBlock.CurrentSubexpression.ScheduleParameterizedOperation(
                null,
                (ex, parameters) => Expression.New(constructor, parameters)
                );
            _JustStartedCall = true;
        }

        /// <summary>
        /// Adds a new object of specific type to the built expression, constructed with a parameterless constructor. No list of arguments needs to be provided.
        /// </summary>
        /// <param name="newType">The type of the constructed object.</param>
        public void New(Type newType)
        {
            ConstructorInfo constructor = newType.GetConstructor(new Type[] { });
            CurrentFragment = Expression.New(constructor);
        }

        /// <summary>
        /// Performs a method call on the current operand, with a specific method and parameter names. That action must be followed by a list of arguments.
        /// </summary>
        /// <param name="name">The name of the method to call.</param>
        /// <param name="parameterTypes">The types of the method parameters.</param>
        public void Call(string name, params Type[] parameterTypes)
        {
            MethodInfo method = _DoGetMethod(CurrentFragment.Type, (t) => t.GetMethod(name, parameterTypes));
            CurrentBlock.CurrentSubexpression.ScheduleParameterizedOperation(
                CurrentFragment,
                (ex, parameters) => Expression.Call(ex, method, parameters.ToArray())
                );
            CurrentFragment = null;
            _JustStartedCall = true;
        }

        /// <summary>
        /// Performs a method call on the current operand for a given parameterless method. No list of arguments needs to be provided.
        /// </summary>
        /// <param name="name"></param>
        public void CallNoArgs(string name)
        {
            MethodInfo method = _DoGetMethod(CurrentFragment.Type, (t) => t.GetMethod(name, new Type[] { }));
            CurrentFragment = Expression.Call(CurrentFragment, method);
        }

        // finding class or interface method, including methods declared on another interface 
        // if you know some better way to find an interface method up the hierarchy, please do tell!
        private MethodInfo _DoGetMethod(Type classOrInterfaceType, Func<Type, MethodInfo> methodSelector)
        {
            if (!classOrInterfaceType.IsInterface) return methodSelector(classOrInterfaceType);

            MethodInfo result = null;
            result = methodSelector(classOrInterfaceType);
            if (result == null)
            {
                foreach (var superInterface in classOrInterfaceType.GetInterfaces())
                {
                    result = _DoGetMethod(superInterface, methodSelector);
                    if (result != null) break;
                }
            }
            return result;
        }

        /// <summary>
        /// Performs an indexation on the current operand, with specific index parameter types. That action must be followed by a list of arguments.
        /// </summary>
        /// <param name="indexTypes">The types of the index parameters.</param>
        public void IndexBy(params Type[] indexTypes)
        {
            // first, a correct indexer property needs to be found
            if (indexTypes.Length == 0) throw new ArgumentException("At least one type to index by must be provided.");

            var type = CurrentFragment.Type;
            var indexerCandidates = type.GetProperties().Where(prop => prop.GetIndexParameters().Select(param => param.ParameterType).SequenceEqual(indexTypes)).ToList();
            if (indexerCandidates.Count == 0) throw new ArgumentException("The given type sequence doesn't correspond to any defined indexer.");
            else if (indexerCandidates.Count > 1) throw new ArgumentException("Multiple index types has been defined for that type sequence.");     // that would be quite odd, but it's better to be prepared, I guess...?
            PropertyInfo indexer = indexerCandidates.First();

            // scheduling an indexation using the found property
            CurrentBlock.CurrentSubexpression.ScheduleParameterizedOperation(
                CurrentFragment,
                (ex, parameters) => Expression.MakeIndex(ex, indexer, parameters)
                );
            CurrentFragment = null;
            _JustStartedCall = true;
        }

        /// <summary>
        /// Starts a new argument for the currently built parameterized action (method call, object construction or indexation).
        /// </summary>
        public void Argument()
        {
            if (!_JustStartedCall)
            {
                CurrentFragment = CurrentBlock.CurrentSubexpression.ResolveExpression(CurrentFragment);
                CurrentBlock.EndSubexpression();
                CurrentBlock.CurrentSubexpression.AddPrimaryParameter(CurrentFragment);
            }
            CurrentBlock.BeginSubexpression();
            CurrentFragment = null;
            _JustStartedCall = false;
        }

        /// <summary>
        /// Ends a list arguments for the currently built parameterized action (method call, object construction or indexation) and completes an expression for that action.
        /// </summary>
        public void EndArgs()
        {
            if (!_JustStartedCall)
            {
                CurrentFragment = CurrentBlock.CurrentSubexpression.ResolveExpression(CurrentFragment);
                CurrentBlock.EndSubexpression();
                CurrentBlock.CurrentSubexpression.AddPrimaryParameter(CurrentFragment);
            }
            CurrentFragment = CurrentBlock.CurrentSubexpression.ResolveLayer(null, ExpressiveLasagna.Precedence.Primary);
            _JustStartedCall = false;
        }

        #endregion

        #region Unary operations

        // Stacks an unary operator to apply to the following operand.
        private void _NextUnary(ExpressiveLasagna.UnaryWrapper wrapper)
        {
            if (CurrentFragment != null) throw new InvalidOperationException("This unary operator must be provided before the operand.");
            CurrentBlock.CurrentSubexpression.ScheduleUnaryOperation(wrapper);
        }

        /// <summary>
        /// Imaginarily applies an arithmetic positive plus to the following operand, which essentially does nothing. You might as well multiply it by 1.
        /// </summary>
        public void Plus() { /* do nothing */ }
        /// <summary>
        /// Changes the sign of the following operand to its negative.
        /// </summary>
        public void Minus() => _NextUnary(Expression.Negate);
        /// <summary>
        /// Switches the boolean value of the following operand.
        /// </summary>
        public void Not() => _NextUnary(Expression.Not);
        /// <summary>
        /// Returns a bitwise negation of the following operand.
        /// </summary>
        public void BitwiseNot() => _NextUnary(Expression.OnesComplement);
        /// <summary>
        /// Performs a pre-incrementation on the following operand.
        /// </summary>
        public void PreIncrement() => _NextUnary(Expression.PreIncrementAssign);
        /// <summary>
        /// Performs a pre-decrementation on the following operand.
        /// </summary>
        public void PreDecrement() => _NextUnary(Expression.PreDecrementAssign);
        /// <summary>
        /// Casts the following operand to the given type. If during execution the cast will turn out to be impossible, an exception will be thrown.
        /// </summary>
        /// <param name="type">The type to cast the operand to.</param>
        public void Convert(Type type) => _NextUnary((exp) => Expression.Convert(exp, type));

        #endregion

        #region Basic binary operations (+ is/as)

        // Schedules the binary operation taking the previous and the following operand as arguments.
        private void _NextBinary(ExpressiveLasagna.BinaryWrapper wrapper, ExpressiveLasagna.Precedence prec)
        {
            CurrentFragment = CurrentBlock.CurrentSubexpression.ResolveLayer(CurrentFragment, prec);
            CurrentBlock.CurrentSubexpression.ScheduleBinaryOperation(CurrentFragment, wrapper, prec);
            CurrentFragment = null;
        }

        /// <summary>
        /// Multiplies the previous and the following operand.
        /// </summary>
        public void Multiply() => _NextBinary(Expression.Multiply, ExpressiveLasagna.Precedence.Multiplicative);
        /// <summary>
        /// Divides the previous and the following operand.
        /// </summary>
        public void Divide() => _NextBinary(Expression.Divide, ExpressiveLasagna.Precedence.Multiplicative);
        /// <summary>
        /// Returns the remainder of a division of the previous operand by the following one.
        /// </summary>
        public void Modulo() => _NextBinary(Expression.Modulo, ExpressiveLasagna.Precedence.Multiplicative);

        /// <summary>
        /// Adds the previous and the following operand.
        /// </summary>
        public void Add() => _NextBinary(Expression.Add, ExpressiveLasagna.Precedence.Additive);
        /// <summary>
        /// Subtracts the previous and the following operand.
        /// </summary>
        public void Subtract() => _NextBinary(Expression.Subtract, ExpressiveLasagna.Precedence.Additive);

        /// <summary>
        /// Shifts bits of the previous operand to the left, by the number equal to the following operand.
        /// </summary>
        public void ShiftLeft() => _NextBinary(Expression.LeftShift, ExpressiveLasagna.Precedence.Shift);
        /// <summary>
        /// Shifts bits of the previous operand to the right, by the number equal to the following operand.
        /// </summary>
        public void ShiftRight() => _NextBinary(Expression.RightShift, ExpressiveLasagna.Precedence.Shift);

        /// <summary>
        /// Checks whether the previous operand is greater than the following one.
        /// </summary>
        public void GreaterThan() => _NextBinary(Expression.GreaterThan, ExpressiveLasagna.Precedence.Relational);
        /// <summary>
        /// Checks whether the previous operand is greater than or equal to the following one.
        /// </summary>
        public void GreaterThanOrEqual() => _NextBinary(Expression.GreaterThanOrEqual, ExpressiveLasagna.Precedence.Relational);
        /// <summary>
        /// Checks whether the previous operand is lower than the following one.
        /// </summary>
        public void LessThan() => _NextBinary(Expression.LessThan, ExpressiveLasagna.Precedence.Relational);
        /// <summary>
        /// Checks whether the previous operand is lower than or equal to the following one.
        /// </summary>
        public void LessThanOrEqual() => _NextBinary(Expression.LessThanOrEqual, ExpressiveLasagna.Precedence.Relational);
        /// <summary>
        /// Checks whether the current operand is of a given type.
        /// </summary>
        /// <param name="type">The expected type of the current operand.</param>
        public void Is(Type type)
        {
            CurrentFragment = CurrentBlock.CurrentSubexpression.ResolveLayer(CurrentFragment, ExpressiveLasagna.Precedence.Relational);
            CurrentFragment = Expression.TypeIs(CurrentFragment, type);
        }
        /// <summary>
        /// Passes the current operand as an instance of a given type, or null if the operand isn't of that type.
        /// </summary>
        /// <param name="type">The expected type of the current operand.</param>
        public void As(Type type)
        {
            CurrentFragment = CurrentBlock.CurrentSubexpression.ResolveLayer(CurrentFragment, ExpressiveLasagna.Precedence.Relational);
            CurrentFragment = Expression.TypeAs(CurrentFragment, type);
        }

        /// <summary>
        /// Checks whether the previous operand is equal to the following one.
        /// </summary>
        public void Equal() => _NextBinary(Expression.Equal, ExpressiveLasagna.Precedence.Equality);
        /// <summary>
        /// Checks whether the previous operand is not equal to the following one.
        /// </summary>
        public void NotEqual() => _NextBinary(Expression.NotEqual, ExpressiveLasagna.Precedence.Equality);

        /// <summary>
        /// Performs a bitwise AND operation on the previous and the following operand.
        /// </summary>
        public void BitwiseAnd() => _NextBinary(Expression.And, ExpressiveLasagna.Precedence.BitwiseAND);
        /// <summary>
        /// Performs a bitwise XOR operation on the previous and the following operand.
        /// </summary>
        public void BitwiseXor() => _NextBinary(Expression.ExclusiveOr, ExpressiveLasagna.Precedence.BitwiseXOR);
        /// <summary>
        /// Performs a bitwise OR operation on the previous and the following operand.
        /// </summary>
        public void BitwiseOr() => _NextBinary(Expression.Or, ExpressiveLasagna.Precedence.BitwiseOR);
        /// <summary>
        /// Checks whether both the previous and the following operand have "true" boolean value.
        /// </summary>
        public void And() => _NextBinary(Expression.AndAlso, ExpressiveLasagna.Precedence.ConditionalAND);
        /// <summary>
        /// Checks whether the previous and/or the following operand have "true" boolean value.
        /// </summary>
        public void Or() => _NextBinary(Expression.OrElse, ExpressiveLasagna.Precedence.ConditionalOR);

        #endregion

        #region Ternary conditional operator

        /// <summary>
        /// Begins an inline if operation, taking the previous operand as the condition and two following operands as the values.
        /// It's equivalent to placing a questionmark part of the ternary conditional operator ('?:').
        /// </summary>
        public void InlineIf()
        {
            CurrentBlock.CurrentSubexpression.ScheduleConditionalOperation(CurrentFragment);
            CurrentFragment = null;
        }
        /// <summary>
        /// Continues an inline if operation, taking the previous operand as the value to pass if the condition is true, and the following operand as the value to pass otherwise.
        /// It's equivalent to placing a colon part of the ternary conditional operator ('?:').
        /// </summary>
        public void InlineElse()
        {
            CurrentFragment = CurrentBlock.CurrentSubexpression.ResolveLayer(CurrentFragment, ExpressiveLasagna.Precedence.ConditionalQuestionmark);
            if (CurrentFragment != null) throw new InvalidOperationException("Cannot add inline else (':') without matching inline if ('?').");
        }

        #endregion

        #region Assignments

        // Schedules the assignment of the previous operand to the value of the following operand.
        private void _NextAssignment(ExpressiveLasagna.BinaryWrapper wrapper)
        {
            CurrentFragment = CurrentBlock.CurrentSubexpression.ResolveLayer(CurrentFragment, ExpressiveLasagna.Precedence.ConditionalQuestionmark);
            CurrentBlock.CurrentSubexpression.ScheduleAssigment(CurrentFragment, wrapper);
            CurrentFragment = null;
        }

        /// <summary>
        /// Assigns the variable in the previous operand to the value of the following operand.
        /// </summary>
        public void Assign() => _NextAssignment(Expression.Assign);

        /// <summary>
        /// Multiplies the variable in the previous operand by the value of the following operand.
        /// </summary>
        public void MultiplyAssign() => _NextAssignment(Expression.MultiplyAssign);
        /// <summary>
        /// Divides the variable in the previous operand by the value of the following operand.
        /// </summary>
        public void DivideAssign() => _NextAssignment(Expression.DivideAssign);
        /// <summary>
        /// Performs a modulo operation on the variable in the previous operand using the value of the following operand.
        /// </summary>
        public void ModuloAssign() => _NextAssignment(Expression.ModuloAssign);

        /// <summary>
        /// Increases the variable in the previous operand by the value of the following operand.
        /// </summary>
        public void AddAssign() => _NextAssignment(Expression.AddAssign);
        /// <summary>
        /// Decreases the variable in the previous operand by the value of the following operand.
        /// </summary>
        public void SubtractAssign() => _NextAssignment(Expression.SubtractAssign);

        /// <summary>
        /// Shifts bits of the variable in the previous operand to the left, by the value of the following operand.
        /// </summary>
        public void ShiftLeftAssign() => _NextAssignment(Expression.LeftShiftAssign);
        /// <summary>
        /// Shifts bits of the variable in the previous operand to the right, by the value of the following operand.
        /// </summary>
        public void ShiftRightAssign() => _NextAssignment(Expression.RightShiftAssign);

        /// <summary>
        /// Performs a bitwise AND operation on the variable in the previous operand with the value of the following operand.
        /// </summary>
        public void BitwiseAndAssign() => _NextAssignment(Expression.AndAssign);
        /// <summary>
        /// Performs a bitwise XOR operation on the variable in the previous operand with the value of the following operand.
        /// </summary>
        public void BitwiseXorAssign() => _NextAssignment(Expression.ExclusiveOrAssign);
        /// <summary>
        /// Performs a bitwise OR operation on the variable in the previous operand with the value of the following operand.
        /// </summary>
        public void BitwiseOrAssign() => _NextAssignment(Expression.OrAssign);

        #endregion

        /// <summary>
        /// Starts a nested subexpression, which corresponds to placing an opening bracket.
        /// </summary>
        public void Brace()
        {
            CurrentBlock.BeginSubexpression();
            CurrentFragment = null;
        }

        /// <summary>
        /// Ends a nested subexpression, which corresponds to placing a closing bracket.
        /// </summary>
        public void Unbrace()
        {
            CurrentFragment = CurrentBlock.CurrentSubexpression.ResolveExpression(CurrentFragment);
            CurrentBlock.EndSubexpression();
        }

        /// <summary>
        /// Ends the currently built statement.
        /// </summary>
        public void EndStatement()
        {
            CurrentBlock.AddStatement(CurrentBlock.CurrentSubexpression.ResolveExpression(CurrentFragment));
            CurrentFragment = null;
        }

        #region Completing the expression

        /// <summary>
        /// Passes the built expression.
        /// It provides no context for included parameters. If that context is needed, use CompleteLambda instead.
        /// </summary>
        /// <returns>The expression body built.</returns>
        public Expression Complete()
        {
            return CurrentBlock.WrapStatements();
        }
        /// <summary>
        /// Passes a lambda expression using the built expression as a body.
        /// </summary>
        /// <returns>The lambda expression built.</returns>
        public Expression<TDelegate> CompleteLambda()
        {
            return Expression.Lambda<TDelegate>(CurrentBlock.WrapStatements(), PassedParameters);
        }
        /// <summary>
        /// Passes a compiled delegate using the built expression as a body.
        /// </summary>
        /// <returns>The compiled executable delegate.</returns>
        public TDelegate CompleteDelegate()
        {
            return CompleteLambda().Compile();
        }

        #endregion
    }
}
