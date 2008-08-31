using System.Linq;
using VIMControls.Contracts;
using VIMControls.Controls.Misc;

namespace VIMControls.Controls
{
    public interface IListController : IVIMMotionController, IVIMCharacterController, IVIMControl
    {
        void Select(int index);
    }

    public interface ICanvasChild
    {
        bool Fill { get; }
        double Height { set; }
        double Width { set; }
    }

    public abstract class VIMListBrowser : VIMTextControl, IListController, ICanvasChild
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
        }

        public void MoveVertically(int i)
        {
            _selectedIndex += i;
            if (_selectedIndex >= _textData.Length)
                _selectedIndex = _textData.Length - 1;
            if (_selectedIndex < 0)
                _selectedIndex = 0;
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
        public void Select(int index)
        {
            _selectedIndex = index;
            NextLine();
        }

        public bool Fill
        {
            get { return true; }
        }
    }

    public class ListMotionWrapper : IVIMMotionController
    {
        private readonly IVIMListMotionController _controller;

        public ListMotionWrapper(IVIMListMotionController controller)
        {
            _controller = controller;
        }

        public void MoveVertically(int i)
        {
            _controller.MoveVertically(i);
        }

        public void NextLine()
        {
            _controller.NextLine();
        }

        public void MoveHorizontally(int i)
        {
        }

        public void EndOfLine()
        {
        }

        public void BeginningOfLine()
        {
        }
    }

    public class VIMListCursor
    {}
}