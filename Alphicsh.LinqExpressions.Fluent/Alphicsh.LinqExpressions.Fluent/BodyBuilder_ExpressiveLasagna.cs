using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Alphicsh.LinqExpressions.Fluent
{
    public sealed partial class BodyBuilder<TDelegate>
        where TDelegate : class
    {
        /// <summary>
        /// This is a private class and I'm very happy that I don't need to come up with some overly descriptive name of it.
        /// That way I can use whichever name GitHub throws at me. *whistling*
        /// </summary>
        private class ExpressiveLasagna
        {
            // Some delegate types for wrapping different kinds of expressions.

            // Since LINQ expressions require all operands ready to go, and often an operation is scheduled before the operand is known
            // these delegates are required to build the expressions needed when all required operands are gathered.

            public delegate Expression UnaryWrapper(Expression exp);

            public delegate Expression BinaryWrapper(Expression left, Expression right);

            public delegate Expression ParameterizedWrapper(Expression target, IEnumerable<Expression> expressions);

            // An enumeration for the operator precedence "layers"
            // Ironically, the lower the enumeration value is, the higher precedence it corresponds to.

            public enum Precedence
            {
                Primary = 0,
                Unary = 1,
                Multiplicative = 2,
                Additive = 3,
                Shift = 4,
                Relational = 5,
                Equality = 6,
                BitwiseAND = 7,
                BitwiseXOR = 8,
                BitwiseOR = 9,
                ConditionalAND = 10,
                ConditionalOR = 11,
                ConditionalQuestionmark = 12,
                Assignment = 13,
                Expression = 14
            };

            #region Resolving precedence layers

            // Completes the entire expression stored; usually a single statement or a bracketed subexpression.
            public Expression ResolveExpression(Expression exp)
            {
                return ResolveLayer(exp, Precedence.Expression);
            }

            // Completes a subexpression from the highest to the provided precedence

            // For example, if addition operand is expected
            // a primary layer operand is obtained
            // then potentially altered by the pending unary expressions
            // then the result of that might be combined with some previous operand using a multiplication/division operation
            // then the result of the multiplication might be combined with yet another pending operand, using some addition or subtraction
            // and finally, that potential addition containing possible multiplication that involves optional alterations of the primary operand
            // is returned to whichever code needs that addition operand.

            // TL;DR: It works!
            public Expression ResolveLayer(Expression exp, Precedence prec)
            {
                switch (prec)
                {
                    case Precedence.Primary:
                        return _ResolvePrimaryLayer(exp);

                    case Precedence.Unary:
                        return _ResolveUnaryLayer(ResolveLayer(exp, prec - 1));

                    case Precedence.Multiplicative:
                    case Precedence.Additive:
                    case Precedence.Shift:
                    case Precedence.Relational:
                    case Precedence.Equality:
                    case Precedence.BitwiseAND:
                    case Precedence.BitwiseXOR:
                    case Precedence.BitwiseOR:
                    case Precedence.ConditionalAND:
                    case Precedence.ConditionalOR:
                        return _ResolveBinaryLayer(ResolveLayer(exp, prec - 1), prec);

                    case Precedence.ConditionalQuestionmark:
                        return _ResolveConditionalQuestionmarkLayer(ResolveLayer(exp, prec - 1));

                    case Precedence.Assignment:
                        return _ResolveAssignmentLayer(ResolveLayer(exp, prec - 1));

                    case Precedence.Expression:
                        return ResolveLayer(exp, prec - 1);

                    default:
                        throw new ArgumentException($"Unknown precedence of value {prec}.");
                }
            }

            // Retrieves a primary layer operand
            // If a parameterised action (method call, object construction, indexation) has its arguments defined
            // an expression corresponding to that action is built.
            private Expression _ResolvePrimaryLayer(Expression exp)
            {
                if (PendingPrimaryWrapper != null)
                {
                    exp = PendingPrimaryWrapper(PendingExpressions[Precedence.Primary], PendingPrimaryParameters);
                    PendingExpressions.Remove(Precedence.Primary);
                    PendingPrimaryWrapper = null;
                    PendingPrimaryParameters.Clear();
                }
                return exp;
            }

            // Retrieves a unary layer operand
            // Basically, it applies earlier declared unary operations (e.g. negation, type conversion) to the passed operand
            private Expression _ResolveUnaryLayer(Expression exp)
            {
                while (PendingUnaryOperations.Any())
                {
                    exp = PendingUnaryOperations.Pop()(exp);
                }
                return exp;
            }

            // Retrieves a binary layer operand
            // If an operation has been scheduled for that layer, the passed operand and earlier stored operand are wrapped in a nice binary expression corresponding to the operation
            private Expression _ResolveBinaryLayer(Expression exp, Precedence prec)
            {
                if (PendingBinaryOperations.ContainsKey(prec))
                {
                    exp = PendingBinaryOperations[prec](PendingExpressions[prec], exp);
                    PendingBinaryOperations.Remove(prec);
                    PendingExpressions.Remove(prec);
                }
                return exp;
            }

            // Retrieves an inline if layer operand
            // which I myself like to call "conditional questionmark" layer, because I think it sounds silly

            // It applies an operand to the most recently scheduled conditional operation
            // If it's the first value of that operation (i.e. returned if true), it's stored for later use and the function returns null (null null, not expression representing null)
            // If it's the second value of that operation (i.e. returned if false), the expression for that operation is built using earlier stored condition and if-true value
            // Then, if there are more scheduled conditional operations, that expression will be passed to next pending operation using the just described rules
            // and if there are no more operations left, it will (finally!) pass the resulting conditional operation to the calling code.

            // TL;DR: It works, too!
            private Expression _ResolveConditionalQuestionmarkLayer(Expression exp)
            {
                while (PendingConditionals.Any())
                {
                    var currentConditional = PendingConditionals.Pop();
                    if (currentConditional.Item2 == null)
                    {
                        PendingConditionals.Push(new Tuple<Expression, Expression>(currentConditional.Item1, exp));
                        return null;
                    }
                    else
                    {
                        exp = Expression.Condition(currentConditional.Item1, currentConditional.Item2, exp);
                    }
                }
                return exp;
            }
            
            // Retrieves an assignment layer operand
            // It builds a chain of scheduled assignment operations, starting from the latest assignment operation and going back to the first one
            // That way the assignment acts in a right-associative way, as it should
            private Expression _ResolveAssignmentLayer(Expression exp)
            {
                while (PendingAssignments.Any())
                {
                    var assigment = PendingAssignments.Pop();
                    exp = assigment.Item2(assigment.Item1, exp);
                }
                return exp;
            }

            #endregion

            // Schedules a parameterized action (method call, object construction or indexation)
            // to wrap when all of its arguments are known
            public void ScheduleParameterizedOperation(Expression target, ParameterizedWrapper wrapper)
            {
                PendingExpressions[Precedence.Primary] = target;
                PendingPrimaryWrapper = wrapper;
            }
            
            // Adds an argument of the parameterized action
            public void AddPrimaryParameter(Expression parameter)
            {
                PendingPrimaryParameters.Add(parameter);
            }

            // Schedules an unary operation on to-be-made operand
            public void ScheduleUnaryOperation(UnaryWrapper wrapper)
            {
                PendingUnaryOperations.Push(wrapper);
            }

            // Schedules a binary operation, storing a left-side operand to use when both sides are known
            // The binary operation is associated with its respective precedence
            public void ScheduleBinaryOperation(Expression target, BinaryWrapper wrapper, Precedence prec)
            {
                PendingExpressions[prec] = target;
                PendingBinaryOperations[prec] = wrapper;
            }

            // Schedules a new inline if operation
            // to be resoled when true-value operand and false-value operand are known
            public void ScheduleConditionalOperation(Expression condition)
            {
                PendingConditionals.Push(new Tuple<Expression, Expression>(condition, null));
            }

            // Schedules a new assignment, storing the assignable expression to combine with to-be-resolved value
            public void ScheduleAssigment(Expression target, BinaryWrapper wrapper)
            {
                PendingAssignments.Push(new Tuple<Expression, BinaryWrapper>(target, wrapper));
            }

            // State of the expression building

            private IDictionary<Precedence, Expression> PendingExpressions { get; } = new Dictionary<Precedence, Expression>();

            private ParameterizedWrapper PendingPrimaryWrapper { get; set; }
            private ICollection<Expression> PendingPrimaryParameters { get; } = new List<Expression>();

            private Stack<UnaryWrapper> PendingUnaryOperations { get; } = new Stack<UnaryWrapper>();

            private IDictionary<Precedence, BinaryWrapper> PendingBinaryOperations { get; } = new Dictionary<Precedence, BinaryWrapper>();

            private Stack<Tuple<Expression, Expression>> PendingConditionals { get; } = new Stack<Tuple<Expression, Expression>>();

            private Stack<Tuple<Expression, BinaryWrapper>> PendingAssignments { get; } = new Stack<Tuple<Expression, BinaryWrapper>>();
        }
    }
}
