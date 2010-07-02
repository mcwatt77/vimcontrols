using System.IO;

namespace Navigator.UI.Attributes
{
    public interface IFileViewer : IAttribute
    {
        FileInfo File { get; }
    }
}