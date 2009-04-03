using System;

namespace Utility.Core
{
    public class ExceptionWrapper : Exception
    {
        private readonly Exception _innerException;

        public ExceptionWrapper(Exception innerException)
        {
            _innerException = innerException;
        }
    }

    public static class ExceptionExtensions
    {
        public static void Rethrow(this Exception src)
        {
            throw new ExceptionWrapper(src);
        }
    }
}
