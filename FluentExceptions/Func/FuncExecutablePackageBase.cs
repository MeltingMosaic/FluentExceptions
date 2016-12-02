 
 
 
namespace FluentExceptions
{
    using System;

    public abstract class FuncExecutablePackageBase<T> : FuncHandlerPackageBase<T, Exception>
    {
        protected FuncExecutablePackageBase(Func<T> invocationTarget, Func<Func<T>, Action<Exception>, bool, T> exceptionWrapper, Action<Exception> handler, bool rethrow)
            : base(invocationTarget, exceptionWrapper, rethrow)
        {
            this.Handler = handler;
        }

        public Func<T> Function
        {
            get
            {
                return () => this.HandlerBlock(this.InvocationTarget, this.Handler, this.Rethrow);
            }
        }

        protected Action<Exception> Handler { get; private set; }

        public T WhenExecuted()
        {
            return this.Function();
        }
    }
}