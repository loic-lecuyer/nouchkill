using NouchKill.Models;
using System;
using System.Diagnostics;

namespace NouchKill.Services
{
    public class SettingService
    {
        public Settings LoadSetting()
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string settingsPath = System.IO.Path.Combine(appPath, "settings.json");
            Settings settings = new Settings();

            if (System.IO.File.Exists(settingsPath))
            {
                string json = System.IO.File.ReadAllText(settingsPath);
                try
                {
                    settings = System.Text.Json.JsonSerializer.Deserialize<Settings>(json);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error " + ex.Message);
                }
            }
            else
            {
                SaveSetting(settings);
            }
            return settings;

        }

        public void SaveSetting(Settings settings)
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string settingsPath = System.IO.Path.Combine(appPath, "settings.json");
            string json = System.Text.Json.JsonSerializer.Serialize(settings);
            System.IO.File.WriteAllText(settingsPath, json);
        }
    }
}
