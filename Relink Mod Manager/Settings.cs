using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Relink_Mod_Manager
{
    public class Settings
    {
        public int? ModManagerConfigFormatVersion;
        public string GameExecutableFilePath;
        public bool CopyModArchivesToStorage;
        public bool CheckForUpdateOnStartup;
        public string ModManagerLatestVersionCheckURL;
        public string ModManagerAlertsURL;
        public string ModManagerUpdateURL;
        public string ModArchivesDirectory;
        public bool ReduceVolatileModWarningText;

        [JsonIgnore]
        public Version ModManagerVersion;
        [JsonIgnore]
        public string ManagerDirectory;
        [JsonIgnore]
        public string ManagerAppDataDirectory;

        public List<ModEntry> ModList;

        [JsonIgnore]
        public bool WaitingOnStartupPopup;

        public Settings()
        {
            ModManagerConfigFormatVersion = 1;
            ModList = new List<ModEntry>();
            GameExecutableFilePath = "";
            CopyModArchivesToStorage = true;
            CheckForUpdateOnStartup = true;
            ModManagerLatestVersionCheckURL = "https://raw.githubusercontent.com/Zetas-Workshop/Mod-Manager-Metadata/master/LatestVersion.txt";
            ModManagerAlertsURL = "https://raw.githubusercontent.com/Zetas-Workshop/Mod-Manager-Metadata/master/Alerts.json";
            ModManagerUpdateURL = "https://github.com/Zetas-Workshop/Relink-Mod-Manager/releases";
            ReduceVolatileModWarningText = false;
            ManagerDirectory = AppDomain.CurrentDomain.BaseDirectory;
            ModArchivesDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Relink Mod Archives");

            var AppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            ManagerAppDataDirectory = Path.Combine(AppData, "Relink Mod Manager");

            ModManagerVersion = new Version();

            WaitingOnStartupPopup = false;
        }

        public void Load()
        {
            ModManagerVersion = Assembly.GetExecutingAssembly().GetName().Version;

            var FilePath = Path.Combine(ManagerAppDataDirectory, "Config.json");

            if (File.Exists(FilePath))
            {
                var Content = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(FilePath));
                ModManagerConfigFormatVersion = Content.ModManagerConfigFormatVersion;
                ModList = Content.ModList;
                GameExecutableFilePath = Content.GameExecutableFilePath;
                CopyModArchivesToStorage = Content.CopyModArchivesToStorage;
                CheckForUpdateOnStartup = Content.CheckForUpdateOnStartup;
                ModManagerLatestVersionCheckURL = Content.ModManagerLatestVersionCheckURL;
                ModManagerAlertsURL = Content.ModManagerAlertsURL;
                ModManagerUpdateURL = Content.ModManagerUpdateURL;
                ModArchivesDirectory = Content.ModArchivesDirectory;
                ReduceVolatileModWarningText = Content.ReduceVolatileModWarningText;
            }
        }
    }

    public static class SettingsExtensions
    {
        public static void Save<T>(this T instance) where T : Settings
        {
            var AppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var FilePath = Path.Combine(AppData, "Relink Mod Manager", "Config.json");
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));

            File.WriteAllText(FilePath, JsonConvert.SerializeObject(instance, Formatting.Indented));
        }
    }
}
