using System.IO;
using System.Xml.Linq;
using Utility.Core;

namespace UITemplateViewer.DynamicPath
{
    public class Decoder
    {
        private readonly XElement _element;

        private Decoder(XElement element)
        {
            _element = element;
        }

        public override string ToString()
        {
            return _element.ToString();
        }

        public static Decoder FromPath(string path)
        {
            var fileInfo = new FileInfo(@"..\..\DynamicPath\syntax.p");
            var parser = new PathParser(fileInfo.OpenText());

            var doc = parser.Parse(path);

            return new Decoder(doc);
        }
    }
}