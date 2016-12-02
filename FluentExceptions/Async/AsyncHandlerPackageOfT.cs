 
 
 
namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class AsyncHandlerPackage<TResult>
    {
        public AsyncHandlerPackage(
            Func<Task<TResult>> invocationTarget, 
            Func<Task<TResult>, Action<Exception>, bool, Task<TResult>> handlerFunc, 
            bool canRetry,
            bool rethrow)
        {
            this.InvocationTarget = invocationTarget;
            this.HandlerFunc = handlerFunc;
            this.CanRetry = canRetry;
            this.Rethrow = rethrow;
        }

        protected bool Rethrow { get; private set; }

        protected bool CanRetry { get; private set; }

        protected Func<Task<TResult>> InvocationTarget { get; private set; }

        protected Func<Task<TResult>, Action<Exception>, bool, Task<TResult>> HandlerFunc { get; private set; }

        public AsyncExecutablePackage<TResult> HandledBy(Action<Exception> handlerAction)
        {
            return new AsyncExecutablePackage<TResult>(this.InvocationTarget, handlerAction, this.HandlerFunc, this.CanRetry, this.Rethrow);
        }

        public AsyncHandlerPackage<TResult> Or<TException>() where TException : Exception
        {
            var originalHandler = this.HandlerFunc;

            Func<Task<TResult>, Action<Exception>, bool, Task<TResult>> newHandler = async (f, h, r) =>
            {
                TResult retVal = default(TResult);

                try
                {
                    retVal = await originalHandler(f, h, r);
                }
                catch (TException ex)
                {
                    h(ex);

                    if (r)
                    {
                        throw;
                    }
                }

                return retVal;
            };

            return new AsyncHandlerPackage<TResult>(this.InvocationTarget, newHandler, this.CanRetry, this.Rethrow);
        }

        public AsyncRetryExecutablePackage<TResult> RetryingXTimes(int times)
        {
            return this.RetryingXTimesWithDelayFunction(times, r => 0);
        }

        public AsyncRetryExecutablePackage<TResult> RetryingXTimesWithDelayFunction(int times, Func<int, int> retryDelayFunc)
        {
            if (this.CanRetry == false)
            {
                throw new InvalidOperationException("You cannot use Retry operations when executing a Task. To use Retry, use Func<Task>");
            }

            var currentHandler = this.HandlerFunc;
            Func<Func<Task<TResult>>, Action<Exception>, bool, Task<TResult>> newHandler = async (f, h, r) =>
            {
                TResult retVal = default(TResult);
                int retryTimes = 0;
                bool handlerCalled = false;
                while (retryTimes < times)
                {
                    handlerCalled = false;

                    retVal = await currentHandler(f(), e => { handlerCalled = true; }, false);

                    if (!handlerCalled)
                    {
                        // if no handler was called, then the call was successful
                        return retVal;
                    }

                    await Task.Delay(retryDelayFunc(retryTimes));

                    retryTimes++;
                }

                if (h != null)
                {
                    retVal = await currentHandler(f(), h, r);
                }
                else
                {
                    retVal = await f();
                }

                return retVal;
            };
            return new AsyncRetryExecutablePackage<TResult>(this.InvocationTarget, newHandler, this.Rethrow);
        }
    }
}
