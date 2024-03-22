using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Relink_Mod_Manager.Windows
{
    public partial class ModManagerWindow : Window
    {
        void ShowModAlreadyImportedDialog()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(new Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.SetNextWindowSize(new Vector2(900, 300), ImGuiCond.Appearing);

            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, Colors.Goldenrod_Transparent);

            if (ImGui.BeginPopupModal("Mod Already Imported###ModAlreadyImported"))
            {
                ImGui.Text("The following mods were found in Mod Archive Storage already.\nPlease select the action to apply to them:");

                if (ImGui.BeginListBox("##AlreadyImportedModsList", new Vector2(-1, 0)))
                {
                    foreach (var ModItem in AlreadyImportedModsList)
                    {
                        ImGui.Selectable(ModItem);
                    }
                    ImGui.EndListBox();
                }

                if (ImGui.Button("Skip Mod Import", new Vector2(Util.BUTTON_ITEM_WIDTH_SECOND, 0)))
                {
                    AlreadyImportedModsList.Clear();
                    ImGui.CloseCurrentPopup();
                }
                ImGui.SameLine();
                if (ImGui.Button("Overwrite Existing", new Vector2(Util.BUTTON_ITEM_WIDTH_SECOND, 0)))
                {
                    foreach (var ModItem in AlreadyImportedModsList)
                    {
                        // TODO: This currently does not fully support having CopyModArchivesToStorage as false

                        // We already verified the package contents prior to this dialog, trust it hopefully hasn't changed
                        ModPackage modPackage = ReadModArchive(ModItem);

                        string ModItemArchivePath = ModItem;
                        if (Settings.CopyModArchivesToStorage)
                        {
                            ModItemArchivePath = Path.Combine(Settings.ModArchivesDirectory, Path.GetFileName(ModItem));
                        }

                        ModEntry modEntry = new ModEntry()
                        {
                            Name = modPackage.Name,
                            ModPack = modPackage,
                            IsEnabled = false,
                            IsUsingArchiveStorage = Settings.CopyModArchivesToStorage,
                            ModArchivePath = ModItemArchivePath
                        };

                        var ExistingModIdx = Settings.ModList.FindIndex(item => item.ModArchivePath == ModItemArchivePath);
                        if (ExistingModIdx != -1)
                        {
                            // Reset the Conflicts before we briefly remove it in case file paths inside are different now
                            Settings.ModList[ExistingModIdx].ClearConflicts();

                            // TODO: Improve logic for replacing the entry but keep settings, will be a constant work-in-progress
                            // For now we just do some minor state saving if possible
                            modEntry.IsEnabled = Settings.ModList[ExistingModIdx].IsEnabled;
                            modEntry.Priority = Settings.ModList[ExistingModIdx].Priority;
                            for (int groupIdx = 0; groupIdx < Settings.ModList[ExistingModIdx].ModPack.ModGroups.Count; groupIdx++)
                            {
                                var group = Settings.ModList[ExistingModIdx].ModPack.ModGroups[groupIdx];

                                if (modPackage.ModGroups[groupIdx] != null && modPackage.ModGroups[groupIdx].GroupName == group.GroupName)
                                {
                                    if (modPackage.ModGroups[groupIdx].SelectionType == group.SelectionType)
                                    {
                                        // Same group index, name, and selection type
                                        for (int optionIdx = 0; optionIdx < group.OptionList.Count; optionIdx++)
                                        {
                                            var option = group.OptionList[optionIdx];

                                            // Restore checked status when index and name matches
                                            if (modPackage.ModGroups[groupIdx].OptionList[optionIdx] != null && modPackage.ModGroups[groupIdx].OptionList[optionIdx].Name == option.Name)
                                            {
                                                modPackage.ModGroups[groupIdx].OptionList[optionIdx].IsChecked = option.IsChecked;
                                            }
                                        }
                                    }
                                }
                            }
                            Settings.ModList[ExistingModIdx] = modEntry;

                            Settings.ModList[ExistingModIdx].BuildEffectiveFileList(Settings.ModList);
                        }
                        else
                        {
                            // Mod existed in storage but wasn't imported somehow
                            Settings.ModList.Add(modEntry);
                        }

                        CopyModArchiveToStorage(ModItem);

                        Settings.Save();
                    }

                    AlreadyImportedModsList.Clear();

                    UpdateModDetailsPanel(Settings.ModList[SelectedModIndex]);

                    ImGui.CloseCurrentPopup();
                }
                ImGui.SetItemTooltip("The existing mod will attempt an in-place upgrade and replace.\nYour current mod settings will be preserved as best they can.");
                ImGui.SameLine();
                if (ImGui.Button("Import As New Mod", new Vector2(Util.BUTTON_ITEM_WIDTH_SECOND, 0)))
                {
                    foreach (var ModItem in AlreadyImportedModsList)
                    {
                        // Determine the next valid incremented file name to store this mod as
                        // Expected to only run this loop at most 5 times per conflict ever so it will be fast enough
                        for (int i = 2; i < 999; i++)
                        {
                            string NewFileName = $"{Path.GetFileNameWithoutExtension(ModItem)} ({i}){Path.GetExtension(ModItem)}";
                            string NewAbsolutePath = Path.Combine(Settings.ModArchivesDirectory, NewFileName);
                            if (!File.Exists(NewAbsolutePath))
                            {
                                // We've found a new valid name, time to use it
                                CopyModArchiveToStorage(ModItem, NewAbsolutePath);

                                ModPackage modPackage = ReadModArchive(ModItem);

                                ModEntry modEntry = new ModEntry()
                                {
                                    Name = modPackage.Name,
                                    ModPack = modPackage,
                                    IsEnabled = false,
                                    IsUsingArchiveStorage = Settings.CopyModArchivesToStorage,
                                    ModArchivePath = NewAbsolutePath
                                };
                                Settings.ModList.Add(modEntry);

                                Settings.Save();
                                break;
                            }

                            // Hit another conflicting name, keep incrementing
                            // If by chance we manage to hit the 999 limit, we'll just fail silently - no sane person will hit it
                        }
                    }

                    AlreadyImportedModsList.Clear();
                    ImGui.CloseCurrentPopup();
                }
                ImGui.SetItemTooltip("The archive file name will be slightly modified and a new mod entry will be created.\nYou may have multiple mods with the same display name.");
                ImGui.EndPopup();
            }

            ImGui.PopStyleColor();
        }
    }
}
