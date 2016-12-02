 
 
 
namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class ActionHandlerPackage : ActionHandlerPackageBase
    {
        public ActionHandlerPackage(Action invocationTarget, Action<Action, Action<Exception>, bool> handlerBlock) 
            : base(invocationTarget, handlerBlock, false)
        {
        }

        public ActionExecutablePackage HandledBy(Action<Exception> handler)
        {
            return new ActionExecutablePackage(this.InvocationTarget, this.HandlerBlock, handler);
        }

        public ActionHandlerPackage Or<TException>() where TException : Exception
        {
            var currentHandlerBlock = this.HandlerBlock;

            Action<Action, Action<Exception>, bool> newHandlerBlock = (a, h, r) =>
            {
                try
                {
                    currentHandlerBlock(a, h, r);
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

            return new ActionHandlerPackage(this.InvocationTarget, newHandlerBlock);
        }

        public ActionRetryExecutablePackage RetryingXTimes(int times)
        {
            return this.RetryingXTimesWithDelayFunction(times, r => 0);
        }

        public ActionRetryExecutablePackage RetryingXTimesWithDelayFunction(int times, Func<int, int> retryDelayFunc)
        {
            var currentHandlerBlock = this.HandlerBlock;

            Action<Action, Action<Exception>, bool> newHandlerBlock = (a, h, r) =>
            {
                int retryTimes = 0;
                bool handlerCalled = false;
                while (retryTimes < times)
                {
                    handlerCalled = false;

                    currentHandlerBlock(a, e => { handlerCalled = true; }, false);

                    if (!handlerCalled)
                    {
                        // if no handler was called, then the call was successful
                        return;
                    }

                    Task.Delay(retryDelayFunc(retryTimes)).Wait();
                    retryTimes++;
                }

                if (h != null)
                {
                    currentHandlerBlock(a, h, r);
                }
                else
                {
                    a();
                }
            };

            return new ActionRetryExecutablePackage(this.InvocationTarget, newHandlerBlock, null);
        }
    }
}