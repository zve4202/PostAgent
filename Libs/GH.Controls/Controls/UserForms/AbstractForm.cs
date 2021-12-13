using GH.Forms;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace GH.Controls.UserForms
{
    public class AbstractForm : Form
    {
        public bool IsDesignMode => this.IsDesignMode();
        public bool IsRuntimeMode => !IsDesignMode;

        public AbstractForm()
        {
            Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().ProcessName);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (IsRuntimeMode)
            {
                Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }

            base.OnLoad(e);
        }
    }
}
