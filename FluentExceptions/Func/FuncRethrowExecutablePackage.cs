namespace FluentExceptions
{
    using System;

    public class FuncRethrowExecutablePackage<T> : FuncExecutablePackageCore<T>
    {
        public FuncRethrowExecutablePackage(
            Func<T> invocationTarget, 
            Action<Exception> exceptionHandlerAction, 
            Func<Func<T>, Action<Exception>, bool, T> handlerBlock)
            : base (invocationTarget, exceptionHandlerAction, handlerBlock, true)
        {
        }
    }
}