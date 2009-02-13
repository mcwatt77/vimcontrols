using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActionDictionary;
using ActionDictionary.Interfaces;
using Utility.Core;

namespace DataProcessors
{
    public interface IAppLauncherUpdate
    {
        void Update(IEnumerable<int> indexes);
        //TODO: Perhaps I should be able to update in any of the following:  fixed screen size, screen %, number of some other unit, like line nos.
    }

    public class UpdatePipe
    {
        private readonly IAppLauncherUpdate _updater;

        public UpdatePipe(IAppLauncherUpdate updater)
        {
            _updater = updater;
        }

        public void Send(params IEnumerable<int>[] indexes)
        {
            indexes.Do(list => _updater.Update(list));
        }
    }

    public class AppLauncher : INavigation, IEnterProcessor
    {
        private readonly MessagePipe _messagePipe;
        private readonly UpdatePipe _updatePipe;

        public AppLauncher(MessagePipe messagePipe, UpdatePipe updatePipe, params Assembly[] appControlAssemblies)
        {
            var types = appControlAssemblies
                .Select(ass => ass.GetTypes().AsEnumerable())
                .Flatten()
                .Where(type => typeof(IAppControl).IsAssignableFrom(type));
            //now look at the generic type parameter constraints...
            //then look for classes that implement those constraints



            _messagePipe = messagePipe;
            _updatePipe = updatePipe;
            AppLines = new List<AppLine>
                           {
                               new SingleAppLine("Project Tracker"),
                               new SingleAppLine("Text Editor"),
                               new SingleAppLine("SG Viewer"),
                               new SingleAppLine("Command Stack"),
                               new SingleAppLine("Files"),
                               new SingleAppLine("Audio"),
                               new SingleAppLine("Movies"),
                               new SingleAppLine("Objects"),
                               new SingleAppLine("Notes"),
                               new SingleAppLine("Grapher"),
                               new SingleAppLine("3d Sketcher")
                           };
            //get all of the class that implement IAppControl
        }

        public IEnumerable<AppLine> AppLines { get; private set; }
        public int HilightIndex { get; private set; }

        public void MoveUp()
        {
            HilightIndex--;
            if (HilightIndex < 0) HilightIndex = 0;
            else _updatePipe.Send(new[] {HilightIndex + 1, HilightIndex});
        }

        public void MoveDown()
        {
            HilightIndex++;
            if (HilightIndex >= AppLines.Count()) HilightIndex = AppLines.Count() - 1;
            else _updatePipe.Send(new[] {HilightIndex - 1, HilightIndex});
        }

        public void MoveRight()
        {
        }

        public void MoveLeft()
        {
        }

        public void Enter()
        {
            _messagePipe.SendMessage(Message.Create<IWindow>(window => window.Navigate(Type.GetType("AppViewer.SgViewerControl, AppViewer"))));
//            _messagePipe.SendMessage(Message.Create<IWindow>(window => window.Navigate(Type.GetType("AppViewer.CommandStackControl, AppViewer"))));
        }
    }

    public abstract class AppLine
    {
        public abstract string AppText { get; }
    }

    public class SingleAppLine : AppLine
    {
        private readonly string _appName;

        public SingleAppLine(string appName)
        {
            _appName = appName;
        }

        public override string AppText
        {
            get { return _appName; }
        }
    }
}