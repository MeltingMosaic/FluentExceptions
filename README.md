#FluentExceptions

FluentExceptions allows you to write code that handles exceptions in a fluent manner, rather than using try/catch blocks. 

###Simple Example:

This snippet will eat an ArithmeticException

```c#
var exceptionThrower = new ExceptionThrower();

exceptionThrower.Invoking(e => e.MethodThatThrowsArithmeticException())
    .MayThrow<ArithmeticException>()
    .WhenExecuted();
```

You can also handle the exception:

```c#
var exceptionThrower = new ExceptionThrower();

exceptionThrower.Invoking(e => e.MethodThatThrowsArithmeticException())
    .MayThrow<ArithmeticException>()
    .HandledBy(e => Console.WriteLine(e.ToString()))
    .WhenExecuted();
```

Or you can catch multiple exceptions with one handler

```c#
var exceptionThrower = new ExceptionThrower();

exceptionThrower.Invoking(e => e.MethodThatThrowsSomeStuff())
    .MayThrow<ArithmeticException>()
    .Or<TimeoutException>()
    .Or<ArgumentNullException>()
    .HandledBy(e => Console.WriteLine(e.ToString()))
    .WhenExecuted();
```

Or define separate handling for different groups of exceptions:

```c#
var exceptionThrower = new ExceptionThrower();

exceptionThrower
    .Invoking(e => e.MethodThatThrowsSomeStuff())
        .MayThrow<ArithmeticException>()
        .Or<TimeoutException>()
        .HandledBy(e => Console.WriteLine(e.ToString()))
    .OrIt()
        .MayThrow<ArgumentNullException>()
        .HandledBy(e.=> Debug.WriteLine(e.ToString()))
    .WhenExecuted();
```

What about async/await?

Change `.Invoking()` to `.Awaiting()` and the handler now returns a `Task` (or `Task<T>`)

```c#
await exceptionThrower.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
    .MayThrow<ArithmeticException>()
    .HandledBy(e => Console.WriteLine(e))
    .WhenExecuted();
```
