using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace PostAgent.domain
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

        public void CancelEdit()
        {
            if (_savedCopy == null)
                return;

            var copyProps = GetWritePropertys(_savedCopy as IEditableObject);
            foreach (var prop in GetWritePropertys(this))
            {
                var copyProp = copyProps.First(x => x.Name == prop.Name);
                prop.SetValue(this, copyProp.GetValue(_savedCopy));
            }
            EndEdit();
        }

        public void EndEdit()
        {
            _savedCopy = null;
        }

        public static IEnumerable<PropertyInfo> GetWritePropertys(IEditableObject model)
        {
            return from p in model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.FlattenHierarchy)
                   where p.SetMethod != null
                   select p;
        }

    }
}
