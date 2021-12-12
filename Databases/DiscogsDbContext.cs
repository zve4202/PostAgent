using PostAgent.Domain.App;
using PostAgent.Domain.Cfgs;

namespace PostAgent.databases
{
    public class DiscogsDbContext : DbContecxtAbstract
    {
        public DiscogsDbContext()
        {

        }
        protected override CfgFirebird GetCfg()
        {
            CfgFirebird cfg = base.GetCfg();
            cfg.Selected = EnumBaseId.DiscogsId;
            return cfg;
        }
    }
}
