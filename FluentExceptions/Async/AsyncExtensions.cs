 
 
 

namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public static class AsyncExtensions
    {
        public static AsyncPackage Awaiting<T>(this T subject, Func<T, Task> func)
        {
            return new AsyncPackage(() => func(subject), canRetry: true);
        }

        public static AsyncPackage Awaiting(this Task task)
        {
            return new AsyncPackage(() => task, canRetry: false);
        }

        public static AsyncPackage Awaiting(this Func<Task> func)
        {
            return new AsyncPackage(func, canRetry: true);
        }

        public static TaskErrorHandlerPackage GetAwaitable<T>(this T subject, Func<T, Task> func)
        {
            Func<Task<object>> typedTask = async () =>
            {
                await func(subject);
                return new object();
            };

            return new TaskErrorHandlerPackage(typedTask);
        }

        public static TaskErrorHandlerPackage<TReturn> GetAwaitable<T, TReturn>(this T subject, Func<T, Task<TReturn>> func)
        {
            return new TaskErrorHandlerPackage<TReturn>(() => func(subject));   
        }

        public static AsyncPackage<TResult> Awaiting<T, TResult>(this T subject, Func<T, Task<TResult>> func)
        {
            return new AsyncPackage<TResult>(() => func(subject), canRetry: true);
        }

        public static AsyncPackage<TResult> Awaiting<TResult>(this Task<TResult> task)
        {
            return new AsyncPackage<TResult>(() => task, canRetry: false);
        }

        public static AsyncPackage<TResult> Awaiting<TResult>(this Func<Task<TResult>> func)
        {
            return new AsyncPackage<TResult>(func, canRetry: true);
        }

        //public static TaskErrorHandlerPackage<T> GetAwaitable<T>(this T subject, Func<T, Task<T>> func)
        //{
        //    return new TaskErrorHandlerPackage(() => func(subject));
        //}
    }
}
