using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Alphicsh.LinqExpressions.Fluent
{
    /// <summary>
    /// As its name clearly implies, it's responsible for building lambdas/method bodies. It's probably the strongest component in the entire Fluent Express library.
    /// It can be used to build custom fluent interfaces while still getting access to the core expression building functionality. Generally, it shouldn't be used in the main code.
    /// Now stop staring at these muscles. Shoo!
    /// </summary>
    /// <typeparam name="TDelegate">The type of the delegate built.</typeparam>
    public sealed partial class BodyBuilder<TDelegate>
        where TDelegate : class
    {
        /// <summary>
        /// Creates a new expression body builder without any parameters.
        /// </summary>
        public BodyBuilder()
        {
            PassedParameters = new ParameterExpression[] { };
            CurrentBlock = new StatementBlock();
        }

        /// <summary>
        /// Creates a new expression body builder with a collection of parameters.
        /// </summary>
        /// <param name="parameters">The parameters of the resulting expression.</param>
        public BodyBuilder(IEnumerable<ParameterExpression> parameters)
        {
            PassedParameters = parameters;
            CurrentBlock = new StatementBlock(parameters.ToDictionary(p => p.Name, p => p));
        }

        private IEnumerable<ParameterExpression> PassedParameters { get; } 

        private StatementBlock CurrentBlock { get; set; }

        private Expression CurrentFragment { get; set; }
    }
}
