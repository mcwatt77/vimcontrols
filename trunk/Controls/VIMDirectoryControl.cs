using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VIMControls.Controls
{
    //IDirectoryBrowser implementations are gonna want to call Navigate back to a container
    public interface IFactory<T>
    {
    }

    public interface IDirectoryBrowserFactory : IFactory<IDirectoryBrowser>
    {
        IDirectoryBrowser Create(IVIMContainer container);
    }

    public interface IDirectoryBrowser : IVIMMotionController, IVIMCharacterController, IVIMControl
    {
//        void NavigateToDirectory(string uri);
    }

    //in the future the ServiceLocator could implicitly create this
    public class DirectoryBrowserFactory : IDirectoryBrowserFactory
    {
        public IDirectoryBrowser Create(IVIMContainer container)
        {
            return new VIMDirectoryControl(container);
        }
    }

    public class VIMDirectoryControl : VIMListBrowser
    {
        public VIMDirectoryControl()
        {
        }

        public VIMDirectoryControl(IVIMContainer parent) : base(parent)
        {

        }

        public override void NextLine()
        {
            var data = _textData[_selectedIndex].Text;
            if (data[0] == '[')
            {
                if (_directory == "computer")
                    _directory = "";

                data = data.Substring(1, data.Length - 2);

                Unhilight(_selectedIndex);
                _selectedIndex = 0;
                var oldDirectory = _directory;
                _directory = new DirectoryInfo(Path.Combine(_directory, data)).FullName;
                if (_directory == oldDirectory)
                    _directory = "computer";
                _parent.StatusLine(_directory);

                UpdateData();

                RenderLines();
            }
            else
            {
                SelectedFile = new FileInfo(Path.Combine(_directory, data)).FullName;
                SelectedFile.Persist(VIMMRUControl.Guid);
                _parent.Navigate(SelectedFile);
            }
        }

        protected override void UpdateData()
        {
            var files = new List<string>{"[..]"};
            if (_directory != "computer")
            {
                files.AddRange(new DirectoryInfo(_directory).GetDirectories().Select(dir => "[" + dir.Name + "]"));
                files.AddRange(new DirectoryInfo(_directory).GetFiles().Select(file => file.Name));
            }
            else
                files.AddRange(DriveInfo.GetDrives().Select(drive => "[" + drive.Name + "]"));

            _itemList = files.ToArray();
        }
    }
}