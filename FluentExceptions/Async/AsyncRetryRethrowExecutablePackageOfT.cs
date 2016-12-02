namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class AsyncRetryRethrowExecutablePackage<TResult> : AsyncRetryHandledPackage<TResult>
    {
        public AsyncRetryRethrowExecutablePackage(Func<Task<TResult>> invocationTarget, Func<Func<Task<TResult>>, bool, Task<TResult>> deferredFunc)
            : base (invocationTarget, deferredFunc, true)
        {
        }
    }
}