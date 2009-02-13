using System;
using System.Windows;
using System.Windows.Controls;
using ActionDictionary;
using ActionDictionary.Interfaces;
using AppControlInterfaces.ListView;
using DataProcessors;
using Utility.Core;

namespace AppViewer
{
    public class AppWindow : Window, IMissing, IWindow
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

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            var messages = _mDict.ProcessKey(e.Key);
            messages.Do(msg => msg.Invoke(this));
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

            //TODO: this assumes empty constructor.  This will need to be some sort of container implementation
            var con = type.GetConstructor(new Type[] {});
            _ctrl = (IAppControl)con.Invoke(new object[] {});
            grid.Children.Add(_ctrl.GetControl());

            //do something like...
/*            var view = new GridControl<ListTest>();
            grid.Children.Add(view.GetControl());*/
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
    }
}