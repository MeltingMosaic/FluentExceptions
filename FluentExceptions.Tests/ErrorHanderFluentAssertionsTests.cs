namespace FluentExceptions.Tests
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ErrorHanderFluentAssertionsTests
    {
        [TestMethod]
        public void ShouldExtensionShouldReturnErrorHandlerAssertions()
        {
            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { })
                .AsErrorHandler();

            var shouldAssertions = handler.Should();

            shouldAssertions.Should().BeOfType<ErrorHandlerAssertions>();
        }

        [TestMethod]
        public void HandleShouldNotFailIfHandlerHandlesType()
        {
            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { })
                .AsErrorHandler();

            handler.Should().Handle<ArithmeticException>();
        }

        [TestMethod]
        public void HandleShouldFailIfHandlerDoesNotHandleType()
        {
            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { })
                .AsErrorHandler();

            Action act = () => handler.Should().Handle<ArgumentException>();

            act.ShouldThrow<AssertFailedException>().WithMessage("Expected to handle*");
        }

        [TestMethod]
        public void NotHandleShouldNotFailIfHandlerHandlesType()
        {
            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { })
                .AsErrorHandler();

            handler.Should().NotHandle<ArgumentException>();
        }

        [TestMethod]
        public void NotHandleShouldFailIfHandlerDoesNotHandleType()
        {
            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { })
                .AsErrorHandler();

            Action act = () => handler.Should().NotHandle<ArithmeticException>();

            act.ShouldThrow<AssertFailedException>().WithMessage("Expected not to handle*");
        }

        [TestMethod]
        public void HandleAndHandleShouldNotThrowIfConditionsMet()
        {
            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .Or<ArgumentException>()
                .HandledBy(e => { })
                .AsErrorHandler();
    
            handler.Should().Handle<ArithmeticException>()
                .And.Handle<ArgumentException>();
        }

        [TestMethod]
        public void HandleAndNotHandleShouldNotThrowIfConditionsMet()
        {
            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .Or<ArgumentException>()
                .HandledBy(e => { })
                .AsErrorHandler();
    
            handler.Should().Handle<ArithmeticException>()
                .And.NotHandle<IndexOutOfRangeException>();
        }

        [TestMethod]
        public void NotHandleAndHandleShouldNotThrowIfConditionsMet()
        {
            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .Or<ArgumentException>()
                .HandledBy(e => { })
                .AsErrorHandler();
    
            handler.Should().NotHandle<IndexOutOfRangeException>()
                .And.Handle<ArgumentException>();
        }

    }
}
