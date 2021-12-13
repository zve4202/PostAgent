using System.ComponentModel;
using System.Windows.Forms;

namespace GH.Controls.Utils.Controls
{
    public class ControlActions : ComponentActions
    {
        public virtual void DockInParentContainer(IComponent component)
        {
            if (!(component is Control control))
                return;
            control.Dock = DockStyle.Fill;
        }

        public virtual void UndockFromParentContainer(IComponent component)
        {
            if (!(component is Control control))
                return;
            control.Dock = DockStyle.None;
        }
    }

}
