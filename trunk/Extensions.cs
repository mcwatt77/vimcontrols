using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;

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
            var cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.StoredProcedure;
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

        public static TData Load<TData>(int id)
        {
            return default(TData);
        }
    }
}