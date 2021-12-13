using System.ComponentModel;
using System.Windows.Forms;

namespace GH.Controls.Utils.Controls
{
    public class ControlFilter : ISmartTagFilter
    {
        public Control Control { get; internal set; }

        protected virtual bool AllowDock
        {
            get
            {
                return false;
            }
        }

        public virtual void SetComponent(IComponent component)
        {
            this.Control = component as Control;
        }

        public virtual bool FilterMethod(string MethodName, object actionMethodItem)
        {
            return this.Control == null || (this.AllowDock || !(MethodName == "DockInParentContainer") && !(MethodName == "UndockFromParentContainer")) && ((this.Control.Dock != DockStyle.Fill || !(MethodName == "DockInParentContainer")) && (this.Control.Dock == DockStyle.Fill || !(MethodName == "UndockFromParentContainer")));
        }

        public virtual bool FilterProperty(MemberDescriptor descriptor)
        {
            return true;
        }
    }

}
