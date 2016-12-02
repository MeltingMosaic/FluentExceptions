 
 
 
namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class AsyncRetryHandledPackage : AsyncRetryHandledPackageBase
    {
        public AsyncRetryHandledPackage(
            Func<Task> invocationTarget, 
            Func<Func<Task>, bool, Task> deferredFunc,
            bool canRetry,
            bool rethrow) : base(invocationTarget, deferredFunc, canRetry, rethrow)
        {
        }

        public AsyncRetryRethrowExecutablePackage AndRethrows()
        {
            return new AsyncRetryRethrowExecutablePackage(this.InvocationTarget, this.DeferredFunc, this.CanRetry);
        }
    }
}