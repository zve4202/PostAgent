using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace GH.Forms
{
    [ToolboxItem(true)]
    [ToolboxItemFilter("GH Controls")]
    public class DatasourceControl : AbstractControl
    {
        public BindingSource dataSource;
        private IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public DatasourceControl()
        {
            components = new System.ComponentModel.Container();
            dataSource = new System.Windows.Forms.BindingSource(components);
        }

        public void Open()
        {
            dataSource.DataSource = GetBindingList();
        }

        public virtual IList GetBindingList()
        {
            return (IList)null;
        }

    }
}
