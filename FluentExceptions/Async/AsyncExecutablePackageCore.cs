namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class AsyncExecutablePackageCore : AsyncExecutablePackageBase
    {
        public AsyncExecutablePackageCore(
            Func<Task> invocationTarget, 
            Action<Exception> exceptionHandlerAction, 
            Func<Task, Action<Exception>, bool, Task> handlerFunc,
            bool canRetry,
            bool rethrow) 
            : base(invocationTarget, exceptionHandlerAction, handlerFunc, canRetry, rethrow)
        {
        }

        public AsyncFinalExecutablePackage AndFinally(Action finallyAction)
        {
            Func<Func<Task>, bool, Task> finalTask = async (t, r) =>
            {
                try
                {
                    await t();
                }
                finally
                {
                    finallyAction();
                }
            };
            return new AsyncFinalExecutablePackage(this.WhenDeferred(), finalTask, this.Rethrow);
        }

        public AsyncPackage OrIt()
        {
            return new AsyncPackage(this.WhenDeferred(), this.CanRetry);
        }
    }
}