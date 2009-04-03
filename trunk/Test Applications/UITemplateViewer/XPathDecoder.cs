using System;
using System.Collections.Generic;
using NodeMessaging;

namespace UITemplateViewer
{
    public class XPathDecoder
    {
        private readonly Func<IParentNode, IEndNode> fnGetDesc = node => node.Attribute("desc");
        private readonly Func<IParentNode, IEndNode> fnGetText = node => node.Attribute("body");
        private readonly Func<IParentNode, IEnumerable<IParentNode>> fnGetNotes = node => node.Nodes("note");

        public Delegate GetPathFunc(string path)
        {
            object obj;
            switch (path)
            {
                case "@descr":
                    obj = fnGetDesc;
                    break;
                case "@body":
                    obj = fnGetText;
                    break;
                case "//note":
                    obj = fnGetNotes;
                    break;
                default:
                    throw new Exception("Could not parse XPath");
            }
            return (Delegate) obj;
        }

        public Func<IParentNode, T> GetPathFunc<T>(string path)
        {
            object obj;
            switch (path)
            {
                case "@descr":
                    obj = fnGetDesc;
                    break;
                case "@body":
                    obj = fnGetText;
                    break;
                case "//note":
                    obj = fnGetNotes;
                    break;
                default:
                    throw new Exception("Could not parse XPath");
            }
            return (Func<IParentNode, T>) obj;
        }
    }
}