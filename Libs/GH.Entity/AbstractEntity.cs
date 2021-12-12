using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace GH.Entity
{
    public class AbstractEntity : IEditableObject
    {
        private object _savedCopy;

        public void BeginEdit()
        {
            if (_savedCopy != null)
                return;
            _savedCopy = this.MemberwiseClone();
        }

        public virtual void CancelEdit()
        {
            if (_savedCopy == null)
                return;

            var copyProps = GetPropertys(_savedCopy as IEditableObject);
            foreach (var prop in GetPropertys(this))
            {
                var copyProp = copyProps.First(x => x.Name == prop.Name);
                prop.SetValue(this, copyProp.GetValue(_savedCopy));
            }
            _savedCopy = null;

        }

        public virtual void EndEdit()
        {
            _savedCopy = null;
        }

        public bool IsEdit()
        {
            return _savedCopy != null;
        }


        public static IEnumerable<PropertyInfo> GetPropertys(IEditableObject model)
        {
            return from p in model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.FlattenHierarchy)
                   where p.SetMethod != null
                   select p;
        }

    }
}
