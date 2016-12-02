using System;
using System.Threading.Tasks;

namespace FluentExceptions.Tests
{
    public class TestObject
    {
        public const int IntResult = 42;

        private int retryTimes = 0;

        public void MethodThatDoesNothing()
        { }

        public void VoidMethodThatThrowsArithmeticException()
        {
            throw new ArithmeticException();
        }

        public int IntMethodThatThrowsArithmeticException()
        {
            throw new ArithmeticException();
        }

        public int IntMethodThatThrowsArgumentNullException()
        {
            throw new ArgumentNullException();
        }

        public int IntMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(int times)
        {
            if (this.retryTimes < times)
            {
                this.retryTimes++;
                throw new ArithmeticException();
            }
            else
            {
                return IntResult;
            }
        }
        public void VoidMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(int times)
        {
            if (this.retryTimes < times)
            {
                this.retryTimes++;
                throw new ArithmeticException();
            }
        }

        public async Task AsyncVoidMethodThatThrowsArithmeticException()
        {
            await Task.Delay(0);
            throw new ArithmeticException();
        }

        public async Task<int> AsyncIntMethodThatThrowsArithmeticException()
        {
            await Task.Delay(0);
            throw new ArithmeticException();
        }

        public async Task AsyncVoidMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(int times)
        {
            await Task.Delay(0);
            if (this.retryTimes < times)
            {
                this.retryTimes++;
                throw new ArithmeticException();
            }
        }

        public async Task<int> AsyncIntMethodThatThrowsArithmeticExceptionXTimesBeforeSuccess(int times)
        {
            await Task.Delay(0);

            if (this.retryTimes < times)
            {
                this.retryTimes++;
                throw new ArithmeticException();
            }
            else
            {
                return IntResult;
            }
        }
    }
}
