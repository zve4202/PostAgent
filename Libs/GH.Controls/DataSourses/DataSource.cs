using GH.DataSourses.Args;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace GH.DataSourses
{
    [ToolboxItem(true)]
    [DefaultProperty(nameof(OnRefreshControls))]
    [ToolboxItemFilter("GH Controls")]
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

        public DataSource(IContainer container) : base(container)
        {
        }

        public DataSource()
        {
        }

        public DataSource(object dataSource, string dataMember) : base(dataSource, dataMember)
        {
        }

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
                RefreshControls(this, new RefreshArgs(e.ListChangedType, control, Current));

        }
    }
}
