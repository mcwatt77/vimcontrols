using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using VIMControls.Controls;

namespace VIMControls
{
    public static class Extensions
    {
        public static IEnumerable<Type> GetImplementations(this Type interfaceType)
        {
            return typeof (Services).Assembly
                .GetTypes()
                .Where(type => interfaceType.IsAssignableFrom(type)
                               && !type.IsAbstract);
        }

        public static void Do<TSource>(this IEnumerable<TSource> src, Action<TSource> fn)
        {
            foreach (TSource item in src)
                fn(item);
        }
        
        public static void Do<TSource>(this IEnumerable<TSource> src, Action<int, TSource> fn)
        {
            var i = 0;
            foreach (TSource item in src)
                fn(i++, item);
        }

        public static Type SystemType(this MemberInfo src)
        {
            if (src is PropertyInfo) return ((PropertyInfo) src).PropertyType;
            if (src is FieldInfo) return ((FieldInfo) src).FieldType;
            return null;
        }

        public static int IndexOf<TItem>(this IEnumerable<TItem> src, TItem item)
        {
            var e = src.GetEnumerator();
            var i = 0;
            while (e.MoveNext())
            {
                if (e.Current.Equals(item))
                    return i;
                i++;
            }
            return -1;
        }

        public static IEnumerable<TItem> Flatten<TItem>(this IEnumerable<IEnumerable<TItem>> src)
        {
            foreach (var list in src)
                foreach (var item in list)
                    yield return item;
        }

        public static IEnumerable<MethodInfo> GetMethodsWithCustomAttribute<T>(this Assembly assembly)
        {
            return assembly
                .GetTypes()
                .Select(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static).AsEnumerable())
                .Flatten()
                .Where(method => method.AttributesOfType<T>().Count() == 1);
        }

        public static IEnumerable<TAttributeType> AttributesOfType<TAttributeType>(this ICustomAttributeProvider attr)
        {
            return attr
                .GetCustomAttributes(false)
                .Where(o => o.GetType() == typeof (TAttributeType))
                .Cast<TAttributeType>();
        }

        public static TDelegateType CreateDelegate<TDelegateType>(this MethodInfo method)
        {
            object o = Delegate.CreateDelegate(typeof (TDelegateType), method);
            return (TDelegateType) o;
        }

        public static bool HasSetter(this PropertyInfo propertyInfo)
        {
            var accessors = propertyInfo.GetAccessors();
            if (accessors == null) return false;
            return accessors.Count() == 2;
        }

        public static string Delimit<TItem>(this IEnumerable<TItem> items, string delimiter)
        {
            var sb = new StringBuilder();
            var e = items.GetEnumerator();
            var hasRecords = e.MoveNext();
            if (hasRecords)
            {
                var prevRecord = e.Current;

                while (e.MoveNext())
                {
                    sb.Append(prevRecord.ToString());
                    sb.Append(delimiter);
                    prevRecord = e.Current;
                }

                sb.Append(prevRecord);
            }

            return sb.ToString();
        }

        public static int Persist(this object src)
        {
            return Persist(src, null);
        }

        public static int Persist(this object src, Guid? guid)
        {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, src);

            var sql = "binary_data_insert";
            var conn = new SqlConnection("Data Source=localhost;Initial Catalog=vim_persist;UID=sa;PWD=d0nkey");
            conn.Open();
            var cmd = new SqlCommand(sql, conn) {CommandType = CommandType.StoredProcedure};
            cmd.Parameters.Add("data", SqlDbType.Image).Value = stream.GetBuffer();
            cmd.Parameters.Add("class_name", SqlDbType.VarChar, 2000).Value = src.GetType().ToString();
            if (guid != null)
                cmd.Parameters.Add("guid", SqlDbType.UniqueIdentifier).Value = guid;
            cmd.ExecuteNonQuery();

            return  0;
        }

        public static void Persist(this object src, int id)
        {
        }

        public static TData Load<TData>(this Guid guid) where TData : class
        {
            var fields = Sql.Exec(string.Format("select top 1 data, class_name from image_data where guid = '{0}' order by id desc", guid))
                .FirstOrDefault();
            if (fields == null) return null;

            var objVal = fields["data"];
            var stream = new MemoryStream((byte[]) objVal);

            if (!typeof(TData).IsAbstract) return Serializer.Deserialize<TData>(stream);

            var classType = Type.GetType(fields["class_name"].ToString());
            return (TData)typeof (Serializer)
                .GetMethod("Deserialize")
                .MakeGenericMethod(new[] {classType})
                .Invoke(null, new object[] {stream});
        }
    }
}