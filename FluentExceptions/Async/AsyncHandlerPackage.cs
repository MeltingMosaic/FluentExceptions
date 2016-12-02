 
 
 
namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class AsyncHandlerPackage : AsyncHandlerPackageBase
    {
        protected Func<Task, Action<Exception>, bool, Task> HandlerFunc { get; private set; }

        public AsyncHandlerPackage(
            Func<Task> invocationTarget, 
            Func<Task, Action<Exception>, bool, Task> handlerFunc,
            bool canRetry,
            bool rethrow) : base (invocationTarget, canRetry, rethrow)
        {
            this.HandlerFunc = handlerFunc;
        }

        public AsyncExecutablePackage HandledBy(Action<Exception> exceptionHandler)
        {
            return new AsyncExecutablePackage(this.InvocationTarget, exceptionHandler, this.HandlerFunc, this.CanRetry, this.Rethrow);
        }

        public AsyncHandlerPackage Or<TException>() where TException : Exception
        {
            Func<Task, Action<Exception>, bool, Task> newHandler = async(t, h, r) =>
            {
                try
                {
                    await t;
                }
                catch (TException e)
                {
                    h(e);

                    if (r)
                    {
                        throw;
                    }
                }
            };

            return new AsyncHandlerPackage(this.InvocationTarget, newHandler, this.CanRetry, this.Rethrow);
        }

        public AsyncRetryExecutablePackage RetryingXTimes(int times)
        {
            return this.RetryingXTimesWithDelayFunction(times, r => 0);
        }

        public AsyncRetryExecutablePackage RetryingXTimesWithDelayFunction(int times, Func<int, int> retryDelayFunc)
        {
            if (this.CanRetry == false)
            {
                throw new InvalidOperationException("You cannot use Retry operations when executing a Task. To use Retry, use Func<Task>");
            }

            var currentHandler = this.HandlerFunc;
            Func<Func<Task>, Action<Exception>, bool, Task> newHandler = async (f, h, r) =>
            {
                int retryTimes = 0;
                bool handlerCalled = false;
                while (retryTimes < times)
                {
                    handlerCalled = false;

                    await currentHandler(f(), e => { handlerCalled = true; }, false);

                    if (!handlerCalled)
                    {
                        // if no handler was called, then the call was successful
                        return;
                    }

                    await Task.Delay(retryDelayFunc(retryTimes));
                    retryTimes++;
                }

                if (h != null)
                {
                    await currentHandler(f(), h, r);
                }
                else
                {
                    await f();
                }
            };
            return new AsyncRetryExecutablePackage(this.InvocationTarget, newHandler, this.CanRetry, this.Rethrow);
        }
    }
}