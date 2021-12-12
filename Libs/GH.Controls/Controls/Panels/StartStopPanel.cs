using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GH.Controls
{
    public class StartStopPanel : BasePanel
    {
        protected Button btnStart;
        protected Button btnStop;
        protected ProgressBar ProgressBar;
        public StartStopPanel(string name, Control parent) : base(name, parent)
        {
            CreateButtons();
        }
        protected void CreateButtons()
        {
            btnStart = GetButton(null, nameof(btnStart), "Начать", "Начать рассылку");
            btnStart.BackColor = Color.CornflowerBlue;
            btnStart.ForeColor = Color.White;
            btnStart.Click += StartClick;

            btnStop = GetButton(btnStart, nameof(btnStop), "Остановить", "Остановить рассылку");
            btnStop.BackColor = Color.OrangeRed;
            btnStop.ForeColor = Color.White;
            btnStop.Click += StopClick;

            ProgressBar = GetProgressBar("ProgressBar", "Прогресс отправки");

        }

        private void StopClick(object sender, EventArgs e)
        {

        }

        private void StartClick(object sender, EventArgs e)
        {

        }

        protected ProgressBar GetProgressBar(string name, string toolTip)
        {
            ProgressBar bar = new ProgressBar()
            {
                Name = name,
                Top = Padding.Top,
                Width = ClientRectangle.Right - (Left + Padding.Right),
                TabStop = false,
            };

            ControlsHelper.SetToolTip(bar, toolTip);

            Controls.Add(bar);
            return bar;
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            ProgressBar.Width = ClientRectangle.Right - (Left + Padding.Right);
        }

        public override int AjastLayout(int top)
        {
            var controls = Controls.OfType<Control>().Where(ctrl => ctrl.GetType() != typeof(Label)).ToList();
            if (controls.Count > 0)
            {
                SuspendLayout();

                foreach (var control in Controls.OfType<Label>().ToArray())
                    control.Width = LabelWidth;

                foreach (var control in controls)
                {
                    control.Left = _label.Right + margin;
                    control.Width = ClientRectangle.Right - (control.Left + Padding.Right);
                }

                Height = (controls.Count * (ControlHeight + margin) - margin) + Padding.Vertical;
                Width = Parent.ClientRectangle.Right - Padding.Horizontal;

                Top = top;

                ResumeLayout(false);
            }
            return top += Height + 1;
        }

        protected override void RefreshControls(bool enabled)
        {
            var controls = Controls.OfType<Button>().ToList();
            foreach (var item in controls)
            {
                item.Enabled = enabled;
            }
        }
    }
}