using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GH.Controls.Utils.Controls
{
    public class ControlBoundsProvider : ISmartTagClientBoundsProvider
    {
        protected virtual Point OffsetLocation
        {
            get
            {
                return new Point(-5, -8);
            }
        }

        public virtual Rectangle GetBounds(IComponent component)
        {
            Control ownerControl = this.GetOwnerControl(component);
            return ownerControl != null ? new Rectangle(this.OffsetLocation, ownerControl.Bounds.Size) : Rectangle.Empty;
        }

        public virtual Control GetOwnerControl(IComponent component)
        {
            return component as Control;
        }
    }

}
