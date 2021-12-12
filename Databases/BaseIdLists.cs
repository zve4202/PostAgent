using PostAgent.Domain;
using PostAgent.Domain.App;
using System.Collections.Generic;
using System.Linq;

namespace PostAgent.databases
{
    public class BaseIdLists : Dictionary<EnumBaseId, List<Letter>>
    {
        public int TotalCount
        {
            get
            {
                int result = 0;
                foreach (var item in this)
                {
                    result += item.Value.Count;
                }
                return result;
            }
        }

        public int SentCount
        {
            get
            {
                int result = 0;
                foreach (var item in this.ToList())
                {
                    result += item.Value.Where(x => x.Sended == true).ToList().Count;
                }
                return result;
            }
        }
    }
}
