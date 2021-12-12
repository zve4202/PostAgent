using PostAgent.databases;
using PostAgent.frames;
using PostAgent.mail;
using Serilog;
using System;
using System.Windows.Forms;

namespace PostAgent.forms
{
    public partial class MasterForm : Form, IMaimForm
    {
        Databases _databases = null;
        public MasterForm()
        {
            InitializeComponent();
            try
            {
                _databases = Databases.GetModule();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_databases != null)
            {
                _databases.Dispose();
                _databases = null;
            }
            base.OnClosed(e);
        }
        public void BeginSending()
        {
            Action action = () =>
            {
                tabDbSetting.Hide();
                tabPostSetting.Hide();
                tabControl.SelectedTab = tabSend;
                buttonSend.Enabled = false;
                textInfo.Clear();
            };

            if (InvokeRequired)
                Invoke(action);
            else
                action();
        }

        public void FinishSending()
        {
            Action action = () =>
            {
                tabDbSetting.Show();
                tabPostSetting.Show();
                tabControl.SelectedTab = tabSend;
                buttonSend.Enabled = true;
            };

            if (InvokeRequired)
                Invoke(action);
            else
                action();
        }

        public void ShowMessage(string msgType, string message)
        {
            message = $"{msgType}: {message}\r\n";
            Action action = () => textInfo.Text += message;
            if (InvokeRequired)
                Invoke(action);
            else
                action();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {

            MailAgent.Send();


        }

        private void MasterForm_Load(object sender, EventArgs e)
        {
            try
            {
                _databases.Open();
            }
            catch (Exception err)
            {
                Log.Error(err, "Error");
            }


            var postFrame = new PostControl();
            postFrame.Dock = DockStyle.Fill;
            tabPostSetting.Controls.Add(postFrame);
            postFrame.Open();

            var firebirdFrame = new FirebirdControl();
            firebirdFrame.Dock = DockStyle.Fill;
            tabDbSetting.Controls.Add(firebirdFrame);
            firebirdFrame.Open();
        }
    }
}
