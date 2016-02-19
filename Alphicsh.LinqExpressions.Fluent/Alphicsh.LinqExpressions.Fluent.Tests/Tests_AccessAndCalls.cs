using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Alphicsh.LinqExpressions.Fluent.Tests
{
    /// <summary>
    /// A collection of tests checking whether Fluent Express API builds proper member access and parameterized calls expressions.
    /// Parameterized calls cover methods, constructors and indexers execution.
    /// </summary>
    [TestClass]
    public class Tests_AccessAndCalls
    {
        private class Stuff
        {
            public Stuff() { }
            public Stuff(string str)
            {
                SomeString = str;
            }

            public string SomeString { get; set; } = "default";
            public int SomeInt;
            
            public int SomeMethod() => 42;
            public int SomeMethod(int toPass) => toPass;
            public int SomeMethod(int a, int b) => a + b;

            public int this[int val] => val;
            public int this[string str] => str.Length;
            public int this[int val, string str] => val + str.Length;
        }

        [TestMethod]
        public void MemberAccess()
        {
            var stuff = new Stuff() { SomeString = "name", SomeInt = 17 };

            Assert.AreEqual("name", Flex.StartExpression<string>().Const(stuff).Property("SomeString").sc().CompleteDelegate()());
            Assert.AreEqual(17, Flex.StartExpression<int>().Const(stuff).Field("SomeInt").sc().CompleteDelegate()());
        }

        [TestMethod]
        public void MethodCalls()
        {
            var stuff = new Stuff();

            Assert.AreEqual(42, Flex.StartExpression<int>().Const(stuff).CallNoArgs("SomeMethod").sc().CompleteDelegate()());
            Assert.AreEqual(17, Flex.StartExpression<int>().Const(stuff).Call("SomeMethod", typeof(int)).Argument().Const(17).EndArgs().sc().CompleteDelegate()());
            Assert.AreEqual(59, Flex.StartExpression<int>().Const(stuff).Call("SomeMethod", typeof(int), typeof(int)).Argument().Const(17).Argument().Const(42).EndArgs().sc().CompleteDelegate()());
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void InterfaceMethodCalls()
        {
            var sequence = new List<int>();
            sequence.Add(17);

            Flex.StartLambda<Action<IList<int>>>("sequence")
                .Var("sequence").CallNoArgs("Clear").sc()
                .Var("sequence").Call("Add", typeof(int)).Argument().Const(42).EndArgs().sc()
                .CompleteDelegate()(sequence);

            Assert.IsTrue(sequence[0] == 42);
        }

        [TestMethod]
        public void Constructors()
        {
            Assert.AreEqual("default", Flex.StartExpression<string>().New<Stuff>().Property("SomeString").sc().CompleteDelegate()());
            Assert.AreEqual("name", Flex.StartExpression<string>().New<Stuff>(typeof(string)).Argument().Const("name").EndArgs().Property("SomeString").sc().CompleteDelegate()());
        }

        [TestMethod]
        public void Indexers()
        {
            var stuff = new Stuff();
            Assert.AreEqual(42, Flex.StartExpression<int>().Const(stuff).IndexBy(typeof(int)).Argument().Const(42).EndArgs().sc().CompleteDelegate()());
            Assert.AreEqual(4, Flex.StartExpression<int>().Const(stuff).IndexBy(typeof(string)).Argument().Const("name").EndArgs().sc().CompleteDelegate()());
            Assert.AreEqual(46, Flex.StartExpression<int>().Const(stuff).IndexBy(typeof(int), typeof(string)).Argument().Const(42).Argument().Const("name").EndArgs().sc().CompleteDelegate()());
            FlexAssert.Throws<ArgumentException>(() => Flex.StartExpression<int>().Const(stuff).IndexBy(typeof(string), typeof(int)));
        }
    }
}
