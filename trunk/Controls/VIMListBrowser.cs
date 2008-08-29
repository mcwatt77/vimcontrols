using System.Linq;
using System.Windows.Media;

namespace VIMControls.Controls
{
    public abstract class VIMListBrowser : VIMTextControl, IDirectoryBrowser
    {
        protected IVIMContainer _parent;
        protected string _directory = "computer";
        protected string[] _itemList;
        protected int _selectedIndex;

        protected VIMListBrowser() : this(null)
        {
        }

        protected VIMListBrowser(IVIMContainer parent)
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

    public class VIMListCursor
    {}
}