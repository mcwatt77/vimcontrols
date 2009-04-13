using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ActionDictionary;
using ActionDictionary.Interfaces;
using UITemplateViewer.Element;
using Utility.Core;

namespace UITemplateViewer
{
    public class AppWindow : Window, IWindow, IMissing, IError, IContainer
    {
        private static readonly MessageDictionary _mDict = new MessageDictionary();
        private readonly object _controller;

        public AppWindow()
        {
            Content = new StackPanel();

            var template = new DynamicTemplate2();
            var ui = template.GetUI();
            ui.Parent = this;
            ui.Initialize();
            _controller = ui;

/*            var template = new DynamicTemplate();
            _controller = template.InitializeController(this);*/
        }

        public void Maximize()
        {
            throw new System.NotImplementedException();
        }

        public void Navigate(Type type)
        {
            throw new System.NotImplementedException();
        }

        public void Quit()
        {
            throw new System.NotImplementedException();
        }

        public object ProcessMissingCmd(Message msg)
        {
            return msg.Invoke(_controller);
        }

        public void Report(Exception e)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
//            if (!ProcessKeyState(e.KeyboardDevice, e.Key, Key.LeftCtrl, Key.RightCtrl)) return;

            var messages = _mDict.ProcessKey(e.Key);
            messages.Do(InvokeMessage);
        }

        private void InvokeMessage(Message message)
        {
            message.Invoke(this, false);
            message.Errors.Do(m => m.Invoke(this));
        }

        public void AddChild(FrameworkElement element)
        {
            ((StackPanel) Content).Children.Add(element);
        }

        public FrameworkElement ControlById(string id)
        {
            return ((StackPanel) Content).Children.Cast<FrameworkElement>().Where(child => child.Name == id).SingleOrDefault();
        }
    }
}