using PostAgent.Domain.App;
using PostAgent.Domain.Cfgs;

namespace PostAgent.databases
{
    public class ShopDbContext : DbContecxtAbstract
    {
        public ShopDbContext()
        {

        }
        protected override CfgFirebird GetCfg()
        {
            CfgFirebird cfg = base.GetCfg();
            cfg.Selected = EnumBaseId.ShopId;
            return cfg;
        }
    }
}
