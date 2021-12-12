using GH.Entity;

namespace GH.Cfg
{
    public class SectionCfg : AbstractEntity
    {

        public AppCfg AppCfg { get => _app; set => _app = value; }
        private AppCfg _app = null;

        public override void EndEdit()
        {
            base.EndEdit();
            AppCfg?.Save();
        }

    }
}
