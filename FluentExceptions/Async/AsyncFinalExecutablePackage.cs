 
 
 

namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class AsyncFinalExecutablePackage : AsyncDeferredExecutablePackageBase
    {
        public AsyncFinalExecutablePackage(Func<Task> deferredTask, Func<Func<Task>, bool, Task> finalTask, bool rethrow)
            : base(deferredTask, finalTask, canRetry:false, rethrow: rethrow)
        {
        }
    }
}