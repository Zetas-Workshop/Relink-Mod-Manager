using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Relink_Mod_Manager.Windows
{
    public class ModDetailsPanel : Window
    {
        static ModEntry CachedModEntry = new ModEntry();
        static bool ModHasShortDescription = false;

        public static void UpdateModDetails(ModEntry modEntry)
        {
            CachedModEntry = modEntry;

            ModHasShortDescription = false;
            if (CachedModEntry.ModPack.Description.Length <= 128 && !CachedModEntry.ModPack.Description.Contains("\r\n"))
            {
                ModHasShortDescription = true;
            }
        }

        public static void Draw()
        {
            if (CachedModEntry.Name == "" && CachedModEntry.ModPack.ModGroups.Count == 0 && CachedModEntry.ModPack.Author == "Unknown")
            {
                return;
            }

            if (ImGui.BeginChild("ModDetailsChild", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y)))
            {
                // Mod Name
                FontManager.PushFont("SegoeUI-Bold");

                string ModName = CachedModEntry.ModPack.Name;
                var NameWidth = ImGui.CalcTextSize(ModName).X;
                ImGui.SetCursorPosX((ImGui.GetWindowWidth() - NameWidth) * 0.5f);
                ImGui.Text(ModName);

                FontManager.PopFont();

                FontManager.PushFont("SegoeUI-SemiBold");

                // Mod URL
                string ModURL = CachedModEntry.ModPack.URL;
                if (!string.IsNullOrEmpty(ModURL))
                {
                    if (ModURL.StartsWith("http://") || ModURL.StartsWith("https://"))
                    {
                        ImGui.SameLine(ImGui.GetWindowWidth() - Util.BUTTON_ITEM_WIDTH_BASE);
                        if (ImGui.Button("Visit Website", new Vector2(Util.BUTTON_ITEM_WIDTH_BASE, 0)))
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                            {
                                FileName = ModURL,
                                UseShellExecute = true,
                            });
                        }
                        ImGui.SetItemTooltip($"Visit [ {ModURL} ]");
                    }
                }

                // Mod Author
                string ModAuthor = "Author: " + CachedModEntry.ModPack.Author;
                var AuthorWidth = ImGui.CalcTextSize(ModAuthor).X;
                ImGui.SetCursorPosX((ImGui.GetWindowWidth() - AuthorWidth) * 0.5f);
                ImGui.Text(ModAuthor);

                // Mod Version
                if (!string.IsNullOrEmpty(CachedModEntry.ModPack.Version))
                {
                    string ModVersion = "v: " + CachedModEntry.ModPack.Version;
                    var VersionWidth = ImGui.CalcTextSize(ModVersion).X;
                    ImGui.SameLine(ImGui.GetWindowWidth() - VersionWidth);
                    ImGui.TextDisabled(ModVersion);
                }

                FontManager.PopFont();

                // Mod Details
                ImGui.Separator();

                if (ImGui.BeginTabBar("##ModDetailsTabs", ImGuiTabBarFlags.None))
                {
                    if (ImGui.BeginTabItem("Settings"))
                    {
                        SettingsTab();

                        ImGui.EndTabItem();
                    }

                    // Mods with a relatively short single line description will show it under Settings
                    if (!ModHasShortDescription)
                    {
                        if (ImGui.BeginTabItem("Description"))
                        {
                            DescriptionTab();

                            ImGui.EndTabItem();
                        }
                    }

                    if (ImGui.BeginTabItem("Conflicts"))
                    {
                        ConflictsTab();

                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Metadata"))
                    {
                        MetadataTab();

                        ImGui.EndTabItem();
                    }
                    ImGui.EndTabBar();
                }

                ImGui.EndChild();
            }
        }

        static void SettingsTab()
        {
            ImGui.AlignTextToFramePadding();

            // Show warning message if volatile game paths are used by the mod
            if (CachedModEntry.ModifiedPaths.Where(item => Util.PathContainsVolatileGamePaths(item.DestinationPath)).Count() > 0)
            {
                ImGui.PushStyleColor(ImGuiCol.ChildBg, Colors.DarkRed_Transparent);

                if (ImGui.BeginChild("WarningChild", new Vector2(0, 0), ImGuiChildFlags.AutoResizeY))
                {
                    FontManager.PushFont("SegoeUI-SemiBold");

                    string WarningHeader = "WARNING";
                    var HeaderWidth = ImGui.CalcTextSize(WarningHeader).X;
                    ImGui.SetCursorPosX((ImGui.GetWindowWidth() - HeaderWidth) * 0.5f);
                    ImGui.Text(WarningHeader);

                    string WarningText = "";
                    if (Settings.ReduceVolatileModWarningText)
                    {
                        WarningText += "This mod has an option enabled that may replace a [/data/sound/] or [/data/ui/] file.";
                    }
                    else
                    {
                        WarningText += "This mod has an option enabled that may replace a [/data/sound/] or [/data/ui/] file. ";
                        WarningText += "Relink Mod Manager will attempt to backup this game file into [/data_volatile_backups/] and restore it on mod uninstall. ";
                        WarningText += "If you encounter issues after uninstalling, select the Game in Steam and 'Verify Integrity of Game Files' to have Steam restore the missing files.\n";
                        WarningText += "All mods will be uninstalled when you perform the above action in Steam.";
                    }

                    ImGui.TextWrapped(WarningText);
                    FontManager.PopFont();

                    ImGui.EndChild();
                }

                ImGui.PopStyleColor();
            }

            if (ModHasShortDescription)
            {
                ImGui.Text(CachedModEntry.ModPack.Description);
            }

            bool enabled = CachedModEntry.IsEnabled;
            ImGui.NewLine();
            if (ImGui.Checkbox("Enabled", ref enabled))
            {
                CachedModEntry.IsEnabled = enabled;
                CachedModEntry.BuildEffectiveFileList(Settings.ModList);
            }
            ImGui.SameLine();
            // TODO: Only update Priority on ImGui.IsItemDeactivatedAfterEdit() instead of real-time?
            ImGui.SetNextItemWidth(50);
            int load_priority = CachedModEntry.Priority;
            if (ImGui.InputInt("Priority", ref load_priority, 0, 0))
            {
                CachedModEntry.Priority = load_priority;
                CachedModEntry.BuildEffectiveFileList(Settings.ModList);
            }
            ImGui.SetItemTooltip("Mods with a higher priority will take precedence over mods with a lower priority.");
            ImGui.NewLine();

            for (int groupIdx = 0; groupIdx < CachedModEntry.ModPack.ModGroups.Count; groupIdx++)
            {
                var group = CachedModEntry.ModPack.ModGroups[groupIdx];
                if (group.SelectionType == SelectionType.Single)
                {
                    // Single selection type groups with only 1 option are considered base/core mod files
                    // These will be automatically included if this mod is Enabled, no user-option is required
                    if (group.OptionList.Count > 1)
                    {
                        string combo_preview_value = "";
                        int combo_preview_index = -1;
                        for (int optionIdx = 0; optionIdx < group.OptionList.Count; optionIdx++)
                        {
                            if (group.OptionList[optionIdx].IsChecked)
                            {
                                combo_preview_value = group.OptionList[optionIdx].Name;
                                combo_preview_index = optionIdx;
                            }
                        }

                        ImGui.SetNextItemWidth(300);
                        if (ImGui.BeginCombo($"{group.GroupName}##ComboBox_{group.GroupName}_{groupIdx}", combo_preview_value, ImGuiComboFlags.None))
                        {
                            for (int optionIdx = 0; optionIdx < group.OptionList.Count; optionIdx++)
                            {
                                bool is_selected = combo_preview_index == optionIdx;
                                if (ImGui.Selectable(group.OptionList[optionIdx].Name, is_selected))
                                {
                                    combo_preview_index = optionIdx;
                                    group.OptionList.ForEach(item => { item.IsChecked = false; });
                                    group.OptionList[optionIdx].IsChecked = true;
                                    CachedModEntry.BuildEffectiveFileList(Settings.ModList);
                                }
                                if (group.OptionList[optionIdx].Description != "")
                                {
                                    ImGui.SetItemTooltip(group.OptionList[optionIdx].Description);
                                }

                                if (is_selected)
                                {
                                    ImGui.SetItemDefaultFocus();
                                }
                            }
                            ImGui.EndCombo();
                        }
                    }
                }
                else if (group.SelectionType == SelectionType.Multi)
                {
                    ImGui.NewLine();
                    ImGui.BeginGroup();
                    ImGui.SeparatorText(group.GroupName);
                    for (int optionIdx = 0; optionIdx < group.OptionList.Count; optionIdx++)
                    {
                        var option = group.OptionList[optionIdx];
                        var should_checked = option.IsChecked;
                        if (ImGui.Checkbox($"{option.Name}##CheckBox_{option.Name}_{optionIdx}", ref should_checked))
                        {
                            option.IsChecked = should_checked;
                            CachedModEntry.BuildEffectiveFileList(Settings.ModList);
                        }
                        if (option.Description != "")
                        {
                            ImGui.SetItemTooltip(option.Description);
                        }
                    }
                    ImGui.EndGroup();
                }
            }
        }

        static void DescriptionTab()
        {
            ImGui.AlignTextToFramePadding();
            if (!string.IsNullOrEmpty(CachedModEntry.ModPack.URL))
            {
                FontManager.PushFont("UbuntuMono-Bold", 15);
                ImGui.Text("Website: ");
                ImGui.SameLine();
                ImGui.TextColored(Colors.SkyBlue, CachedModEntry.ModPack.URL);
                FontManager.PopFont();
            }

            ImGui.Text(CachedModEntry.ModPack.Description);
        }

        static void ConflictsTab()
        {
            ImGui.Text("The following enabled mods have conflicts with one or more files.");
            ImGui.Text("Please adjust the load priority to resolve conflicts.");

            if (ImGui.BeginTable("Mod Conflicts Table", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                FontManager.PushFont("SegoeUI-Bold");
                ImGui.Text("Conflicting Mod");
                ImGui.TableNextColumn();
                ImGui.Text("Mod Priority");
                FontManager.PopFont();

                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.PushStyleColor(ImGuiCol.Text, Colors.LightBlue);
                ImGui.Text($"{CachedModEntry.Name}");

                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(50);
                int load_priority_base = CachedModEntry.Priority;
                ImGui.InputInt($"##ConflictMod_Priority_Base", ref load_priority_base, 0, 0);
                if (ImGui.IsItemDeactivatedAfterEdit())
                {
                    CachedModEntry.Priority = load_priority_base;
                    CachedModEntry.BuildEffectiveFileList(Settings.ModList);
                }

                ImGui.PopStyleColor();

                for (int i = 0; i < CachedModEntry.ConflictingMods.Count; i++)
                {
                    var ConflictMod = CachedModEntry.ConflictingMods[i];

                    ImGui.TableNextColumn();
                    ImGui.Text($"{ConflictMod.Name}");

                    ImGui.TableNextColumn();
                    ImGui.SetNextItemWidth(50);
                    int load_priority = ConflictMod.Priority;
                    ImGui.InputInt($"##ConflictMod_Priority_{i}", ref load_priority, 0, 0);
                    if (ImGui.IsItemDeactivatedAfterEdit())
                    {
                        ConflictMod.Priority = load_priority;
                        ConflictMod.BuildEffectiveFileList(Settings.ModList);
                    }
                }

                ImGui.EndTable();
            }
            
        }

        static void MetadataTab()
        {
            ImGui.AlignTextToFramePadding();

            ImGui.TextDisabled("Read-only Mod Package Metadata");

            string ModName = CachedModEntry.Name;
            ImGui.InputText("Mod Name", ref ModName, 512, ImGuiInputTextFlags.ReadOnly);
            bool IsUsingArchiveStorage = CachedModEntry.IsUsingArchiveStorage;
            ImGui.Checkbox("Is Using Archive Storage", ref IsUsingArchiveStorage);
            string ArchivePath = CachedModEntry.ModArchivePath;
            ImGui.InputText("Mod Archive Path", ref ArchivePath, 1024, ImGuiInputTextFlags.ReadOnly);

            ImGui.SeparatorText("Mod Pack");
            ImGui.Indent();

            string ModPackName = CachedModEntry.ModPack.Name;
            ImGui.InputText("Name", ref ModPackName, 1024, ImGuiInputTextFlags.ReadOnly);
            string ModPackAuthor = CachedModEntry.ModPack.Author;
            ImGui.InputText("Author", ref ModPackAuthor, 1024, ImGuiInputTextFlags.ReadOnly);
            string ModPackVersion = CachedModEntry.ModPack.Version;
            ImGui.InputText("Version", ref ModPackVersion, 32, ImGuiInputTextFlags.ReadOnly);
            string ModPackURL = CachedModEntry.ModPack.URL;
            ImGui.InputText("URL", ref ModPackURL, 1024, ImGuiInputTextFlags.ReadOnly);

            if (ImGui.CollapsingHeader($"Groups [{CachedModEntry.ModPack.ModGroups.Count}]##GroupsHeader"))
            {
                for (int groupIdx = 0; groupIdx < CachedModEntry.ModPack.ModGroups.Count; groupIdx++)
                {
                    var group = CachedModEntry.ModPack.ModGroups[groupIdx];

                    ImGui.Indent();

                    string GroupName = group.GroupName;
                    ImGui.InputText($"Group Name##GroupName_{groupIdx}", ref GroupName, 128, ImGuiInputTextFlags.ReadOnly);
                    string GroupSelectionType = group.SelectionType.ToString();
                    ImGui.InputText($"Group Selection Type##GroupSelectionType_{groupIdx}", ref GroupSelectionType, 32, ImGuiInputTextFlags.ReadOnly);

                    if (ImGui.CollapsingHeader($"Options [{group.OptionList.Count}]##OptionsHeader_{groupIdx}"))
                    {
                        for (int optionIdx = 0; optionIdx < group.OptionList.Count; optionIdx++)
                        {
                            var option = group.OptionList[optionIdx];

                            ImGui.Indent();

                            ImGui.SeparatorText($"[Option {optionIdx}] Checked={option.IsChecked}");
                            string OptionName = option.Name;
                            ImGui.InputText($"Name##OptionName_{groupIdx}_{optionIdx}", ref OptionName, 128, ImGuiInputTextFlags.ReadOnly);
                            string OptionDescription = option.Description;
                            ImGui.InputText($"Description##OptionDescription_{groupIdx}_{optionIdx}", ref OptionDescription, 256, ImGuiInputTextFlags.ReadOnly);

                            if (ImGui.CollapsingHeader($"Bindings [{option.FilePaths.Count}]##BindingsHeader_{groupIdx}_{optionIdx}"))
                            {
                                for (int bindingIdx = 0; bindingIdx < option.FilePaths.Count; bindingIdx++)
                                {
                                    var binding = option.FilePaths[bindingIdx];

                                    ImGui.Indent();

                                    ImGui.SeparatorText($"[Binding {bindingIdx}]");
                                    string BindingSourcePath = binding.SourcePath;
                                    ImGui.InputText($"Source Path##BindingSourcePath_{groupIdx}_{optionIdx}_{bindingIdx}", ref BindingSourcePath, 256, ImGuiInputTextFlags.ReadOnly);
                                    string BindingDestinationPath = binding.DestinationPath;
                                    ImGui.InputText($"Destination Path##BindingDestinationPath_{groupIdx}_{optionIdx}_{bindingIdx}", ref BindingDestinationPath, 256, ImGuiInputTextFlags.ReadOnly);

                                    ImGui.Unindent();
                                }
                            }

                            ImGui.Unindent();
                        }
                    }

                    ImGui.Unindent();
                }
            }

            ImGui.Unindent();
        }
    }
}
