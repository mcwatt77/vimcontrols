using System;
using System.Windows;

namespace VIMControls
{
    public class App : Application
    {
        private static App app;
        private static Window1 window;

        [STAThread]
        static void Main()
        {
            app = new App();
            app.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(app_DispatcherUnhandledException);
            window = new Window1 {Height = 300, Width = 300};
            app.Run(window);
        }

        static void app_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception.Source == "PresentationCore")
            {
                MessageBox.Show("PresentationCore failure");

                e.Handled = true;
                app.Shutdown();
                app.Run(window);

                //send a restart message
            }
        }
    }
}
