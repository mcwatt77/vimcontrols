using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using ActionDictionary.Interfaces;
using AppControlInterfaces.ListView;
using AppControlInterfaces.MediaViewer;
using NUnit.Framework;
using Utility.Core;

namespace DataProcessors.Tests
{
    public class ControlTest<TDataProcessor> : IAppControl
        where TDataProcessor : IMediaViewerData
    {
        public UIElement GetControl()
        {
            throw new System.NotImplementedException();
        }
    }

    [TestFixture]
    public class AppLauncherTest : IAppLauncherUpdate
    {
        private Type GetAppControlType(Type launchableType, Assembly[] assemblies)
        {
            var types = launchableType.FindInterfaces(filter, null).Where(type => type.Assembly == typeof(IListViewData).Assembly);
            if (types.Count() != 1) throw new Exception("Currently only supporting a DataProcessor to implement a single AppControlInterface");
            var appControlInterfaceType = types.Single();

            //from there, look in assemblies for appControlInterfaceType

            return assemblies
                .Select(assembly => GetAppControlType(assembly, appControlInterfaceType))
                .SingleOrDefault();
        }

        private Type GetAppControlType(Assembly assembly, Type appControlInterfaceType)
        {
            return assembly
                .GetTypes()
                .Where(type => typeof (IAppControl).IsAssignableFrom(type)
                               && type.GetGenericArguments().Count() == 1)
                .Where(type => type.GetGenericArguments().Single().GetGenericParameterConstraints().SingleOrDefault() == appControlInterfaceType)
                .SingleOrDefault();
        }

        private bool filter(Type m, object filterCriteria)
        {
            return true;
        }

        [Test]
        public void TestAssembly()
        {
            //get Launchable types from this assembly.
            //Find interfaces from that Type that come from AppControlInterfaces
            //Look for a class with a GenericParameter constrained to that type
            //Instantiate that class with the Launchable Type

            var appControlAssemblies = new[] {GetType().Assembly};
            var launchableTypes = GetType().Assembly.GetTypesWithCustomAttribute<LaunchableAttribute>();
            var types = launchableTypes.Select(type => GetAppControlType(type, appControlAssemblies)).ToList();
            Assert.AreEqual(typeof (ControlTest<>), types.SingleOrDefault());
        }

        [Test]
        public void TestHilight()
        {
            var launcher = new AppLauncher(new MessagePipe(this), new UpdatePipe(this));
            Assert.AreEqual(0, launcher.HilightIndex);
            launcher.MoveDown();
            Assert.AreEqual(1, launcher.HilightIndex);
            launcher.MoveUp();
            Assert.AreEqual(0, launcher.HilightIndex);
        }

        public void Update(IEnumerable<int> indexes)
        {
        }
    }
}