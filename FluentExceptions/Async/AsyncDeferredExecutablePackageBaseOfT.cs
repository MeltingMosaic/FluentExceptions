 
 
 
namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public abstract class AsyncDeferredExecutablePackageBase<TResult>
    {
        protected AsyncDeferredExecutablePackageBase(
            Func<Task<TResult>> invocationTarget, 
            Func<Func<Task<TResult>>, bool, Task<TResult>> deferredFunc,
            bool rethrow)
        {
            this.InvocationTarget = invocationTarget;
            this.DeferredFunc = deferredFunc;
            this.Rethrow = rethrow;
        }

        protected bool Rethrow { get; private set; }

        protected Func<Task<TResult>> InvocationTarget { get; private set; }

        protected Func<Func<Task<TResult>>, bool, Task<TResult>> DeferredFunc { get; private set; }

        public Task<TResult> WhenExecuted()
        {
            return this.DeferredFunc(this.InvocationTarget, this.Rethrow);
        }

        public Func<Task<TResult>> WhenDeferred()
        {
            return () => this.DeferredFunc(this.InvocationTarget, this.Rethrow);
        }
    }
}