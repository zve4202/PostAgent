using System.ComponentModel;
using System.Drawing;

namespace GH.Controls
{
    [ToolboxItem(true)]
    [ToolboxItemFilter("GH Controls")]
    public class ButtonOK : BaseButton
    {
        public ButtonOK()
        {
            BackColor = Color.CornflowerBlue;
            ForeColor = Color.White;
        }
    }
}
