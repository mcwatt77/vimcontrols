using System.Linq;
using ActionDictionary.Interfaces;
using UITemplateViewer.Element;

namespace UITemplateViewer.Controllers
{
    public class EntityListController : IPaging
    {
        private int _selectedIndex;

        public IEntitySelector EntityList { get; set; }

        public void MoveUp()
        {
            if (_selectedIndex <= 0) return;

            EntityList.SelectedRow = EntityList.Rows.Skip(--_selectedIndex).First();
        }

        public void MoveDown()
        {
            if (_selectedIndex + 1 >= EntityList.Rows.Count()) return;

            EntityList.SelectedRow = EntityList.Rows.Skip(++_selectedIndex).First();
        }

        public void Beginning()
        {
            throw new System.NotImplementedException();
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