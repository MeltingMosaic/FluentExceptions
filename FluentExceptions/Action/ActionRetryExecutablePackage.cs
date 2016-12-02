 
 
 

namespace FluentExceptions
{
    using System;

    public class ActionRetryExecutablePackage : ActionExecutablePackageCore
    {
        public ActionRetryExecutablePackage(Action invocationTarget, Action<Action, Action<Exception>, bool> handlerBlock, Action<Exception> handler)
            : base(invocationTarget, handler, handlerBlock, false)
        {
        }

        public ActionExecutablePackage ThenHandledBy(Action<Exception> handler)
        {
            return new ActionExecutablePackage(this.InvocationTarget, this.HandlerBlock, handler);
        }
    }
}