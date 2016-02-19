using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphicsh.LinqExpressions.Fluent
{
    /// <summary>
    /// A base class of fluent interfaces for expressions building. Custom fluent interfaces should derive from it, directly or not. 
    /// </summary>
    /// <typeparam name="TDelegate">The type of the delegate built.</typeparam>
    public abstract class BaseFluentExpressBuilder<TDelegate>
        where TDelegate : class
    {
        /// <summary>
        /// Creates a fluent builder with a given underlying expression body builder.
        /// </summary>
        /// <param name="builder">The underlying expression body builder.</param>
        protected BaseFluentExpressBuilder(BodyBuilder<TDelegate> builder)
        {
            InnerBuilder = builder;
        }

        /// <summary>
        /// Gets the underlying expression body builder.
        /// </summary>
        protected BodyBuilder<TDelegate> InnerBuilder { get; }

        /// <summary>
        /// Performs an expression building action and passes a fluent builder; whether itself or a new one to change available methods.
        /// </summary>
        /// <typeparam name="TBuilder">The exact type of the next fluent builder to pass.</typeparam>
        /// <param name="next">The fluent builder itself ("this") or a new fluent builder with different methods.</param>
        /// <param name="toExecute">The expression building action to execute; typically that of inner expression body builder.</param>
        /// <returns>The next fluent builder to build expressions from.</returns>
        protected TBuilder FluentAction<TBuilder>(TBuilder next, Action toExecute)
            where TBuilder : BaseFluentExpressBuilder<TDelegate>
        {
            toExecute();
            return next;
        }

        /// <summary>
        /// Performs an expression building action and passes a fluent builder; whether itself or a new one to change available methods.
        /// </summary>
        /// <typeparam name="TBuilder">The exact type of the next fluent builder to pass.</typeparam>
        /// <param name="next">The fluent builder itself ("this") or a new fluent builder with different methods.</param>
        /// <param name="toExecute">The expression building action to execute; typically that of inner expression body builder.</param>
        /// <param name="a1">The first argument of the expression building action.</param>
        /// <returns>The next fluent builder to build expressions from.</returns>
        protected TBuilder FluentAction<TBuilder, A1>(TBuilder next, Action<A1> toExecute, A1 a1)
            where TBuilder : BaseFluentExpressBuilder<TDelegate>
        {
            toExecute(a1);
            return next;
        }

        /// <summary>
        /// Performs an expression building action and passes a fluent builder; whether itself or a new one to change available methods.
        /// </summary>
        /// <typeparam name="TBuilder">The exact type of the next fluent builder to pass.</typeparam>
        /// <param name="next">The fluent builder itself ("this") or a new fluent builder with different methods.</param>
        /// <param name="toExecute">The expression building action to execute; typically that of inner expression body builder.</param>
        /// <param name="a1">The first argument of the expression building action.</param>
        /// <param name="a2">The second argument of the expression building action.</param>
        /// <returns>The next fluent builder to build expressions from.</returns>
        protected TBuilder FluentAction<TBuilder, A1, A2>(TBuilder next, Action<A1, A2> toExecute, A1 a1, A2 a2)
            where TBuilder : BaseFluentExpressBuilder<TDelegate>
        {
            toExecute(a1, a2);
            return next;
        }

        /// <summary>
        /// Performs an expression building action and passes a fluent builder; whether itself or a new one to change available methods.
        /// </summary>
        /// <typeparam name="TBuilder">The exact type of the next fluent builder to pass.</typeparam>
        /// <param name="next">The fluent builder itself ("this") or a new fluent builder with different methods.</param>
        /// <param name="toExecute">The expression building action to execute; typically that of inner expression body builder.</param>
        /// <param name="a1">The first argument of the expression building action.</param>
        /// <param name="a2">The second argument of the expression building action.</param>
        /// <param name="a3">The third argument of the expression building action.</param>
        /// <returns>The next fluent builder to build expressions from.</returns>
        protected TBuilder FluentAction<TBuilder, A1, A2, A3>(TBuilder next, Action<A1, A2, A3> toExecute, A1 a1, A2 a2, A3 a3)
            where TBuilder : BaseFluentExpressBuilder<TDelegate>
        {
            toExecute(a1, a2, a3);
            return next;
        }

        // I might spawn more overloads if BodyBuilder methods grow too strong
    }
}
