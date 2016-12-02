 
 
 
namespace FluentExceptions
{
    using System;

    public abstract class ActionHandlerPackageBase
    {
        protected ActionHandlerPackageBase(Action invocationTarget, Action<Action, Action<Exception>, bool> handlerBlock, bool rethrow)
        {
            this.InvocationTarget = invocationTarget;
            this.HandlerBlock = handlerBlock;
            this.Rethrow = rethrow;
        }

        protected bool Rethrow { get; private set; }

        protected Action<Action, Action<Exception>, bool> HandlerBlock { get; private set; }

        protected Action InvocationTarget { get; private set; }
    }
}