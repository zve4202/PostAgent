using System.ComponentModel;
using System.Drawing;

namespace GH.Forms
{
    [ToolboxItem(true)]
    [ToolboxItemFilter("GH Controls")]
    public class ButtonCancel : BaseButton
    {
        public ButtonCancel()
        {
            BackColor = Color.OrangeRed;
            ForeColor = Color.White;
        }
    }
}
