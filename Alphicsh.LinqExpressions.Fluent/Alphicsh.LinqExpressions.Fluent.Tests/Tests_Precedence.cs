using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Alphicsh.LinqExpressions.Fluent.Tests
{
    /// <summary>
    /// A collection of tests checking whether Fluent Express API maintains operators precedence properly.
    /// </summary>
    [TestClass]
    public class Tests_Precedence
    {
        [TestMethod]
        public void ArithmeticPrecedence()
        {
            Assert.AreEqual(335, 
                Flex.StartExpression<int>().Const(72).BitwiseOr().Const(399).BitwiseXor().Const(131).BitwiseAnd().Const(5).ShiftLeft().Const(1).Add().Const(2).Multiply().Const(3).sc().CompleteDelegate()()
                );
            Assert.AreEqual(149,
                Flex.StartExpression<int>().Const(2).Multiply().Const(3).Add().Const(1).ShiftLeft().Const(1).BitwiseAnd().Const(7).BitwiseXor().Const(18).BitwiseOr().Const(133).sc().CompleteDelegate()()
                );
        }

        [TestMethod]
        public void RelationalAndEqualityPrecedence()
        {
            Assert.AreEqual(true, Flex.StartExpression<bool>().Const(42).GreaterThan().Const(21).Equal().Const(21).LessThanOrEqual().Const(33).sc().CompleteDelegate()());
            FlexAssert.Throws<InvalidOperationException>(() => Flex.StartExpression<bool>().Const(42).GreaterThan().Brace().Const(21).Equal().Const(21).Unbrace().LessThanOrEqual().Const(33).sc().CompleteDelegate()());
        }
    }
}
