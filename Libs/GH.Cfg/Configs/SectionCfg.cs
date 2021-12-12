using GH.Entity;

namespace GH.Cfg
{
    public class SectionCfg : AbstractEntity
    {
        private AppCfg _app = null;

        public SectionCfg(AppCfg app)
        {
            _app = app;
        }

        public override void EndEdit()
        {
            base.EndEdit();
            _app?.Save();
        }

    }
}
