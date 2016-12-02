namespace FluentExceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ErrorHandlerPackage
    {
        public ErrorHandlerPackage(IEnumerable<Type> handledTypes, Func<Func<object>, Action<Exception>, bool, object> funcHandlerBlock, Func<Func<Task<object>>, Action<Exception>, bool, Task<object>> taskHandlerBlock)
        {
            this.FuncHandlerBlock = funcHandlerBlock;
            this.TaskHandlerBlock = taskHandlerBlock;
            this.HandledTypes = handledTypes.ToList();
        }

        public Func<Func<Task<object>>, Action<Exception>, bool, Task<object>> TaskHandlerBlock { get; private set; }

        public Func<Func<object>, Action<Exception>, bool, object> FuncHandlerBlock { get; private set; }

        public IList<Type> HandledTypes { get; private set; }

        public CompleteErrorHandlerPackage HandledBy(Action<Exception> exceptionHandlerAction)
        {
            return new CompleteErrorHandlerPackage(this.HandledTypes, this.FuncHandlerBlock, this.TaskHandlerBlock, exceptionHandlerAction);
        }

        public ErrorHandlerPackage Or<T>() where T : Exception
        {
            Func<Func<object>, Action<Exception>, bool, object> newFuncHandler = (f, h, r) =>
            {
                object retVal = null;

                try
                {
                    retVal = this.FuncHandlerBlock(f, h, r);
                }
                catch (T ex)
                {
                    h(ex);

                    if (r)
                    {
                        throw;
                    }
                }

                return retVal;
            };

            Func<Func<Task<object>>, Action<Exception>, bool, Task<object>> newTaskHandler = async (t, h, r) =>
            {
                object retVal = default(object);

                try
                {
                    retVal = await this.TaskHandlerBlock(t, h, r);
                }
                catch (T ex)
                {
                    h(ex);

                    if (r)
                    {
                        throw;
                    }
                }

                return retVal;
            };

            return new ErrorHandlerPackage(this.HandledTypes.AsEnumerable().Concat(new[]{ typeof(T) }), newFuncHandler, newTaskHandler);
        }
    }
}