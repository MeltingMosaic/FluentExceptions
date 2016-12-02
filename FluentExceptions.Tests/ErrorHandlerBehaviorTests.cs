
namespace FluentExceptions.Tests
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ErrorHandlerBehaviorTests
    {
        [TestMethod]
        public void CreatingANewErrorHandlerReturnsHandler()
        {
            var handler = GetArithmeticExceptionHandler();

            handler.Should().NotBeNull();
        }

        [TestMethod]
        public void WithAnActionForExpectedExceptionThrownExpectNoException()
        {
            var handler = GetArithmeticExceptionHandler();
            var testObject = new TestObject();
            Action act = () => testObject.Invoke(t => t.VoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldNotThrow();
        }

        [TestMethod]
        public void WithAFuncForExpectedExceptionThrownExpectNoException()
        {
            var handler = GetArithmeticExceptionHandler();
            var testObject = new TestObject();
            Action act = () => testObject.Invoke(t => t.IntMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldNotThrow();
        }

        [TestMethod]
        public void WithATaskForExpectedExceptionExpectNoException()
        {
            var handler = GetArithmeticExceptionHandler();
            var testObject = new TestObject();
            Func<Task> act = () => testObject.GetAwaitable(t => t.AsyncVoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldNotThrow();
        }
        
        [TestMethod]
        public void WithATaskTForExpectedExceptionExpectNoException()
        {
            var handler = GetArithmeticExceptionHandler();
            var testObject = new TestObject();
            Func<Task> act = () => testObject.GetAwaitable(t => t.AsyncIntMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldNotThrow();
        }

        [TestMethod]
        public void WithTwoTasksExpectProperHandling()
        {
            var handler = GetArithmeticExceptionHandler();
            var testObject = new TestObject();
            Func<Task> act = () => testObject.GetAwaitable(t => t.AsyncVoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldNotThrow();
            act.ShouldNotThrow();
        }
        
        [TestMethod]
        public void WithAnActionForUnexpectedExceptionExpectException()
        {
            var handler = GetArgumentExceptionHandler();
            var testObject = new TestObject();
            Action act = () => testObject.Invoke(t => t.VoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldThrow<ArithmeticException>();
        }

        [TestMethod]
        public void WithATaskForUnexpectedExceptionExpectException()
        {
            var handler = GetArgumentExceptionHandler();
            var testObject = new TestObject();
            Func<Task> act = () => testObject.GetAwaitable(t => t.AsyncVoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldThrow<ArithmeticException>();
        }

        [TestMethod]
        public void WithAnActionWithOrClauseExpectHandling()
        {
            bool handled = false;
            var handler = ErrorHandler.Define()
                .MayThrow<ArgumentException>()
                .Or<ArithmeticException>()
                .HandledBy(e => { handled = true; })
                .AsErrorHandler();

            var testObject = new TestObject();

            Action act = () => testObject.Invoke(t => t.VoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldNotThrow();
            handled.Should().BeTrue();
        }

        [TestMethod]
        public void WithAnActionWithOrClauseForFirstExceptionExpectHandling()
        {
            bool handled = false;
            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .Or<ArgumentException>()
                .HandledBy(e => { handled = true; })
                .AsErrorHandler();

            var testObject = new TestObject();

            Action act = () => testObject.Invoke(t => t.VoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldNotThrow();
            handled.Should().BeTrue();
        }

        [TestMethod]
        public void WithATaskWithOrClauseExpectHandling()
        {
            bool handled = false;
            var handler = ErrorHandler.Define()
                .MayThrow<ArgumentException>()
                .Or<ArithmeticException>()
                .HandledBy(e => { handled = true; })
                .AsErrorHandler();

            var testObject = new TestObject();

            Func<Task> act = () => testObject.GetAwaitable(t => t.AsyncVoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldNotThrow();
            handled.Should().BeTrue();
        }

        [TestMethod]
        public void WithATaskWithOrClauseForFirstExceptionExpectHandling()
        {
            bool handled = false;
            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .Or<ArgumentException>()
                .HandledBy(e => { handled = true; })
                .AsErrorHandler();

            var testObject = new TestObject();

            Func<Task> act = () => testObject.GetAwaitable(t => t.AsyncVoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldNotThrow();
            handled.Should().BeTrue();
        }

        [TestMethod]
        public void WithAnActionForExceptionInThirdOrClauseExpectHandling()
        {
            bool handled = false;
            var handler = ErrorHandler.Define()
                .MayThrow<ArgumentException>()
                .Or<IndexOutOfRangeException>()
                .Or<ArithmeticException>()
                .HandledBy(e => { handled = true; })
                .AsErrorHandler();

            var testObject = new TestObject();

            Action act = () => testObject.Invoke(t => t.VoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldNotThrow();
            handled.Should().BeTrue();
        }

        [TestMethod]
        public void WithATaskWithExceptionInThirdOrClauseExpectHandling()
        {
            bool handled = false;
            var handler = ErrorHandler.Define()
                .MayThrow<ArgumentException>()
                .Or<IndexOutOfRangeException>()
                .Or<ArithmeticException>()
                .HandledBy(e => { handled = true; })
                .AsErrorHandler();

            var testObject = new TestObject();

            Func<Task> act = () => testObject.GetAwaitable(t => t.AsyncVoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldNotThrow();
            handled.Should().BeTrue();
        }

        [TestMethod]
        public void ForAnActionWithAndFinallyExpectAndExceptionCaughtExpectAndFinallyToBeCalled()
        {
            bool finallyCalled = false;
            bool handledCalled = false;

            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { handledCalled = true; })
                .AndFinally(() => { finallyCalled = true; })
                .AsErrorHandler();

            var testObject = new TestObject();

            Action act = () => testObject.Invoke(t => t.VoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldNotThrow();
            handledCalled.Should().BeTrue();
            finallyCalled.Should().BeTrue();
        }

        [TestMethod]
        public void ForATaskWithAndFinallyExpectAndExceptionCaughtExpectAndFinallyToBeCalled()
        {
            bool finallyCalled = false;
            bool handledCalled = false;

            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { handledCalled = true; })
                .AndFinally(() => { finallyCalled = true; })
                .AsErrorHandler();

            var testObject = new TestObject();

            Func<Task> act = () => testObject.GetAwaitable(t => t.AsyncVoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldNotThrow();
            handledCalled.Should().BeTrue();
            finallyCalled.Should().BeTrue();
        }

        [TestMethod]
        public void ForAnActionWithAndFinallyExpectAndExceptionUncaughtExpectAndFinallyToBeCalled()
        {
            bool finallyCalled = false;
            bool handledCalled = false;

            var handler = ErrorHandler.Define()
                .MayThrow<ArgumentException>()
                .HandledBy(e => { handledCalled = true; })
                .AndFinally(() => { finallyCalled = true; })
                .AsErrorHandler();

            var testObject = new TestObject();

            Action act = () => testObject.Invoke(t => t.VoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldThrow<ArithmeticException>();
            handledCalled.Should().BeFalse();
            finallyCalled.Should().BeTrue();
        }

        [TestMethod]
        public void ForATaskWithAndFinallyExpectAndExceptionUncaughtExpectAndFinallyToBeCalled()
        {
            bool finallyCalled = false;
            bool handledCalled = false;

            var handler = ErrorHandler.Define()
                .MayThrow<ArgumentException>()
                .HandledBy(e => { handledCalled = true; })
                .AndFinally(() => { finallyCalled = true; })
                .AsErrorHandler();

            var testObject = new TestObject();

            Func<Task> act = () => testObject.GetAwaitable(t => t.AsyncVoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldThrow<ArithmeticException>();
            handledCalled.Should().BeFalse();
            finallyCalled.Should().BeTrue();
        }

        [TestMethod]
        public void ForAnActionOrItShouldCallFirstHandlerForFirstExceptionBlock()
        {
            bool firstHandlerCalled = false;
            bool secondHandlerCalled = false;

            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { firstHandlerCalled = true; })
                .OrIt()
                .MayThrow<ArgumentException>()
                .HandledBy(e => { secondHandlerCalled = true; })
                .AsErrorHandler();

            var testObject = new TestObject();

            Action act = () => testObject.Invoke(t => t.VoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldNotThrow();
            firstHandlerCalled.Should().BeTrue();
            secondHandlerCalled.Should().BeFalse();
        }

        [TestMethod]
        public void ForAnActionOrItShouldCallSecondHandlerForSecondExceptionBlock()
        {
            bool firstHandlerCalled = false;
            bool secondHandlerCalled = false;

            var handler = ErrorHandler.Define()
                .MayThrow<ArgumentException>()
                .HandledBy(e => { firstHandlerCalled = true; })
                .OrIt()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { secondHandlerCalled = true; })
                .AsErrorHandler();

            var testObject = new TestObject();

            Action act = () => testObject.Invoke(t => t.VoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldNotThrow();
            firstHandlerCalled.Should().BeFalse();
            secondHandlerCalled.Should().BeTrue();
        }

        [TestMethod]
        public void ForATaskOrItShouldCallFirstHandlerForFirstExceptionBlock()
        {
            bool firstHandlerCalled = false;
            bool secondHandlerCalled = false;

            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { firstHandlerCalled = true; })
                .OrIt()
                .MayThrow<ArgumentException>()
                .HandledBy(e => { secondHandlerCalled = true; })
                .AsErrorHandler();

            var testObject = new TestObject();

            Func<Task> act = () => testObject.GetAwaitable(t => t.AsyncVoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldNotThrow();
            firstHandlerCalled.Should().BeTrue();
            secondHandlerCalled.Should().BeFalse();
        }

        [TestMethod]
        public void ForATaskOrItShouldCallSecondHandlerForSecondExceptionBlock()
        {
            bool firstHandlerCalled = false;
            bool secondHandlerCalled = false;

            var handler = ErrorHandler.Define()
                .MayThrow<ArgumentException>()
                .HandledBy(e => { firstHandlerCalled = true; })
                .OrIt()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { secondHandlerCalled = true; })
                .AsErrorHandler();

            var testObject = new TestObject();

            Func<Task> act = () => testObject.GetAwaitable(t => t.AsyncVoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldNotThrow();
            firstHandlerCalled.Should().BeFalse();
            secondHandlerCalled.Should().BeTrue();
        }

        [TestMethod]
        public void ForAnActionWithOrItAndFinallyAndFinallyShouldBeCalled()
        {
            bool finallyCalled = false;

            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { })
                .OrIt()
                .MayThrow<ArgumentException>()
                .HandledBy(e => { })
                .AndFinally(() => { finallyCalled = true; })
                .AsErrorHandler();

            var testObject = new TestObject();

            Action act = () => testObject.Invoke(t => t.VoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldNotThrow();
            finallyCalled.Should().BeTrue();
        }

        [TestMethod]
        public void WithAnActionWithAndRethrowsForSingleExceptionExpectRethrow()
        {
            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { })
                .AndRethrows()
                .AsErrorHandler();

            var testObject = new TestObject();

            Action act = () => testObject.Invoke(t => t.VoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldThrow<ArithmeticException>();
        }

        [TestMethod]
        public void WithATaskWithAndRethrowsForSingleExceptionExpectRethrow()
        {
            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { })
                .AndRethrows()
                .AsErrorHandler();

            var testObject = new TestObject();

            Func<Task> act = () => testObject.GetAwaitable(t => t.AsyncVoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldThrow<ArithmeticException>();
        }

        [TestMethod]
        public void WithAnActionWithAndRethrowsWithAndFinallyExpectRethrow()
        {
            bool finallyCalled = false;
            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { })
                .AndRethrows()
                .AndFinally(() => { finallyCalled = true; })
                .AsErrorHandler();

            var testObject = new TestObject();

            Action act = () => testObject.Invoke(t => t.VoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldThrow<ArithmeticException>();
            finallyCalled.Should().BeTrue();
        }

        [TestMethod]
        public void WithATaskWithAndRethrowsWithAndFinallyExpectRethrow()
        {
            bool finallyCalled = false;
            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { })
                .AndRethrows()
                .AndFinally(() => { finallyCalled = true; })
                .AsErrorHandler();

            var testObject = new TestObject();

            Func<Task> act = () => testObject.GetAwaitable(t => t.AsyncVoidMethodThatThrowsArithmeticException())
                .WithHandler(handler);

            act.ShouldThrow<ArithmeticException>();
            finallyCalled.Should().BeTrue();
        }

        private static ErrorHandler GetArithmeticExceptionHandler()
        {
            return ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { })
                .AsErrorHandler();
        }

        private static ErrorHandler GetArgumentExceptionHandler()
        {
            return ErrorHandler.Define()
                .MayThrow<ArgumentException>()
                .HandledBy(e => { })
                .AsErrorHandler();
        }
    }
}
