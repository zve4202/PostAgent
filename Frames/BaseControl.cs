using GH.Cfg;
using GH.Context;
using GH.Entity;
using GH.Forms;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PostAgent.frames
{
    public class BaseControl<T> : AbstractControl
        where T : SectionCfg
    {

        public BaseControl()
        {
            Padding = new Padding(10);
            dataSource.DataSource = typeof(T);
            LayoutHelper.CreateEntityGroup<T>(this);
        }

        public void Open()
        {
            dataSource.DataSource = GetBindingList();
        }

        public List<T> GetBindingList()
        {
            var list = new List<T>() 
            {
                RunContext.AppCfg().Get<T>()
            };
            return list;
        }
    }
}
