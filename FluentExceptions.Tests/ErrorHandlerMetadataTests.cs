namespace FluentExceptions.Tests
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ErrorHandlerMetadataTests
    {
        [TestMethod]
        public void ForAHandlerExpectExceptionTypeInMetadata()
        {
            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { })
                .AsErrorHandler();

            handler.HandledTypes.Should().Contain(typeof(ArithmeticException));
        }
        
        [TestMethod]
        public void ForAHandlerExpectUnhandledExceptionTypeNotInMetadata()
        {
            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { })
                .AsErrorHandler();

            handler.HandledTypes.Should().NotContain(typeof(ArgumentException));
        }

        [TestMethod]
        public void ForMultipleExceptionsExpectAllTypesInMetadata()
        {
            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .Or<ArgumentException>()
                .HandledBy(e => { })
                .AsErrorHandler();

            handler.HandledTypes.Should().Contain(typeof(ArithmeticException))
                .And.Contain(typeof(ArgumentException));
        }

        [TestMethod]
        public void ForHandlerWithOrItExpectAllTypesInMetadata()
        {
            var handler = ErrorHandler.Define()
                .MayThrow<ArithmeticException>()
                .HandledBy(e => { })
                .OrIt()
                .MayThrow<ArgumentException>()
                .HandledBy(e => { })
                .AsErrorHandler();

            handler.HandledTypes.Should().Contain(typeof(ArithmeticException))
                .And.Contain(typeof(ArgumentException));
        }
    }
}
