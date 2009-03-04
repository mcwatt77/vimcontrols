using System.Windows;
using System.Windows.Controls;
using ActionDictionary;
using ActionDictionary.Interfaces;
using AppControlInterfaces.TextView;

namespace AppViewer.Controls
{
    public class TextBlockControl<TDataProcessor> : IAppControl, IMissing, ITextViewUpdate
        where TDataProcessor : ITextViewData, new()
    {
        private readonly TDataProcessor _processor = new TDataProcessor();
        private TextBlock _control;

        public UIElement GetControl()
        {
            _control = new TextBlock {Text = _processor.GetData()};
            return _control;
        }

        public void Update()
        {
            _control.Text = _processor.GetData();
        }

        public void ProcessMissingCmd(Message msg)
        {
            msg.Invoke(_processor);
        }
    }
}
