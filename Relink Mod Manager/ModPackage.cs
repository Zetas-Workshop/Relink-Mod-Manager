using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relink_Mod_Manager
{
    public class ModPackage
    {
        public int? ModFormatVersion { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Author { get; set; }
        public string URL { get; set; }
        public List<ModGroups> ModGroups { get; set; }

        public ModPackage()
        {
            ModFormatVersion = null;
            Name = string.Empty;
            Description = string.Empty;
            Version = string.Empty;
            Author = "Unknown";
            URL = string.Empty;
            ModGroups = new List<ModGroups>();
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SelectionType
    {
        Single,
        Multi
    }

    public class ModGroups
    {
        public string GroupName { get; set; }
        public SelectionType SelectionType { get; set; }
        public List<ModOption> OptionList { get; set; }

        public ModGroups()
        {
            GroupName = string.Empty;
            SelectionType = SelectionType.Single;
            OptionList = new List<ModOption>();
        }
    }

    public class ModOption
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsChecked { get; set; }
        public List<ModFilePath> FilePaths { get; set; }

        public ModOption()
        {
            Name = string.Empty;
            Description = string.Empty;
            IsChecked = false;
            FilePaths = new List<ModFilePath>();
        }
    }

    public class ModFilePath
    {
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }

        public ModFilePath()
        {
            SourcePath = string.Empty;
            DestinationPath = string.Empty;
        }
    }
}
