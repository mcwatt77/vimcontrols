using System.IO;
using System.Linq;
using ActionDictionary.Interfaces;
using Utility.Core;

namespace DataProcessors
{
    public class SgNavigator : INavigation
    {
        private const string _sgDirectory = @"d:\media\images\sg";
        private string _currentDirectory;

        private FileInfo[] _files;
        private int _currentIndex;
        private string _currentFile;

        private int _oldIndex;

        public SgNavigator()
        {
            Initialize();
        }

        private void Initialize()
        {
            var di = new DirectoryInfo(_sgDirectory);
            _files = di.GetFiles();
            _currentIndex = 0;
            _currentFile = _files[_currentIndex].FullName;
            _currentDirectory = null;
        }

        public void MoveUp()
        {
            _currentIndex--;
            if (_currentIndex < 0) _currentIndex = 0;
            _currentFile = _files[_currentIndex].FullName;
        }

        public void MoveDown()
        {
            _currentIndex++;
            if (_currentIndex >= _files.Length) _currentIndex = _files.Length - 1;
            _currentFile = _files[_currentIndex].FullName;
        }

        private static FileInfo[] GetFilesFromSubDirectories(DirectoryInfo di)
        {
            return di.GetDirectories().Select(d => d.GetFiles().AsEnumerable()).Flatten().ToArray();
        }

        public void MoveRight()
        {
            if (_currentDirectory != null) return;

            var file = _files[_currentIndex];
            var name = file.Name.Replace(file.Extension, "");
            var di = new DirectoryInfo(Path.Combine(_sgDirectory, name));
            if (!di.Exists) return;

            var files = GetFilesFromSubDirectories(di);
            if (files.Length == 0) return;

            _currentDirectory = di.FullName;
            _files = files;
            _oldIndex = _currentIndex;
            _currentIndex = 0;
            _currentFile = _files[_currentIndex].FullName;
        }

        public void MoveLeft()
        {
            if (_currentDirectory == null) return;

            Initialize();
            _currentIndex = _oldIndex;
            _currentFile = _files[_currentIndex].FullName;
        }

        public string GetFileToDisplay()
        {
            return _currentFile;
        }
    }
}
