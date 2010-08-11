using System.Xml.Linq;

namespace Navigator.UI.Attributes
{
    public interface IHasXml
    {
        XElement Xml { get; }
    }
}