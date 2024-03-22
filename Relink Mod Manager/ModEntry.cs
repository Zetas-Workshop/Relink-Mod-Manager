using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relink_Mod_Manager
{
    public class ModEntry
    {
        public string Name { get; set; }
        public ModPackage ModPack { get; set; }
        public bool IsEnabled { get; set; }
        public int Priority { get; set; }
        public List<ModFilePath> ModifiedPaths { get; set; }
        public bool IsUsingArchiveStorage { get; set; }
        public string ModArchivePath { get; set; }
        [JsonIgnore]
        public List<ModEntry> ConflictingMods { get; set; }

        public ModEntry()
        {
            Name = string.Empty;
            ModPack = new ModPackage();
            IsEnabled = false;
            Priority = 0;
            ModifiedPaths = new List<ModFilePath>();
            IsUsingArchiveStorage = true;
            ModArchivePath = string.Empty;
            ConflictingMods = new List<ModEntry>();
        }

        /// <summary>
        /// Populates the ModifiedPaths property based on mod status and selected options
        /// </summary>
        /// <param name="ModList">List of Mods to compare against for Conflict checking</param>
        public void BuildEffectiveFileList(List<ModEntry> ModList)
        {
            ModifiedPaths.Clear();

            if (IsEnabled)
            {
                // Loop through all settings
                // Any "IsChecked" setting has its FilePaths added to ModifiedPaths
                // The last encountered path conflict is given priority and replaces earlier path
                foreach (var group in ModPack.ModGroups)
                {
                    foreach (var option in group.OptionList)
                    {
                        if (option.IsChecked)
                        {
                            ModifiedPaths.RemoveAll(item => option.FilePaths.Any(opt => item.DestinationPath == opt.DestinationPath));
                            ModifiedPaths.AddRange(option.FilePaths);
                        }
                    }
                }
            }

            // Check if new status has created/resolved any conflicts
            HasConflicts(ModList);
        }

        public void ClearConflicts()
        {
            // Remove self from referenced conflicts
            foreach (var Mod in ConflictingMods)
            {
                Mod.ConflictingMods.Remove(this);
            }

            ConflictingMods.Clear();
        }

        public void HasConflicts(List<ModEntry> ModList)
        {
            ClearConflicts();

            // Don't filter out by Priority so that we can always display potential conflicts
            var MatchingPriorityList = ModList.Where(item => item.IsEnabled && item != this);

            Dictionary<ModEntry, List<string>> Conflicts = new Dictionary<ModEntry, List<string>>();
            for (int i = 0; i < MatchingPriorityList.Count(); i++)
            {
                for (int x = 0; x < MatchingPriorityList.ElementAt(i).ModifiedPaths.Count; x++)
                {
                    var Match = this.ModifiedPaths.Any(item => item.DestinationPath == MatchingPriorityList.ElementAt(i).ModifiedPaths[x].DestinationPath);
                    if (Match)
                    {
                        // Conflicting files but we don't know if they have different priorities already to resolve it
                        if (Conflicts.ContainsKey(MatchingPriorityList.ElementAt(i)))
                        {
                            Conflicts[MatchingPriorityList.ElementAt(i)].Add(MatchingPriorityList.ElementAt(i).ModifiedPaths[x].DestinationPath);
                        }
                        else
                        {
                            Conflicts.Add(MatchingPriorityList.ElementAt(i), new List<string> { MatchingPriorityList.ElementAt(i).ModifiedPaths[x].DestinationPath });
                        }
                    }
                }
            }

            // Active conflicts
            var ModsInConflict = Conflicts.Where(item => item.Key.Priority == this.Priority);
            foreach (var Mod in ModsInConflict)
            {
                Mod.Key.ConflictingMods.Add(this);
                this.ConflictingMods.Add(Mod.Key);
            }
        }
    }
}
