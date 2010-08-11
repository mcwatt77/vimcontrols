using System.IO;
using Navigator.Path.KevinBacon;
using NUnit.Framework;

namespace Navigator.Test
{
    [TestFixture]
    public class KevinBaconCollectionTest
    {
        [Test]
        public void TestSearch()
        {
            var fileInfo = new FileInfo("kevin_bacon.xml");
            fileInfo.Delete();

            var collection = new KevinBaconCollection(null);
            collection.CommitSearch("Al Pacino");
        }
    }
}