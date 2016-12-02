 
 
 
namespace FluentExceptions
{
    using System;

    public class FuncRetryExecutablePackage<T> : FuncExecutablePackage<T>
    {
        public FuncRetryExecutablePackage(Func<T> invocationTarget, Func<Func<T>, Action<Exception>, bool, T> handlerBlock, Action<Exception> handler)
            : base(invocationTarget, handlerBlock, handler, false)
        {
        }

        public FuncExecutablePackage<T> ThenHandledBy(Action<Exception> action)
        {
            return new FuncExecutablePackage<T>(this.InvocationTarget, this.HandlerBlock, action, this.Rethrow);
        }
    }
}