namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class AsyncRetryRethrowExecutablePackage : AsyncRetryHandledPackageBase
    {
        public AsyncRetryRethrowExecutablePackage(Func<Task> invocationTarget, Func<Func<Task>, bool, Task> deferredFunc, bool canRetry)
            : base (invocationTarget, deferredFunc, canRetry, rethrow: true)
        {
        }
    }
}