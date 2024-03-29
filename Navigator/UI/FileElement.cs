using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VIControls.Commands.Interfaces;

namespace Navigator.UI
{
    public class FileElement : IUIElement
    {
        private readonly TextBlock _block;
        private readonly Run _run;
        private readonly TextBlock _bodyBlock;
        private readonly Run _bodyRun;
        private readonly FileInfo _fileInfo;
        private IStackPanel _stackPanel;

        public FileElement(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;

            _run = new Run(_fileInfo.Name);
            _block = new TextBlock(_run);
            _block.TextWrapping = TextWrapping.Wrap;
            _bodyRun = new Run(_fileInfo.FullName + "\r\n" + _fileInfo.Length + "\r\n" + _fileInfo.LastWriteTime);
            _bodyBlock = new TextBlock(_bodyRun);
            _bodyBlock.TextWrapping = TextWrapping.Wrap;
        }

        public void Render(IUIContainer container)
        {
            _stackPanel = container.GetInterface<IStackPanel>();

            if (!_stackPanel.DisplaySummary)
            {
                if (_fileInfo.Extension == ".jpg")
                {
                    var image = new Image {Source = new BitmapImage(new Uri(_fileInfo.FullName))};
                    _stackPanel.AddChild(image);
                }
                else
                {
                    if (_bodyBlock.Parent != null)
                        ((StackPanel)_bodyBlock.Parent).Children.Remove(_bodyBlock);
                    _stackPanel.AddChild(_bodyBlock);
                }
            }
            else
            {
                if (_block.Parent != null)
                    ((StackPanel)_block.Parent).Children.Remove(_block);
                _stackPanel.AddChild(_block);
            }
        }

        public void SetFocus(bool on)
        {
            _run.Background = on ? Brushes.Bisque : Brushes.White;

            if (_stackPanel == null || !on) return;
            _stackPanel.EnsureVisible(_block);
        }

        public void Update()
        {
        }
    }
}