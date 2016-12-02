 
 
 
namespace FluentExceptions
{
    using System;

    public static class FuncExtensions
    {
        public static FuncPackage<TResult> Invoking<T, TResult>(this T subject, Func<T, TResult> func)
        {
            return new FuncPackage<TResult>(() => func(subject));
        }

        public static FuncPackage<TResult> Invoking<TResult>(this Func<TResult> func)
        {
            return new FuncPackage<TResult>(func);
        }

        public static FuncErrorHandlerPackage<TResult> Invoke<T, TResult>(this T subject, Func<T, TResult> func)
        {
            return new FuncErrorHandlerPackage<TResult>(() => func(subject));
        }
    }
}