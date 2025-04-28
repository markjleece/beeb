// This file is Copyright © 2025 - Mark John Leece - All rights reserved
using System.Configuration;

namespace LevelEditor
{
    class AppSettings
    {
        static internal readonly AppSettings Instance = new AppSettings();

        // Settings...
        internal string RecentFilePathName
        {
            get { return Get(RecentFilePathName_PropertyName, string.Empty/*defaultValue*/); }
            set { Set(RecentFilePathName_PropertyName, value); }
        }

        internal int EditTilePrimaryColor
        {
            get { return int.Parse(Get(EditTilePrimaryColor_PropertyName, "3"/*defaultValue*/)); }
            set { Set(EditTilePrimaryColor_PropertyName, value.ToString()); }
        }

        internal int EditTileSecondaryColor
        {
            get { return int.Parse(Get(EditTileSecondaryColor_PropertyName, "0"/*defaultValue*/)); }
            set { Set(EditTileSecondaryColor_PropertyName, value.ToString()); }
        }

        // Implementation...
        private AppSettings()
        {
            Config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        private string Get(string name, string defaultValue)
        {
            var nameValue = Config.AppSettings.Settings[name];
            return nameValue != null ? nameValue.Value : defaultValue;
        }

        private void Set(string name, string value)
        {
            while (Config.AppSettings.Settings[name] != null)
            {
                Config.AppSettings.Settings.Remove(name);
            }

            Config.AppSettings.Settings.Add(name, value);
            Config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }

        private Configuration Config;

        private const string RecentFilePathName_PropertyName = "RecentFilePathName";
        private const string EditTilePrimaryColor_PropertyName = "EditTilePrimaryColor";
        private const string EditTileSecondaryColor_PropertyName = "EditTileSecondaryColor";
    }
}
