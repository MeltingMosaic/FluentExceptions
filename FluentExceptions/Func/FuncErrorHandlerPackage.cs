namespace FluentExceptions
{
    using System;

    public class FuncErrorHandlerPackage<T>
    {
        public FuncErrorHandlerPackage(Func<T> func)
        {
            this.InvocationTarget = func;
        }

        public Func<T> InvocationTarget { get; private set; }

        public T WithHandler(ErrorHandler handler)
        {
            object result = handler.FuncHandler(() => this.InvocationTarget());

            if (result == null)
            {
                return default(T);
            }
            else
            {
                return (T)result;
            }
        }
    }
}