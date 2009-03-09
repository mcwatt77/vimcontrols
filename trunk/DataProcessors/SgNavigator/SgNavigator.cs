using System.IO;
using System.Linq;
using ActionDictionary.Interfaces;
using AppControlInterfaces.MediaViewer;
using Utility.Core;

namespace DataProcessors.SgNavigator
{
    [Launchable("SG Viewer 2")]
    public class Sg2 : SgNavigator
    {
        public Sg2(IMediaViewerUpdate updater) : base(updater, @"d:\media\images\sg")
        {
        }
    }

    [Launchable("SG Viewer")]
    public class SgNavigator : INavigation, IMediaViewerData, IPaging
    {
        private readonly IMediaViewerUpdate _updater;

//        private const string _sgDirectory = @"d:\media\images\sg";
        protected string _sgDirectory = @"D:\media\images\vacation";
        private string _currentDirectory;

        private FileInfo[] _files;
        private int _currentIndex;
        private string _currentFile;
        private string CurrentFile
        {
            get
            {
                return _currentFile;
            }
            set
            {
                _currentFile = value;
                _updater.Update();
            }
        }

        private int _oldIndex;

        protected SgNavigator(IMediaViewerUpdate updater, string directory)
        {
            _sgDirectory = directory;
            _updater = updater;
            Initialize();
        }

        public SgNavigator(IMediaViewerUpdate updater)
        {
            _updater = updater;
            Initialize();
        }

        private void Initialize()
        {
            var di = new DirectoryInfo(_sgDirectory);
            _files = di.GetFiles();
            _currentIndex = 0;
            CurrentFile = _files[_currentIndex].FullName;
            _currentDirectory = null;
        }

        public void MoveUp()
        {
            _currentIndex--;
            if (_currentIndex < 0) _currentIndex = 0;
            CurrentFile = _files[_currentIndex].FullName;
        }

        public void MoveDown()
        {
            _currentIndex++;
            if (_currentIndex >= _files.Length) _currentIndex = _files.Length - 1;
            CurrentFile = _files[_currentIndex].FullName;
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
            CurrentFile = _files[_currentIndex].FullName;
        }

        public void MoveLeft()
        {
            if (_currentDirectory == null) return;

            Initialize();
            _currentIndex = _oldIndex;
            CurrentFile = _files[_currentIndex].FullName;
        }

        public void Beginning()
        {
        }

        public void End()
        {
        }

        public void PageUp()
        {
        }

        public void PageDown()
        {
        }

        public string GetFileToDisplay()
        {
            return CurrentFile;
        }

        public string UpdateImage()
        {
            return CurrentFile;
        }
    }
}