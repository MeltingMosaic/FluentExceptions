 
 
 

namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class FuncHandlerPackage<T> : FuncHandlerPackageBase<T, Exception>
    {
        public FuncHandlerPackage(Func<T> func, Func<Func<T>, Action<Exception>, bool, T> handlerBlock) : base(func, handlerBlock, false)
        {
        }

        public FuncExecutablePackage<T> HandledBy(Action<Exception> handler)
        {
            return new FuncExecutablePackage<T>(this.InvocationTarget, this.HandlerBlock, handler, this.Rethrow);
        }

        public FuncHandlerPackage<T> Or<TException>() where TException : Exception
        {
            var currentWrapper = this.HandlerBlock;
            Func<Func<T>, Action<Exception>, bool, T> handlerBlock = (f, h, r) =>
            {
                T retVal = default(T);
                try
                {
                    retVal = currentWrapper(f, h, r);
                }
                catch (TException e)
                {
                    h(e);

                    if (r)
                    {
                        throw;
                    }
                }

                return retVal;
            };

            return new FuncHandlerPackage<T>(this.InvocationTarget, handlerBlock);
        }

        public FuncRetryExecutablePackage<T> RetryingXTimes(int times)
        {
            return this.RetryingXTimesWithDelayFunction(times, r => 0);
        }

        public FuncRetryExecutablePackage<T> RetryingXTimesWithDelayFunction(int times, Func<int, int> retryDelayFunc)
        {
            var currentHandlerBlock = this.HandlerBlock;
            var originalFunc = this.InvocationTarget;
            Func<Func<T>, Action<Exception>, bool, T> newHandlerBlock = (f, h, r) =>
            {
                T retVal = default(T);
                bool handlerCalled = false;
                int retryCount = 0;
                while (retryCount < times)
                {
                    handlerCalled = false;
                    retVal = currentHandlerBlock(f, e => { handlerCalled = true; }, false);

                    if (!handlerCalled)
                    {
                        // If the handler lambda was never called, the function succeeded.
                        break;
                    }

                    Task.Delay(retryDelayFunc(retryCount)).Wait();
                    retryCount++;
                }

                if (retryCount == times)
                {
                    // Call one more time
                    if (h != null)
                    {
                        currentHandlerBlock(f, h, r);
                    }
                    else
                    {
                        originalFunc();
                    }
                }

                return retVal;
            };

            return new FuncRetryExecutablePackage<T>(this.InvocationTarget, newHandlerBlock, null);
        }
    }
}