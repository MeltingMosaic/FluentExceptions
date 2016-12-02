 
 
 

namespace FluentExceptions
{
    using System;

    public static class ActionExtensions
    {
        public static ActionPackage Invoking<T>(this T subject, Action<T> action)
        {
            return new ActionPackage(() => action(subject));
        }

        public static ActionPackage Invoking(this Action action)
        {
            return new ActionPackage(action);
        }

        public static ActionErrorHandlerPackage Invoke<T>(this T subject, Action<T> action)
        {
            return new ActionErrorHandlerPackage(() => action(subject));
        }
    }
}
