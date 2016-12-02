 
 
 
namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class AsyncRetryExecutablePackage<TResult>
    {
        public AsyncRetryExecutablePackage(
            Func<Task<TResult>> invocationTarget,
            Func<Func<Task<TResult>>, Action<Exception>, bool, Task<TResult>> handlerFunc,
            bool rethrow)
        {
            this.InvocationTarget = invocationTarget;
            this.HandlerFunc = handlerFunc;
            this.Rethrow = rethrow;
        }

        protected bool Rethrow { get; private set; }

        protected Func<Task<TResult>> InvocationTarget { get; private set; }

        protected Func<Func<Task<TResult>>, Action<Exception>, bool, Task<TResult>> HandlerFunc { get; private set; }

        public Task<TResult> WhenExecuted()
        {
            return this.HandlerFunc(this.InvocationTarget, null, this.Rethrow);
        }

        public Func<Task<TResult>> WhenDeferred()
        {
            return this.WhenExecuted;
        }

        public AsyncRetryHandledPackage<TResult> ThenHandledBy(Action<Exception> handlerAction)
        {
            var currentHandler = this.HandlerFunc;
            Func<Func<Task<TResult>>, bool, Task<TResult>> newHandler = async (f, r) => await currentHandler(f, handlerAction, r);

            return new AsyncRetryHandledPackage<TResult>(this.InvocationTarget, newHandler, this.Rethrow);
        }

        public AsyncPackage<TResult> OrIt()
        {
            return new AsyncPackage<TResult>(this.WhenDeferred(), canRetry: true);
        }
    }
}