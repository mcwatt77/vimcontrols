﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ActionDictionary;
using ActionDictionary.Interfaces;
using AppControlInterfaces.MediaViewer;

namespace AppViewer.Controls
{
    public class MediaViewerControl<TDataProcessor> : IAppControl, IMissing, IMediaViewerUpdate
        where TDataProcessor : class, IMediaViewerData
    {
        private readonly TDataProcessor _processor;
        private readonly Image _image;

        public MediaViewerControl()
        {
            _image = new Image {Stretch = Stretch.Uniform};
            var constructor = typeof (TDataProcessor).GetConstructors().Single();
            _processor = (TDataProcessor)constructor.Invoke(new object[] {this});
            Update();
        }

        public UIElement GetControl()
        {
            return _image;
        }

        public object ProcessMissingCmd(Message msg)
        {
            return msg.Invoke(_processor);
        }

        //TODO:  I don't like this... the Update has to beware of _processor not being set yet
        public void Update()
        {
            if (_processor == null) return;
            try
            {
                var bitmap = new BitmapImage(new Uri(_processor.UpdateImage()));
                _image.Source = bitmap;

                var grid = _image.Parent as Grid;
                if (grid != null)
                {
                    var app = grid.Parent as AppWindow;
                    if (app != null)
                        app.Title = bitmap.UriSource.LocalPath;
                }
            }
            catch (NotSupportedException)
            {}
        }
    }
}