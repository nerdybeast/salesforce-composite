using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace salesforce_composite
{
    public static class ObjectExtensions
    {
        public static T PrependValueToStringProperties<T>(this T obj, string value)
        {
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in properties)
            {
                if ((p.PropertyType != typeof(string)) || !p.CanWrite || !p.CanRead)
                {
                    continue;
                }

                MethodInfo mSet = p.GetSetMethod(false);
                if (mSet == null)
                {
                    continue;
                }

                p.SetValue(obj, $"@{{{value}.{p.Name}}}");
            }

            foreach (FieldInfo f in fields)
            {
                if (f.FieldType != typeof(string))
                {
                    continue;
                }

                f.SetValue(obj, $"@{{{value}.{f.Name}}}");
            }

            return obj;
        }
    }
}
