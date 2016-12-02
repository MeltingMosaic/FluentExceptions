 
 
 

namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class AsyncSingleExceptionHandlerPackage<TException> : AsyncHandlerPackageBase where TException : Exception
    {
        public AsyncSingleExceptionHandlerPackage(Func<Task> invocationTarget, bool canRetry)
            : base(invocationTarget, canRetry, false)
        {
        }

        public AsyncExecutablePackage HandledBy(Action<TException> handler)
        {
            Func<Task, Action<Exception>, bool, Task> handlerFunc = async (t, h, r) =>
            {
                try
                {
                    await t;
                }
                catch (TException e)
                {
                    handler(e);

                    if (r)
                    {
                        throw;
                    }
                }
            };

            return new AsyncExecutablePackage(this.InvocationTarget, null, handlerFunc, canRetry: this.CanRetry, rethrow: false);
        }

        public AsyncHandlerPackage Or<TException2>() where TException2 : Exception
        {
            Func<Task, Action<Exception>, bool, Task> newHandler = async (t, h, r) =>
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
                catch (TException2 e)
                {
                    h(e);

                    if (r)
                    {
                        throw;
                    }
                }
            };

            return new AsyncHandlerPackage(this.InvocationTarget, newHandler, canRetry: this.CanRetry, rethrow: false);
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

            Func<Func<Task>, Action<Exception>, bool, Task> newHandler = async (f, h, r) =>
            {
                int retryTimes = 0;

                while (retryTimes < times)
                {
                    bool handlerCalled = false;

                    try
                    {
                        await f();
                    }
                    catch (TException)
                    {
                        handlerCalled = true;
                    }

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
                    try
                    {
                        await f();
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
                    await f();
                }
            };
            return new AsyncRetryExecutablePackage(this.InvocationTarget, newHandler, this.CanRetry, this.Rethrow);
        }
    }
}