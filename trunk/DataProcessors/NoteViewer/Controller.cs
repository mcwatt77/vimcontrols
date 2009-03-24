using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Media;
using ActionDictionary;
using ActionDictionary.Interfaces;
using AppControlInterfaces.ListView;
using AppControlInterfaces.NoteViewer;

namespace DataProcessors.NoteViewer
{
    //TODO: Need to generate a proxy class... where I don't need to implement INavigation, but it will still get called on my children...
    //and also I don't need to be invoked through an action... a direct invocation will still route correctly

    //TODO: IPaging gets directed to TextMetricAdapter
    [Launchable("Notes Viewer")]
    public class Controller : INoteViewData, IControlKeyProcessor, IMissing, IListViewUpdate
    {
        //TODO: This should be able to initialize with an empty updater...
        private INoteViewUpdate _updater;
        private readonly List<NoteData> _data = new List<NoteData>();
        private readonly LeftNavController _leftNavController;
        private IFullNavigation _currentNav;
        private readonly TextMetricAdapter _textMetricAdapter;

        public Controller()
        {
            var connString = "Data Source=localhost;Initial Catalog=vim_persist;UID=sa;PWD=d0nkey;";
            var conn = new SqlConnection(connString);
            conn.Open();
            var sql = "select descr, body from note";
            var cmd = new SqlCommand(sql, conn);
            var rdr = cmd.ExecuteReader();
            if (rdr == null) throw new Exception(".NET stopped working!");

            while (rdr.Read()) _data.Add(NoteData.FromReader(rdr));

            _leftNavController = new LeftNavController(_data) {Updater = this};
            var cursor = new TextCursor();
            Cursor = cursor;

            _currentNav = _leftNavController;

            _textMetricAdapter = new TextMetricAdapter(cursor) {TextProvider = _data[HilightIndex].Body};
        }

        public class NoteData
        {
            public string Desc { get; set; }
            public TextProvider Body { get; set; }

            public static NoteData FromReader(IDataRecord rdr)
            {
                return new NoteData
                               {
                                   Desc = rdr["descr"].ToString(),
                                   Body = new TextProvider(rdr["body"].ToString())
                               };
            }
        }

        public string GetData(int row, int col)
        {
            return _leftNavController.GetData(row, col);
        }

        public int RowCount
        {
            get { return _leftNavController.RowCount; }
        }

        public int ColCount
        {
            get { return _leftNavController.ColCount; }
        }

        public int HilightIndex
        {
            get
            {
                return _leftNavController.HilightIndex;
            }
        }

        public FormattedText GetTextRow(int row)
        {
            return _textMetricAdapter.GetRow(row);
        }

        public int TextRowCount
        {
            get
            {
                return _textMetricAdapter.RowsByHeight().Count();
            }
        }

        public int TopTextRow
        {
            get
            {
                return _textMetricAdapter.TopTextRow;
            }
        }

        public ITextCursor Cursor { get; private set; }

        public INoteViewUpdate Updater
        {
            set
            {
                _updater = value;
                _textMetricAdapter.Updater = value;
            }
        }

        private double _height;
        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                _textMetricAdapter.Height = value;
                _updater.UpdateTextRows();
            }
        }

        public double Width { get; set; }

        public void Enter()
        {
            _currentNav = _textMetricAdapter;
            _updater.UpdateCursor();
        }

        public void WindowScroll()
        {
            if (_currentNav == _textMetricAdapter) _currentNav = _leftNavController;
        }

        public void LocalScroll()
        {
        }

        public object ProcessMissingCmd(Message msg)
        {
            if (msg.MethodType == typeof(ILeftNavData)) msg.Invoke(_leftNavController);
            if (msg.MethodType == typeof(ITextData)) msg.Invoke(_textMetricAdapter);

            return msg.Invoke(_currentNav);
        }

        public void Update(int row, int col)
        {
            _textMetricAdapter.TextProvider = _data[HilightIndex].Body;
            _updater.Update(row, col);
        }

        public void Update(int row)
        {
            _textMetricAdapter.TextProvider = _data[HilightIndex].Body;
            _updater.Update(row);
        }
    }
}