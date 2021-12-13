using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GH.Controls.DataSourses.Args;

namespace GH.Controls.DataSourses
{
    [ToolboxItem(true)]
    [DefaultProperty(nameof(OnRefreshControls))]
    public class DataSource : BindingSource, ISupportInitialize
    {
        internal class ControlList : List<Control>
        {
            internal void AddControl(Control control)
            {
                if (this.Contains(control))
                    return;
                Add(control);
            }
        }
        private readonly ControlList Controls = new ControlList();

        [Category(nameof(DataSource))]
        public event EventHandler<RefreshArgs> RefreshControls;        

        protected override void OnListChanged(ListChangedEventArgs e)
        {
            OnRefreshControls(e);
            base.OnListChanged(e);            
        }
        protected override void OnBindingComplete(BindingCompleteEventArgs e)
        {
            if (e.BindingCompleteState == BindingCompleteState.Success)
            {
                Controls.AddControl(e.Binding.BindableComponent.DataBindings.Control);
            }

            base.OnBindingComplete(e);
        }
        protected void OnRefreshControls(ListChangedEventArgs e)
        {
            if (RefreshControls == null)
                return;

            List<Control> controls = Controls.ToList();

            foreach (var control in controls)
                RefreshControls(this, new RefreshArgs(e.ListChangedType, control, e));
            
        }
    }
}
