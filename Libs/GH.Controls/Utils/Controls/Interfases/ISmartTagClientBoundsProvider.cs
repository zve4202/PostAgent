using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GH.Controls.Utils.Controls
{
    public interface ISmartTagClientBoundsProvider
    {
        Rectangle GetBounds(IComponent component);

        Control GetOwnerControl(IComponent component);
    }

}
