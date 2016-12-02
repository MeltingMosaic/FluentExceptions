using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentExceptions.Tests
{
    [TestClass]
    public class FuncBehaviorTests
    {
        [TestMethod]
        public void InvokingWhenExtendingFuncExpectHandlingToWork()
        {
            Func<int> func = () => { throw new ArithmeticException(); };

            bool handlerCalled = false;
            func.Invoking()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeTrue();
        }

        [TestMethod]
        public void MayThrowWhenPassedFuncExpectHandlingToWork()
        {
            TestObject obj = new TestObject();
            bool handlerCalled = false;

            obj.Invoking(o => o.IntMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeTrue();
        }

        [TestMethod]
        public void MayThrowWhenExceptionThrownNotHandledExpectException()
        {
            TestObject obj = new TestObject();

            var package = obj.Invoking(o => o.IntMethodThatThrowsArithmeticException());

            package.Should().NotBeNull();
            bool handlerCalled = false;
            AssertionExtensions.Invoking(
                package, 
                p => p.MayThrow<ArgumentNullException>()
                    .HandledBy(e => { handlerCalled = true; })
                    .WhenExecuted())
                .ShouldThrow<ArithmeticException>();

            handlerCalled.Should().BeFalse();
        }

        [TestMethod]
        public void OrWhenHandlingDesignatedExceptionExpectHandling()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;
            obj.Invoking(o => o.IntMethodThatThrowsArithmeticException())
                .MayThrow<ArgumentNullException>()
                .Or<ArithmeticException>()
                .HandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeTrue();
        }
        
        [TestMethod]
        public void OrWhenHandlingMayThrowExceptionExpectHandling()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;
            obj.Invoking(o => o.IntMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .Or<ArgumentException>()
                .HandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeTrue();
        }

        [TestMethod]
        public void OrShouldHandleThirdException()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;
            obj.Invoking(o => o.IntMethodThatThrowsArithmeticException())
                .MayThrow<ArgumentNullException>()
                .Or<ArgumentException>()
                .Or<ArithmeticException>()
                .HandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeTrue();
        }

        [TestMethod]
        public void AndFinallyWhenExecutedWithCaughtExceptionFinallyShouldBeCalled()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;
            bool finallyCalled = false;

            obj.Invoking(o => o.IntMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { handlerCalled = true; })
                .AndFinally(() => { finallyCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeTrue();
            finallyCalled.Should().BeTrue();
        }

        [TestMethod]
        public void AndFinallyWhenExceptionNotCaughtFinallyCalled()
        {
            TestObject obj = new TestObject();
            bool handlerCalled = false;
            bool finallyCalled = false;

            var func = obj.Invoking(o => o.IntMethodThatThrowsArithmeticException())
                .MayThrow<ArgumentException>()
                .HandledBy(e => { handlerCalled = true; })
                .AndFinally(() => { finallyCalled = true; })
                .Function;

            Action act = () => func();

            act.ShouldThrow<ArithmeticException>();

            handlerCalled.Should().BeFalse();
            finallyCalled.Should().BeTrue(); 
        }

        [TestMethod]
        public void OrItShouldCallFirstHandlerForFirstExceptionBlock()
        {
            TestObject obj = new TestObject();

            bool firstHandlerCalled = false;
            bool secondHandlerCalled = false;
            obj.Invoking(o => o.IntMethodThatThrowsArithmeticException())
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
        public void OrItShouldCallSecondHandlerForSecondExceptionBlock()
        {
            TestObject obj = new TestObject();

            bool firstHandlerCalled = false;
            bool secondHandlerCalled = false;
            obj.Invoking(o => o.IntMethodThatThrowsArithmeticException())
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
        public void OrItShouldPreserveDerivedExceptionOrder()
        {
            TestObject obj = new TestObject();
            bool firstHandlerCalled = false;
            bool secondHandlerCalled = false;
            obj.Invoking(o => o.IntMethodThatThrowsArgumentNullException())
                .MayThrow<ArgumentException>()
                .HandledBy(e => { firstHandlerCalled = true; })
                .OrIt()
                .MayThrow<ArgumentNullException>()
                .HandledBy(e => { secondHandlerCalled = true; })
                .WhenExecuted();

            firstHandlerCalled.Should().BeTrue();
            secondHandlerCalled.Should().BeFalse();
        }

        [TestMethod]
        public void OrItAndFinallyShouldBeCalledRegardlessOfPath()
        {
            TestObject obj = new TestObject();

            bool finallyCalled = false;
            obj.Invoking(o => o.IntMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { })
                .OrIt()
                .MayThrow<ArgumentException>()
                .HandledBy(e => { })
                .AndFinally(() => { finallyCalled = true; })
                .WhenExecuted();

            finallyCalled.Should().BeTrue();
        }
    }
}