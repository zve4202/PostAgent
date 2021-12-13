using System;

namespace GH.Controls.Utils.Controls
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class SmartTagActionAttribute : Attribute
    {
        private Type type;
        private string methodName;
        private string displayName;
        private int sortOrder;
        private SmartTagActionType finalAction;

        public SmartTagActionAttribute(string displayName)
          : this((Type)null, (string)null, displayName)
        {
        }

        public SmartTagActionAttribute(Type type, string methodName, string displayName)
          : this(type, methodName, displayName, SmartTagActionType.None)
        {
        }

        public SmartTagActionAttribute(
          Type type,
          string methodName,
          string displayName,
          SmartTagActionType finalAction)
          : this(type, methodName, displayName, -1, finalAction)
        {
        }

        public SmartTagActionAttribute(
          Type type,
          string methodName,
          string displayName,
          int sortOrder,
          SmartTagActionType finalAction)
        {
            this.type = type;
            this.methodName = methodName;
            this.displayName = displayName;
            this.sortOrder = sortOrder;
            this.finalAction = finalAction;
        }

        public Type Type
        {
            get
            {
                return this.type;
            }
        }

        public string MethodName
        {
            get
            {
                return this.methodName;
            }
        }

        public string DisplayName
        {
            get
            {
                return this.displayName;
            }
        }

        public int SortOrder
        {
            get
            {
                return this.sortOrder;
            }
        }

        public SmartTagActionType FinalAction
        {
            get
            {
                return this.finalAction;
            }
        }
    }

}
