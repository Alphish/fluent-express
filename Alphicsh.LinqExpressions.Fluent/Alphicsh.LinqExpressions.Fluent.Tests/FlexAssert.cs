using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Alphicsh.LinqExpressions.Fluent.Tests
{
    /// <summary>
    /// A helper class for custom assertions.
    /// </summary>
    public static class FlexAssert
    {
        /// <summary>
        /// Checks whether a given action throws a specified exception during execution.
        /// </summary>
        /// <typeparam name="TException">The exception the action should throw.</typeparam>
        /// <param name="action">The action to execute.</param>
        public static void Throws<TException>(Action action)
            where TException : Exception
        {
            try
            {
                action();
                Assert.Fail($"No exception of type {typeof(TException).Name} has been thrown.");
            }
            catch (AssertFailedException fail)
            {
                throw fail;
            }
            catch (TException)
            {
                // all good, do nothing
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception of type {ex.GetType().Name} has been thrown instead of {typeof(TException).Name}.");
            }
        }

        /// <summary>
        /// Checks whether a given expression delegate throws a specified exception during evaluation.
        /// </summary>
        /// <typeparam name="TException">The exception the function should throw.</typeparam>
        /// <typeparam name="TResult">The type of the function result.</typeparam>
        /// <param name="expression">The expression delegate to evaluate.</param>
        public static void Throws<TException, TResult>(Func<TResult> expression)
            where TException : Exception
        {
            try
            {
                expression();
                Assert.Fail($"No exception of type {typeof(TException).Name} has been thrown.");
            }
            catch (AssertFailedException fail)
            {
                throw fail;
            }
            catch (TException)
            {
                // all good, do nothing
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception of type {ex.GetType().Name} has been thrown instead of {typeof(TException).Name}.");
            }
        }

    }
}
