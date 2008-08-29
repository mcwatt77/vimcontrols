using System;
using System.Windows;

namespace VIMControls
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public class App : Application
    {
        public App()
        {
//            StartupUri 
        }

        [STAThread]
        static void Main()
        {
            var app = new App();
            var window = new Window1 {Height = 300, Width = 300};
            app.Run(window);
        }
    }
}
