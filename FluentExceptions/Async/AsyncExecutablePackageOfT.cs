 
 
 
namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class AsyncExecutablePackage<TResult> : AsyncExecutablePackageBase<TResult>
    {
        public AsyncExecutablePackage(
            Func<Task<TResult>> invocationTarget,
            Action<Exception> exceptionHandlerAction,
            Func<Task<TResult>, Action<Exception>, bool, Task<TResult>> handlerFunc,
            bool canRetry,
            bool rethrow) : base(invocationTarget, exceptionHandlerAction, handlerFunc, canRetry, rethrow)
        {
        }

        public AsyncRethrowExecutablePackage<TResult> AndRethrows()
        {
            return new AsyncRethrowExecutablePackage<TResult>(
                this.InvocationTarget, 
                this.ExceptionHandlerAction,
                this.HandlerFunc,
                this.CanRetry);
        }
    }

    public class AsyncRethrowExecutablePackage<TResult> : AsyncExecutablePackageBase<TResult>
    {
        public AsyncRethrowExecutablePackage(
            Func<Task<TResult>> invocationTarget, 
            Action<Exception> exceptionHandlerAction, 
            Func<Task<TResult>, Action<Exception>, bool, Task<TResult>> handlerFunc,
            bool canRetry)
            : base (invocationTarget, exceptionHandlerAction, handlerFunc, canRetry, rethrow: true)
        {
        }
    }
}