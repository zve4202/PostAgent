using System.ComponentModel;
using System.Windows.Forms;

namespace GH.Forms
{
    [ToolboxItem(true)]
    [ToolboxItemFilter("GH Controls")]
    public class AbstractControl : UserControl
    {

        public AbstractControl()
        {
            Padding = new Padding(10);
            this.Size = new System.Drawing.Size(434, 334);
        }

    }
}
