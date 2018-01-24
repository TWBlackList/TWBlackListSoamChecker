namespace CNBlackListSoamChecker
{
    class ConfigManager
    {
        private static string ConfigPath = null;

        internal static string GetConfigPath()
        {
            if (ConfigPath == null)
            {
                string configPath = System.Environment.GetEnvironmentVariable("BOT_CONFIGPATH");
                if (configPath == "" || configPath == null)
                {
                    ConfigPath = @"plugincfg/soamchecker/";
                }
                else
                {
                    ConfigPath = configPath + "/";
                }
            }
            return ConfigPath;
        }
    }
}
