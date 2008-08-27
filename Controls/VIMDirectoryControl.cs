using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Media;

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

    public abstract class VIMDirectoryControlBase : VIMTextControl, IDirectoryBrowser
    {
        protected IVIMContainer _parent;
        protected string _directory = "computer";
        protected string[] _itemList;
        protected int _selectedIndex;

        protected VIMDirectoryControlBase() : this(null)
        {
        }

        protected VIMDirectoryControlBase(IVIMContainer parent)
        {
            _parent = parent;
        }

        public string SelectedFile { get; set; }

        protected abstract void UpdateData();

        protected override void OnRenderSizeChanged(System.Windows.SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            RenderLines();
        }

        protected void RenderLines()
        {
            if (_itemList == null)
                UpdateData();

            _itemList
                .Take(_textData.Length)
                .Do((i, s) => _textData[i].Text = s);
            _textData
                .Skip(_itemList.Count())
                .Do(text => text.Text = "");

            Hilight(_selectedIndex);
        }

        protected void Hilight(int index)
        {
            _textData[index].Background = Brushes.LightGray;
        }

        protected void Unhilight(int index)
        {
            _textData[index].Background = Brushes.White;
        }

        public void MoveVertically(int i)
        {
            Unhilight(_selectedIndex);
            _selectedIndex += i;
            if (_selectedIndex >= _textData.Length)
                _selectedIndex = _textData.Length - 1;
            if (_selectedIndex < 0)
                _selectedIndex = 0;
            Hilight(_selectedIndex);
        }

        public new void MoveHorizontally(int i)
        {
        }

        public new void EndOfLine()
        {
        }

        public new void BeginningOfLine()
        {
        }

        public new abstract void NextLine();
    }

    public class VIMMRUControl : VIMDirectoryControlBase
    {
        public VIMMRUControl()
        {
        }

        public VIMMRUControl(IVIMContainer parent) : base(parent)
        {
        }

        protected override void UpdateData()
        {
            var sql = "select top 10 data, class_name from image_data order by id desc";
            var conn = new SqlConnection("Server=localhost;Initial catalog=vim_persist;UID=sa;PWD=d0nkey;");
            conn.Open();
            var cmd = new SqlCommand(sql, conn);
            var rdr = cmd.ExecuteReader();

            var displayLines = new List<string>();
            while (rdr.Read())
            {
                var className = rdr["class_name"].ToString();
                var data = rdr["data"] as byte[];
                if (className.IndexOf("Enumerable") > 0)
                {
                    var stream = new MemoryStream(data);
                    var list = Serializer.Deserialize<List<KeyValuePair<string, string>>>(stream);
                    var line = "";
                    if (list.Count >= 1)
                        line += list[0].Key + ": " + list[0].Value + ";";
                    if (list.Count >= 2)
                        line += list[1].Key + ": " + list[1].Value + ";";
                    displayLines.Add(line);
                }
            }

            _itemList = displayLines.ToArray();
        }

        public override void NextLine()
        {
        }
    }

    public class VIMDirectoryControl : VIMDirectoryControlBase
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