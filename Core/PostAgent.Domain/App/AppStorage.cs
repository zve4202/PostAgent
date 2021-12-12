using GH.Cfg;

namespace PostAgent.Domain.App
{
    public static class AppStorage
    {
        public static CfgApp GetCfgApp()
        {
            if (cfgApp == null)
            {
                cfgApp = CfgApp.Load();
            }

            return cfgApp;
        }
        private static CfgApp cfgApp = null;

        public static void SaveCfgApp()
        {
            if (cfgApp != null)
            {
                CfgApp.Save(cfgApp);
            }
        }
    }
}
