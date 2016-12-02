namespace FluentExceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ErrorHandlerDefinition
    {
        private readonly List<Type> handledTypes = new List<Type>();

        public ErrorHandlerDefinition(
            IList<Type> types, 
            Action<Action> actionHandler, 
            Func<Func<object>, object> funcHandler, 
            Func<Func<Task<object>>, Task<object>> taskHandler)
        {
            this.ActionHandler = actionHandler;
            this.FuncHandler = funcHandler;
            this.TaskHandler = taskHandler;
            this.handledTypes.AddRange(types);
        }

        public ErrorHandlerDefinition()
        {
            this.ActionHandler = a => a();
            this.FuncHandler = f => f();
            this.TaskHandler = async t => await t();
        }

        public Action<Action> ActionHandler { get; set; }
        public Func<Func<object>, object> FuncHandler { get; set; }
        public Func<Func<Task<object>>, Task<object>> TaskHandler { get; set; }

        public ErrorHandlerPackage MayThrow<T>() where T : Exception
        {
            Func<Func<object>, Action<Exception>, bool, object> funcHandlerBlock = (f, h, r) =>
            {
                object retVal = null;

                try
                {
                    retVal = this.FuncHandler(f);
                }
                catch (T e)
                {
                    h(e);

                    if (r)
                    {
                        throw;
                    }
                }

                return retVal;
            };

            Func<Func<Task<object>>, Action<Exception>, bool, Task<object>> taskHandlerBlock = async (f, h, r) =>
            {
                object retVal = null;
                try
                {
                    retVal = await this.TaskHandler(f);
                }
                catch (T e)
                {
                    h(e);

                    if (r)
                    {
                        throw;
                    }
                }

                return retVal;
            };

            return new ErrorHandlerPackage(this.handledTypes.Concat(new[]{ typeof(T) }), funcHandlerBlock, taskHandlerBlock);
        }
    }
}