using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Alphicsh.LinqExpressions.Fluent.Tests
{
    /// <summary>
    /// A collection of tests checking whether Fluent Express API builds proper expressions for miscellaneous operations.
    /// </summary>
    [TestClass]
    public class Tests_OtherOperations
    {
        [TestMethod]
        public void TernaryConditionalOperation()
        {
            var a = 42;
            var b = 17;

            Assert.AreEqual(a, Flex.StartExpression<int>().Const(true).InlineIf().Const(a).InlineElse().Const(b).sc().CompleteDelegate()());
            Assert.AreEqual(b, Flex.StartExpression<int>().Const(false).InlineIf().Const(a).InlineElse().Const(b).sc().CompleteDelegate()());

            //associativity
            Assert.AreEqual(true ? true : false ? false : true, Flex.StartExpression<bool>().Const(true).InlineIf().Const(true).InlineElse().Const(false).InlineIf().Const(false).InlineElse().Const(true).sc().CompleteDelegate()());
            Assert.AreNotEqual((true ? true : false) ? false : true, Flex.StartExpression<bool>().Const(true).InlineIf().Const(true).InlineElse().Const(false).InlineIf().Const(false).InlineElse().Const(true).sc().CompleteDelegate()());
            Assert.AreEqual(true ? true : (false ? false : true), Flex.StartExpression<bool>().Const(true).InlineIf().Const(true).InlineElse().Const(false).InlineIf().Const(false).InlineElse().Const(true).sc().CompleteDelegate()());
        }
    }
}
