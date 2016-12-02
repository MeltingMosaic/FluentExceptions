 
 
 
namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public abstract class AsyncDeferredExecutablePackageBase
    {
        protected AsyncDeferredExecutablePackageBase(
            Func<Task> invocationTarget, 
            Func<Func<Task>, bool, Task> deferredFunc,
            bool canRetry,
            bool rethrow)
        {
            this.InvocationTarget = invocationTarget;
            this.DeferredFunc = deferredFunc;
            this.CanRetry = canRetry;
            this.Rethrow = rethrow;
        }

        protected Func<Task> InvocationTarget { get; set; }

        protected Func<Func<Task>, bool, Task> DeferredFunc { get; private set; }

        protected bool Rethrow { get; private set; }

        protected bool CanRetry { get; private set; }

        public Task WhenExecuted()
        {
            return this.DeferredFunc(this.InvocationTarget, this.Rethrow);
        }

        public Func<Task> WhenDeferred()
        {
            return () => this.DeferredFunc(this.InvocationTarget, this.Rethrow);
        }
    }
}