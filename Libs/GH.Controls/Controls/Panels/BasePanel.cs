using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;

namespace GH.Forms
{
    [ToolboxItem(true)]
    [ToolboxItemFilter("GH Controls")]
    public class BasePanel : Panel
    {

        protected const int margin = 1;

        private static Dictionary<Control, int> labelWidth = new Dictionary<Control, int>();
        public static void AjastLayoutWithParent(Control parent)
        {
            int top = parent.Padding.Top;
            var panels = parent.Controls.OfType<BasePanel>().ToList();
            foreach (var panel in panels)
            {
                top = panel.AjastLayout(top);
            }
        }

        public static FieldsPanel CreateGroupPanel(string name, Control parent)
        {
            return new FieldsPanel(name, parent);
        }
        public static void CreateOkCancelPanel(Control parent)
        {
            new OkCancelPanel(nameof(OkCancelPanel), parent);
        }

        public static void CreateStartPanel(Control parent)
        {
            new StartStopPanel(nameof(StartStopPanel), parent);
        }

        public int ControlHeight
        {
            get { return _controlHeight; }
            set { _controlHeight = value; }
        }
        private int _controlHeight = 21;
        protected Label _label => Controls.OfType<Label>().FirstOrDefault();
        protected Control _control => Controls.OfType<Control>().Where(ctrl => ctrl.GetType() != typeof(Label)).LastOrDefault();


        protected int LabelWidth
        {
            get { return labelWidth[Parent] + margin; }
            set { labelWidth[Parent] = value; }
        }

        protected BindingSource dataSource = null;
        
        public BasePanel(string name, Control parent)
        {

            if (!labelWidth.ContainsKey(parent))
                labelWidth.Add(parent, 0);
            parent.Controls.Add(this);


            dataSource = ObjectHelper.EnumerateFields<BindingSource>(parent).FirstOrDefault();
            Name = name;
            Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            Left = parent.Padding.Left;
            Size = new Size(parent.Width, parent.Height);
            BorderStyle = BorderStyle.FixedSingle;
            Padding = new Padding(10);


        }


        public virtual int AjastLayout(int top)
        {
            var controls = Controls.OfType<Control>().ToList();
            if (controls.Count > 0)
            {
                SuspendLayout();

                var btn = controls.LastOrDefault();
                Height = btn.Height + Padding.Vertical;

                Width = Parent.ClientRectangle.Right - Parent.Padding.Horizontal;
                Top = top;

                ResumeLayout(false);
            }
            return top += Height + 1;
        }


        protected Button GetButton(Button btn, string name, string text, string toolTip)
        {
            var newBtn = new Button()
            {
                AutoSize = true,
                Text = text,
                Top = Padding.Top,
                Name = name,
                Width = LabelWidth,
                TabStop = false,
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat
            };

            if (btn != null)
                newBtn.Left = btn.Right + margin;
            else
                newBtn.Left = Padding.Left;


            ControlsHelper.SetToolTip(newBtn, toolTip);

            Controls.Add(newBtn);
            newBtn.AutoSize = false;
            newBtn.Enabled = false;

            return newBtn;
        }

        protected virtual void RefreshControls(bool enabled)
        {
            var controls = Controls.OfType<Control>().ToList();
            foreach (var item in controls)
            {
                item.Enabled = enabled;
            }
        }

    }
}
