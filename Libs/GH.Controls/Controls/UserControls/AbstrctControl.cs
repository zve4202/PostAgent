using System.ComponentModel;
using System.Windows.Forms;

namespace GH.Controls
{
    [ToolboxItem(true)]
    [ToolboxItemFilter("GH Controls")]
    public class AbstrctControl : UserControl
    {
        public BindingSource dataSource;
        private IContainer components;

        public AbstrctControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataSource)).BeginInit();
            this.SuspendLayout();
            // 
            // AbstrctControl
            // 
            this.Name = "AbstrctControl";
            this.Size = new System.Drawing.Size(554, 309);
            ((System.ComponentModel.ISupportInitialize)(this.dataSource)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
