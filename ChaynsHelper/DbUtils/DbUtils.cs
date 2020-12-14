using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace ChaynsHelper.DbUtils
{
    public static class DbUtils
    {
        public static Func<T, TListed, T> CreateOneToManyMapper<T, TListed>(
            Func<T, int> idSelector,
            Func<T, IEnumerable<TListed>> listGetter,
            Action<T, IEnumerable<TListed>> listSetter) where T : new() where TListed : new()
        {
            var dictionary = new Dictionary<int, T>();
            return (element, listed) =>
            {
                if (!dictionary.TryGetValue(idSelector(element), out var entry))
                {
                    entry = element;
                    listSetter(entry, new List<TListed>());
                    dictionary.Add(idSelector(element), element);
                }

                if (listed != null)
                {
                    listSetter(entry, (listGetter(entry) ?? new List<TListed>()).Append(listed));
                }

                return entry;
            };
        }

        public static Func<T, TListed, T> CreateOneToManyMapper<T, TListed>()
            where T : new()
        {
            if (!OneToManyMappingAttribute.IsValid<T, TListed>())
            {
                throw new Exception(
                    "[DbUtils] Must use OneToManyMapping Attribute to specify key and list properties");
            }

            var key = OneToManyMappingAttribute.GetKeyName<T>();
            var list = OneToManyMappingAttribute.GetListName<T, TListed>();

            var dictionary = new Dictionary<int, T>();
            return (element, listed) =>
            {
                var id = (int) typeof(T).GetProperty(key).GetValue(element);
                if (!dictionary.TryGetValue(id, out var entry))
                {
                    entry = element;
                    typeof(T).GetProperty(list).SetValue(entry, new List<TListed>());
                    dictionary.Add(id, element);
                }

                if (listed == null) return entry;

                var originalList = (typeof(T).GetProperty(list)
                    .GetValue(entry) ?? new List<TListed>()) as IEnumerable<TListed>;
                typeof(T).GetProperty(list)
                    .SetValue(entry, originalList.Append(listed));

                return entry;
            };
        }

        // Convert IEnumerable to DateTable by Eike Wolff
        public static DataTable ToDataTable<T>(this IEnumerable<T> iList)
        {
            var dataTable = new DataTable();
            var propertyDescriptorCollection =
                TypeDescriptor.GetProperties(typeof(T), new Attribute[] {new ColumnAttribute()});

            for (var i = 0; i < propertyDescriptorCollection.Count; i++)
            {
                var propertyDescriptor = propertyDescriptorCollection[i];

                var type = propertyDescriptor.PropertyType;
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    type = Nullable.GetUnderlyingType(type);
                dataTable.Columns.Add(propertyDescriptor.Name, type);
            }

            var values = new object[propertyDescriptorCollection.Count];
            foreach (var iListItem in iList)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    values[i] = propertyDescriptorCollection[i].GetValue(iListItem);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
    }

    public enum MappingAttributeType
    {
        Key,
        List
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class OneToManyMappingAttribute : Attribute
    {
        public static string GetKeyName<T>() where T : new()
        {
            var type = typeof(T);
            var props = type.GetProperties();
            return (from prop in props
                let attList = prop.GetCustomAttributes().ToList()
                from attributeInfo in attList
                let att = (OneToManyMappingAttribute) attributeInfo
                where att.MapType == MappingAttributeType.Key && prop.PropertyType == typeof(int)
                select prop.Name).FirstOrDefault();
        }

        public static string GetListName<T, TList>() where T : new()
        {
            var type = typeof(T);
            var props = type.GetProperties();
            return (from prop in props
                let attList = prop.GetCustomAttributes().ToList()
                from attributeInfo in attList
                let att = (OneToManyMappingAttribute) attributeInfo
                where att.MapType == MappingAttributeType.List &&
                      prop.PropertyType == typeof(IEnumerable<TList>)
                select prop.Name).FirstOrDefault();
        }

        public static bool IsValid<T, TList>() where T : new()
        {
            var hasKey = false;
            var hasList = false;

            var type = typeof(T);
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                var attList = prop.GetCustomAttributes().ToList();
                foreach (var att in attList.Cast<OneToManyMappingAttribute>())
                {
                    if (att.MapType == MappingAttributeType.Key) hasKey = true;
                    if (att.MapType == MappingAttributeType.List &&
                        prop.PropertyType == typeof(IEnumerable<TList>))
                    {
                        hasList = true;
                    }
                }
            }

            return hasKey && hasList;
        }

        public MappingAttributeType MapType { get; }

        public OneToManyMappingAttribute(MappingAttributeType mapType)
        {
            MapType = mapType;
        }
    }
}