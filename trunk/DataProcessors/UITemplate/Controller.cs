using System.Collections.Generic;
using System.Data.SqlClient;
using ActionDictionary.Interfaces;
using AppControlInterfaces.ListView;
using DataProcessors;

namespace DataProcessors.UITemplate
{
    //TODO: I thought about combining the Dual IListViewData/IListViewUpdate model into one file...
    //the data could be a sub interface within the abstract ListViewUpdate class?
    [Launchable("New Issue Tracker")]
    public class Controller : IListViewData, IPaging
    {
        private readonly List<List<string>> _displayData = new List<List<string>>();

        public Controller()
        {
            var conn = new SqlConnection("Data Source=localhost;Initial Catalog=bug_tracking;Integrated Security=SSPI;");
            conn.Open();
            var cmd = new SqlCommand("select creation_date, severity_id, short_desc from issue", conn);
            var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var list = new List<string>
                               {
                                   rdr["creation_date"].ToString(),
                                   rdr["severity_id"].ToString(),
                                   rdr["short_desc"].ToString()
                               };
                _displayData.Add(list);
            }
        }

        public string GetData(int row, int col)
        {
            return _displayData[row][col];
        }

        public int RowCount
        {
            get { return _displayData.Count; }
        }

        public int ColCount
        {
            get { return _displayData[0].Count; }
        }

        public int HilightIndex
        {
            get { return 0; }
        }

        public IListViewUpdate Updater
        {
            set { throw new System.NotImplementedException(); }
        }

        public void MoveUp()
        {
            throw new System.NotImplementedException();
        }

        public void MoveDown()
        {
            throw new System.NotImplementedException();
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
