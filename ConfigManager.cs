using System;

namespace TWBlackListSoamChecker
{
    internal class ConfigManager
    {
        private static string ConfigPath;

        internal static string GetConfigPath()
        {
            if (ConfigPath == null)
            {
                var configPath = Environment.GetEnvironmentVariable("BOT_CONFIGPATH");
                if (configPath == "" || configPath == null)
                    ConfigPath = @"plugincfg/soamchecker/";
                else
                    ConfigPath = configPath + "/";
            }

            return ConfigPath;
        }
    }
}