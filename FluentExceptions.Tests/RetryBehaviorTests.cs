using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentExceptions.Tests
{
    using System.Threading.Tasks;

    [TestClass]
    public class RetryBehaviorTests
    {
        [TestMethod]
        public void WithAVoidMethodRetryXTimesForSingleExceptionShouldThrowWhenRetryCountExceeded()
        {
            TestObject obj = new TestObject();

            var action = obj.Invoking(o => o.VoidMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(2))
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(1)
                .WhenDeferred();

            action.ShouldThrow<ArithmeticException>();
        }

        [TestMethod]
        public void WithAVoidMethodRetryXTimesForSingleExceptionShouldNotThrowOnSuccess()
        {
            TestObject obj = new TestObject();

            obj.Invoking(o => o.VoidMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(2))
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(4)
                .WhenExecuted();
        }

        [TestMethod]
        public void WithAVoidMethodRetryXTimesForMultipleExceptionsShouldThrowWhenRetryCountExceeded()
        {
            TestObject obj = new TestObject();

            var action = obj.Invoking(o => o.VoidMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(2))
                .MayThrow<ArgumentException>()
                .Or<IndexOutOfRangeException>()
                .Or<ArithmeticException>()
                .RetryingXTimes(1).WhenDeferred();

            action.ShouldThrow<ArithmeticException>();
        }

        [TestMethod]
        public void WithAVoidMethodRetryXTimesForMultipleExceptionsShouldNotThrowOnSuccess()
        {
            TestObject obj = new TestObject();

            obj.Invoking(o => o.VoidMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(2))
                .MayThrow<ArgumentException>()
                .Or<IndexOutOfRangeException>()
                .Or<ArithmeticException>()
                .RetryingXTimes(4)
                .WhenExecuted();
        }

        [TestMethod]
        public void WithAVoidMethodThenHandledByShouldBeCalledWhenExceptionThrown()
        {
            TestObject obj = new TestObject();
            bool handlerCalled = false;

            obj.Invoking(o => o.VoidMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(2))
                .MayThrow<ArithmeticException>()
                .Or<IndexOutOfRangeException>()
                .RetryingXTimes(1)
                .ThenHandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeTrue();
        }

        [TestMethod]
        public void WithAVoidMethodThenHandledByShouldNotBeCalledOnSuccess()
        {
            TestObject obj = new TestObject();
            bool handlerCalled = false;

            obj.Invoking(o => o.VoidMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(2))
                .MayThrow<ArithmeticException>()
                .Or<IndexOutOfRangeException>()
                .RetryingXTimes(4)
                .ThenHandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeFalse();
        }

        [TestMethod]
        public void WithANonVoidMethodRetryTimesForSingleExceptionShouldRetryThenThrow()
        {
            TestObject obj = new TestObject();

            var func = obj.Invoking(o => o.IntMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(2))
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(1)
                .Function;

            Action act = () => func();

            act.ShouldThrow<ArithmeticException>();
        }


        [TestMethod]
        public void WithANonVoidMethodRetryTimesForMultipleExceptionsShouldRetryThenThrow()
        {
            TestObject obj = new TestObject();

            var func = obj.Invoking(o => o.IntMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(2))
                .MayThrow<ArgumentException>()
                .Or<IndexOutOfRangeException>()
                .Or<ArithmeticException>()
                .RetryingXTimes(1)
                .Function;

            Action act = () => func();

            act.ShouldThrow<ArithmeticException>();
        }

        [TestMethod]
        public void WithANonVoidMethodRetryTimesForMultipleExceptionsWhenSuccessBeforeRetryCountShouldNotThrow()
        {
            TestObject obj = new TestObject();

            int result = obj.Invoking(o => o.IntMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(2))
                .MayThrow<ArgumentException>()
                .Or<IndexOutOfRangeException>()
                .Or<ArithmeticException>()
                .RetryingXTimes(4)
                .WhenExecuted();

            result.Should().Be(42);
        }

        [TestMethod]
        public void WithANonVoidMethodThenHandledByForSingleExceptionWhenRetryCountExceededHandlerShouldBeCalled()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;
            obj.Invoking(o => o.IntMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(2))
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(1)
                .ThenHandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeTrue();
        }
        [TestMethod]
        public void WithANonVoidMethodThenHandledByForMultipleExceptionsWhenRetryCountExceededHandlerShouldBeCalled()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;
            obj.Invoking(o => o.IntMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(2))
                .MayThrow<ArithmeticException>()
                .Or<ArgumentNullException>()
                .Or<IndexOutOfRangeException>()
                .RetryingXTimes(1)
                .ThenHandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeTrue();
        }

        [TestMethod]
        public void WithANonVoidMethodRetryTimesForMultipleExceptionsAndFinallyShouldBeCalled()
        {
            TestObject obj = new TestObject();

            bool finallyCalled = false;
            var func = obj.Invoking(o => o.IntMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(2))
                .MayThrow<ArgumentException>()
                .Or<IndexOutOfRangeException>()
                .Or<ArithmeticException>()
                .RetryingXTimes(1)
                .AndFinally(() => { finallyCalled = true; })
                .Function;

            Action act = () => func();

            act.ShouldThrow<ArithmeticException>();
            finallyCalled.Should().BeTrue();
        }

        [TestMethod]
        public async Task WithAnAsyncVoidMethodRetryXTimesForSingleExceptionShouldNotThrowOnSuccess()
        {
            TestObject obj = new TestObject();

            await obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(2))
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(4)
                .WhenExecuted();
        }

        [TestMethod]
        public void WithAnAsyncVoidMethodRetryXTimesForSingleExceptionShouldThrowOnRetryCountExceeded()
        {
            TestObject obj = new TestObject();

            Func<Task> func = () => obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(1)
                .WhenExecuted();

            func.ShouldThrow<ArithmeticException>();
        }

        [TestMethod]
        public void WithAnAsyncVoidMethodRetryXTimesForMultipleExceptionsShouldThrowWhenRetryCountExceeded()
        {
            TestObject obj = new TestObject();

            Func<Task> func =
                () => obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
                    .MayThrow<ArgumentException>()
                    .Or<IndexOutOfRangeException>()
                    .Or<ArithmeticException>()
                    .RetryingXTimes(1)
                    .WhenExecuted();

            func.ShouldThrow<ArithmeticException>();
        }

        [TestMethod]
        public void WithAnAsyncVoidMethodRetryXTimesForMultipleExceptionsShouldNotThrowOnSuccess()
        {
            TestObject obj = new TestObject();

            Func<Task> func =
                () => obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(2))
                    .MayThrow<ArgumentException>()
                    .Or<IndexOutOfRangeException>()
                    .Or<ArithmeticException>()
                    .RetryingXTimes(2)
                    .WhenExecuted();

            func.ShouldNotThrow();
        }

        [TestMethod]
        public void WithAnAsyncVoidMethodRetryXTimesForTaskExpectInvalidOperationException()
        {
            Func<Task> func = () => Task.Delay(0)
                .Awaiting()
                .MayThrow<ArgumentException>()
                .RetryingXTimes(1)
                .WhenExecuted();

            func.ShouldThrow<InvalidOperationException>().WithMessage("*cannot use Retry*");
        }

        [TestMethod]
        public async Task WithAnAsyncVoidMethodThenHandledByShouldBeCalledWhenExceptionThrown()
        {
            TestObject obj = new TestObject();
            bool handlerCalled = false;

            await obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(1)
                .ThenHandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeTrue();
        }

        [TestMethod]
        public async Task WithAnAsyncVoidMethodThenHandledByShouldNotBeCalledOnSuccess()
        {
            TestObject obj = new TestObject();
            bool handlerCalled = false;

            await obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(1))
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(2)
                .ThenHandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeFalse();
        }

        [TestMethod]
        public async Task WithAnAsyncVoidMethodAndFinallyWithThenHandledShouldCallFinallyForHandledException()
        {
            TestObject obj = new TestObject();
            bool handlerCalled = false;
            bool finallyCalled = false;

            await obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(1))
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(2)
                .ThenHandledBy(e => { handlerCalled = true; })
                .AndFinally(() => { finallyCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeFalse();
            finallyCalled.Should().BeTrue();
        }

        [TestMethod]
        public void WithAnAsyncVoidMethodAndFinallyWithThenHandledShouldCallFinallyForUnhandledException()
        {
            TestObject obj = new TestObject();
            bool handlerCalled = false;
            bool finallyCalled = false;

            var func = obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(1))
                .MayThrow<ArgumentException>()
                .RetryingXTimes(2)
                .ThenHandledBy(e => { handlerCalled = true; })
                .AndFinally(() => { finallyCalled = true; })
                .WhenDeferred();

            func.ShouldThrow<ArithmeticException>();
            handlerCalled.Should().BeFalse();
            finallyCalled.Should().BeTrue();
        }

        [TestMethod]
        public async Task WithAnAsyncVoidMethodOrItWithRetryXTimesShouldHandleFirstBlock()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;

            await obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(1))
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(2)
                .OrIt()
                .MayThrow<ArgumentException>()
                .HandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeFalse();
        }

        [TestMethod]
        public async Task WithAnAsyncVoidMethodOrItThenHandledByWithRetryXTimesShouldHandleFirstBlock()
        {
            TestObject obj = new TestObject();

            bool firstHandlerCalled = false;
            bool secondHandlerCalled = false;

            await obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(4))
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(2)
                .ThenHandledBy(e => { firstHandlerCalled = true; })
                .OrIt()
                .MayThrow<ArgumentException>()
                .HandledBy(e => { secondHandlerCalled = true; })
                .WhenExecuted();

            firstHandlerCalled.Should().BeTrue();
            secondHandlerCalled.Should().BeFalse();
        }

        [TestMethod]
        public async Task WithAnAsyncVoidMethodOrItWithRetryXTimesInSecondBlockShouldHandleSecondBlock()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;

            await obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(1))
                .MayThrow<ArgumentException>()
                .HandledBy(e => { handlerCalled = true; })
                .OrIt()
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(2)
                .WhenExecuted();

            handlerCalled.Should().BeFalse();
        }

        [TestMethod]
        public async Task WithAnAsyncVoidMethodOrItWithRetryXTimesShouldHandleSecondBlock()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;

            await obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(1))
                .MayThrow<ArgumentException>()
                .RetryingXTimes(2)
                .OrIt()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeTrue();
        }

        [TestMethod]
        public async Task WithAnAsyncNonVoidMethodRetryXTimesForSingleExceptionShouldNotThrowOnSuccess()
        {
            TestObject obj = new TestObject();

            var result = await obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(2))
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(4)
                .WhenExecuted();

            result.Should().Be(TestObject.IntResult);
        }

        [TestMethod]
        public void WithAnAsyncNonVoidMethodRetryXTimesForSingleExceptionShouldThrowOnRetryCountExceeded()
        {
            TestObject obj = new TestObject();

            var func = obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(1)
                .WhenDeferred();

            func.ShouldThrow<ArithmeticException>();
        }

        [TestMethod]
        public void WithAnAsyncNonVoidMethodRetryXTimesForMultipleExceptionsShouldThrowWhenRetryCountExceeded()
        {
            TestObject obj = new TestObject();

            var func = obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticException())
                    .MayThrow<ArgumentException>()
                    .Or<IndexOutOfRangeException>()
                    .Or<ArithmeticException>()
                    .RetryingXTimes(1)
                    .WhenDeferred();

            func.ShouldThrow<ArithmeticException>();
        }

        [TestMethod]
        public async Task WithAnAsyncNonVoidMethodRetryXTimesForMultipleExceptionsShouldNotThrowOnSuccess()
        {
            TestObject obj = new TestObject();

            var result = await obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(2))
                    .MayThrow<ArgumentException>()
                    .Or<IndexOutOfRangeException>()
                    .Or<ArithmeticException>()
                    .RetryingXTimes(2)
                    .WhenExecuted();

            result.Should().Be(TestObject.IntResult);
        }

        [TestMethod]
        public void WithAnAsyncNonVoidMethodRetryXTimesForTaskExpectInvalidOperationException()
        {
            Func<Task<int>> func = () => Task.FromResult(0)
                .Awaiting()
                .MayThrow<ArgumentException>()
                .RetryingXTimes(1)
                .WhenExecuted();

            func.ShouldThrow<InvalidOperationException>().WithMessage("*cannot use Retry*");
        }

        [TestMethod]
        public async Task WithAnAsyncNonVoidMethodThenHandledByShouldBeCalledWhenExceptionThrown()
        {
            TestObject obj = new TestObject();
            bool handlerCalled = false;

            int result = await obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(1)
                .ThenHandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeTrue();
            result.Should().Be(default(int));
        }

        [TestMethod]
        public async Task WithAnAsyncNonVoidMethodThenHandledByShouldNotBeCalledOnSuccess()
        {
            TestObject obj = new TestObject();
            bool handlerCalled = false;

            var result = await obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(1))
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(2)
                .ThenHandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeFalse();
            result.Should().Be(TestObject.IntResult);
        }

        [TestMethod]
        public async Task WithAnAsyncNonVoidMethodAndFinallyWithThenHandledShouldCallFinallyForHandledException()
        {
            TestObject obj = new TestObject();
            bool handlerCalled = false;
            bool finallyCalled = false;

            await obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(1))
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(2)
                .ThenHandledBy(e => { handlerCalled = true; })
                .AndFinally(() => { finallyCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeFalse();
            finallyCalled.Should().BeTrue();
        }

        [TestMethod]
        public void WithAnAsyncNonVoidMethodAndFinallyWithThenHandledShouldCallFinallyForUnhandledException()
        {
            TestObject obj = new TestObject();
            bool handlerCalled = false;
            bool finallyCalled = false;

            var func = obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(1))
                .MayThrow<ArgumentException>()
                .RetryingXTimes(2)
                .ThenHandledBy(e => { handlerCalled = true; })
                .AndFinally(() => { finallyCalled = true; })
                .WhenDeferred();

            func.ShouldThrow<ArithmeticException>();
            handlerCalled.Should().BeFalse();
            finallyCalled.Should().BeTrue();
        }

        [TestMethod]
        public async Task WithAnAsyncNonVoidMethodOrItWithRetryXTimesShouldHandleFirstBlock()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;

            int result = await obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(1))
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(2)
                .OrIt()
                .MayThrow<ArgumentException>()
                .HandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            result.Should().Be(TestObject.IntResult);
            handlerCalled.Should().BeFalse();
        }

        [TestMethod]
        public async Task WithAnAsyncNonVoidMethodOrItWithRetryXTimesInSecondBlockShouldHandleSecondBlock()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;

            int result = await obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(1))
                .MayThrow<ArgumentException>()
                .HandledBy(e => { handlerCalled = true; })
                .OrIt()
                .MayThrow<ArithmeticException>()
                .RetryingXTimes(2)
                .WhenExecuted();

            result.Should().Be(TestObject.IntResult);
            handlerCalled.Should().BeFalse();
        }

        [TestMethod]
        public async Task WithAnAsyncNonVoidMethodOrItWithRetryXTimesShouldHandleSecondBlock()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;

            int result = await obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(1))
                .MayThrow<ArgumentException>()
                .RetryingXTimes(2)
                .OrIt()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeTrue();
            result.Should().Be(default(int));
        }

        [TestMethod]
        public void WithAVoidMethodAndASingleExceptionForRetryDelayExpectDelayFuncToBeCalledForEachIteration()
        {
            TestObject obj = new TestObject();

            int delayCallCount = 0;
            obj.Invoking(o => o.VoidMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(3))
                .MayThrow<ArithmeticException>()
                .RetryingXTimesWithDelayFunction(
                3, 
                r => 
                { 
                    delayCallCount++;
                    return 0;
                })
                .WhenExecuted();

            delayCallCount.Should().Be(3);
        }

        [TestMethod]
        public void WithAVoidMethodAndMultipleExceptionsForRetryDelayExpectDelayFuncToBeCalledForEachIteration()
        {
            TestObject obj = new TestObject();

            int delayCallCount = 0;
            obj.Invoking(o => o.VoidMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(3))
                .MayThrow<ArithmeticException>()
                .Or<ArgumentException>()
                .RetryingXTimesWithDelayFunction(
                3, 
                r => 
                { 
                    delayCallCount++;
                    return 0;
                })
                .WhenExecuted();

            delayCallCount.Should().Be(3);
        }

        [TestMethod]
        public void WithANonVoidMethodAndASingleExceptionForRetryDelayExpectDelayFuncToBeCalledForEachIteration()
        {
            TestObject obj = new TestObject();

            int delayCallCount = 0;
            obj.Invoking(o => o.IntMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(3))
                .MayThrow<ArithmeticException>()
                .RetryingXTimesWithDelayFunction(
                3, 
                r => 
                { 
                    delayCallCount++;
                    return 0;
                })
                .WhenExecuted();

            delayCallCount.Should().Be(3);
        }

        [TestMethod]
        public void WithANonVoidMethodAndMultipleExceptionsForRetryDelayExpectDelayFuncToBeCalledForEachIteration()
        {
            TestObject obj = new TestObject();

            int delayCallCount = 0;
            obj.Invoking(o => o.IntMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(3))
                .MayThrow<ArithmeticException>()
                .Or<ArgumentException>()
                .RetryingXTimesWithDelayFunction(
                3, 
                r => 
                { 
                    delayCallCount++;
                    return 0;
                })
                .WhenExecuted();

            delayCallCount.Should().Be(3);
        }

        [TestMethod]
        public async Task WithAnAsyncVoidMethodAndASingleExceptionForRetryDelayExpectDelayFuncToBeCalledForEachIteration()
        {
            TestObject obj = new TestObject();

            int delayCallCount = 0;
            await obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(3))
                .MayThrow<ArithmeticException>()
                .RetryingXTimesWithDelayFunction(
                3, 
                r => 
                { 
                    delayCallCount++;
                    return 0;
                })
                .WhenExecuted();

            delayCallCount.Should().Be(3);
        }

        [TestMethod]
        public async Task WithAnAsyncVoidMethodAndMultipleExceptionsForRetryDelayExpectDelayFuncToBeCalledForEachIteration()
        {
            TestObject obj = new TestObject();

            int delayCallCount = 0;
            await obj.Awaiting(o => o.AsyncVoidMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(3))
                .MayThrow<ArithmeticException>()
                .Or<ArgumentException>()
                .RetryingXTimesWithDelayFunction(
                3, 
                r => 
                { 
                    delayCallCount++;
                    return 0;
                })
                .WhenExecuted();

            delayCallCount.Should().Be(3);
        }

        [TestMethod]
        public async Task WithAnAsyncNonVoidMethodAndASingleExceptionForRetryDelayExpectDelayFuncToBeCalledForEachIteration()
        {
            TestObject obj = new TestObject();

            int delayCallCount = 0;
            await obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(3))
                .MayThrow<ArithmeticException>()
                .RetryingXTimesWithDelayFunction(
                3, 
                r => 
                { 
                    delayCallCount++;
                    return 0;
                })
                .WhenExecuted();

            delayCallCount.Should().Be(3);
        }

        [TestMethod]
        public async Task WithAnAsyncNonVoidMethodAndMultipleExceptionsForRetryDelayExpectDelayFuncToBeCalledForEachIteration()
        {
            TestObject obj = new TestObject();

            int delayCallCount = 0;
            await obj.Awaiting(o => o.AsyncIntMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(3))
                .MayThrow<ArithmeticException>()
                .Or<ArgumentException>()
                .RetryingXTimesWithDelayFunction(
                3, 
                r => 
                { 
                    delayCallCount++;
                    return 0;
                })
                .WhenExecuted();

            delayCallCount.Should().Be(3);
        }
    }
}
