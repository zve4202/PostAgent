using PostAgent.Domain;
using GH.Forms;
using System.Collections;
using System.Collections.Generic;
using GH.DataSourses.Args;

namespace PostAgent.frames
{
    public class SendControl : DataSourceControl
    {
        SendContext sendContext = new SendContext();
        public SendControl()
        {
            dataSource.DataSource = typeof(SendContext);
            dataSource.RefreshControls += RefreshControls;
            LayoutHelper.CreateEntityGroup<SendContext>(this);
        }

        private void RefreshControls(object sender, RefreshArgs e)
        {
            if (e.Current is SendContext current)
            {

            }
        }

        public override IList GetBindingList()
        {
            IList list = new List<SendContext>()
            {
                sendContext
            };
            return list;
        }


    }
}
