namespace FluentExceptions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class CompleteErrorHandlerPackage
    {
        public CompleteErrorHandlerPackage(IList<Type> handledTypes, Func<Func<object>, Action<Exception>, bool, object> funcHandlerBlock, Func<Func<Task<object>>, Action<Exception>, bool, Task<object>> taskHandlerBlock, Action<Exception> exceptionHandlerAction)
        {
            this.HandledTypes = handledTypes;
            this.FuncHandlerBlock = funcHandlerBlock;
            this.TaskHandlerBlock = taskHandlerBlock;
            this.ExceptionHandlerAction = exceptionHandlerAction;
        }

        public Action<Exception> ExceptionHandlerAction { get; private set; }

        public IList<Type> HandledTypes { get; private set; }

        public Func<Func<object>, Action<Exception>, bool, object> FuncHandlerBlock { get; private set; }

        public Func<Func<Task<object>>, Action<Exception>, bool, Task<object>> TaskHandlerBlock { get; private set; }

        public ErrorHandler AsErrorHandler()
        {
            var actionHandler = this.GenerateActionHandler(rethrow: false);
            var funcHandler = this.GenerateFuncHandler(rethrow: false);
            var taskHandler = this.GenerateTaskHandler(rethrow: false);
            return new ErrorHandler(this.HandledTypes, actionHandler, funcHandler, taskHandler);
        }

        public FinalErrorHandlerPackage AndFinally(Action finallyAction)
        {
            return new FinalErrorHandlerPackage(this.HandledTypes,
                this.GenerateActionHandler(finallyAction),
                this.GenerateFuncHandler(finallyAction),
                this.GenerateTaskHandler(finallyAction));
        }

        public ErrorHandlerDefinition OrIt()
        {
            return new ErrorHandlerDefinition(this.HandledTypes, this.GenerateActionHandler(rethrow: false), this.GenerateFuncHandler(rethrow: false), this.GenerateTaskHandler(rethrow: false));
        }

        public RethrowErrorHandlerPackage AndRethrows()
        {
            return new RethrowErrorHandlerPackage(this.HandledTypes, this.GenerateActionHandler(rethrow: true), this.GenerateFuncHandler(rethrow: true), this.GenerateTaskHandler(rethrow: true));
        }

        private Func<Func<Task<object>>, Task<object>> GenerateTaskHandler(bool rethrow)
        {
            return f => this.TaskHandlerBlock(f, this.ExceptionHandlerAction, rethrow);
        }

        private Func<Func<object>, object> GenerateFuncHandler(bool rethrow)
        {
            return f => this.FuncHandlerBlock(f, this.ExceptionHandlerAction, rethrow);
        }

        private Action<Action> GenerateActionHandler(bool rethrow)
        {
            Func<Action, Func<object>> convertedAction = a => () =>
            {
                a();
                return null;
            };

            return a => this.FuncHandlerBlock(convertedAction(a), this.ExceptionHandlerAction, rethrow);
        }
        private Func<Func<Task<object>>, Task<object>> GenerateTaskHandler(Action finallyAction)
        {
            return f =>
            {
                try
                {
                    return this.TaskHandlerBlock(f, this.ExceptionHandlerAction, false);
                }
                finally
                {
                    finallyAction();
                }
            };
        }

        private Func<Func<object>, object> GenerateFuncHandler(Action finallyAction)
        {
            return f =>
            {
                try
                {
                    return this.FuncHandlerBlock(f, this.ExceptionHandlerAction, false);
                }
                finally
                {
                    finallyAction();
                }
            };
        }

        private Action<Action> GenerateActionHandler(Action finallyAction)
        {
            Func<Action, Func<object>> convertedAction = a => () =>
            {
                try
                {
                    a();
                    return null;
                }
                finally
                {
                    finallyAction();
                }
            };

            return a => this.FuncHandlerBlock(convertedAction(a), this.ExceptionHandlerAction, false);
        }
    }
}