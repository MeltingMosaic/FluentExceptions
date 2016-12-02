 
 
 
namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class AsyncFinalExecutablePackage<TResult> : AsyncDeferredExecutablePackageBase<TResult>
    {
        public AsyncFinalExecutablePackage(
            Func<Task<TResult>> invocationTarget, 
            Func<Func<Task<TResult>>, bool, Task<TResult>> deferredFunc,
            bool rethrow) 
            : base(invocationTarget, deferredFunc, rethrow)
        {
        }
    }
}
