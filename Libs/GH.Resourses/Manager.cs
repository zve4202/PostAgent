using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Resources;

namespace GH.Resourses
{
    public static class Manager
    {
        private static ManagerClass managerClass = null;

        public static T GetResouce<T>(string name)
        {
            return (managerClass ?? new ManagerClass()).GetResouce<T>(name);
        }

    }
    internal class ManagerClass
    {
        private ResourceManager resource;
        public ManagerClass()
        {
            resource = new ResourceManager(nameof(Manager), typeof(ManagerClass).Assembly);
        }

        internal T GetResouce<T>(string name)
        {
            try
            {
                object o = resource.GetObject(name);
                if (o is T type)
                    return type;
            }
            catch { }

            return (T)Activator.CreateInstance(typeof(T));
        }
    }
}
