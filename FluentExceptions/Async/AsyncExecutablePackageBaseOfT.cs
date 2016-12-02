namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class AsyncExecutablePackageBase<TResult>
    {
        public AsyncExecutablePackageBase(
            Func<Task<TResult>> invocationTarget,
            Action<Exception> exceptionHandlerAction,
            Func<Task<TResult>, Action<Exception>, bool, Task<TResult>> handlerFunc,
            bool canRetry,
            bool rethrow)
        {
            this.InvocationTarget = invocationTarget;
            this.ExceptionHandlerAction = exceptionHandlerAction;
            this.HandlerFunc = handlerFunc;
            this.CanRetry = canRetry;
            this.Rethrow = rethrow;
        }

        protected bool Rethrow { get; private set; }

        protected bool CanRetry { get; private set; }
        protected Func<Task<TResult>> InvocationTarget { get; private set; }
        protected Func<Task<TResult>, Action<Exception>, bool, Task<TResult>> HandlerFunc { get; private set; }
        protected Action<Exception> ExceptionHandlerAction { get; private set; }

        public Task<TResult> WhenExecuted()
        {
            return this.HandlerFunc(this.InvocationTarget(), this.ExceptionHandlerAction, this.Rethrow);
        }

        public Func<Task<TResult>> WhenDeferred()
        {
            return () => this.HandlerFunc(this.InvocationTarget(), this.ExceptionHandlerAction, this.Rethrow);
        }

        public AsyncFinalExecutablePackage<TResult> AndFinally(Action finallyAction)
        {
            Func<Func<Task<TResult>>, bool, Task<TResult>> finalFunc = async (f, r) =>
            {
                TResult retVal = default(TResult);
                try
                {
                    retVal = await f();
                }
                finally
                {
                    finallyAction();
                }

                return retVal;
            };

            return new AsyncFinalExecutablePackage<TResult>(this.WhenDeferred(), finalFunc, this.Rethrow);
        }

        public AsyncPackage<TResult> OrIt()
        {
            return new AsyncPackage<TResult>(this.WhenDeferred(), this.CanRetry);
        }
    }
}