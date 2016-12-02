namespace FluentExceptions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class FinalErrorHandlerPackage
    {
        public FinalErrorHandlerPackage(IList<Type> handledTypes, 
            Action<Action> actionHandler, 
            Func<Func<object>, object> funcHandler, 
            Func<Func<Task<object>>, Task<object>> taskHandler)
        {
            this.HandledTypes = handledTypes;
            this.ActionHandler = actionHandler;
            this.FuncHandler = funcHandler;
            this.TaskHandler = taskHandler;
        }

        public IList<Type> HandledTypes { get; private set; }

        protected Action<Action> ActionHandler { get; private set; }

        protected Func<Func<object>, object> FuncHandler { get; private set; }

        protected Func<Func<Task<object>>, Task<object>> TaskHandler { get; private set; }

        public ErrorHandler AsErrorHandler()
        {
            return new ErrorHandler(this.HandledTypes, this.ActionHandler, this.FuncHandler, this.TaskHandler);
        }
    }
}