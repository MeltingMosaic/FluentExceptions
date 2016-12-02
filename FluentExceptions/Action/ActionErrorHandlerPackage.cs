namespace FluentExceptions
{
    using System;

    public class ActionErrorHandlerPackage
    {
        public ActionErrorHandlerPackage(Action action)
        {
            this.InvocationTarget = action;
        }

        protected Action InvocationTarget { get; private set; }

        public void WithHandler(ErrorHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            if (handler.ActionHandler == null)
            {
                throw new InvalidOperationException("This handler is not configured to handle Actions");
            }

            handler.ActionHandler(this.InvocationTarget);
        }
    }
}