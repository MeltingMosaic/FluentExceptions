using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentExceptions;
using FluentExceptions.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentExceptions.Tests
{
    [TestClass]
    public class AsyncBehaviorTests
    {
        [TestMethod]
        public void AsyncAwaitingShouldProduceTask()
        {
            TestObject obj = new TestObject();
            var task = obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException());

            task.Should().NotBeNull();
        }

        [TestMethod]
        public void AsyncMayThrowShouldReturnPackage()
        {
            TestObject obj = new TestObject();
            var package = obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>();

            package.Should().NotBeNull();
        }

        [TestMethod]
        public async Task AsyncMayThrowOnExpectedExceptionExpectHandling()
        {
            TestObject obj = new TestObject();

            bool handled = false;
            await obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { handled = true; })
                .WhenExecuted();

            handled.Should().BeTrue();
        }

        [TestMethod]
        public async Task AsyncWhenDeferredWhenNotAwaitedExpectNoHandlingUntilAfterCall()
        {
            TestObject obj = new TestObject();

            bool handled = false;
            var task = obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { handled = true; })
                .WhenDeferred();

            handled.Should().BeFalse();

            await task();
            handled.Should().BeTrue();
        }

        [TestMethod]
        public void AsyncMayThrowOnUnexpectedExceptionExpectException()
        {
            TestObject obj = new TestObject();

            bool handled = false;

            Func<Task> func = async () => await obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
                .MayThrow<ArgumentException>()
                .HandledBy(e => { handled = true; })
                .WhenExecuted();

            func.ShouldThrow<ArithmeticException>();
            handled.Should().BeFalse();
        }

        [TestMethod]
        public async Task AsyncOrWhenSecondExceptionThrownExpectHandling()
        {
            TestObject obj = new TestObject();
            bool handled = false;

            await obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
                .MayThrow<ArgumentException>()
                .Or<ArithmeticException>()
                .HandledBy(e => { handled = true; })
                .WhenExecuted();

            handled.Should().BeTrue();
        }

        [TestMethod]
        public async Task AsyncOrWhenFirstExceptionThrownExpectHandling()
        {
            TestObject obj = new TestObject();
            bool handled = false;

            await obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .Or<ArgumentException>()
                .HandledBy(e => { handled = true; })
                .WhenExecuted();

            handled.Should().BeTrue();
        }

        [TestMethod]
        public async Task AsyncOrWhenThirdExceptionThrownExpectHandling()
        {
            TestObject obj = new TestObject();
            bool handled = false;

            await obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
                .MayThrow<ArgumentException>()
                .Or<NotSupportedException>()
                .Or<ArithmeticException>()
                .HandledBy(e => { handled = true; })
                .WhenExecuted();

            handled.Should().BeTrue();
        }

        [TestMethod]
        public async Task AsyncAndFinallyWhenExceptionCaughtFinallyShouldBeCalled()
        {
            TestObject obj = new TestObject();
            bool finallyCalled = false;
            bool handledCalled = false;
            await obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { handledCalled = true; })
                .AndFinally(() => { finallyCalled = true; })
                .WhenExecuted();

            handledCalled.Should().BeTrue();
            finallyCalled.Should().BeTrue();
        }

        [TestMethod]
        public void AsyncAndFinallyWhenExceptionUncaughtFinallyShouldBeCalled()
        {
            TestObject obj = new TestObject();
            bool finallyCalled = false;
            bool handledCalled = false;
            Func<Task> func = async () => await obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
                .MayThrow<ArgumentException>()
                .HandledBy(e => { handledCalled = true; })
                .AndFinally(() => { finallyCalled = true; })
                .WhenExecuted();

            func.ShouldThrow<ArithmeticException>();
            handledCalled.Should().BeFalse();
            finallyCalled.Should().BeTrue();
        }

        [TestMethod]
        public async Task AsyncOrItWhenSecondGroupHandledExpectHandling()
        {
            TestObject obj = new TestObject();
            bool firstHandlerCalled = false;
            bool secondHandlerCalled = false;

            await obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
                .MayThrow<ArgumentException>()
                .HandledBy(e => { firstHandlerCalled = true; })
                .OrIt()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { secondHandlerCalled = true; })
                .WhenExecuted();

            firstHandlerCalled.Should().BeFalse();
            secondHandlerCalled.Should().BeTrue();
        }

        [TestMethod]
        public async Task AsyncOrItWhenFirstGroupHandledExpectHandling()
        {
            TestObject obj = new TestObject();
            bool firstHandlerCalled = false;
            bool secondHandlerCalled = false;

            await obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { firstHandlerCalled = true; })
                .OrIt()
                .MayThrow<ArgumentException>()
                .HandledBy(e => { secondHandlerCalled = true; })
                .WhenExecuted();

            firstHandlerCalled.Should().BeTrue();
            secondHandlerCalled.Should().BeFalse();
        }

        [TestMethod]
        public async Task AsyncOrItAndFinallyShouldBeCalledRegardlessOfPath()
        {
            TestObject obj = new TestObject();
            bool finallyCalled = false;

            await obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { })
                .OrIt()
                .MayThrow<ArgumentException>()
                .HandledBy(e => { })
                .AndFinally(() => { finallyCalled = true; })
                .WhenExecuted();

            finallyCalled.Should().BeTrue();
        }

        [TestMethod]
        public async Task AsyncOrItDeferredTaskShouldNotCallHandlerUntilAfterCall()
        {
            TestObject obj = new TestObject();
            bool firstHandlerCalled = false;
            bool secondHandlerCalled = false;
            bool thirdHandlerCalled = false;

            var task = obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { firstHandlerCalled = true; })
                .OrIt()
                .MayThrow<InvalidOperationException>()
                .HandledBy(e => { secondHandlerCalled = true; })
                .OrIt()
                .MayThrow<ArgumentException>()
                .HandledBy(e => { thirdHandlerCalled = true; })
                .WhenDeferred();

            firstHandlerCalled.Should().BeFalse();
            secondHandlerCalled.Should().BeFalse();
            thirdHandlerCalled.Should().BeFalse();

            await task();

            firstHandlerCalled.Should().BeTrue();
            secondHandlerCalled.Should().BeFalse();
            thirdHandlerCalled.Should().BeFalse();
        }
    }
}
