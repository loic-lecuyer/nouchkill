using NouchKill.Models;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

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
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
                    };
                    settings = System.Text.Json.JsonSerializer.Deserialize<Settings>(json, options);
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
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            string json = System.Text.Json.JsonSerializer.Serialize(settings, options);
            System.IO.File.WriteAllText(settingsPath, json);
        }
    }
}
