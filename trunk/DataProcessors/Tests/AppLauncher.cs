using System;
using System.Collections.Generic;
using System.Linq;
using ActionDictionary.Interfaces;
using NUnit.Framework;
using Utility.Core;

namespace DataProcessors.Tests
{
    [TestFixture]
    public class AppLauncherTest : IAppLauncherUpdate
    {
        [Test]
        public void TestAssembly()
        {
            Type type1 = Type.GetType("AppViewer.CommandStackControl, AppViewer");
            var appControlAssemblies = new[] {type1.Assembly};
            var types = appControlAssemblies
                .Select(ass => ass.GetTypes().AsEnumerable())
                .Flatten()
                .Where(type => typeof(IAppControl).IsAssignableFrom(type))
                .ToList();

            Assert.Fail("Test not completed");
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