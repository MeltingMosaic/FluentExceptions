 
 
 
namespace FluentExceptions
{
    using System;

    public abstract class FuncHandlerPackageBase<T, TException>
    {
        protected FuncHandlerPackageBase(Func<T> func, Func<Func<T>, Action<TException>, bool, T> handlerBlock, bool rethrow)
        {
            this.InvocationTarget = func;
            this.HandlerBlock = handlerBlock;
            this.Rethrow = rethrow;
        }

        protected Func<T> InvocationTarget { get; private set; }

        protected Func<Func<T>, Action<TException>, bool, T> HandlerBlock { get; private set; }

        protected bool Rethrow { get; private set; }
    }
}