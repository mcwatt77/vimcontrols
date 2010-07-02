using System.IO;
using Navigator.UI.Attributes;

namespace Navigator.Path.Hd
{
    public class FilePathNode : IFileViewer
    {
        private readonly FileInfo _fileInfo;

        public FilePathNode(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }

        public FileInfo File
        {
            get { return _fileInfo; }
        }
    }
}