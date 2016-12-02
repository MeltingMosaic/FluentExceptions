// Believe it or not, it actually matters putting the usings outside the namespace due to ambiguous method names
using System;
using FluentAssertions;
using FluentExceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentExceptions.Tests
{
    [TestClass]
    public class RethrowBehaviorTests
    {
        private const string SetName = "Test Set";

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestMethod]
        public void ActionAndRethrowsForSingleExceptionExpectRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;

            var action = obj.Invoking(o => o.VoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows().WhenDeferred();

            action.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
        }        
        
        [TestMethod]
        public void ActionAndRethrowsForMultipleExceptionsExpectFirstRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;

            var action = obj.Invoking(o => o.VoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .Or<ArgumentException>()
                .HandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows().WhenDeferred();

            action.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
        }

        [TestMethod]
        public void ActionAndRethrowsForMultipleExceptionsExpectSecondRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;

            var action = obj.Invoking(o => o.VoidMethodThatThrowsArithmeticException())
                .MayThrow<ArgumentException>()
                .Or<ArithmeticException>()
                .HandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows().WhenDeferred();

            action.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
        }

        [TestMethod]
        public void ActionAndRethrowsForMultipleExceptionsExpectThirdRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;

            var action = obj.Invoking(o => o.VoidMethodThatThrowsArithmeticException())
                .MayThrow<ArgumentException>()
                .Or<InvalidOperationException>()
                .Or<ArithmeticException>()
                .HandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows().WhenDeferred();

            action.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
        }

        [TestMethod]
        public void ActionAndRethrowsWithRetryForSingleExceptionExpectRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;

            Action action = () => obj.Invoking(o => o.VoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(2)
                .ThenHandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows()
                .WhenExecuted();

            action.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
        }

        [TestMethod]
        public void ActionAndRethrowsWithRetryForMultipleExceptionsExpectRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;

            Action action = () => obj.Invoking(o => o.VoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .Or<ArgumentException>()
                .RetryingXTimes(2)
                .ThenHandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows()
                .WhenExecuted();

            action.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
        }


        [TestMethod]
        public void ActionAndRethrowsAndFinallyForSingleExceptionExpectRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;
            bool finallyCalled = false;

            var action = obj.Invoking(o => o.VoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows()
                .AndFinally(() => { finallyCalled = true; }).WhenDeferred();

            action.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
            finallyCalled.Should().BeTrue();
        }       
        
        [TestMethod]
        public void FuncAndRethrowsForSingleExceptionExpectRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;

            Action action = () => obj.Invoking(o => o.IntMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows()
                .WhenExecuted();

            action.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
        }

        [TestMethod]
        public void FuncAndRethrowsForMultipleExceptionsExpectFirstRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;

            Action action = () => obj.Invoking(o => o.IntMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .Or<ArgumentException>()
                .HandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows()
                .WhenExecuted();

            action.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
        }

        [TestMethod]
        public void FuncAndRethrowsForMultipleExceptionsExpectThirdRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;

            Action action = () => obj.Invoking(o => o.IntMethodThatThrowsArithmeticException())
                .MayThrow<ArgumentException>()
                .Or<InvalidOperationException>()
                .Or<ArithmeticException>()
                .HandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows()
                .WhenExecuted();

            action.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
        }

        [TestMethod]
        public void FuncAndRethrowsWithRetryForSingleExceptionExpectRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;

            Action action = () => obj.Invoking(o => o.IntMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(2)
                .ThenHandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows()
                .WhenExecuted();

            action.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
        }

        [TestMethod]
        public void FuncAndRethrowsWithRetryForMultipleExceptionsExpectRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;

            Action action = () => obj.Invoking(o => o.IntMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .Or<ArgumentException>()
                .RetryingXTimes(2)
                .ThenHandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows()
                .WhenExecuted();

            action.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
        }

        [TestMethod]
        public void FuncAndRethrowsAndFinallyForSingleExceptionExpectRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;
            bool finallyCalled = false;

            Action action = () => obj.Invoking(o => o.IntMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows()
                .AndFinally(() => { finallyCalled = true; })
                .WhenExecuted();

            action.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
            finallyCalled.Should().BeTrue();
        }

        [TestMethod]
        public void AsyncAndRethrowsForSingleExceptionExpectRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;

            var task = obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows()
                .WhenDeferred();

            task.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
        }

        [TestMethod]
        public void AsyncAndRethrowsForMultipleExceptionsExpectFirstRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;

            var task = obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .Or<ArgumentException>()
                .HandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows()
                .WhenDeferred();

            task.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
        }

        [TestMethod]
        public void AsyncAndRethrowsForMultipleExceptionsExpectThirdRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;

            var task = obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
                .MayThrow<ArgumentException>()
                .Or<InvalidOperationException>()
                .Or<ArithmeticException>()
                .HandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows()
                .WhenDeferred();

            task.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
        }

        [TestMethod]
        public void AsyncAndRethrowsWithRetryForSingleExceptionExpectRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;

            var task = obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(2)
                .ThenHandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows()
                .WhenDeferred();

            task.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
        }

        [TestMethod]
        public void AsyncAndRethrowsWithRetryForMultipleExceptionsExpectRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;

            var task = obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .Or<ArgumentException>()
                .RetryingXTimes(2)
                .ThenHandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows()
                .WhenDeferred();

            task.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
        }

        [TestMethod]
        public void AsyncAndRethrowsAndFinallyForSingleExceptionExpectRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;
            bool finallyCalled = false;

            var task = obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows()
                .AndFinally(() => { finallyCalled = true; })
                .WhenDeferred();

            task.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
            finallyCalled.Should().BeTrue();
        }

        [TestMethod]
        public void AsyncTAndRethrowsForSingleExceptionExpectRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;

            var task = obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows()
                .WhenDeferred();

            task.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
        }

        [TestMethod]
        public void AsyncTAndRethrowsForMultipleExceptionsExpectFirstRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;

            var task = obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .Or<ArgumentException>()
                .HandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows()
                .WhenDeferred();

            task.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
        }

        [TestMethod]
        public void AsyncTAndRethrowsForMultipleExceptionsExpectThirdRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;

            var task = obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticException())
                .MayThrow<ArgumentException>()
                .Or<InvalidOperationException>()
                .Or<ArithmeticException>()
                .HandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows()
                .WhenDeferred();

            task.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
        }

        [TestMethod]
        public void AsyncTAndRethrowsWithRetryForSingleExceptionExpectRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;

            var task = obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(2)
                .ThenHandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows()
                .WhenDeferred();

            task.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
        }

        [TestMethod]
        public void AsyncTAndRethrowsWithRetryForMultipleExceptionsExpectRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;

            var task = obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .Or<ArgumentException>()
                .RetryingXTimes(2)
                .ThenHandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows()
                .WhenDeferred();

            task.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
        }

        [TestMethod]
        public void AsyncTAndRethrowsAndFinallyForSingleExceptionExpectRethrow()
        {
            TestObject obj = new TestObject();
            bool rethrowFilterCalled = false;
            bool finallyCalled = false;

            var task = obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { rethrowFilterCalled = true; })
                .AndRethrows()
                .AndFinally(() => { finallyCalled = true; })
                .WhenDeferred();

            task.ShouldThrow<ArithmeticException>();
            rethrowFilterCalled.Should().BeTrue();
            finallyCalled.Should().BeTrue();
        }
    }
}
