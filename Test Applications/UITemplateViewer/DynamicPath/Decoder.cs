using System.IO;
using Utility.Core;

namespace UITemplateViewer.DynamicPath
{
    public class Decoder
    {
        private Decoder()
        {}

        public static Decoder FromPath(string path)
        {
            var fileInfo = new FileInfo(@"..\..\DynamicPath\syntax.p");
            var parser = new Parser(fileInfo.OpenText());
/*            parser.Parse("[*]{/note}");
            parser.Parse("[*]");
            parser.Parse("{@descr}");
            parser.Parse("{/dyn::rowSelector}");
            parser.Parse(":noteList/@rows");
            parser.Parse("[../@rowSelector]{@body}");*/

            var doc = parser.Parse(path);

            return new Decoder();
        }
    }
}