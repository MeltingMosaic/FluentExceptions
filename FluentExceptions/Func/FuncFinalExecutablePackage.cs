 
 
 
namespace FluentExceptions
{
    using System;

    public class FuncFinalExecutablePackage<T> : FuncExecutablePackageBase<T>
    {
        public FuncFinalExecutablePackage(
            Func<T> invocationTarget, 
            Func<Func<T>, Action<Exception>, bool, T> exceptionWrapper, 
            Action<Exception> handler, bool rethrow)
            : base(invocationTarget, exceptionWrapper, handler, rethrow)
        {
        }
    }
}