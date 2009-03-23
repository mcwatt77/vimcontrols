using System;
using System.Windows;

namespace UITemplateViewer
{
    public class AppViewer : Application
    {
        [STAThread]
        static void Main()
        {
            var app = new AppViewer();
            var window = new AppWindow {Height = 300, Width = 300};
            app.Run(window);
        }
    }
}