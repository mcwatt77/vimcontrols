using System.Collections.Generic;
using ActionDictionary.Interfaces;
using AppControlInterfaces.NoteViewer;

namespace DataProcessors.NoteViewer
{
    public class LeftNavController : ILeftNavData, IFullNavigation
    {
        //TODO: This should be able to initialize with an empty updater...
        public INoteViewUpdate Updater { get; set; }
        private readonly List<Controller.NoteData> _data;

        public LeftNavController(List<Controller.NoteData> data)
        {
            _data = data;
        }

        public string GetData(int row, int col)
        {
            return _data[row].Desc;
        }

        public int RowCount
        {
            get { return _data.Count; }
        }

        public int ColCount
        {
            get { return 1; }
        }

        public int HilightIndex { get; private set; }

        public void MoveUp()
        {
            HilightIndex--;
            if (HilightIndex < 0) HilightIndex = 0;
            else
            {
                Updater.Update(HilightIndex + 1);
                Updater.Update(HilightIndex);
                Updater.UpdateTextRows();
            }
        }

        public void MoveDown()
        {
            HilightIndex++;
            if (HilightIndex >= RowCount) HilightIndex = RowCount - 1;
            else
            {
                Updater.Update(HilightIndex - 1);
                Updater.Update(HilightIndex);
                Updater.UpdateTextRows();
            }
        }

        public void Beginning()
        {
            var oldIndex = HilightIndex;
            HilightIndex = 0;
            Updater.Update(oldIndex);
            Updater.Update(HilightIndex);
        }

        public void End()
        {
            var oldIndex = HilightIndex;
            HilightIndex = RowCount - 1;
            Updater.Update(oldIndex);
            Updater.Update(HilightIndex);
        }

        public void PageUp()
        {
        }

        public void PageDown()
        {
        }

        public void MoveRight()
        {
        }

        public void MoveLeft()
        {
        }
    }
}