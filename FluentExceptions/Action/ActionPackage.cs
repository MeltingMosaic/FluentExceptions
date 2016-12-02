 
 
 

namespace FluentExceptions
{
    using System;

    public class ActionPackage
    {
        protected Action InvocationTarget { get; private set; }

        public ActionPackage(Action invocationTarget)
        {
            this.InvocationTarget = invocationTarget;
        }

        public ActionSingleExceptionHandlerPackage<TException> MayThrow<TException>() where TException : Exception
        {
            return new ActionSingleExceptionHandlerPackage<TException>(this.InvocationTarget);
        }
    }
}