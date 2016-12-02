 
 
 
namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class AsyncPackage<T>
    {
        public AsyncPackage(Func<Task<T>> originalTask, bool canRetry)
        {
            this.InvocationTarget = originalTask;
            this.CanRetry = canRetry;
        }

        protected bool CanRetry { get; private set; }

        protected Func<Task<T>> InvocationTarget { get; private set; }

        public AsyncSingleExceptionHandlerPackage<T, TException> MayThrow<TException>() where TException : Exception
        {
            return new AsyncSingleExceptionHandlerPackage<T, TException>(this.InvocationTarget, canRetry: this.CanRetry);
        }
    }
}
