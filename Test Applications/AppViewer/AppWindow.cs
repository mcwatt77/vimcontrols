﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ActionDictionary;
using ActionDictionary.Interfaces;
using AppControlInterfaces.ListView;
using DataProcessors;
using Utility.Core;

namespace AppViewer
{
    public class AppWindow : Window, IMissing, IWindow, IError
    {
        private static readonly MessageDictionary _mDict = new MessageDictionary();
        private static IAppControl _ctrl;

        public AppWindow()
        {
            var grid = new Grid();
            _ctrl = new AppLauncherControl(new MessagePipe(this));
            grid.Children.Add(_ctrl.GetControl());
            Content = grid;
        }

        private bool ProcessKeyState(KeyboardDevice device, Key curKey, Key retKey, params Key[] addlKeys)
        {
            if (curKey == Key.LeftCtrl || curKey == Key.RightCtrl) return false;

            if (!(addlKeys.Any(aKey => device.IsKeyDown(aKey)) || device.IsKeyDown(retKey)))
                return true;

            var msgs = _mDict.ProcessKey(retKey);
            msgs.Do(msg => msg.Invoke(this));
            return true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!ProcessKeyState(e.KeyboardDevice, e.Key, Key.LeftCtrl, Key.RightCtrl)) return;

            var messages = _mDict.ProcessKey(e.Key);
            messages.Do(msg => msg.Invoke(this, false).Do(m => m.Invoke(this)));
        }

        public void ProcessMissingCmd(Message msg)
        {
            msg.Invoke(_ctrl);
        }

        public void Maximize()
        {
            WindowState = WindowState.Maximized;
        }

        public void Navigate(Type type)
        {
            var grid = (Grid) Content;
            grid.Children.Clear();

            var typeDict = new Dictionary<Type, object>();
            typeDict[typeof (MessagePipe)] = new MessagePipe(this);
            var constructor = type.GetConstructors().Single();
            constructor
                .GetParameters()
                .Where(parameter => !typeDict.ContainsKey(parameter.ParameterType))
                .Do(parameter => { typeDict[parameter.ParameterType] = parameter.ParameterType.NewInstance<object>(); });

            var parameters = constructor.GetParameters().Select(parameter => typeDict[parameter.ParameterType]).ToArray();

            _ctrl = (IAppControl) constructor.Invoke(parameters);

            grid.Children.Add(_ctrl.GetControl());
        }

        public void Quit()
        {
            Navigate(typeof(AppLauncherControl));
        }

        public void Report(Exception ex)
        {
            var inner = ex.ChainWithSelf(e => e.InnerException).Last();
            MessageBox.Show(inner.Message + "\r\n\r\n\r\n" + inner.StackTrace);
        }
    }

    public class ListTest : IListViewData
    {
        private readonly string[][] _data = new[]
                                       {
                                           new[] {"1", "2", "3"},
                                           new[] {"Alpha", "Beta", "Charlie"},
                                           new[] {"One", "Two", "Three"}
                                       };

        public string GetData(int row, int col)
        {
            return _data[row][col];
        }

        public int RowCount
        {
            get { return 3; }
        }

        public int ColCount
        {
            get { return 3; }
        }

        public int HilightIndex
        {
            get { return 0; }
        }

        public IListViewUpdate Updater
        {
            set { }
        }
    }
}