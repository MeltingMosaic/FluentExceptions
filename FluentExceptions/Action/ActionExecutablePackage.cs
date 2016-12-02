 
 
 
namespace FluentExceptions
{
    using System;

    public class ActionExecutablePackage : ActionExecutablePackageCore
    {
        public ActionExecutablePackage(Action invocationTarget, Action<Action, Action<Exception>, bool> handlerBlock, Action<Exception> handler)
            : base(invocationTarget, handler, handlerBlock, false)
        {
        }

        public ActionRethrowExecutablePackage AndRethrows()
        {
            return new ActionRethrowExecutablePackage(this.InvocationTarget, this.Handler, this.HandlerBlock);
        }
    }
}