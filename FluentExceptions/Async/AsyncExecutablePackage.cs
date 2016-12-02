 
 
 

namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class AsyncExecutablePackage : AsyncExecutablePackageCore
    {
        public AsyncExecutablePackage(
            Func<Task> invocationTarget, 
            Action<Exception> exceptionHandlerAction, 
            Func<Task, Action<Exception>, bool, Task> handlerFunc,
            bool canRetry,
            bool rethrow) : base(invocationTarget, exceptionHandlerAction, handlerFunc, canRetry, rethrow)
        {
        }

        public AsyncRethrowExecutablePackage AndRethrows()
        {
            return new AsyncRethrowExecutablePackage(this.InvocationTarget, this.ExceptionHandlerAction, this.HandlerFunc, this.CanRetry);
        }
    }
}
