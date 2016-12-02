 
 
 

namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public abstract class AsyncHandlerPackageBase
    {
        protected AsyncHandlerPackageBase(Func<Task> invocationTarget, bool canRetry, bool rethrow)
        {
            this.InvocationTarget = invocationTarget;
            this.CanRetry = canRetry;
            this.Rethrow = rethrow;
        }

        protected bool Rethrow { get; private set; }

        protected bool CanRetry { get; private set; }

        protected Func<Task> InvocationTarget { get; private set; }
    }
}