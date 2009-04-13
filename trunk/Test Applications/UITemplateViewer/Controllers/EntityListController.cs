using System.Linq;
using ActionDictionary.Interfaces;
using UITemplateViewer.Element;

namespace UITemplateViewer.Controllers
{
    public class EntityListController : IPaging
    {
        private int _selectedIndex;

        public IEntitySelector EntityList { get; set; }
        public IEntitySelector Selector { get; set; }

        public void MoveUp()
        {
            if (Selector != null)
            {
                if (_selectedIndex <= 0) return;
                Selector.SelectedRow = Selector.Rows.Skip(--_selectedIndex).First();
                return;
            }
            if (_selectedIndex <= 0) return;

            EntityList.SelectedRow = EntityList.Rows.Skip(--_selectedIndex).First();
        }

        public void MoveDown()
        {
            if (Selector != null)
            {
                if (_selectedIndex + 1 >= Selector.Rows.Count()) return;
                Selector.SelectedRow = Selector.Rows.Skip(++_selectedIndex).First();
                return;
            }
            if (_selectedIndex + 1 >= EntityList.Rows.Count()) return;

            EntityList.SelectedRow = EntityList.Rows.Skip(++_selectedIndex).First();
        }

        public void Beginning()
        {
            EntityList.SelectedRow = EntityList.Rows.First();
        }

        public void End()
        {
            throw new System.NotImplementedException();
        }

        public void PageUp()
        {
            throw new System.NotImplementedException();
        }

        public void PageDown()
        {
            throw new System.NotImplementedException();
        }
    }
}