 
 
 

namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public abstract class AsyncExecutablePackageBase
    {
        protected AsyncExecutablePackageBase(
            Func<Task> invocationTarget, 
            Action<Exception> exceptionHandlerAction, 
            Func<Task, Action<Exception>, bool, Task> handlerFunc,
            bool canRetry,
            bool rethrow)
        {
            this.InvocationTarget = invocationTarget;
            this.ExceptionHandlerAction = exceptionHandlerAction;
            this.HandlerFunc = handlerFunc;
            this.CanRetry = canRetry;
            this.Rethrow = rethrow;
        }

        protected bool CanRetry { get; private set; }

        protected bool Rethrow { get; private set; }

        protected Func<Task> InvocationTarget { get; private set; }

        protected Action<Exception> ExceptionHandlerAction { get; private set; }

        protected Func<Task, Action<Exception>, bool, Task> HandlerFunc { get; private set; }

        public Task WhenExecuted()
        {
            return this.HandlerFunc(this.InvocationTarget(), this.ExceptionHandlerAction, this.Rethrow);
        }

        public Func<Task> WhenDeferred()
        {
            return () => this.HandlerFunc(this.InvocationTarget(), this.ExceptionHandlerAction, this.Rethrow);
        }
    }
}