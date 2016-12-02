namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class AsyncRethrowExecutablePackage : AsyncExecutablePackageCore
    {
        public AsyncRethrowExecutablePackage(
            Func<Task> invocationTarget, 
            Action<Exception> exceptionHandlerAction, 
            Func<Task, Action<Exception>, bool, Task> handlerFunc, 
            bool canRetry)
            : base (invocationTarget, exceptionHandlerAction, handlerFunc, canRetry, true)
        {
        }
    }
}