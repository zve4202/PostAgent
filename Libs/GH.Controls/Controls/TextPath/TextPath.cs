using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GH.Forms
{
    [ToolboxItem(true)]
    [ToolboxItemFilter("GH Controls")]
    public class TextPath : TextBox
    {
        private readonly Button button;
        private OpenFileDialog openFileDialog;

        public event EventHandler ButtonClick
        {
            add
            {
                button.Click -= buttonClick;
                button.Click += value;
            }
            remove
            {
                button.Click -= value;
                button.Click += buttonClick;
            }
        }

        public TextPath()
        {
            button = new Button { Cursor = Cursors.Default };
            AutoSize = false;

            button.AutoSize = false;
            button.TabStop = false;
            button.FlatStyle = FlatStyle.Flat;
            button.Cursor = Cursors.Hand;

            button.TextAlign = ContentAlignment.MiddleCenter;
            //button.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            button.Width = 30;
            button.Size = new Size(button.Width, this.ClientSize.Height - 2);
            button.Location = new Point(this.ClientSize.Width - button.Width, 1);

            Controls.Add(button);

            openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "FDB";
            openFileDialog.Filter = "Firebird database file (*.FDB)|*.FDB|Interbase database file (*.GDB)|*.GDB";


            ControlsHelper.SetToolTip(button, "Нажмите, чтобы найти путь БД.");
            ControlsHelper.SetToolTip(this, "Введите путь до БД. Или нажмите кнопку, чтоб найти");

            button.Click += buttonClick;




            button.SizeChanged += (o, e) => OnResize(e);
            this.Controls.Add(button);
        }

        public Button Button
        {
            get
            {
                return button;
            }
        }
        private void buttonClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Text))
            {
                openFileDialog.FileName = Path.GetFileName(Text);
                openFileDialog.InitialDirectory = Path.GetDirectoryName(Text);
            }
            else
            if (!string.IsNullOrEmpty(openFileDialog.InitialDirectory))
                openFileDialog.InitialDirectory = Application.StartupPath;

            if (openFileDialog.ShowDialog(FindForm()) == DialogResult.OK)
            {
                Text = openFileDialog.FileName;
                Invalidate();
            }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            button.Size = new Size(button.Width, this.ClientSize.Height - 2);
            button.Location = new Point(this.ClientSize.Width - button.Width, 1);
            // Send EM_SETMARGINS to prevent text from disappearing underneath the button
            SendMessage(this.Handle, 0xd3, (IntPtr)2, (IntPtr)(button.Width << 16));
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

    }
}
