using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace VIMControls.Controls
{
    public static class Sql
    {
        private const string _connString = "Server=localhost;Initial catalog=vim_persist;UID=sa;PWD=d0nkey;";

        public static IEnumerable<IDictionary<string, object>> Exec(string sql)
        {
            using (var conn = new SqlConnection(_connString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr == null) yield break;
                    while (rdr.Read())
                    {
                        var dict = new Dictionary<string, object>();
                        Enumerable.Range(0, rdr.FieldCount)
                            .Do(i => dict[rdr.GetName(i)] = rdr[i]);
                        yield return dict;
                    }
                }
            }
        }
    }
}
