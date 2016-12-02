 
 
 
namespace FluentExceptions
{
    using System;

    public abstract class ActionExecutablePackageBase : ActionHandlerPackageBase
    {
        protected ActionExecutablePackageBase(Action invocationTarget, Action<Exception> handler, Action<Action, Action<Exception>, bool> handlerBlock, bool rethrow)
            : base(invocationTarget, handlerBlock, rethrow)
        {
            this.Handler = handler;
        }
        protected Action<Exception> Handler { get; private set; }

        public Action WhenDeferred()
        {
            return () => this.HandlerBlock(this.InvocationTarget, this.Handler, this.Rethrow);
        }

        public void WhenExecuted()
        {
            this.WhenDeferred()();
        }
    }
}