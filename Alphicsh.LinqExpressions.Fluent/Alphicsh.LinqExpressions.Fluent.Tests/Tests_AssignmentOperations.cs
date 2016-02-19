using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Alphicsh.LinqExpressions.Fluent.Tests
{
    /// <summary>
    /// A collection of tests checking whether Fluent Express API builds proper assignment expressions.
    /// </summary>
    [TestClass]
    public class Tests_AssignmentOperations
    {
        [TestMethod]
        public void NPlusOneAssignments()
        {
            var sequence = new List<int>();

            Flex.StartLambda<Action<List<int>>>("sequence")
                .Declare<int>("x")                                                                                          // int x;
                .Var("sequence").Call("Add", typeof(int)).Argument().Var("x").Assign().Const(1).EndArgs().sc()              // add x = 1
                .Var("sequence").Call("Add", typeof(int)).Argument().Var("x").AddAssign().Const(1).EndArgs().sc()           // add x += 1
                .Var("sequence").Call("Add", typeof(int)).Argument().Var("x").SubtractAssign().Const(1).EndArgs().sc()      // add x -= 1
                .Var("sequence").Call("Add", typeof(int)).Argument().Var("x").MultiplyAssign().Const(8).EndArgs().sc()      // add x *= 8
                .Var("sequence").Call("Add", typeof(int)).Argument().Var("x").DivideAssign().Const(2).EndArgs().sc()        // add x /= 2
                .Var("sequence").Call("Add", typeof(int)).Argument().Var("x").ModuloAssign().Const(3).EndArgs().sc()        // add x %= 3
                .Var("sequence").Call("Add", typeof(int)).Argument().Var("x").ShiftLeftAssign().Const(5).EndArgs().sc()     // add x <<= 5
                .Var("sequence").Call("Add", typeof(int)).Argument().Var("x").ShiftRightAssign().Const(3).EndArgs().sc()    // add x >> 3
                .Var("sequence").Call("Add", typeof(int)).Argument().Var("x").BitwiseXorAssign().Const(42).EndArgs().sc()   // add x ^= 42
                .Var("sequence").Call("Add", typeof(int)).Argument().Var("x").BitwiseAndAssign().Const(26).EndArgs().sc()   // add x &= 26
                .Var("sequence").Call("Add", typeof(int)).Argument().Var("x").BitwiseOrAssign().Const(3).EndArgs().sc()     // add x |= 3
                .Var("sequence").Call("Add", typeof(int)).Argument().Var("x").BitwiseXorAssign().Const(19).EndArgs().sc()   // add x ^= 19; that's the "plus one"
                .CompleteDelegate()(sequence);

            Assert.IsTrue(sequence.SequenceEqual(new int[] { 1, 2, 1, 8, 4, 1, 32, 4, 46, 10, 11, 24 }));
        }

        [TestMethod]
        public void AssignmentsAssociativity()
        {
            var sequence = new List<int>();

            Flex.StartLambda<Action<List<int>>>("sequence")
                .DeclareAndAssign<int>("x", 0)
                .DeclareAndAssign<int>("y", 1)
                .Var("sequence").Call("Add", typeof(int)).Argument().Var("x").EndArgs().sc()
                .Var("sequence").Call("Add", typeof(int)).Argument().Var("y").EndArgs().sc()
                .Var("x").Assign().Var("y").Assign().Const(2).sc()
                .Var("sequence").Call("Add", typeof(int)).Argument().Var("x").EndArgs().sc()
                .Var("sequence").Call("Add", typeof(int)).Argument().Var("y").EndArgs().sc()
                .CompleteDelegate()(sequence);

            Assert.IsTrue(sequence.SequenceEqual(new int[] { 0, 1, 2, 2 }));
            FlexAssert.Throws<ArgumentException>(
                () => Flex.StartExpression<int>()
                    .DeclareAndAssign<int>("x", 0).DeclareAndAssign<int>("y", 1)
                    .Brace().Var("x").Assign().Var("y").Unbrace().Assign().Const(2).sc()
                );
        }
    }
}
