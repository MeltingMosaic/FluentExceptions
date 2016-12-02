 
 
 
namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class AsyncRetryHandledPackage<TResult> : AsyncRetryHandledPackageBase<TResult>
    {
        public AsyncRetryHandledPackage(
            Func<Task<TResult>> invocationTarget, 
            Func<Func<Task<TResult>>, bool, Task<TResult>> deferredFunc,
            bool rethrow) 
            : base(invocationTarget, deferredFunc, rethrow)
        {
        }

        public AsyncRetryRethrowExecutablePackage<TResult> AndRethrows()
        {
            return new AsyncRetryRethrowExecutablePackage<TResult>(this.InvocationTarget, this.DeferredFunc);
        }
    }
}