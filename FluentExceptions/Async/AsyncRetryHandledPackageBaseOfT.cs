namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class AsyncRetryHandledPackageBase<TResult> : AsyncDeferredExecutablePackageBase<TResult>
    {
        public AsyncRetryHandledPackageBase(
            Func<Task<TResult>> invocationTarget, 
            Func<Func<Task<TResult>>, bool, Task<TResult>> deferredFunc,
            bool rethrow) 
            : base(invocationTarget, deferredFunc, rethrow)
        {
        }

        public AsyncFinalExecutablePackage<TResult> AndFinally(Action finallyAction)
        {
            var currentHandler = this.DeferredFunc;

            Func<Func<Task<TResult>>, bool, Task<TResult>> newHandler = async (f, r) =>
            {
                try
                {
                    return await currentHandler(f, r);
                }
                finally
                {
                    finallyAction();
                }
            };

            return new AsyncFinalExecutablePackage<TResult>(this.InvocationTarget, newHandler, this.Rethrow);
        }
    }
}