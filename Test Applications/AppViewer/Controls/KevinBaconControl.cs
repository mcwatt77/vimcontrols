using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ActionDictionary;
using ActionDictionary.Interfaces;
using AppControlInterfaces.KevinBacon;

namespace AppViewer.Controls
{
    public class KevinBaconControl<TDataProcessor> : IAppControl, IKevinBaconUpdater, IMissing
        where TDataProcessor : IKevinBaconData
    {
        private ListBox _listBox;
        private TextBox _textbox;
        private readonly TDataProcessor _processor;

        public KevinBaconControl(TDataProcessor processor)
        {
            _processor = processor;
            _processor.Updater = this;
        }

        public UIElement GetControl()
        {
//            var _textbox = new TextBox {Text = "Kevin Bacon's not here man"};
//            var control = new ListBox();
            var stackPanel = new StackPanel();

            _textbox = new TextBox();
            stackPanel.Children.Add(_textbox);
            _textbox.TextChanged += textBox_TextChanged;

            _listBox = new ListBox();
            _listBox.IsTextSearchEnabled = true;
//            _listBox.ItemsSource = new[] {"Apple", "Beta", "Charlie", "Crazy"};
            _listBox.Height = 23 * 3;
            stackPanel.Children.Add(_listBox);
            return stackPanel;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox) sender;
            if (textBox.Text.Length >= 1)
            {
                var c = textBox.Text.ToLower()[0];
                var items = new[] {"Apple", "Beta", "Charlie", "Crazy"};

                _listBox.ItemsSource = items.Where(item => item.ToLower()[0] == c).ToList();
            }
            else
            {
                _listBox.ItemsSource = new string[] {};
            }
        }

        public void UpdateText(string newText)
        {
            _textbox.Text = newText;
        }

        public object ProcessMissingCmd(Message msg)
        {
            return msg.Invoke(_processor);
        }
    }
}