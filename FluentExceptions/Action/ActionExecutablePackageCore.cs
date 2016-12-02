namespace FluentExceptions
{
    using System;

    public class ActionExecutablePackageCore : ActionExecutablePackageBase
    {
        public ActionExecutablePackageCore(Action invocationTarget, Action<Exception> handler, Action<Action, Action<Exception>, bool> handlerBlock, bool rethrow)
            : base(invocationTarget, handler, handlerBlock, rethrow)
        {
        }

        public ActionFinalExecutablePackage AndFinally(Action finallyAction)
        {
            var currentHandlerBlock = this.HandlerBlock;

            Action<Action, Action<Exception>, bool> newHandlerBlock = (a, h, r) =>
            {
                try
                {
                    currentHandlerBlock(a, h, r);
                }
                finally
                {
                    finallyAction();
                }
            };

            return new ActionFinalExecutablePackage(this.InvocationTarget, newHandlerBlock, this.Handler, this.Rethrow);
        }

        public ActionPackage OrIt()
        {
            return new ActionPackage(this.WhenDeferred());
        }
    }
}