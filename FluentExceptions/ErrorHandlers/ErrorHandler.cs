namespace FluentExceptions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ErrorHandler
    {
        public ErrorHandler(
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

        public IList<Type> HandledTypes { get; private set; }

        public Action<Action> ActionHandler { get; private set; }

        public Func<Func<object>, object> FuncHandler { get; private set; }

        public Func<Func<Task<object>>, Task<object>> TaskHandler { get; private set; }

        public static ErrorHandlerDefinition Define()
        {
            return new ErrorHandlerDefinition();
        }
    }
}
