 
 
 
namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class ActionSingleExceptionHandlerPackage<TException> where TException : Exception
    {
        public ActionSingleExceptionHandlerPackage(Action invocationTarget)
        {
            this.InvocationTarget = invocationTarget;
        }

        protected Action InvocationTarget { get; private set; }

        public ActionExecutablePackage HandledBy(Action<TException> handler)
        {
            Action<Action, Action<Exception>, bool> handlerBlock = (a, h, r) =>
            {
                try
                {
                    a();
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

            return new ActionExecutablePackage(this.InvocationTarget, handlerBlock, e => { });
        }

        public ActionHandlerPackage Or<TException2>() where TException2 : Exception
        {
            Action<Action, Action<Exception>, bool> handlerBlock = (a, h, r) =>
            {
                try
                {
                    a();
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

            return new ActionHandlerPackage(this.InvocationTarget, handlerBlock);
        }

        public ActionRetryExecutablePackage RetryingXTimes(int times)
        {
            return this.RetryingXTimesWithDelayFunction(times, r => 0);
        }

        public ActionRetryExecutablePackage RetryingXTimesWithDelayFunction(int times, Func<int, int> retryDelayFunc)
        {
            Action<Action, Action<Exception>, bool> handlerBlock = (a, h, r) =>
            {
                int retryTimes = 0;

                while (retryTimes < times)
                {
                    try
                    {
                        a();

                        // On success, return
                        return;
                    }
                    catch (TException)
                    {
                        // Do Nothing
                    }

                    Task.Delay(retryDelayFunc(retryTimes)).Wait();

                    retryTimes++;
                }

                // Try one more time
                if (h != null)
                {
                    try
                    {
                        a();
                    }
                    catch (TException e)
                    {
                        h(e);

                        if (r)
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    a();
                }
            };

            return new ActionRetryExecutablePackage(this.InvocationTarget, handlerBlock, null);
        }
    }
}