using GH.Cfg;

namespace PostAgent.Domain.App
{
    public static class AppStorage
    {
        public static AppCfg GetCfgApp()
        {
            if (cfgApp == null)
            {
                cfgApp = AppCfg.Load();
            }

            return cfgApp;
        }
        private static AppCfg cfgApp = null;

        public static void SaveCfgApp()
        {
            if (cfgApp != null)
            {
                AppCfg.Save(cfgApp);
            }
        }
    }
}
