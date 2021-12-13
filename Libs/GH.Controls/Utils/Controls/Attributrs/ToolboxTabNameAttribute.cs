using System;

namespace GH.Controls.Utils.Controls
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ToolboxTabNameAttribute : Attribute
    {
        private string tabName;

        public ToolboxTabNameAttribute(string tabName)
        {
            this.tabName = tabName;
        }

        public string TabName
        {
            get
            {
                return this.tabName;
            }
        }
    }

}
