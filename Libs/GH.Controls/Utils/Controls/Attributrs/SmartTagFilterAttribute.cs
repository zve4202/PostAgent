using System;

namespace GH.Controls.Utils.Controls
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SmartTagFilterAttribute : Attribute
    {
        private Type filterProviderType;

        public SmartTagFilterAttribute(Type filterProviderType)
        {
            this.filterProviderType = filterProviderType;
        }

        public Type FilterProviderType
        {
            get
            {
                return this.filterProviderType;
            }
        }
    }

}
