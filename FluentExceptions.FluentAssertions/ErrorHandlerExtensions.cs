namespace FluentAssertions
{
    using FluentExceptions;

    public static class ErrorHandlerExtensions
    {
        public static ErrorHandlerAssertions Should(this ErrorHandler subject)
        {
            return new ErrorHandlerAssertions(subject);
        }
    }
}
