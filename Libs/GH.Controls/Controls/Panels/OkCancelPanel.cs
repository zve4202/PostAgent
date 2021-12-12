using GH.Context;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GH.Controls
{
    public class OkCancelPanel : BasePanel
    {
        public OkCancelPanel(string name, Control parent) : base(name, parent)
        {
            dataSource.ListChanged += DataSource_ListChanged;
            BorderStyle = BorderStyle.None;
            Padding = new Padding(10) { Top = margin, Bottom = 2 * margin };
            CreateButtons();

        }

        protected Button btnOk;
        protected Button btnCancel;
        protected virtual void CreateButtons()
        {
            btnOk = GetButton(null, "btnOK", "Сохранить", "Сохранить изменения");
            btnOk.BackColor = Color.CornflowerBlue;
            btnOk.ForeColor = Color.White;
            btnOk.Click += OkClick;

            btnCancel = GetButton(btnOk, "btnCancel", "Отменить", "Отменить сделанные изменения");
            btnCancel.BackColor = Color.OrangeRed;
            btnCancel.ForeColor = Color.White;
            btnCancel.Click += CancelClick;
        }

        public override int AjastLayout(int top)
        {
            SuspendLayout();


            Height = btnOk.Height + Padding.Vertical;
            Width = Parent.ClientRectangle.Right - Parent.Padding.Horizontal;
            Top = top;

            ResumeLayout(false);

            return top += Height + 1;
        }

        protected virtual void CancelClick(object sender, EventArgs e)
        {
            dataSource.CancelEdit();
            dataSource.ResetBindings(false);
        }

        protected virtual void OkClick(object sender, EventArgs e)
        {
            dataSource.EndEdit();
            dataSource.ResetBindings(false);
            RunContext.SaveCfgApp();
        }
        protected virtual void DataSource_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.Reset:
                    RefreshControls(false);
                    break;
                case ListChangedType.ItemAdded:
                    RefreshControls(true);
                    break;
                case ListChangedType.ItemDeleted:
                    break;
                case ListChangedType.ItemMoved:
                    break;
                case ListChangedType.ItemChanged:
                    RefreshControls(true);
                    break;
                case ListChangedType.PropertyDescriptorAdded:
                    break;
                case ListChangedType.PropertyDescriptorDeleted:
                    break;
                case ListChangedType.PropertyDescriptorChanged:
                    break;
                default:
                    break;
            }
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
