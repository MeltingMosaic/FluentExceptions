namespace FluentExceptions
{
    using System;

    public class FuncExecutablePackageCore<T> : FuncExecutablePackageBase<T>
    {
        public FuncExecutablePackageCore(Func<T> invocationTarget, Action<Exception> handler, Func<Func<T>, Action<Exception>, bool, T> exceptionWrapper, bool rethrow)
            : base(invocationTarget, exceptionWrapper, handler, rethrow)
        {
        }

        public FuncFinalExecutablePackage<T> AndFinally(Action finallyAction)
        {
            var handlerBlock = this.HandlerBlock;
            Func<Func<T>, Action<Exception>, bool, T> exceptionWrapper = (f, h, r) =>
            {
                T retVal = default(T);

                try
                {
                    retVal = handlerBlock(f, h, r);
                }
                finally
                {
                    finallyAction();
                }

                return retVal;
            };
            return new FuncFinalExecutablePackage<T>(this.InvocationTarget, exceptionWrapper, this.Handler, this.Rethrow);
        }

        public FuncPackage<T> OrIt()
        {
            return new FuncPackage<T>(this.Function);
        }
    }
}