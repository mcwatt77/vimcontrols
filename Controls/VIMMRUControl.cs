using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VIMControls.Controls
{
    public class VIMMRUControl : VIMListBrowser
    {
        public static Guid Guid = new Guid("{F93410EB-5E8A-44c5-9A38-79AE094DA53F}");
        private IEnumerable<IEnumerable<KeyValuePair<string, string>>> _itemData;

        public VIMMRUControl()
        {
        }

        public VIMMRUControl(IVIMContainer parent) : base(parent)
        {
        }

        protected override void UpdateData()
        {
            var sql = string.Format("select top 10 data, class_name from image_data where class_name like 'system.linq.enumerable%' and guid = '{0}' order by id desc", Guid);

            var sqlResult = Sql.Exec(sql)
                .Select(dict => new KeyValuePair<string, object>(dict["class_name"].ToString(), dict["data"]))
                .ToList();

            _itemData = sqlResult
                .Where(keyValuePair => keyValuePair.Key.IndexOf("Enumerable") > 0)
                .Select(keyValuePair => Serializer.Deserialize<List<KeyValuePair<string, string>>>(new MemoryStream((byte[])keyValuePair.Value)).AsEnumerable())
                .ToList();

            _itemList = _itemData
                .Select(list =>
                            {
                                var line = "";
                                if (list.Count() >= 1)
                                    line += list.First().Key + ": " + list.First().Value + ";";
                                if (list.Count() >= 2)
                                    line += list.Skip(1).First().Key + ": " + list.Skip(1).First().Value + ";";
                                return line;
                            })
                .ToArray();
        }

        public override void NextLine()
        {
            var item = _itemData.Skip(_selectedIndex).First();
            _parent.Navigate(item);
        }
    }
}