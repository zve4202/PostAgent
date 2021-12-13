using System.ComponentModel;
using System.Windows.Forms;

namespace GH.Controls.Utils.Controls
{
    public class BaseControlBoundsProvider : ControlBoundsProvider
    {
        public override Control GetOwnerControl(IComponent component)
        {
            return (Control)(component as BaseControl);
        }
    }

}
