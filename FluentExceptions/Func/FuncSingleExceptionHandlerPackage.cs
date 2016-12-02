 
 
 

namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class FuncSingleExceptionHandlerPackage<T, TException> : FuncHandlerPackageBase<T, TException> where TException : Exception
    {
        public FuncSingleExceptionHandlerPackage(Func<T> funcToExecute)
            : base(funcToExecute, null, false)
        {
        }

        public FuncExecutablePackage<T> HandledBy(Action<TException> handler)
        {
            Func<Func<T>, Action<Exception>, bool, T> handlerBlock = (f, h, r) =>
            {
                T retVal = default(T);
                try
                {
                    retVal = f();
                }
                catch (TException e)
                {
                    handler(e);

                    if (r)
                    {
                        throw;
                    }
                }

                return retVal;
            };

            return new FuncExecutablePackage<T>(this.InvocationTarget, handlerBlock, e => { }, this.Rethrow);
        }
        
        public FuncHandlerPackage<T> Or<TException2>() where TException2 : Exception
        {
            var originalFunc = this.InvocationTarget;
            Func<Func<T>, Action<Exception>, bool, T> handlerBlock = (f, h, r) =>
            {
                T retVal = default(T);
                try
                {
                    retVal = originalFunc();
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

                return retVal;
            };

            return new FuncHandlerPackage<T>(this.InvocationTarget, handlerBlock);
        }

        public FuncRetryExecutablePackage<T> RetryingXTimes(int retryTimes)
        {
            return this.RetryingXTimesWithDelayFunction(retryTimes, r => 0);
        }

        public FuncRetryExecutablePackage<T> RetryingXTimesWithDelayFunction(int retryTimes, Func<int, int> retryDelayFunc)
        {
            if (retryTimes < 1)
            {
                throw new ArgumentOutOfRangeException("retryTimes");
            }

            var originalFunc = this.InvocationTarget;

            Func<Func<T>, Action<Exception>, bool, T> handlerBlock = (f, h, r) =>
            {
                int retryCount = 0;
                T retVal = default(T);
                while (retryCount < retryTimes)
                {
                    retryCount++;
                    try
                    {
                        retVal = originalFunc();
                    }
                    catch (TException)
                    {
                        Task.Delay(retryDelayFunc(retryCount)).Wait();
                        continue;
                    }

                    return retVal;
                }

                if (h != null)
                {
                    try
                    {
                        retVal = originalFunc();
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
                }
                else
                {
                    return originalFunc();
                }
            };

            return new FuncRetryExecutablePackage<T>(this.InvocationTarget, handlerBlock, null);
        }
    }
}