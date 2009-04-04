using System.IO;
using System.Linq.Expressions;
using System.Xml.Linq;
using Utility.Core;

namespace UITemplateViewer.DynamicPath
{
    public class Decoder
    {
        private Decoder(XElement element)
        {
            Element = element;
        }

        public Expression Local { get; set; }
        public Expression Data { get; set; }

        public XElement Element { get; private set; }

        public override string ToString()
        {
            return Element.ToString();
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