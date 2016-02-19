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
        /// I decided not to abuse the fact that the class is private this time, so it has rather dull name.
        /// </summary>
        private class StatementBlock
        {
            // Creating a new statement block, with or without external parameters

            public StatementBlock()
            {
                VariablesKnown = new Dictionary<string, ParameterExpression>();
                CurrentSubexpression = new ExpressiveLasagna();
            }

            public StatementBlock(IDictionary<string, ParameterExpression> outerParameters)
            {
                VariablesKnown = new Dictionary<string, ParameterExpression>(outerParameters);
                CurrentSubexpression = new ExpressiveLasagna();
            }

            #region Statements management

            // The statements gathered so far
            private ICollection<Expression> Statements { get; } = new List<Expression>();

            // Adds a new statement to include in the statement block
            public void AddStatement(Expression statement) => Statements.Add(statement);

            // Creates a block expression from the statements gathered
            public Expression WrapStatements()
            {
                if (Statements.Count == 0) return Expression.Empty();
                else if (Statements.Count == 1) return Statements.First();
                else return Expression.Block(VariablesDeclared, Statements);
            }

            #endregion

            #region Variables management

            // Variables that are in the statement block's scope
            private IDictionary<string, ParameterExpression> VariablesKnown { get; }

            // Variables that were specifically declared in that statement block
            private ICollection<ParameterExpression> VariablesDeclared { get; } = new List<ParameterExpression>();

            // Declares a new statement block variable
            public void DeclareVar(Type type, string name)
            {
                if (VariablesKnown.ContainsKey(name)) throw new InvalidOperationException($"A variable with name \"{name}\" is already defined in this scope.");

                var exp = Expression.Variable(type, name);
                VariablesDeclared.Add(exp);
                VariablesKnown[name] = exp;
            }

            // Retrieves an expression for a known variable
            public ParameterExpression GetVar(string name) => VariablesKnown[name];

            #endregion

            #region Subexpression management

            // The current subexpression
            // It can correspond to a whole statement, a parameterized action argument or a bracketed expression.
            public ExpressiveLasagna CurrentSubexpression { get; set; }

            private Stack<ExpressiveLasagna> PreviousSubexpressions { get; } = new Stack<ExpressiveLasagna>();

            // Starts a new subexpression (i.e. an argument or a bracketed expression)
            public void BeginSubexpression()
            {
                PreviousSubexpressions.Push(CurrentSubexpression);
                CurrentSubexpression = new ExpressiveLasagna();
            }
            // Ends the current subexpression
            public void EndSubexpression()
            {
                CurrentSubexpression = PreviousSubexpressions.Pop();
            }

            #endregion
        }
    }
}
