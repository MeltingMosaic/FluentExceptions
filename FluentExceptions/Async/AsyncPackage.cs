 
 
 

namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class AsyncPackage
    {
        public AsyncPackage(Func<Task> originalTask, bool canRetry)
        {
            this.InvocationTarget = originalTask;
            this.CanRetry = canRetry;
        }

        protected bool CanRetry { get; private set; }

        protected Func<Task> InvocationTarget { get; private set; }

        public AsyncSingleExceptionHandlerPackage<TException> MayThrow<TException>() where TException : Exception
        {
            return new AsyncSingleExceptionHandlerPackage<TException>(this.InvocationTarget, canRetry: this.CanRetry);
        }
    }
}