using System;
using System.Windows;

namespace VIMControls
{
    public class App : Application
    {
        [STAThread]
        static void Main()
        {
            var app = new App();
            var window = new Window1 {Height = 300, Width = 300};
            app.Run(window);
        }
    }
}
