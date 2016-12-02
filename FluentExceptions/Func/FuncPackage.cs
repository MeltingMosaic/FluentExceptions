 
 
 
namespace FluentExceptions
{
    using System;

    public class FuncPackage<T>
    {
        public FuncPackage(Func<T> func)
        {
            this.InvocationTarget = func;
        }

        protected Func<T> InvocationTarget { get; private set; }

        public FuncSingleExceptionHandlerPackage<T, TException> MayThrow<TException>() where TException : Exception
        {
            return new FuncSingleExceptionHandlerPackage<T, TException>(this.InvocationTarget);
        }
    }
}