namespace FluentExceptions
{
    using System;
    using System.Threading.Tasks;

    public class TaskErrorHandlerPackage
    {
        public TaskErrorHandlerPackage(Func<Task<object>> func)
        {
            this.InvocationTarget = func;
        }

        protected Func<Task<object>> InvocationTarget { get; private set; }

        public Task WithHandler(ErrorHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            if (handler.TaskHandler == null)
            {
                throw new InvalidOperationException("This handler is not configured to handle Tasks");
            }

            return handler.TaskHandler(this.InvocationTarget);
        }
    }

    public class TaskErrorHandlerPackage<T>
    {
        public TaskErrorHandlerPackage(Func<Task<T>> func)
        {
            this.InvocationTarget = func;
        }

        protected Func<Task<T>> InvocationTarget { get; private set; }

        public async Task<T> WithHandler(ErrorHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            if (handler.TaskHandler == null)
            {
                throw new InvalidOperationException("This handler is not configured to handle Tasks");
            }

            Func<Task<object>> untypedFunc = async () => await this.InvocationTarget(); 
            
            var taskHandler = handler.TaskHandler(untypedFunc);

            await taskHandler;

            // Need special handling for value types
            if (typeof (T).IsValueType)
            {
                return taskHandler.Result == null ? default(T) : (T)taskHandler.Result;
            }
            else
            {
                return (T)taskHandler.Result;
            }
        }
    }
}