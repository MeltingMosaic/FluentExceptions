namespace FluentExceptions
{
    using System;

    public class ActionRethrowExecutablePackage : ActionExecutablePackageCore
    {
        public ActionRethrowExecutablePackage(Action invocationTarget, Action<Exception> handler, Action<Action, Action<Exception>, bool> handlerBlock)
            : base (invocationTarget, handler, handlerBlock, true)
        {
        }
    }
}