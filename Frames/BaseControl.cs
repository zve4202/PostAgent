using GH.Cfg;
using GH.Context;
using GH.Forms;
using System.Collections;
using System.Collections.Generic;

namespace PostAgent.frames
{
    public class BaseControl<T> : DatasourceControl
        where T : SectionCfg
    {
        public BaseControl()
        {
            dataSource.DataSource = typeof(T);
            LayoutHelper.CreateEntityGroup<T>(this);
        }

        public override IList GetBindingList()
        {
            IList list = new List<T>()
            {
                RunContext.AppCfg().Get<T>()
            };
            return list;
        }
    }
}
