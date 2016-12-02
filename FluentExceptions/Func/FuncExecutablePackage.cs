 
 
 
namespace FluentExceptions
{
    using System;

    public class FuncExecutablePackage<T> : FuncExecutablePackageCore<T>
    {
        public FuncExecutablePackage(Func<T> invocationTarget, Func<Func<T>, Action<Exception>, bool, T> exceptionWrapper, Action<Exception> handler, bool rethrow)
            : base(invocationTarget, handler, exceptionWrapper, rethrow)
        {
        }

        public FuncRethrowExecutablePackage<T> AndRethrows()
        {
            return new FuncRethrowExecutablePackage<T>(this.InvocationTarget, this.Handler, this.HandlerBlock);
        }
    }
}