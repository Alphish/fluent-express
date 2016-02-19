using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Alphicsh.LinqExpressions.Fluent.Tests
{
    /// <summary>
    /// A collection of tests checking whether Fluent Express API builds proper binary and unary expressions.
    /// </summary>
    [TestClass]
    public class Tests_BasicBinaryAndUnaryOperations
    {
        [TestMethod]
        public void UnaryCalculations()
        {
            int a = 42;

            Assert.AreEqual(+a, Flex.StartExpression<int>().Plus().Const(42).sc().CompleteDelegate()());
            Assert.AreEqual(-a, Flex.StartExpression<int>().Minus().Const(42).sc().CompleteDelegate()());
            Assert.AreEqual(~a, Flex.StartExpression<int>().BitwiseNot().Const(42).sc().CompleteDelegate()());

            Assert.AreEqual(false, Flex.StartExpression<bool>().Not().Const(true).sc().CompleteDelegate()());
            Assert.AreEqual(true, Flex.StartExpression<bool>().Not().Const(false).sc().CompleteDelegate()());
        }

        [TestMethod]
        public void IncrementDecrement()
        {
            var sequence = new List<int>();

            Flex.StartLambda<Action<List<int>>>("sequence")
                .DeclareAndAssign<int>("x", 0)                                                                  // int x = 0
                .Var("sequence").Call("Add", typeof(int)).Argument().Var("x").EndArgs().sc()                    // add x
                .Var("sequence").Call("Add", typeof(int)).Argument().Var("x").PostIncrement().EndArgs().sc()    // add x++
                .Var("sequence").Call("Add", typeof(int)).Argument().PreIncrement().Var("x").EndArgs().sc()     // add ++x
                .Var("sequence").Call("Add", typeof(int)).Argument().Var("x").PostDecrement().EndArgs().sc()    // add x--
                .Var("sequence").Call("Add", typeof(int)).Argument().PreDecrement().Var("x").EndArgs().sc()     // add --x
                .CompleteDelegate()(sequence);

            Assert.IsTrue(sequence.SequenceEqual(new int[] { 0, 0, 2, 2, 0}));
        }

        [TestMethod]
        public void TypeCheckAndConversion()
        {
            Assert.AreEqual(true, Flex.StartExpression<bool>().Const("str").Is(typeof(string)).sc().CompleteDelegate()());
            Assert.AreEqual(false, Flex.StartExpression<bool>().Const("str").Is(typeof(int?)).sc().CompleteDelegate()());

            Assert.AreEqual("str", Flex.StartExpression<string>().Const("str").As(typeof(string)).sc().CompleteDelegate()());
            Assert.AreEqual(null, Flex.StartExpression<int?>().Const("str").As(typeof(int?)).sc().CompleteDelegate()());

            Assert.AreEqual("str", Flex.StartExpression<string>().Convert(typeof(string)).Const("str").sc().CompleteDelegate()());
            FlexAssert.Throws<InvalidCastException, IEnumerable<string>>(Flex.StartExpression<IEnumerable<string>>().Convert(typeof(IEnumerable<string>)).Const("str").sc().CompleteDelegate());

            Assert.AreEqual(-1, Flex.StartExpression<int?>().Convert(typeof(int?)).Const(uint.MaxValue).sc().CompleteDelegate()());
            Assert.AreEqual(null, Flex.StartExpression<int?>().Const(uint.MaxValue).As(typeof(int?)).sc().CompleteDelegate()());
        }

        [TestMethod]
        public void ArithmeticOperations()
        {
            int a = 42;
            int b = 17;

            Assert.AreEqual(a * b, Flex.StartExpression<int>().Const(a).Multiply().Const(b).sc().CompleteDelegate()());
            Assert.AreEqual(a / b, Flex.StartExpression<int>().Const(a).Divide().Const(b).sc().CompleteDelegate()());
            Assert.AreEqual(a % b, Flex.StartExpression<int>().Const(a).Modulo().Const(b).sc().CompleteDelegate()());
            Assert.AreEqual(a + b, Flex.StartExpression<int>().Const(a).Add().Const(b).sc().CompleteDelegate()());
            Assert.AreEqual(a - b, Flex.StartExpression<int>().Const(a).Subtract().Const(b).sc().CompleteDelegate()());
        }

        [TestMethod]
        public void BitOperations()
        {
            int a = 42;
            int b = 17;
            int shift = 3;

            Assert.AreEqual(a << shift, Flex.StartExpression<int>().Const(a).ShiftLeft().Const(shift).sc().CompleteDelegate()());
            Assert.AreEqual(a >> shift, Flex.StartExpression<int>().Const(a).ShiftRight().Const(shift).sc().CompleteDelegate()());
            Assert.AreEqual(a & b, Flex.StartExpression<int>().Const(a).BitwiseAnd().Const(b).sc().CompleteDelegate()());
            Assert.AreEqual(a ^ b, Flex.StartExpression<int>().Const(a).BitwiseXor().Const(b).sc().CompleteDelegate()());
            Assert.AreEqual(a | b, Flex.StartExpression<int>().Const(a).BitwiseOr().Const(b).sc().CompleteDelegate()());
        }

        #pragma warning disable CS1718 // Comparison made to same variable

        [TestMethod]
        public void ComparisonOperations()
        {
            int a = 42;
            int b = 17;

            Assert.AreEqual(a > b, Flex.StartExpression<bool>().Const(a).GreaterThan().Const(b).sc().CompleteDelegate()());
            Assert.AreEqual(b > a, Flex.StartExpression<bool>().Const(b).GreaterThan().Const(a).sc().CompleteDelegate()());
            Assert.AreEqual(a > a, Flex.StartExpression<bool>().Const(a).GreaterThan().Const(a).sc().CompleteDelegate()());

            Assert.AreEqual(a >= b, Flex.StartExpression<bool>().Const(a).GreaterThanOrEqual().Const(b).sc().CompleteDelegate()());
            Assert.AreEqual(b >= a, Flex.StartExpression<bool>().Const(b).GreaterThanOrEqual().Const(a).sc().CompleteDelegate()());
            Assert.AreEqual(a >= a, Flex.StartExpression<bool>().Const(a).GreaterThanOrEqual().Const(a).sc().CompleteDelegate()());

            Assert.AreEqual(a < b, Flex.StartExpression<bool>().Const(a).LessThan().Const(b).sc().CompleteDelegate()());
            Assert.AreEqual(b < a, Flex.StartExpression<bool>().Const(b).LessThan().Const(a).sc().CompleteDelegate()());
            Assert.AreEqual(a < a, Flex.StartExpression<bool>().Const(a).LessThan().Const(a).sc().CompleteDelegate()());

            Assert.AreEqual(a <= b, Flex.StartExpression<bool>().Const(a).LessThanOrEqual().Const(b).sc().CompleteDelegate()());
            Assert.AreEqual(b <= a, Flex.StartExpression<bool>().Const(b).LessThanOrEqual().Const(a).sc().CompleteDelegate()());
            Assert.AreEqual(a <= a, Flex.StartExpression<bool>().Const(a).LessThanOrEqual().Const(a).sc().CompleteDelegate()());

            Assert.AreEqual(a == b, Flex.StartExpression<bool>().Const(a).Equal().Const(b).sc().CompleteDelegate()());
            Assert.AreEqual(b == a, Flex.StartExpression<bool>().Const(b).Equal().Const(a).sc().CompleteDelegate()());
            Assert.AreEqual(a == a, Flex.StartExpression<bool>().Const(a).Equal().Const(a).sc().CompleteDelegate()());

            Assert.AreEqual(a != b, Flex.StartExpression<bool>().Const(a).NotEqual().Const(b).sc().CompleteDelegate()());
            Assert.AreEqual(b != a, Flex.StartExpression<bool>().Const(b).NotEqual().Const(a).sc().CompleteDelegate()());
            Assert.AreEqual(a != a, Flex.StartExpression<bool>().Const(a).NotEqual().Const(a).sc().CompleteDelegate()());
        }

        #pragma warning restore CS1718 // Comparison made to same variable

        [TestMethod]
        public void BooleanOperations()
        {
            Assert.AreEqual(true, Flex.StartExpression<bool>().Const(true).And().Const(true).sc().CompleteDelegate()());
            Assert.AreEqual(false, Flex.StartExpression<bool>().Const(true).And().Const(false).sc().CompleteDelegate()());
            Assert.AreEqual(false, Flex.StartExpression<bool>().Const(false).And().Const(true).sc().CompleteDelegate()());
            Assert.AreEqual(false, Flex.StartExpression<bool>().Const(false).And().Const(false).sc().CompleteDelegate()());

            Assert.AreEqual(true, Flex.StartExpression<bool>().Const(true).Or().Const(true).sc().CompleteDelegate()());
            Assert.AreEqual(true, Flex.StartExpression<bool>().Const(true).Or().Const(false).sc().CompleteDelegate()());
            Assert.AreEqual(true, Flex.StartExpression<bool>().Const(false).Or().Const(true).sc().CompleteDelegate()());
            Assert.AreEqual(false, Flex.StartExpression<bool>().Const(false).And().Const(false).sc().CompleteDelegate()());
        }

    }
}
