using System;
using System.Linq;

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

        public static string FormatError(this Exception src)
        {
            var inner = src.ChainWithSelf(e => e.InnerException).Last();
            return inner.Message + "\r\n\r\n\r\n" + inner.StackTrace;
        }
    }
}
