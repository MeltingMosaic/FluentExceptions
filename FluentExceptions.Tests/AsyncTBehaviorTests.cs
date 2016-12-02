
namespace FluentExceptions.Tests
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using FluentExceptions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AsyncTBehaviorTests
    {
        [TestMethod]
        public void AwaitingShouldReturnHandlerOfT()
        {
            TestObject obj = new TestObject();

            AsyncPackage<int> package = obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticException());

            package.Should().NotBeNull();
        }

        [TestMethod]
        public void AwaitingFuncShouldReturnHandlerOfT()
        {
            TestObject obj = new TestObject();

            Func<Task<int>> func = () => obj.AsyncIntMethodThatThrowsArithmeticException();

            AsyncPackage<int> package = func.Awaiting();

            package.Should().NotBeNull();
        }

        [TestMethod]
        public async Task MayThrowExpectHandlingToWork()
        {
            TestObject obj = new TestObject();
            bool handled = false;
            int result = await obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { handled = true; })
                .WhenExecuted();

            handled.Should().BeTrue();
            result.Should().Be(default(int));
        }

        [TestMethod]
        public async Task WithTaskFuncMayThrowShouldWork()
        {
            TestObject obj = new TestObject();
            bool handled = false;

            Func<Task<int>> func = () => obj.AsyncIntMethodThatThrowsArithmeticException();
            int result = await func.Awaiting()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { handled = true; })
                .WhenExecuted();

            handled.Should().BeTrue();
            result.Should().Be(default(int));            
        }

        [TestMethod]
        public async Task WhenDeferredWhenNotAwaitedExpectNoHandlingUntilAfterCall()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;

            var func = obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { handlerCalled = true; })
                .WhenDeferred();

            handlerCalled.Should().BeFalse();

            int result = await func();
            handlerCalled.Should().BeTrue();
            result.Should().Be(default(int));
        }

        [TestMethod]
        public void MayThrowWhenExceptionThrownNotHandledExpectException()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;

            Func<Task> func = () => obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticException())
                .MayThrow<ArgumentNullException>()
                    .HandledBy(e => { handlerCalled = true; })
                    .WhenExecuted();

            func.ShouldThrow<ArithmeticException>();

            handlerCalled.Should().BeFalse();
        }

        [TestMethod]
        public async Task OrWhenHandlingFirstExceptionExpectHandling()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;

            await obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .Or<ArgumentNullException>()
                .HandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeTrue();
        }
        
        [TestMethod]
        public async Task OrWhenHandlingSecondExceptionExpectHandling()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;

            await obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticException())
                .MayThrow<ArgumentException>()
                .Or<ArithmeticException>()
                .HandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeTrue();
        }

        [TestMethod]
        public async Task OrWhenHandlingThirdExceptionExpectHandling()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;

            await obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticException())
                .MayThrow<ArgumentException>()
                .Or<InvalidOperationException>()
                .Or<ArithmeticException>()
                .HandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeTrue();
        }

        [TestMethod]
        public async Task AndFinallyWhenExceptionHandledExpectFinallyCalled()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;
            bool finallyCalled = false;

            await obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { handlerCalled = true; })
                .AndFinally(() => { finallyCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeTrue();
            finallyCalled.Should().BeTrue();
        }

        [TestMethod]
        public void AndFinallyWhenExceptionUnhandledExpectFinallyCalled()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;
            bool finallyCalled = false;

            var task = obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticException())
                .MayThrow<ArgumentException>()
                .HandledBy(e => { handlerCalled = true; })
                .AndFinally(() => { finallyCalled = true; })
                .WhenDeferred();

            task.ShouldThrow<ArithmeticException>();

            handlerCalled.Should().BeFalse();
            finallyCalled.Should().BeTrue();
        }

        [TestMethod]
        public async Task OrItWhenSecondGroupHandledExpectHandling()
        {
            TestObject obj = new TestObject();
            bool firstHandlerCalled = false;
            bool secondHandlerCalled = false;

            await obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticException())
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
        public async Task OrItWhenFirstGroupHandledExpectHandling()
        {
            TestObject obj = new TestObject();
            bool firstHandlerCalled = false;
            bool secondHandlerCalled = false;

            await obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticException())
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
        public async Task OrItAndFinallyWhenDeferredWhenFirstGroupHandledExpectHandlingAfterCall()
        {
            TestObject obj = new TestObject();
            bool firstHandlerCalled = false;
            bool secondHandlerCalled = false;
            bool finallyCalled = false;

            var func = obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { firstHandlerCalled = true; })
                .OrIt()
                .MayThrow<ArgumentException>()
                .HandledBy(e => { secondHandlerCalled = true; })
                .AndFinally(() => { finallyCalled = true; })
                .WhenDeferred();

            firstHandlerCalled.Should().BeFalse();
            secondHandlerCalled.Should().BeFalse();

            await func();

            firstHandlerCalled.Should().BeTrue();
            secondHandlerCalled.Should().BeFalse();
            finallyCalled.Should().BeTrue();
        }
    }
}
