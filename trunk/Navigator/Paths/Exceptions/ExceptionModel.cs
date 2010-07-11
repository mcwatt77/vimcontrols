using System;
using Navigator.UI.Attributes;

namespace Navigator.Path.Exceptions
{
    public class ExceptionModel : ISummaryString, IDescriptionString
    {
        private readonly Exception _exception;
        private readonly string _message;

        public ExceptionModel(Exception exception)
        {
            _exception = exception;

            var message = "";
            var currentException = _exception;
            while (currentException != null)
            {
                if (message.Length > 0)
                    message += "\r\n";
                message += currentException.Message;

                currentException = currentException.InnerException;
            }
            _message = message;
        }

        public string Summary
        {
            get { return _exception.Message; }
        }

        public string Description
        {
            get { return _message + "\r\n\r\n" + _exception.StackTrace; }
        }
    }
}