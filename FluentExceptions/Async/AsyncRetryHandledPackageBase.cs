namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class AsyncRetryHandledPackageBase : AsyncDeferredExecutablePackageBase
    {
        public AsyncRetryHandledPackageBase(
            Func<Task> invocationTarget, 
            Func<Func<Task>, bool, Task> deferredFunc,
            bool canRetry,
            bool rethrow) : base(invocationTarget, deferredFunc, canRetry, rethrow)
        {
        }

        public AsyncFinalExecutablePackage AndFinally(Action finallyAction)
        {
            var currentHandler = this.DeferredFunc;

            Func<Func<Task>, bool, Task> newHandler = async (f, r) =>
            {
                try
                {
                    await currentHandler(f, r);
                }
                finally
                {
                    finallyAction();
                }
            };

            return new AsyncFinalExecutablePackage(this.InvocationTarget, newHandler, this.Rethrow);
        }

        public AsyncPackage OrIt()
        {
            return new AsyncPackage(this.WhenDeferred(), this.CanRetry);
        }
    }
}