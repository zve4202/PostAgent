using System;

namespace GH.Controls.Utils.Controls
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SmartTagSupportAttribute : Attribute
    {
        private Type boundsProviderType;
        private SmartTagSupportAttribute.SmartTagCreationMode creationType;

        public SmartTagSupportAttribute(
          Type boundsProviderType,
          SmartTagSupportAttribute.SmartTagCreationMode creationType)
        {
            this.creationType = creationType;
            this.boundsProviderType = boundsProviderType;
        }

        public Type BoundsProviderType
        {
            get
            {
                return this.boundsProviderType;
            }
        }

        public SmartTagSupportAttribute.SmartTagCreationMode CreationType
        {
            get
            {
                return this.creationType;
            }
        }

        public enum SmartTagCreationMode
        {
            UseComponentDesigner,
            Auto,
        }
    }

}
