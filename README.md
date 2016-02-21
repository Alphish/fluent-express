Fluent Express
==============

The Fluent Express library aims to turn powerful but unwieldy [.NET Expression Trees API](https://msdn.microsoft.com/en-us/library/bb397951.aspx#Anchor_1) into as powerful yet much clearer fluent interface.

**Table of contents**

 - [How is that useful?](#how-is-that-useful)
 - [Features](#features)
 - [Future plans](#future-plans)
 - [How to get it?](#how-to-get-it)

**NOTE: So far only 0.1.0 alpha version is available, and lacks many features available in basic Expression Trees API. These features (and a few more) are going to be progressively added as Fluent Express is developed.**

If it doesn't scare you, check the [How to get it?](#how-to-get-it) section. Or just run this in NuGet Package Manager console:
```
PM> Install-Package Alphicsh.LinqExpressions.Fluent -Pre
```

How is that useful?
===================

Similarly to basic Expression Trees API, Fluent Express allows creating expression trees on runtime. These in turn can be used, for example, to create a transpiler, a scripting engine, or maybe to make flexible database queries; and I'm only scratching a surface here. Whenever you need an executable code generated dynamically, you can hardly go wrong with expression trees.

Why not use what's already there, though? After all, Expression Trees API already has sufficient tools to build the trees, and it already comes with the .NET package. Plus, Fluent Express depends on that API, anyway. Is it really that much easier to use? Is it really much more readable?

Let's see an example from Expression Trees API page, corresponding to a little simple lambda `num => num < 5`.
```csharp
ParameterExpression numParam = Expression.Parameter(typeof(int), "num");
ConstantExpression five = Expression.Constant(5, typeof(int));
BinaryExpression numLessThanFive = Expression.LessThan(numParam, five);
Expression<Func<int, bool>> lambda1 =
    Expression.Lambda<Func<int, bool>>(
        numLessThanFive,
        new ParameterExpression[] { numParam });
```
How does Fluent Express compare?
```csharp
Expression<Func<int, bool>> lambda1 =
    Flex.StartLambda<Func<int, bool>>("num")
        .Var("num").LessThan().Const(5).sc()
        .CompleteLambda();
```

**No variables clutter**

In MSDN example an additional variable has been declared for each tree node needed. Two of these could be avoided by replacing `numLessThanFive` with `Expression.LessThan(numParam, Expression.Constant(5, typeof(int)))`, but parameters (or other nodes that appear more than once) still need to be declared beforehand. Plus, building larger expression trees that way quickly becomes a nightmare for anything remotely complex (good luck keeping track of parentheses).

In contrast, trees built with Fluent Express can be contained in a single chain of methods call (naturally, for more complex trees the chain becomes quite long). No need to come up with genius names like `ifMoreThanThreeReturnXOtherwiseReturnOne`. In fact, no need to come up with any names in the first place (aside from parameters/variables inside the expression itself)!
    
**Familiar expression writing order**
    
When using default Expression Trees API, you are pretty much forced to use something akin to [postfix notation](https://en.wikipedia.org/wiki/Reverse_Polish_notation), [prefix notation](https://en.wikipedia.org/wiki/Polish_notation) or a combination of both. To elaborate, when you declare a variable for each node, you're pretty much using postfix notation style: first operands must be declared, and only then you can build operations on them. When you use a nested call instead, like `Expression.Add(Expression.Constant(2), Expression.Constant(2))`, it looks very similar to prefix notation, where operations are declared before operands.

In comparison, Fluent Express syntax reads like classic [infix notation](https://en.wikipedia.org/wiki/Infix_notation), where operators are places *between* the operands. Basically, structure of Fluent Express API is very similar to expressions written in code. Most of popular programming languages use that notation rather than its prefix or postfix counterpart, and for a good reason - that's how people naturally read or write expressions, ever since they're introduced to maths. To make the experience even more similar to writing expressions directly in code, the operator precedence is kept as well.

You might think all that is mostly a syntactic sugar, and I won't deny it; but it's pretty sweet nonetheless.

Features
========

As of version **0.1.0 alpha**, the following features are included:
 - [initiating, building and completing expression trees](#building-expressions)
 - [adding values to operate on](#values-and-variables)
 - [local variables declaration and use](#values-and-variables)
 - [primary operations](#primary-operations)
 - [basic operators](#basic-operators)
 - [ordering by precedence and bracketing](#basic-operators)
 - [context-based method suggestions](#context-based-method-suggestions)

Keep reading to learn more about them.
 
Building expressions
--------------------

You can initiate expressions building with one of three methods:
 - `Flex.StartAction()` - begins a parameterless expression with no expected return type
 - `Flex.StartExpression<TReturn>()` - begins a parameterless expression that evaluates to a given type
 - `Flex.StartLambda<TDelegate>(parameterNames)` - begins a lambda expression for the given delegate type, with specific parameter names corresponding to delegate's arguments; these names can be used later to refer to specific arguments
 
Once you start expression building, you can use a variety of methods available in API to build the expression tree you need.

As of the current version, the expression body will take the form of one or more statements (if more than one statement is present, the statements are wrapped in a block). Most statements are finished with `EndStatement()` method, or its shorter equivalent `sc()` (which stands for "semicolon"). Some methods already include an entire statement, and thus don't need to be terminated.

After all statements needed are needed, you can wrap the expression with one of the completion methods:
 - `Complete()` - returns only the expression body built, either as a single statement or a block of statements built
 - `CompleteLambda()` - returns a lambda expression with earlier defined parameters (if any) and the body built; the type of delegate depends on starting method used
 - `CompleteDelegate()` - returns a compiled executable delegate; remember that it cannot be examined as an expression tree once compiled
 
Values and variables
--------------------

Each operation requires values to operate on. Fixed values can be added with methods like `Null()`, `Default(type)`, `Const(value)` or their overloads. Aside from that, it's possible to include variable or parameter with method `Var(name)`. You can also add an entire subexpression by using `Subexpression(exp)` method. You can also create a new object with `New()` method, but that'll be explained in more detail later.

Variables themselves can be passed as parameters, or declared inside the expression itself; for both of these cases, `Var(name)` should be used. To define a variable inside the expression, you can use:
 - `Declare(type, name)` to declare a variable in a given block, but without any initialization yet; it counts as a complete statement
 - `DeclareAndAssign(type, name)` to declare a variable and start an expression to be assigned; said expression needs to be built in subsequent Fluent Express calls
 - `DeclareAndAssign(type, name, value)` to declare a variable and initialize it with a constant value; such initialization counts as a complete statement

Primary operations
------------------

**Member access**

To access a field of the current value, use `Field(name)` method. To access a property of the current value, use `Property(name)` method.

As of 0.1.0 alpha, no static properties or fields are supported, neither is method delegates access (such as `someInstance.SomeMethod` without method call; while visually similar to property/field access, it's a bit trickier). These changes are planned for future versions.

**Constructors**

You can add a new object of a given type with `New(newType, parameterTypes)`. If no parameter types are provided, a parameterless constructor is used and you can continue the expression like with any other value (constant, variable, whatever). Otherwise, you need to provide a list of arguments, described in more detail at the end of *Primary operations* section.

**Method calls**

You can call a method on the current instance with `Call(name, parameterTypes)`, where the name of the method and types of expected parameters must be provided. That method must be followed by a list of arguments, described in more detail at the end of *Primary operations* section. If you want use parameterless method and skip arguments list altogether, use `CallNoArgs(name)` instead.

These functions work for "inherited" interface methods as well. So when you e.g. operate on an object of `IList<int>` type and want to obtain its `Add(item)` method, Fluent Express is smart enough to provide that method declared on `ICollection<int>` that `IList<int>` implements without defining `Add(item)` method of its own.

As of 0.1.0 alpha, no static methods or other ways to identify methods are supported. These changes are planned for future versions.

**Indexers**

You can use indexers of the current instance with `IndexBy(indexTypes)` method, where the specific index property is identified by index types provided.  That method must be followed by a list of arguments, described in more detail at the end of *Primary operations* section. There's no method for parameterless indices, because the idea of parameterless indices is too silly even for me.

**Argument lists**

Earlier mentioned parameterized operations use argument lists. Example of how these might look like:
```csharp
Flex.StartAction().
    ...
    .Call("SomeMethod", type1, type2, ...)
        .Argument()...  // expression built for the first argument
        .Argument()...  // expression built for the second argument
        ...
        .EndArgs()
    ...                 // whatever follows the method call
    .Complete();
```
Generally, there are two methods essential for building arguments lists:
 - `Argument()` indicates that the following expression built with Fluent Express API should be used as argument for a parameterized action; that expression is terminated by another `Argument()` call, or `EndArgs()` call
 - `EndArgs()` indicated that the list of arguments has been completed, and the expression for the entire parameterized action is ready to be built

Of course, all arguments built in such a way must be appropriate for whatever action you're about to make. Otherwise, weird things may happen, maybe exceptional, even.

Basic operators
---------------

Expression Trees API supports a wide range of operations, and so does Fluent Express. Usage details depend on specific type of operators. All operators available are listed at the end of *Basic operators* section.

**Unary operators**

They require only one expression to operate on and are typically placed before that expression is even started. So if you want e.g. to perform boolean negation on some value, you must call respective method *before* you build the value.
```csharp
Flex...Not().Var("someBool")...     // correct
Flex...Var("someBool").Not()...     // incorrect
```
The exceptions to that rule are methods: `PostIncrement()`, `PostDecrement()`, `Is(type)` and `As(type)`.

**Binary operators**

They require two expressions to operate on and are placed between the operand expressions. It's supposed to reflect the way expressions are typed in the code.
```csharp
Flex...Add().Var("a").Var("b")...   // incorrect
Flex...Var("a").Add().Var("b")...   // correct
Flex...Var("a").Var("b").Add()...   // incorrect
```

Most binary operators are left-associative, i.e. `a op b op c` evaluates as `(a op b) op c` rather than `a op (b op c)`. Assignment operators are exception; so `a = b = c` is equivalent to `a = (b = c)`.

**Ternary operator**

There is only conditional ternary operator, i.e. '?:' one, also known as inline if. Its structure is very similar to the code counterpart. `c ? t : f` would be represented as:
```csharp
Flex...Var("c").InlineIf().Var("t").InlineElse().Var("f")...
```
Naturally, `InlineIf()` corresponds to the questionmark half of the operator, followed by the value for meeting the condition, and `InlineElse()` corresponds to the colon half of the operator, followed by the value for failing the condition. Similarly to C# implementation, `InlineIf()` and `InlineElse()` are right-associative, so `a ? b : c ? d : e` evaluates as `a ? b : (c ? d : e)`, not `(a ? b : c) ? d : e`.

**Evaluation ordering**

In C#, the operations evaluation order depends on the precedence; the higher the precedence, the sooner a given expression will be evaluated. Thus, with multiplication having higher precedence than addition, `2 + 2 * 2` is equal to 6 (`2 + (2 * 2)`) rather than 8 (`(2 + 2) * 2`), as one might expect by performing operations in the order they appear. Similarly, `Flex...Var("a").Add().Var("b").Multiply().Var("c")...` will produce an expression tree equivalent to `a + (b * c)` rather than `(a + b) * c`.

While it makes Fluent Express API more similar to typical C# behaviour, sometimes it's desired to use lower-precedence operation result in the higher-precedence one, rather than other way round. That's why C# has brackets, and that's why Fluent Express has `Brace()` and `Unbrace()` methods. Thus, if you actually want `(a + b) * c` somewhere in your expression, you can use the following code:
```csharp
Flex...Brace().Var("a").Add().Var("b").Unbrace().Multiply().Var("c")...
```
You might notice that's like writing the expression literally, symbol by symbol: *open bracket, variable a plus variable b, close bracket, times variable c*

(I hope [that poll](https://twitter.com/SciencePorn/status/663318938307141632) isn't representative of programmers' population; otherwise the precedence enforcement might feel more confusing rather than natural >.<)

**Operators reference**

The following operators are supported through Fluent Express methods, grouped by precedence (from higher to lower):
 - `x++` as `[x].PostIncrement()`, `x--` as `[x].PostDecrement()`
 - `+x` as `Plus().[x]`, `-x` as `Minus().[x]`, `!x` as `Not().[x]`, `~x` as `BitwiseNot().[x]`, `++x` as `PreIncrement().[x]`, `--x` as `PreDecrement().[x]`, `(type)x` as `Convert(type).[x]`
 - `x * y` as `[x].Multiply().[y]`, `x / y` as `[x].Divide().[y]`, `x % y` as `[x].Modulo().[y]`
 - `x + y` as `[x].Add().[y]`, `x - y` as `[x].Subtract().[y]`
 - `x << y` as `[x].ShiftLeft().[y]`, `x >> y` as `[x].ShiftRight().[y]`
 - `x < y` as `[x].LessThan().[y]`, `x > y` as `[x].GreaterThan().[y]`, `x <= y` as `[x].LessThanOrEqual.[y]`, `x >= y` as `[x].GreaterThanOrEqual().[y]`, `x is type` as `[x].Is(type)`, `x as type` as `[x].As(type)`
 - `x == y` as `[x].Equal().[y]`, `x != y` as `[x].NotEqual().[y]`
 - `x & y` as `[x].BitwiseAnd().[y]`
 - `x ^ y` as `[x].BitwiseXor().[y]`
 - `x | y` as `[x].BitwiseOr().[y]`
 - `x && y` as `[x].And().[y]`
 - `x || y` as `[x].Or().[y]`
 - `x ? y : z` as `[x].InlineIf().[y].InlineElse().[z]`
 - `x = y` as `[x].Assign().[y]`; other assignment operations like `x [op]= y` take the form of `[x].[operation]Assign.[y]`, i.e. `x -= y` is `[x].SubtractAssign().[y]`

As of 0.1.0 alpha, the following aren't yet supported:
 - checked calculations/assignments/conversions
 - null coalescence ('??' operator)
 - inline lambda expressions ('=>' operator), possibly with both constant and quoted variants (once I find out the exact difference between the two)
 
Support for these is planned for the future versions.

Context-based method suggestions
--------------------------------

To make Fluent Express API as powerful as possible, quite many methods are required, and not all of them are useful all the time. In fact, some of them are obviously inappropriate in certain situations; for example, if you just used an addition operator, you aren't going to follow with a multiplication one (it would make no more sense than `x +* y`). That's why with a bit of programming trickery the API has been constructed in a way that methods clearly wrong for the given situation aren't available; simple as that. With such mechanism at least part of the Intellisense clutter can be avoided, and a good share of simple errors (such as missing semicolons) can be easily caught and fixed before compilation.

Future plans
============

If version identifier *0.1.0-alpha* and various hints spread across this document aren't enough of an indication - right now Fluent Express is in very incomplete state, and there is lot of work to do before it goes from pre-release to release state.

At the moment my plans include the following features:
 - in general: replication of most (if not all) Expression Trees API functionality; in other words, making a counterpart for every static method exposed by `System.Linq.Expressions.Expression` class, overloads included
 - static members and methods support
 - methods delegate member access
 - lambda operator
 - null-coalescence operator
 - overflow checking for some operators
 - block statements support (if, while, etc.)
 - code-jumping statements (return, break, etc.)
 
Optionally, if I figure out how to implement it:
 - null-conditional operators

How to get it?
==============

Technically, you can just get the solution file from that repository and compile it yourself. But you're probably one of those N+1% people who don't feel like going through all that hassle of obtaining a source, compiling it and then checking if *maybe* that random library from Github is any useful. For people like that (myself included), I prepared a NuGet package instead.

To install the current version (0.1.0 alpha) via Package Manager Console, launch the following command:
```
PM> Install-Package Alphicsh.LinqExpressions.Fluent -Pre
```

If instead you prefer to use window-based NuGet package manager, search for *Fluent Express* or *Alphicsh.LinqExpressions.Fluent* on nuget.org feed. Don't forget to check *Include prerelease* option!

Feedback is very welcome, especially during such an early phase; the more advanced the development is, the harder it becomes to change some design decisions. Refer to the **Future plans** section to make sure you're not suggesting something that's already acknowledged and planned.

Contributions are welcome as well, though it's recommended to ask whether some kind of contribution is desired beforehand, especially if it'd involve some non-negligible of effort. I wouldn't want to have anyone's time wasted because their contributions didn't quite line up with planned Fluent Express design.

If you have some suggestion regarding the API design and functionality, feel free to share it. If you want to suggest a new method in the API, keep in mind that overloads are generally preferred to newly named methods (especially if the new method aims to provide easier access to existing functionality and doesn't introduce something completely new).