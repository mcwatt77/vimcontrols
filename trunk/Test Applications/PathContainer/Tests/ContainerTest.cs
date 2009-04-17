using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace PathContainer.Tests
{
    [TestFixture]
    public class ContainerTest
    {

        public class one
        {
            public two Child { get; set; }
            public IEnumerable<IChildType> Children { get; set; }
            public string Value { get; set; }
        }
        public class two : IChildType
        {
            public one Parent { get; set; }
            public string Value { get; set; }
        }
        public class three : IChildType
        {}
        public interface IChildType{}

        [Test]
        public void test()
        {
            LoadObjectWithChildOperator("<one child=\"[*]\"><two/></one>");
            LoadObjectWithParentOperator("<one child=\"[*]\"><two parent=\"[..]\"/></one>");
            LoadObjectById("<one child=\"[:child]\"><two id=\"child\"/></one>");
            LoadObjectWithMultipleChildren("<one children=\"[*]\"><two/><three/></one>");
            LoadObjectWithAttributeSelected("<one child=\"[@children][1]\" children=\"[*]\"><two/><three/></one>");
            LoadObjectLiteral("<one value=\"test\"/>");
            LoadObjectData("<one value=\"{/note[1]/@desc}\"/>");
            LoadAnotherObjectsData("<one value=\"test\" child=\"[two]\"><two value=\"[../@value]\"/></one>");
            LoadDataIntoAChild("<one children=\"{/note}[*]\"><two value=\"{@desc}\"/></one>");
            LoadDataFromAChild("<one value=\"[two/@value]{@desc}\"><two value=\"{/note[1]}\"/></one>");
        }

        private void LoadDataFromAChild(string template)
        {
            var elementResult = ParseTemplate<one>(template);
            Assert.AreEqual("1", elementResult.Value);
        }

        private void LoadDataIntoAChild(string template)
        {
            var elementResult = ParseTemplate<one>(template);
            Assert.AreEqual(2, elementResult.Children.Count(child => typeof(two).IsAssignableFrom(child.GetType())));
            Assert.AreEqual("one", elementResult.Children.Cast<two>().First().Value);
            Assert.AreEqual("two", elementResult.Children.Cast<two>().Skip(1).First().Value);
        }

        private void LoadAnotherObjectsData(string template)
        {
            var elementResult = ParseTemplate<one>(template);
            Assert.AreEqual(elementResult.Value, elementResult.Child.Value);
            elementResult.Value = "newValue";
            Assert.AreEqual(elementResult.Value, elementResult.Child.Value);
        }

        private void LoadObjectData(string template)
        {
            var elementResult = ParseTemplate<one>(template);
            Assert.AreEqual("one", elementResult.Value);
        }

        private void LoadObjectLiteral(string template)
        {
            var elementResult = ParseTemplate<one>(template);
            Assert.AreEqual("test", elementResult.Value);
        }

        private void LoadObjectWithAttributeSelected(string template)
        {
            var elementResult = ParseTemplate<one>(template);
            Assert.IsNotNull(elementResult.Children);
            Assert.IsNotNull(elementResult.Child);
            Assert.AreEqual(2, elementResult.Children.Count(child => child != null));
            Assert.AreSame(elementResult.Children.First(), elementResult.Child);
        }

        private void LoadObjectWithMultipleChildren(string template)
        {
            var elementResult = ParseTemplate<one>(template);
            Assert.IsNotNull(elementResult);
            Assert.IsNotNull(elementResult.Children);
            Assert.AreEqual(2, elementResult.Children.Count(child => child != null));
        }

        private void LoadObjectById(string template)
        {
            var elementResult = ParseTemplate<one>(template);
            Assert.IsNotNull(elementResult);
            Assert.IsNotNull(elementResult.Child);
        }

        private void LoadObjectWithChildOperator(string template)
        {
            var elementResult = ParseTemplate<one>(template);
            Assert.IsNotNull(elementResult);
            Assert.IsNotNull(elementResult.Child);
        }

        private void LoadObjectWithParentOperator(string template)
        {
            var elementResult = ParseTemplate<one>(template);
            Assert.IsNotNull(elementResult.Child.Parent);
            Assert.AreSame(elementResult, elementResult.Child.Parent);
        }

        public T ParseTemplate<T>(string template)
        {
            return default(T);
        }
    }
}
