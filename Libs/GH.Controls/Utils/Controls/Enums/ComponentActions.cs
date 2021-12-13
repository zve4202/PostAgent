using System.ComponentModel;
using System.ComponentModel.Design;

namespace GH.Controls.Utils.Controls
{
    public class ComponentActions
    {
        protected virtual IDesigner GetDesigner(IComponent component)
        {
            return ((IDesignerHost)component?.Site.GetService(typeof(IDesignerHost))).GetDesigner(component);
        }
    }

}
