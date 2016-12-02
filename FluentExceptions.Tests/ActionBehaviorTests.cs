using System;
using FluentAssertions;
using FluentExceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentExceptions.Tests
{
    [TestClass]
    public class ActionBehaviorTests
    {
        [TestMethod]
        public void WhenInvokingWithVoidReturnExpectActionPackageReturned()
        {
            TestObject obj = new TestObject();

            var actionPackage = obj.Invoking(o => o.MethodThatDoesNothing());
            actionPackage.Should().NotBeNull();
        }

        [TestMethod]
        public void InvokingWhenExtendingActionExpectHandlingToWork()
        {
            Action act = () => { throw new ArithmeticException(); };

            bool handlerCalled = false;
            act.Invoking()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeTrue();
        }

        [TestMethod]
        public void MayThrowWhenExpectedExceptionThrownExpectHandling()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;
            obj.Invoking(o => o.VoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeTrue();
        }

        [TestMethod]
        public void MayThrowWhenUnexpectedExceptionThrownExpectException()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;
            Action act = () => obj.Invoking(o => o.VoidMethodThatThrowsArithmeticException())
                .MayThrow<ArgumentException>()
                .HandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            act.ShouldThrow<ArithmeticException>();
            handlerCalled.Should().BeFalse();            
        }

        [TestMethod]
        public void OrWhenSecondExceptionThrownExpectHandling()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;
            obj.Invoking(o => o.VoidMethodThatThrowsArithmeticException())
                .MayThrow<ArgumentException>()
                .Or<ArithmeticException>()
                .HandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeTrue();
        }        
        
        [TestMethod]
        public void OrWhenFirstExceptionThrownExpectHandling()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;
            obj.Invoking(o => o.VoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .Or<ArgumentException>()
                .HandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeTrue();
        }

        [TestMethod]
        public void OrWhenThirdExceptionThrownExpectHandling()
        {
            TestObject obj = new TestObject();

            bool handlerCalled = false;
            obj.Invoking(o => o.VoidMethodThatThrowsArithmeticException())
                .MayThrow<IndexOutOfRangeException>()
                .Or<ArgumentException>()
                .Or<ArithmeticException>()
                .HandledBy(e => { handlerCalled = true; })
                .WhenExecuted();

            handlerCalled.Should().BeTrue();
        }

        [TestMethod]
        public void AndFinallyShouldCallFinallyBlockIfHandled()
        {
            TestObject obj = new TestObject();

            bool finallyCalled = false;
            obj.Invoking(o => o.VoidMethodThatThrowsArithmeticException())
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { })
                .AndFinally(() => { finallyCalled = true; })
                .WhenExecuted();

            finallyCalled.Should().BeTrue();
        }

        [TestMethod]
        public void AndFinallyShouldCallFinallyBlockIfUnhandled()
        {
            TestObject obj = new TestObject();

            bool finallyCalled = false;
            Action act = () => obj.Invoking(o => o.VoidMethodThatThrowsArithmeticException())
                .MayThrow<ArgumentException>()
                .HandledBy(e => { })
                .AndFinally(() => { finallyCalled = true; })
                .WhenExecuted();
            
            act.ShouldThrow<ArithmeticException>();
            finallyCalled.Should().BeTrue();  
        }

        [TestMethod]
        public void OrItShouldCallFirstHandlerForFirstException()
        {
            TestObject obj = new TestObject();

            bool firstHandlerCalled = false;
            bool secondHandlerCalled = false;

            obj.Invoking(o => o.VoidMethodThatThrowsArithmeticException())
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
        public void OrItShouldCallSecondHandlerForSecondException()
        {
            TestObject obj = new TestObject();

            bool firstHandlerCalled = false;
            bool secondHandlerCalled = false;

            obj.Invoking(o => o.VoidMethodThatThrowsArithmeticException())
                .MayThrow<ArgumentException>()
                .HandledBy(e => { firstHandlerCalled = true; })
                .OrIt()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { secondHandlerCalled = true; })
                .WhenExecuted();

            firstHandlerCalled.Should().BeFalse();
            secondHandlerCalled.Should().BeTrue();
        }
    }
}
