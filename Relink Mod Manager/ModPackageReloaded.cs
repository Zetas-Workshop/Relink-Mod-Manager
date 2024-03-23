using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relink_Mod_Manager
{
    /// <summary>
    /// This is only used to support converting a Reloaded II Mod Pack over to a Relink Mod Manager Mod Pack.
    /// </summary>
    public class ModPackageReloaded
    {
        public string ModId { get; set; }
        public string ModName { get; set; }
        public string ModAuthor { get; set; }
        public string ModVersion { get; set; }
        public string ModDescription { get; set; }
        public string ModIcon { get; set; }
        public string ProjectUrl { get; set; }

        public ModPackageReloaded()
        {
            ModId = string.Empty;
            ModName = string.Empty;
            ModAuthor = "Unknown";
            ModVersion = string.Empty;
            ModDescription = string.Empty;
            ModIcon = string.Empty;
            ProjectUrl = string.Empty;
        }
    }
}
