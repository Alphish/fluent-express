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
        /// Begins building a parameterless void procedure.
        /// </summary>
        /// <returns>An expression builder.</returns>
        public static _StatementStarter<Action> StartAction()
        {
            return new _StatementStarter<Action>();
        }

        /// <summary>
        /// Begins building a parameterless expression that evaluates to the type specified.
        /// </summary>
        /// <typeparam name="TReturn">The type the expression should evaluate to.</typeparam>
        /// <returns>An expression builder.</returns>
        public static _StatementStarter<Func<TReturn>> StartExpression<TReturn>()
        {
            return new _StatementStarter<Func<TReturn>>();
        }

        /// <summary>
        /// Begins building a lambda expression with specific parameter types and names.
        /// </summary>
        /// <typeparam name="TDelegate">The delegate type the expression would represent.</typeparam>
        /// <param name="parameterNames">The names of the lambda parameters, in the </param>
        /// <returns></returns>
        public static _StatementStarter<TDelegate> StartLambda<TDelegate>(params string[] parameterNames)
            where TDelegate : class
        {
            var delegateType = typeof(TDelegate);
            if (!typeof(MulticastDelegate).IsAssignableFrom(delegateType) || delegateType == typeof(MulticastDelegate))
            {
                throw new ArgumentException("The type argument provided is not delegate.", nameof(TDelegate));
            }

            var parameters = delegateType.GetMethod("Invoke").GetParameters();

            if (parameterNames.Length != parameters.Length)
            {
                throw new ArgumentException("The number of delegate parameters is different from the number of parameter names.", nameof(parameterNames));
            }

            var parameterExpressions = parameters.Zip(parameterNames, (p, name) => Expression.Parameter(p.ParameterType, name)).ToList();

            return new _StatementStarter<TDelegate>(parameterExpressions);
        }
    }
}
