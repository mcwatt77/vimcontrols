using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ActionDictionary;
using ActionDictionary.Interfaces;
using DataProcessors;

namespace AppViewer
{
    public class SgViewerControl : IAppControl, IMissing
    {
        private static readonly SgNavigator _sg = new SgNavigator();
        private Image _image;

        public UIElement GetControl()
        {
            _image = new Image {Name = "image", Stretch = Stretch.Uniform};
            UpdateImage();
            return _image;
        }

        public void ProcessMissingCmd(Message msg)
        {
            msg.Invoke(_sg);
            UpdateImage();
        }

        private void UpdateImage()
        {
            var bitmap = new BitmapImage(new Uri(_sg.GetFileToDisplay()));
            _image.Source = bitmap;
        }
    }
}