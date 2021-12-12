using GH.Cfg;
using GH.Context;
using GH.Controls;
using GH.Entity;
using PostAgent.Domain;
using PostAgent.Domain.App;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PostAgent.frames
{
    public class BaseControl<T> : AbstrctControl
        where T : AbstractEntity
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
            CfgApp app = RunContext.GetCfgApp();
            T cfg = app.Get<T>();
            var list = new List<T>();
            list.Add(cfg);
            return list;
        }
    }
}
