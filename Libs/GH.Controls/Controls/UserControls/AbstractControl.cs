using System.ComponentModel;
using System.Windows.Forms;

namespace GH.Forms
{
    [ToolboxItem(true)]
    [ToolboxItemFilter("GH Controls")]
    public class AbstractControl : UserControl
    {
        private IContainer components;

        public AbstractControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            // 
            // AbstrctControl
            // 
            this.Name = "AbstrctControl";
            this.Size = new System.Drawing.Size(554, 309);
            this.ResumeLayout(false);
        }
    }
}
