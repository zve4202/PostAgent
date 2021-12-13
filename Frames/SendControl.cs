using PostAgent.Domain;
using GH.Forms;
using System.Collections;
using System.Collections.Generic;

namespace PostAgent.frames
{
    public class SendControl : DatasourceControl
    {
        SendContext sendContext = new SendContext();
        public SendControl()
        {
            dataSource.DataSource = typeof(SendContext);
            LayoutHelper.CreateEntityGroup<SendContext>(this);
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
