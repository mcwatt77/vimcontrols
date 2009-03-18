using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActionDictionary;
using ActionDictionary.Interfaces;
using AppControlInterfaces.ListView;
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

    public class AppLauncher : IControlKeyProcessor, IPaging
    {
        private readonly MessagePipe _messagePipe;
        private readonly UpdatePipe _updatePipe;
        private readonly List<KeyValuePair<Type, string>> _apps = new List<KeyValuePair<Type, string>>();

        private IEnumerable<KeyValuePair<Type, string>> LaunchableTypes(IEnumerable<Assembly> appControlAssemblies)
        {
            var launchableTypes = GetType().Assembly.GetTypesWithCustomAttribute<LaunchableAttribute>();
            var types = launchableTypes
                .Select(type => new KeyValuePair<Type, string>(GetAppControlType(type, appControlAssemblies),GetLaunchableName(type)))
                .ToList();

            var dict = types.ToDictionary(pair => pair.Value.ToLower());
            var preferredSort = new List<string> {"notes viewer", "query analyzer", "command stack", "sg viewer", "civilization"};
            return preferredSort
                .Where(dict.ContainsKey)
                .Select(s => dict[s]).Concat(types.Where(pair => !preferredSort.Contains(pair.Value.ToLower()))).ToList();

//            return types;
        }

        private static string GetLaunchableName(ICustomAttributeProvider type)
        {
            return type.GetCustomAttributes(false).OfType<LaunchableAttribute>().Single().DisplayName;
        }

        private static bool filter(Type m, object filterCriteria)
        {
            return true;
        }

        private static Type GetAppControlType(Type launchableType, IEnumerable<Assembly> assemblies)
        {
            var types = launchableType.FindInterfaces(filter, null).Where(type => type.Assembly == typeof(IListViewData).Assembly);
            //TODO: instead of assuming the first... look for a hierarchy
//            if (types.Count() != 1) throw new Exception("Currently only supporting a DataProcessor to implement a single AppControlInterface");
            var appControlInterfaceType = types.First();

            //from there, look in assemblies for appControlInterfaceType

            return assemblies
                .Select(assembly => GetAppControlType(assembly, appControlInterfaceType, launchableType))
                .SingleOrDefault();
        }

        private static Type GetAppControlType(Assembly assembly, Type appControlInterfaceType, Type launchableType)
        {
            var controlType = assembly
                .GetTypes()
                .Where(type => typeof (IAppControl).IsAssignableFrom(type)
                               && type.GetGenericArguments().Count() == 1)
                .Where(type => type.GetGenericArguments().Single().GetGenericParameterConstraints().SingleOrDefault() == appControlInterfaceType)
                .SingleOrDefault();

            return controlType.MakeGenericType(launchableType);
        }

        public AppLauncher(MessagePipe messagePipe, UpdatePipe updatePipe, params Assembly[] appControlAssemblies)
        {
            var types = LaunchableTypes(appControlAssemblies);
            types.Do(type => _apps.Add(new KeyValuePair<Type, string>(type.Key, type.Value)));

            _messagePipe = messagePipe;
            _updatePipe = updatePipe;
            AppLines = _apps.Select(pair => (AppLine)new SingleAppLine(pair.Value)).ToList();
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

        public void Beginning()
        {
            var oldHilight = HilightIndex;
            HilightIndex = 0;
            _updatePipe.Send(new []{oldHilight, HilightIndex});
        }

        public void End()
        {
            var oldHilight = HilightIndex;
            HilightIndex = AppLines.Count() - 1;
            _updatePipe.Send(new []{oldHilight, HilightIndex});
        }

        public void PageUp()
        {
        }

        public void PageDown()
        {
        }

        public void Enter()
        {
            _messagePipe.SendMessage(Message.Create<IWindow>(window => window.Navigate(_apps[HilightIndex].Key)));
        }

        public void WindowScroll()
        {
            throw new System.NotImplementedException();
        }

        public void LocalScroll()
        {
            throw new System.NotImplementedException();
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