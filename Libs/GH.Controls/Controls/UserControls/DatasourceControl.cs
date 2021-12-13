using System.ComponentModel;
using System.Windows.Forms;

namespace GH.Forms
{
    public class DatasourceControl : UserControl
    {
        public BindingSource dataSource;
        private IContainer components;

        public DatasourceControl()
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
            // DatasourceControl
            // 
            this.Name = "DatasourseControl";
            this.Size = new System.Drawing.Size(554, 309);
            ((System.ComponentModel.ISupportInitialize)(this.dataSource)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
