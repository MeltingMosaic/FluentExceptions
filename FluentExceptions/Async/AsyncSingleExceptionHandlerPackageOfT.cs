 
 
 
namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class AsyncSingleExceptionHandlerPackage<TResult, TException> where TException : Exception
    {
        public AsyncSingleExceptionHandlerPackage(Func<Task<TResult>> invocationTarget, bool canRetry)
        {
            this.InvocationTarget = invocationTarget;
            this.CanRetry = canRetry;
        }

        protected bool CanRetry { get; private set; }

        protected Func<Task<TResult>> InvocationTarget { get; private set; }

        public AsyncExecutablePackage<TResult> HandledBy(Action<TException> handler)
        {
            Func<Task<TResult>, Action<Exception>, bool, Task<TResult>> handlerFunc = async (f, h, r) =>
            {
                TResult retVal = default(TResult);
                try
                {
                    retVal = await f;
                }
                catch (TException ex)
                {
                    handler(ex);

                    if (r)
                    {
                        throw;
                    }
                }

                return retVal;
            };

            return new AsyncExecutablePackage<TResult>(this.InvocationTarget, null, handlerFunc, this.CanRetry, rethrow: false);
        }

        public AsyncHandlerPackage<TResult> Or<TException2>() where TException2 : Exception
        {
            Func<Task<TResult>, Action<Exception>, bool, Task<TResult>> handlerFunc = async (f, h, r) =>
            {
                TResult retVal = default(TResult);
                try
                {
                    retVal = await f;
                }
                catch (TException ex)
                {
                    h(ex);

                    if (r)
                    {
                        throw;
                    }
                }
                catch (TException2 ex)
                {
                    h(ex);

                    if (r)
                    {
                        throw;
                    }
                }

                return retVal;
            };

            return new AsyncHandlerPackage<TResult>(this.InvocationTarget, handlerFunc, this.CanRetry, rethrow: false);
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

            Func<Func<Task<TResult>>, Action<Exception>, bool, Task<TResult>> newHandler = async (f, h, r) =>
            {
                TResult retVal = default(TResult);
                int retryTimes = 0;

                while (retryTimes < times)
                {
                    bool handlerCalled = false;

                    try
                    {
                        retVal = await f();
                    }
                    catch (TException)
                    {
                        handlerCalled = true;
                    }

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
                    try
                    {
                        retVal = await f();
                    }
                    catch (TException ex)
                    {
                        h(ex);

                        if (r)
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    retVal = await f();
                }

                return retVal;
            };

            return new AsyncRetryExecutablePackage<TResult>(this.InvocationTarget, newHandler, rethrow: false);
        }
    }
}
