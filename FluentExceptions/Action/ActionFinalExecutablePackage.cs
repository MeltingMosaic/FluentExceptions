 
 
 
namespace FluentExceptions
{
    using System;

    public class ActionFinalExecutablePackage : ActionExecutablePackageBase
    {
        public ActionFinalExecutablePackage(Action invocationTarget, Action<Action, Action<Exception>, bool> handlerBlock, Action<Exception> handler, bool rethrow)
            : base(invocationTarget, handler, handlerBlock, rethrow)
        {
        }
    }
}