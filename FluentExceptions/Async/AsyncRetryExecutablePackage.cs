 
 
 
namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class AsyncRetryExecutablePackage
    {
        public AsyncRetryExecutablePackage(Func<Task> invocationTarget, Func<Func<Task>, Action<Exception>, bool, Task> handlerFunc, bool canRetry, bool rethrow)
        {
            this.InvocationTarget = invocationTarget;
            this.HandlerFunc = handlerFunc;
            this.Rethrow = rethrow;
            this.CanRetry = canRetry;
        }

        protected bool CanRetry { get; private set; }

        protected bool Rethrow { get; private set; }

        protected Func<Func<Task>, Action<Exception>, bool, Task> HandlerFunc { get; private set; }

        protected Func<Task> InvocationTarget { get; private set; }

        public Task WhenExecuted()
        {
            return this.HandlerFunc(this.InvocationTarget, null, this.Rethrow);
        }

        public Func<Task> WhenDeferred()
        {
            return this.WhenExecuted;
        }

        public AsyncRetryHandledPackage ThenHandledBy(Action<Exception> handlerAction)
        {
            var currentHandler = this.HandlerFunc;
            Func<Func<Task>, bool, Task> newHandler = async (f, r) =>
            {
                await currentHandler(f, handlerAction, r);
            };

            return new AsyncRetryHandledPackage(this.InvocationTarget, newHandler, this.CanRetry, this.Rethrow);
        }

        public AsyncPackage OrIt()
        {
            return new AsyncPackage(this.WhenDeferred(), canRetry: true);
        }
    }
}