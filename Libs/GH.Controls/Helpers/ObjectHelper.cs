using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace GH.Forms
{
    public static class ObjectHelper
    {
        public static IContainer GetComponents(Control control)
        {
            var components = EnumerateFields<IContainer>(control);
            return components.FirstOrDefault();
        }

        public static IEnumerable<T> EnumerateFields<T>(object obj)
        {
            return from x in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty)
                   where typeof(T).IsAssignableFrom(x.FieldType)
                   let f = x.GetValue(obj)
                   where f != null
                   select (T)f;
        }

    }
}
