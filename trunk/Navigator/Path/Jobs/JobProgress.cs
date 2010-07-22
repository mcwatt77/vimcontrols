using System;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using Navigator.UI;
using VIControls.Commands.Interfaces;

namespace Navigator.Path.Jobs
{
    public class JobProgress
    {
        private readonly IUIPort _port;

        public JobProgress(IUIPort port)
        {
            _port = port;
        }

        public void StartJob()
        {
            MessageBox.Show("Started job!");
        }

        public Action<TResult> GetPropertyUpdater<TSource, TResult>(Expression<Func<TSource, TResult>> property, string summary)
        {
// ReSharper disable AccessToModifiedClosure
            //TODO: I need to be able to compare this against some sort of Path value to find the correct UI element
            return o =>
                       {
                           var model = (IUIChildren) _port.ActiveModel;
                           var firstElement = model.UIElements.OfType<StringSummaryElement>().First(s => s.Message == summary);
                           summary = o.ToString();
                           firstElement.Message = o.ToString();
                       };
// ReSharper restore AccessToModifiedClosure
        }
    }
}