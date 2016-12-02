namespace FluentAssertions
{
    using System;
    using Execution;
    using FluentExceptions;
    using Primitives;

    public class ErrorHandlerAssertions : ObjectAssertions 
    {
        public ErrorHandlerAssertions(ErrorHandler subject)
            : base(subject)
        {
        }

        private ErrorHandler Handler
        {
            get { return (ErrorHandler) this.Subject; }
        }

        public AndConstraint<ErrorHandlerAssertions> Handle<T>(string reason = "", params object[] reasonArgs) where T : Exception
        {
            if (!this.Handler.HandledTypes.Contains(typeof(T)))
            {
                Execute.Assertion.BecauseOf(reason, reasonArgs)
                    .FailWith("Expected to handle {0}{reason}.", typeof(T));
            }

            return new AndConstraint<ErrorHandlerAssertions>(this);
        }

        public AndConstraint<ErrorHandlerAssertions> NotHandle<T>(string reason = "", params object[] reasonArgs) where T : Exception
        {
            if (this.Handler.HandledTypes.Contains(typeof(T)))
            {
                Execute.Assertion.BecauseOf(reason, reasonArgs)
                    .FailWith("Expected not to handle {0}{reason}.", typeof(T));
            }

            return new AndConstraint<ErrorHandlerAssertions>(this);
        }
    }
}