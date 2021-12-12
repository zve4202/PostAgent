using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace GH.Forms
{
    [ToolboxItem(true)]
    [ToolboxItemFilter("GH Controls")]
    public class FieldsPanel : BasePanel
    {
        public FieldsPanel(string name, Control parent) : base(name, parent)
        {


        }

        private int GetTop()
        {
            var label = Controls.OfType<Label>().LastOrDefault();
            if (label != null)
                return label.Top + ControlHeight + margin;

            return Padding.Top;
        }


        public void AddField(Field field)
        {
            LabelWidth = Math.Max(LabelWidth, LayoutHelper.TextWidth(field.Caption, Font));
            var top = GetTop();
            var left = Padding.Left;
            var label = ControlsHelper.CreateLabel(field, top, left, ControlHeight, LabelWidth);
            Controls.Add(label);


            var control = ControlsHelper.CreateControl(field, top, label.Right + margin, ControlHeight);
            control.Top = top;
            control.Left = 0;
            control.Height = ControlHeight;
            control.TabIndex = Parent.Controls.Count;


            Controls.Add(control);

            Binding bind = new Binding(control.Tag.ToString(), dataSource, field.Name, true, DataSourceUpdateMode.OnPropertyChanged);
            bind.ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
            control.DataBindings.Add(bind);
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

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            var controls = Controls.OfType<Control>().Where(ctrl => ctrl.GetType() != typeof(Label)).ToList();
            if (_label == null)
                return;

            foreach (var control in controls)
            {
                control.Left = _label.Right + margin;
                control.Width = ClientRectangle.Right - (control.Left + Padding.Right);
            }
        }
    }
}
