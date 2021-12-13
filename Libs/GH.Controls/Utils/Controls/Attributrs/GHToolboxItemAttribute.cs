using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

namespace GH.Controls.Utils.Controls
{
    [ClassInterface(ClassInterfaceType.None)]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GHToolboxItemAttribute : ToolboxItemAttribute
    {
        protected GHToolboxItemAttribute(GHToolboxItemKind kind, string toolboxTypeName)
          : base(toolboxTypeName)
        {
            if (kind != GHToolboxItemKind.Regular || this.Check() != 1)
                return;
            this.Disable();
        }

        public GHToolboxItemAttribute(GHToolboxItemKind kind)
          : base(kind != GHToolboxItemKind.Hidden)
        {
            if (kind != GHToolboxItemKind.Regular || this.Check() != 1)
                return;
            this.Disable();
        }

        public GHToolboxItemAttribute(bool defaultType)
          : this(defaultType ? GHToolboxItemKind.Regular : GHToolboxItemKind.Hidden)
        {
        }

        protected virtual int Check()
        {
            return Utility.IsOnlyWin();
        }

        private void Disable()
        {
            try
            {
                typeof(ToolboxItemAttribute).GetField("toolboxItemTypeName", BindingFlags.Instance | BindingFlags.NonPublic).SetValue((object)this, (object)null);
            }
            catch
            {
            }
        }

        public override object TypeId
        {
            get
            {
                return ToolboxItemAttribute.Default.TypeId;
            }
        }
    }

}
