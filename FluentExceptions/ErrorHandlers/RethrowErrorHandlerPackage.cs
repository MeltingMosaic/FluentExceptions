namespace FluentExceptions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class RethrowErrorHandlerPackage
    {
        public IList<Type> HandledTypes { get; private set; }

        public RethrowErrorHandlerPackage(
            IList<Type> handledTypes, 
            Action<Action> actionHandler, 
            Func<Func<object>, object> funcHandler, 
            Func<Func<Task<object>>, Task<object>> taskHandler)
        {
            this.HandledTypes = handledTypes;
            this.ActionHandler = actionHandler;
            this.FuncHandler = funcHandler;
            this.TaskHandler = taskHandler;
        }

        protected Action<Action> ActionHandler { get; private set; }
        protected Func<Func<object>, object> FuncHandler { get; private set; }
        protected Func<Func<Task<object>>, Task<object>> TaskHandler { get; private set; }

        public ErrorHandler AsErrorHandler()
        {
            return new ErrorHandler(this.HandledTypes, this.ActionHandler, this.FuncHandler, this.TaskHandler);
        }

        public FinalErrorHandlerPackage AndFinally(Action finallyAction)
        {
            Action<Action> newActionHandler = a =>
            {
                try
                {
                    a();
                }
                finally
                {
                    finallyAction();
                }
            };

            Func<Func<object>, object> newFuncHandler = f =>
            {
                try
                {
                    return f();
                }
                finally
                {
                    finallyAction();
                }
            };

            Func<Func<Task<object>>, Task<object>> newTaskHandler = async t =>
            {
                try
                {
                    return await t();
                }
                finally
                {
                    finallyAction();
                }
            };

            return new FinalErrorHandlerPackage(this.HandledTypes, newActionHandler, newFuncHandler, newTaskHandler);
        }
    }
}