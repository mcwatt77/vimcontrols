using System.IO;
using System.Linq.Expressions;
using System.Xml.Linq;
using Utility.Core;

namespace UITemplateViewer.DynamicPath
{
            //TODO: This is pretty sketchy.  I should consider a different approach to handling namespaces.
            //And how will I remember to update Decoder when I change the interfaces around?
            //I need to get decode to build itself from hardcoded interface calls, so they'll break when I change the interface
            //  and not jus during testing
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