﻿using ImGuiNET;
using Relink_Mod_Manager.Widgets;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vortice.Direct3D11;

namespace Relink_Mod_Manager.Windows
{
    public static class CreateModPackWindow
    {
        public const string CREATE_MOD_PACK_WINDOW_ID = "###CreateModPackWindow";
        static ModPackage ModPackage = new ModPackage();
        static string NewGroupNameInput = "";
        static List<string> GroupOptionNameInput = new List<string>();
        static Dictionary<string, string> IncludedModFilesList = new Dictionary<string, string>();
        static string BaseModPathDirectory = "";
        static bool IncludeUnreferencedFiles = false;
        static bool PromptModCreationCompleteDialog = false;
        static string ModCreationError = "";

        public static void InitializeNewCreation()
        {
            ModPackage = new ModPackage();
            ModPackage.ModFormatVersion = Util.MOD_FORMAT_VERSION_CURRENT;

            NewGroupNameInput = "";
            GroupOptionNameInput = new List<string>();
            IncludedModFilesList = new Dictionary<string, string>();
            BaseModPathDirectory = "";
            IncludeUnreferencedFiles = false;
            PromptModCreationCompleteDialog = false;
            ModCreationError = "";
        }

        public static void EditModPackCreation(ModPackage ModPack, string ModPackFilePath)
        {
            ModPackage = ModPack;
            ModPackage.ModFormatVersion = Util.MOD_FORMAT_VERSION_CURRENT;

            NewGroupNameInput = "";
            GroupOptionNameInput = Enumerable.Repeat("", ModPack.ModGroups.Count).ToList();
            IncludedModFilesList = new Dictionary<string, string>();
            BaseModPathDirectory = Path.GetDirectoryName(ModPackFilePath);
            IncludeUnreferencedFiles = false;
            PromptModCreationCompleteDialog = false;
            ModCreationError = "";

            // Rebuild the included mod files list as if a directory for a new mod was selected
            var SelectedModDirectoryInfo = new DirectoryInfo(BaseModPathDirectory);
            var AllDirectoryFiles = SelectedModDirectoryInfo.GetFiles("*", SearchOption.AllDirectories);

            foreach (var ModFilePath in AllDirectoryFiles)
            {
                // Exclude the json file that controls the mod pack from being a valid inclusion item
                if (ModFilePath.Name != "ModConfig.json")
                {
                    IncludedModFilesList.Add(ModFilePath.FullName, "");
                }
            }

            // Rebuild the binding trackers for each file
            for (int groupIdx = 0; groupIdx < ModPackage.ModGroups.Count; groupIdx++)
            {
                var group = ModPackage.ModGroups[groupIdx];

                for (int optionIdx = 0; optionIdx < group.OptionList.Count; optionIdx++)
                {
                    var option = group.OptionList[optionIdx];

                    for (int bindingIdx = 0; bindingIdx < option.FilePaths.Count; bindingIdx++)
                    {
                        var FilePath = option.FilePaths[bindingIdx];

                        FilePath.SourcePath = Path.Combine(BaseModPathDirectory, option.FilePaths[bindingIdx].SourcePath);

                        string SelectedIdentifier = $"G{groupIdx}O{optionIdx}B{bindingIdx};";
                        IncludedModFilesList[FilePath.SourcePath] += SelectedIdentifier;
                    }
                }
            }
        }

        public static void Draw()
        {
            SubmitContent();

            if (PromptModCreationCompleteDialog)
            {
                PromptModCreationCompleteDialog = false;
                ImGui.OpenPopup("###ModCreationComplete");
            }

            ShowModCreationCompleteDialog();
        }

        static void SubmitContent()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(new Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.SetNextWindowSize(new Vector2(900, 710), ImGuiCond.Appearing);

            bool p_open = true;
            if (ImGui.BeginPopupModal($"Create Mod Pack{CREATE_MOD_PACK_WINDOW_ID}", ref p_open))
            {
                ImGui.AlignTextToFramePadding();

                string ModNameInput = ModPackage.Name;
                ImGui.SetNextItemWidth(Util.INPUT_ITEM_WIDTH_BASE);
                if (ImGui.InputText("Mod Name##ModNameInput", ref ModNameInput, 128))
                {
                    ModPackage.Name = ModNameInput;
                }

                string ModDescriptionInput = ModPackage.Description;
                if (ImGui.InputTextMultiline("Description##ModDescriptionInput", ref ModDescriptionInput, 512, new Vector2(Util.INPUT_ITEM_WIDTH_BASE, 75)))
                {
                    ModPackage.Description = ModDescriptionInput;
                }

                string ModAuthorInput = ModPackage.Author;
                ImGui.SetNextItemWidth(Util.INPUT_ITEM_WIDTH_BASE);
                if (ImGui.InputTextWithHint("Author##ModAuthorInput", "Unknown", ref ModAuthorInput, 128))
                {
                    ModPackage.Author = ModAuthorInput;
                }

                string ModVersionInput = ModPackage.Version;
                ImGui.SetNextItemWidth(Util.INPUT_ITEM_WIDTH_BASE);
                if (ImGui.InputTextWithHint("Version##ModVersionInput", "1.0.0", ref ModVersionInput, 32))
                {
                    ModPackage.Version = ModVersionInput;
                }

                string ModURLInput = ModPackage.URL;
                ImGui.SetNextItemWidth(Util.INPUT_ITEM_WIDTH_BASE);
                if (ImGui.InputTextWithHint("URL##ModURLInput", "", ref ModURLInput, 256))
                {
                    ModPackage.URL = ModURLInput;
                }
                ImGui.SetItemTooltip("Optional website link that users can visit to find updates for this mod or additional details");

                ImGui.SeparatorText("Mod Files In Pack");
                ImGui.Text("Below are the files that may be included in your Mod Pack. Only Referenced files are included in Mod Package by default.");
                ImGui.Text("Only paths present here are valid bindings for your Mod Group/Options.");

                // List mod pack file structure to reference in mod options
                if (ImGui.BeginListBox("##ModFilesList", new Vector2(Util.INPUT_ITEM_WIDTH_BASE, 75)))
                {
                    for (int i = 0; i < IncludedModFilesList.Count; i++)
                    {
                        var ModFilePath = IncludedModFilesList.ElementAt(i);

                        if (!string.IsNullOrEmpty(ModFilePath.Value))
                        {
                            ImGui.PushStyleColor(ImGuiCol.Text, Colors.LightGreen);
                        }
                        else
                        {
                            ImGui.PushStyleColor(ImGuiCol.Text, Colors.Red);
                        }

                        if (ImGui.Selectable($"{ModFilePath.Key.Remove(0, BaseModPathDirectory.Length + 1)}##ModFilePath_{i}"))
                        {
                            // TODO: Copy absolute path to clipboard?
                        }
                        ImGui.SetItemTooltip($"File is referenced in {ModFilePath.Value.Split(';').Length - 1} Option Bindings.");

                        ImGui.PopStyleColor();
                    }

                    ImGui.EndListBox();
                }
                // If directory is changed or refreshed, bindings may break, so we rebuild what we can
                if (ImGui.Button("Set Directory", new Vector2(Util.BUTTON_ITEM_WIDTH_BASE, 0)))
                {
                    // Select the folder just before the /data/ folder in your mod directory
                    // This will automatically make paths relative for the game
                    ImFileBrowser.SelectDir((SelectedFolderPath) =>
                    {
                        Console.WriteLine($"Selected Path: {SelectedFolderPath}");
                        BaseModPathDirectory = SelectedFolderPath;
                        var SelectedModDirectoryInfo = new DirectoryInfo(SelectedFolderPath);
                        var AllDirectoryFiles = SelectedModDirectoryInfo.GetFiles("*", SearchOption.AllDirectories);
                        Dictionary<string, string> TempModFilesList = new Dictionary<string, string>();

                        foreach (var ModFilePath in AllDirectoryFiles)
                        {
                            if (IncludedModFilesList.ContainsKey(ModFilePath.FullName))
                            {
                                TempModFilesList.Add(ModFilePath.FullName, IncludedModFilesList[ModFilePath.FullName]);
                            }
                            else
                            {
                                TempModFilesList.Add(ModFilePath.FullName, "");
                            }
                        }
                        IncludedModFilesList = TempModFilesList;
                    }, title: "Select your base mod folder...");
                }
                ImGui.SetItemTooltip("Set the base folder of your working mod files directory.\nTypically this is one folder outside your 'data' directory.");

                ImGui.SameLine();
                ImGui.BeginDisabled(BaseModPathDirectory.Length == 0);
                if (ImGui.Button("Refresh Files", new Vector2(Util.BUTTON_ITEM_WIDTH_BASE, 0)))
                {
                    if (Directory.Exists(BaseModPathDirectory))
                    {
                        var SelectedModDirectoryInfo = new DirectoryInfo(BaseModPathDirectory);
                        var AllDirectoryFiles = SelectedModDirectoryInfo.GetFiles("*", SearchOption.AllDirectories);
                        Dictionary<string, string> TempModFilesList = new Dictionary<string, string>();

                        foreach (var ModFilePath in AllDirectoryFiles)
                        {
                            if (IncludedModFilesList.ContainsKey(ModFilePath.FullName))
                            {
                                TempModFilesList.Add(ModFilePath.FullName, IncludedModFilesList[ModFilePath.FullName]);
                            }
                            else
                            {
                                TempModFilesList.Add(ModFilePath.FullName, "");
                            }
                        }
                        IncludedModFilesList = TempModFilesList;
                    }
                    else
                    {
                        // The directory no longer appears to exist so we can just nuke all bindings and found files
                        ModPackage.ModGroups.ForEach(group =>
                        {
                            group.OptionList.ForEach(option => option.FilePaths.Clear());
                        });
                        IncludedModFilesList.Clear();
                    }
                }
                ImGui.EndDisabled();
                ImGui.SetItemTooltip("Refresh the files found in your selected mod files directory.\nThis will automatically remove Option File Bindings to files no longer found.");

                ImGui.Checkbox("Include Unreferenced Files In Mod Package", ref IncludeUnreferencedFiles);

                ImGui.SeparatorText("Group Options");
                ImGui.SetNextItemWidth(Util.INPUT_ITEM_WIDTH_SECOND);
                ImGui.InputTextWithHint("##NewGroupNameInput", "New Group Name", ref NewGroupNameInput, 128);
                ImGui.SameLine();

                ImGui.PushStyleColor(ImGuiCol.Text, Colors.LightGreen);
                FontManager.PushFont("FAS");
                if (ImGui.Button($"{FASIcons.Plus}##AddNewGroupBtn", new Vector2(ImGui.GetItemRectSize().Y, ImGui.GetItemRectSize().Y)))
                {
                    var NewModGroup = new ModGroups()
                    {
                        GroupName = NewGroupNameInput,
                        SelectionType = SelectionType.Single
                    };
                    ModPackage.ModGroups.Add(NewModGroup);
                    NewGroupNameInput = "";
                    GroupOptionNameInput.Add("");
                }
                FontManager.PopFont();
                ImGui.PopStyleColor();
                ImGui.SetItemTooltip("Add a new options group to the mod pack.");

                ImGui.NewLine();
                for (int groupIdx = 0; groupIdx < ModPackage.ModGroups.Count; groupIdx++)
                {
                    var group = ModPackage.ModGroups[groupIdx];
                    bool QueuedGroupRemoval = false;

                    ImGui.PushID(groupIdx);

                    ImGui.SeparatorText($"Group {groupIdx}");

                    if (group.OptionList.Count == 1 && group.SelectionType == SelectionType.Single)
                    {
                        ImGui.BeginDisabled();
                        ImGui.TextWrapped("Groups in Single Mode with only a Single Option are automatically bound to the overall mod Enabled status and will not be present in the UI.\nThese groups are generally used for storing \"Core\"/\"Base\" mod files.");
                        ImGui.EndDisabled();
                    }

                    string GroupNameValue = group.GroupName;
                    ImGui.SetNextItemWidth(Util.INPUT_ITEM_WIDTH_BASE);
                    if (ImGui.InputTextWithHint($"##GroupNameInput_{groupIdx}", "Group Name", ref GroupNameValue, 128))
                    {
                        group.GroupName = GroupNameValue;
                    }

                    ImGui.SameLine();
                    ImGui.PushStyleColor(ImGuiCol.Text, Colors.LightRed);
                    FontManager.PushFont("FAS");
                    if (ImGui.Button($"{FASIcons.Minus}##RemoveGroupBtn_{groupIdx}", new Vector2(ImGui.GetItemRectSize().Y, ImGui.GetItemRectSize().Y)))
                    {
                        QueuedGroupRemoval = true;
                    }
                    FontManager.PopFont();
                    ImGui.PopStyleColor();
                    ImGui.SetItemTooltip("Delete this entire options group from the mod pack.");

                    ImGui.SameLine();
                    ImGui.BeginDisabled(groupIdx == 0);
                    FontManager.PushFont("FAS");
                    if (ImGui.Button($"{FASIcons.ChevronUp}##MoveGroupUpBtn_{groupIdx}", new Vector2(ImGui.GetItemRectSize().Y, ImGui.GetItemRectSize().Y)))
                    {
                        MoveGroup(groupIdx, groupIdx - 1);
                    }
                    FontManager.PopFont();
                    ImGui.SetItemTooltip("Move Mod Group Up.");
                    ImGui.EndDisabled();

                    ImGui.SameLine();
                    ImGui.BeginDisabled(groupIdx == ModPackage.ModGroups.Count - 1);
                    FontManager.PushFont("FAS");
                    if (ImGui.Button($"{FASIcons.ChevronDown}##MoveGroupDownBtn_{groupIdx}", new Vector2(ImGui.GetItemRectSize().Y, ImGui.GetItemRectSize().Y)))
                    {
                        MoveGroup(groupIdx, groupIdx + 1);
                    }
                    FontManager.PopFont();
                    ImGui.SetItemTooltip("Move Mod Group Down.");
                    ImGui.EndDisabled();

                    int SelectionTypeInput = (int)group.SelectionType;
                    ImGui.SetNextItemWidth(Util.INPUT_ITEM_WIDTH_BASE);
                    if (ImGui.Combo($"##SelectionTypeInput_{groupIdx}", ref SelectionTypeInput, new string[] { "Single", "Multi" }, 2))
                    {
                        group.SelectionType = (SelectionType)SelectionTypeInput;

                        // When user checks multiple options in Multi mode then switches to Single mode we only allow the first item to remain checked
                        var IsCheckedItems = group.OptionList.Where(x => x.IsChecked == true).ToList();
                        if (IsCheckedItems.Count() > 1)
                        {
                            foreach (var CheckedItem in IsCheckedItems)
                            {
                                CheckedItem.IsChecked = false;
                            }
                            IsCheckedItems.First().IsChecked = true;
                        }

                        // If we switched from Multi with nothing checked to Single, ensure an item is checked
                        if (group.SelectionType == SelectionType.Single && group.OptionList.Count() > 1 && IsCheckedItems.Count() == 0)
                        {
                            group.OptionList.First().IsChecked = true;
                        }
                    }
                    ImGui.SetItemTooltip("Set the type of selection options the user will have:\nSingle = Combo Box\nMulti = Check Boxes");

                    // Wrap all options in a table for easy consistent formatting
                    if (ImGui.BeginTable($"##GroupOptionsTable_{groupIdx}", 7))
                    {
                        var OptionLabelWidth = ImGui.CalcTextSize("Option 000 ").X;
                        ImGui.TableSetupColumn("##OptionColumn", ImGuiTableColumnFlags.WidthFixed, OptionLabelWidth);
                        ImGui.TableSetupColumn("##OptionEnabledStatus", ImGuiTableColumnFlags.WidthFixed, ImGui.GetFrameHeight());
                        var InputNameColumnWidth = Util.INPUT_ITEM_WIDTH_BASE - OptionLabelWidth - 0 - ImGui.GetFrameHeight() - ImGui.GetItemRectSize().Y * 2.0f;
                        ImGui.TableSetupColumn("##OptionName", ImGuiTableColumnFlags.WidthFixed, InputNameColumnWidth);
                        ImGui.TableSetupColumn("##OptionEditList", ImGuiTableColumnFlags.WidthFixed, ImGui.GetItemRectSize().Y);
                        ImGui.TableSetupColumn("##OptionRemove", ImGuiTableColumnFlags.WidthFixed, ImGui.GetItemRectSize().Y);
                        ImGui.TableSetupColumn("##OptionUp", ImGuiTableColumnFlags.WidthFixed, ImGui.GetItemRectSize().Y);
                        ImGui.TableSetupColumn("##OptionDown", ImGuiTableColumnFlags.WidthFixed, ImGui.GetItemRectSize().Y);

                        for (int optionIdx = 0; optionIdx < group.OptionList.Count; optionIdx++)
                        {
                            ImGui.PushID(optionIdx);

                            ImGui.TableNextColumn();

                            var option = group.OptionList[optionIdx];
                            ImGui.Text($"Option {optionIdx}");

                            ImGui.TableNextColumn();
                            if (group.SelectionType == SelectionType.Single)
                            {
                                int IsCheckedOptionIdx = option.IsChecked ? optionIdx : -1;
                                if (ImGui.RadioButton($"##OptionIsChecked_{groupIdx}_{optionIdx}", ref IsCheckedOptionIdx, optionIdx))
                                {
                                    group.OptionList.ForEach(item => { item.IsChecked = false; });
                                    option.IsChecked = IsCheckedOptionIdx == optionIdx;
                                }
                                ImGui.SetItemTooltip("Set if this should be the default selected option.");
                            }
                            else if (group.SelectionType == SelectionType.Multi)
                            {
                                bool IsCheckedOption = option.IsChecked;
                                if (ImGui.Checkbox($"##OptionIsChecked_{groupIdx}_{optionIdx}", ref IsCheckedOption))
                                {
                                    option.IsChecked = IsCheckedOption;
                                }
                                ImGui.SetItemTooltip("Set if this option should be default Enabled or Disabled.");
                            }

                            ImGui.TableNextColumn();
                            ImGui.SetNextItemWidth(-1);
                            string OptionNameInput = option.Name;
                            if (ImGui.InputText($"##OptionNameInput_{groupIdx}_{optionIdx}", ref OptionNameInput, 128))
                            {
                                option.Name = OptionNameInput;
                            }

                            ImGui.TableNextColumn();
                            ImGui.PushStyleColor(ImGuiCol.Text, Colors.Goldenrod_Transparent);
                            FontManager.PushFont("FAS");
                            if (ImGui.Button($"{FASIcons.Pen}##EditOptionModFileList_{groupIdx}_{optionIdx}", new Vector2(ImGui.GetItemRectSize().Y, ImGui.GetItemRectSize().Y)))
                            {
                                // Open dialog to specify an arbitrary number of mod file paths and game file paths
                                ModOptionFilePathBindingWindow.SetOptionToEdit(option, IncludedModFilesList, BaseModPathDirectory, groupIdx, optionIdx);
                                ImGui.OpenPopup(ModOptionFilePathBindingWindow.EDITOR_WINDOW_ID);
                            }
                            FontManager.PopFont();
                            ImGui.PopStyleColor();
                            if (ImGui.BeginItemTooltip())
                            {
                                ImGui.Text("Edit the Mod File and Game File bindings when this option is enabled.");
                                if (option.FilePaths.Count > 0)
                                {
                                    ImGui.Text($"[Option {optionIdx}] contains {option.FilePaths.Count} Bindings.");
                                }
                                ImGui.EndTooltip();
                            }
                            ModOptionFilePathBindingWindow.Draw();

                            ImGui.TableNextColumn();
                            ImGui.PushStyleColor(ImGuiCol.Text, Colors.LightRed);
                            FontManager.PushFont("FAS");
                            if (ImGui.Button($"{FASIcons.Minus}##RemoveOptionBtn_{groupIdx}_{optionIdx}", new Vector2(ImGui.GetItemRectSize().Y, ImGui.GetItemRectSize().Y)))
                            {
                                // When Option is removed also remove all Bindings
                                UpdateBindingsReferences(groupIdx, optionIdx);

                                if (group.SelectionType == SelectionType.Single)
                                {
                                    // Make the first entry in the list the new checked one
                                    bool WasChecked = option.IsChecked;
                                    group.OptionList.RemoveAt(optionIdx);
                                    if (group.OptionList.Count > 0 && WasChecked)
                                    {
                                        group.OptionList[0].IsChecked = true;
                                    }
                                }
                                else if (group.SelectionType == SelectionType.Multi)
                                {
                                    group.OptionList.RemoveAt(optionIdx);
                                }
                            }
                            FontManager.PopFont();
                            ImGui.PopStyleColor();
                            ImGui.SetItemTooltip("Delete this option from the group.");

                            // TODO: Potentially merge MoveOption and UpdateBindingsReferences into a single utility function
                            ImGui.TableNextColumn();
                            ImGui.BeginDisabled(optionIdx == 0);
                            FontManager.PushFont("FAS");
                            if (ImGui.Button($"{FASIcons.ChevronUp}##MoveOptionUpBtn_{groupIdx}_{optionIdx}", new Vector2(ImGui.GetItemRectSize().Y, ImGui.GetItemRectSize().Y)))
                            {
                                MoveOption(groupIdx, optionIdx, optionIdx - 1);
                            }
                            FontManager.PopFont();
                            ImGui.SetItemTooltip("Move Mod Option Up.");
                            ImGui.EndDisabled();

                            ImGui.TableNextColumn();
                            ImGui.BeginDisabled(optionIdx == group.OptionList.Count - 1);
                            FontManager.PushFont("FAS");
                            if (ImGui.Button($"{FASIcons.ChevronDown}##MoveOptionDownBtn_{groupIdx}_{optionIdx}", new Vector2(ImGui.GetItemRectSize().Y, ImGui.GetItemRectSize().Y)))
                            {
                                MoveOption(groupIdx, optionIdx, optionIdx + 1);
                            }
                            FontManager.PopFont();
                            ImGui.SetItemTooltip("Move Mod Option Down.");
                            ImGui.EndDisabled();

                            ImGui.PopID();

                        }

                        // Insert AddNewOption entry to group
                        ImGui.TableNextColumn();
                        ImGui.Text($"Option {group.OptionList.Count}");
                        ImGui.TableNextColumn();
                        ImGui.TableNextColumn(); // Skip the radio/checkbox column

                        ImGui.SetNextItemWidth(-1);
                        string NewGroupOptionNameInput = GroupOptionNameInput[groupIdx];
                        if (ImGui.InputTextWithHint($"##NewOptionNameInput_{groupIdx}_{group.OptionList.Count}", "Add New Option", ref NewGroupOptionNameInput, 128))
                        {
                            GroupOptionNameInput[groupIdx] = NewGroupOptionNameInput;
                        }

                        ImGui.TableNextColumn();
                        ImGui.PushStyleColor(ImGuiCol.Text, Colors.LightGreen);
                        FontManager.PushFont("FAS");
                        if (ImGui.Button($"{FASIcons.Plus}##AddNewOptionBtn_{groupIdx}", new Vector2(ImGui.GetItemRectSize().Y, ImGui.GetItemRectSize().Y)))
                        {
                            var NewGroupOption = new ModOption()
                            {
                                Name = GroupOptionNameInput[groupIdx],
                                IsChecked = false,
                                Description = ""
                            };

                            // At least one item must be Checked in a Single SelectionType group
                            // So default the first item added to be Checked
                            if (group.OptionList.Count == 0)
                            {
                                NewGroupOption.IsChecked = true;
                            }

                            group.OptionList.Add(NewGroupOption);
                            GroupOptionNameInput[groupIdx] = "";
                        }
                        FontManager.PopFont();
                        ImGui.PopStyleColor();
                        ImGui.SetItemTooltip("Add a new option to the options group.");

                        ImGui.EndTable();
                    }

                    // Remove group after all logic has finished for this pass to prevent errors
                    if (QueuedGroupRemoval)
                    {
                        // When a group is removed also remove all mod file Bindings
                        UpdateBindingsReferences(groupIdx);

                        ModPackage.ModGroups.RemoveAt(groupIdx);
                        GroupOptionNameInput.RemoveAt(groupIdx);
                    }

                    ImGui.PopID();
                }

                ImGui.BeginDisabled(string.IsNullOrWhiteSpace(ModPackage.Name) || ModPackage.Name.Length < 3);
                if (ImGui.Button("Save Mod Pack", new Vector2(Util.BUTTON_ITEM_WIDTH_BASE * 2.0f, 0)))
                {
                    ImFileBrowser.SaveFile((SelectedFilePath) =>
                    {
                        WritePackToFile(SelectedFilePath);
                        PromptModCreationCompleteDialog = true;
                    }, title: "Select the path to save your Mod Package to...", filter: "Mod Package (*.zip)|*.zip", defaultSaveFileName: $"{ModPackage.Name}");
                }
                ImGui.EndDisabled();

                ImFileBrowser.Draw();

                ImGui.EndPopup();
            }
        }

        static void ShowModCreationCompleteDialog()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(new Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.SetNextWindowSize(new Vector2(0, 0), ImGuiCond.Appearing);

            string DialogTitle = "Mod Pack Created";
            if (ModCreationError != "")
            {
                DialogTitle = "Error Creating Mod Pack";
                ImGui.PushStyleColor(ImGuiCol.TitleBgActive, Colors.DarkRed);
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.TitleBgActive, Colors.DarkGreen);
            }

            if (Util.BeginPopupModal($"{DialogTitle}###ModCreationComplete", ImGuiWindowFlags.None))
            {
                if (ModCreationError == "")
                {
                    ImGui.Text("Your Mod Pack has been successfully created!");
                }
                else
                {
                    ImGui.Text("There was an error creating your Mod Pack!");
                    ImGui.SeparatorText("Error Details");
                    ImGui.TextWrapped(ModCreationError);
                }

                ImGui.Separator();

                if (ImGui.Button("OK", new Vector2(Util.BUTTON_ITEM_WIDTH_BASE, 0)))
                {
                    ModCreationError = "";
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }

            ImGui.PopStyleColor();
        }

        static void MoveGroup(int CurrentGroupIdx, int NewGroupIdx)
        {
            var currentGroup = ModPackage.ModGroups[CurrentGroupIdx];

            for (int optionIdx = 0; optionIdx < currentGroup.OptionList.Count; optionIdx++)
            {
                var option = currentGroup.OptionList[optionIdx];

                for (int bindingIdx = 0; bindingIdx < option.FilePaths.Count; bindingIdx++)
                {
                    var FilePath = option.FilePaths[bindingIdx];

                    if (!string.IsNullOrEmpty(FilePath.SourcePath))
                    {
                        string TempIdentifier = $"[MoveCurrentGroup_{CurrentGroupIdx}]";
                        string SelectedIdentifier = $"G{CurrentGroupIdx}O{optionIdx}B{bindingIdx};";
                        string SourcePathReferences = IncludedModFilesList[option.FilePaths[bindingIdx].SourcePath];
                        IncludedModFilesList[option.FilePaths[bindingIdx].SourcePath] = SourcePathReferences.Replace(SelectedIdentifier, TempIdentifier);
                    }
                }
            }

            var otherGroup = ModPackage.ModGroups[NewGroupIdx];

            for (int optionIdx = 0; optionIdx < otherGroup.OptionList.Count; optionIdx++)
            {
                var option = otherGroup.OptionList[optionIdx];

                for (int bindingIdx = 0; bindingIdx < option.FilePaths.Count; bindingIdx++)
                {
                    var FilePath = option.FilePaths[bindingIdx];

                    if (!string.IsNullOrEmpty(FilePath.SourcePath))
                    {
                        string SelectedIdentifier = $"G{NewGroupIdx}O{optionIdx}B{bindingIdx};";
                        string NewIdentifier = $"G{CurrentGroupIdx}O{optionIdx}B{bindingIdx};";
                        string SourcePathReferences = IncludedModFilesList[option.FilePaths[bindingIdx].SourcePath];
                        IncludedModFilesList[option.FilePaths[bindingIdx].SourcePath] = SourcePathReferences.Replace(SelectedIdentifier, NewIdentifier);
                    }
                }
            }

            for (int optionIdx = 0; optionIdx < currentGroup.OptionList.Count; optionIdx++)
            {
                var option = currentGroup.OptionList[optionIdx];

                for (int bindingIdx = 0; bindingIdx < option.FilePaths.Count; bindingIdx++)
                {
                    var FilePath = option.FilePaths[bindingIdx];

                    if (!string.IsNullOrEmpty(FilePath.SourcePath))
                    {
                        string TempIdentifier = $"[MoveCurrentGroup_{CurrentGroupIdx}]";
                        string NewIdentifier = $"G{NewGroupIdx}O{optionIdx}B{bindingIdx};";
                        string SourcePathReferences = IncludedModFilesList[option.FilePaths[bindingIdx].SourcePath];
                        IncludedModFilesList[option.FilePaths[bindingIdx].SourcePath] = SourcePathReferences.Replace(TempIdentifier, NewIdentifier);
                    }
                }
            }

            ModPackage.ModGroups[CurrentGroupIdx] = ModPackage.ModGroups[NewGroupIdx];
            ModPackage.ModGroups[NewGroupIdx] = currentGroup;
        }

        static void MoveOption(int GroupIdx, int CurrentOptionIdx, int NewOptionIdx)
        {
            var group = ModPackage.ModGroups[GroupIdx];

            var currentOption = group.OptionList[CurrentOptionIdx];
            // Adjust the bindings of the current option to point to the soon to be index
            for (int bindingIdx = 0; bindingIdx < currentOption.FilePaths.Count; bindingIdx++)
            {
                var FilePath = currentOption.FilePaths[bindingIdx];

                if (!string.IsNullOrEmpty(FilePath.SourcePath))
                {
                    // Use a temporary globally unique identifier before we set the real one to prevent clashing with the other option being adjusted
                    string TempIdentifier = $"[MoveCurrentOption_{GroupIdx}_{CurrentOptionIdx}]";
                    string SelectedIdentifier = $"G{GroupIdx}O{CurrentOptionIdx}B{bindingIdx};";
                    string SourcePathReferences = IncludedModFilesList[currentOption.FilePaths[bindingIdx].SourcePath];
                    IncludedModFilesList[currentOption.FilePaths[bindingIdx].SourcePath] = SourcePathReferences.Replace(SelectedIdentifier, TempIdentifier);
                }
            }

            var otherOption = group.OptionList[NewOptionIdx];
            // Adjust the bindings of the second option being moved in response to have their new index
            for (int bindingIdx = 0; bindingIdx < otherOption.FilePaths.Count; bindingIdx++)
            {
                var FilePath = otherOption.FilePaths[bindingIdx];

                if (!string.IsNullOrEmpty(FilePath.SourcePath))
                {
                    string SelectedIdentifier = $"G{GroupIdx}O{NewOptionIdx}B{bindingIdx};";
                    string NewIdentifier = $"G{GroupIdx}O{CurrentOptionIdx}B{bindingIdx};";
                    string SourcePathReferences = IncludedModFilesList[otherOption.FilePaths[bindingIdx].SourcePath];
                    IncludedModFilesList[otherOption.FilePaths[bindingIdx].SourcePath] = SourcePathReferences.Replace(SelectedIdentifier, NewIdentifier);
                }
            }

            // Finish binding adjusmtents by setting the original option causing the adjustment to its new index value
            for (int bindingIdx = 0; bindingIdx < currentOption.FilePaths.Count; bindingIdx++)
            {
                var FilePath = currentOption.FilePaths[bindingIdx];

                if (!string.IsNullOrEmpty(FilePath.SourcePath))
                {
                    string TempIdentifier = $"[MoveCurrentOption_{GroupIdx}_{CurrentOptionIdx}]";
                    string NewIdentifier = $"G{GroupIdx}O{NewOptionIdx}B{bindingIdx};";
                    string SourcePathReferences = IncludedModFilesList[currentOption.FilePaths[bindingIdx].SourcePath];
                    IncludedModFilesList[currentOption.FilePaths[bindingIdx].SourcePath] = SourcePathReferences.Replace(TempIdentifier, NewIdentifier);
                }
            }

            group.OptionList[CurrentOptionIdx] = group.OptionList[NewOptionIdx];
            group.OptionList[NewOptionIdx] = currentOption;
        }

        static void RemoveBindingsFromOption(int GroupIdx, int OptionIdx, ModOption modOption)
        {
            for (int bindingIdx = 0; bindingIdx < modOption.FilePaths.Count; bindingIdx++)
            {
                var FilePath = modOption.FilePaths[bindingIdx];

                if (!string.IsNullOrEmpty(FilePath.SourcePath))
                {
                    string SelectedIdentifier = $"G{GroupIdx}O{OptionIdx}B{bindingIdx};";
                    string SourcePathReferences = IncludedModFilesList[modOption.FilePaths[bindingIdx].SourcePath];
                    IncludedModFilesList[modOption.FilePaths[bindingIdx].SourcePath] = SourcePathReferences.Replace(SelectedIdentifier, "");
                }
            }
        }

        static void UpdateBindingsReferences(int GroupIdxRemoved, int OptionIdxRemoved = -1)
        {
            for (int i = 0; i < IncludedModFilesList.Count; i++)
            {
                var ModFilePath = IncludedModFilesList.ElementAt(i);
                var References = ModFilePath.Value.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
                bool GroupRemoved = OptionIdxRemoved < 0;
                bool OptionRemoved = !GroupRemoved;
                if (GroupRemoved)
                {
                    References.RemoveAll(Reference => Reference.StartsWith($"G{GroupIdxRemoved}"));
                    for (int RefIdx = 0; RefIdx < References.Count; RefIdx++)
                    {
                        var Reference = References[RefIdx];
                        int OptionStart = Reference.IndexOf("O");
                        string CurrentGroupValue = Reference.Substring(1, OptionStart - 1);
                        int CurrentGroupIdx = int.Parse(CurrentGroupValue);

                        if (CurrentGroupIdx > GroupIdxRemoved)
                        {
                            string UpdatedReference = $"G{CurrentGroupIdx - 1}{Reference.Substring(OptionStart)}";
                            References[RefIdx] = UpdatedReference;
                        }
                    }
                }
                else if (OptionRemoved)
                {
                    References.RemoveAll(Reference => Reference.StartsWith($"G{GroupIdxRemoved}O{OptionIdxRemoved}"));
                    for (int RefIdx = 0; RefIdx < References.Count; RefIdx++)
                    {
                        var Reference = References[RefIdx];
                        if (Reference.StartsWith($"G{GroupIdxRemoved}"))
                        {
                            int OptionStart = Reference.IndexOf("O") + 1;
                            int BindingStart = Reference.IndexOf("B");
                            string CurrentOptionValue = Reference.Substring(OptionStart, BindingStart - OptionStart);
                            int CurrentOptionIdx = int.Parse(CurrentOptionValue);

                            if (CurrentOptionIdx > OptionIdxRemoved)
                            {
                                string UpdatedReference = $"G{GroupIdxRemoved}O{CurrentOptionIdx - 1}{Reference.Substring(BindingStart)}";
                                References[RefIdx] = UpdatedReference;
                            }
                        }
                    }
                }
                var UpdatedReferences = string.Join(";", References);
                IncludedModFilesList[ModFilePath.Key] = UpdatedReferences;
            }
        }

        static void MakeAbsoluteBindingsRelative()
        {
            foreach (var group in ModPackage.ModGroups)
            {
                foreach (var option in group.OptionList)
                {
                    // Remove all empty bindings
                    option.FilePaths.RemoveAll(Paths => string.IsNullOrEmpty(Paths.SourcePath) || string.IsNullOrEmpty(Paths.DestinationPath));

                    foreach (var paths in option.FilePaths)
                    {
                        if (!string.IsNullOrEmpty(paths.SourcePath))
                        {
                            paths.SourcePath = paths.SourcePath.Remove(0, BaseModPathDirectory.Length + 1);
                        }
                    }
                }
            }
        }

        static void WritePackToFile(string OutputArchivePath)
        {
            // Before the ModPackage object can be written, the absolute ModFilePath's must be converted to relative
            MakeAbsoluteBindingsRelative();
            string PackJson = Newtonsoft.Json.JsonConvert.SerializeObject(ModPackage, Newtonsoft.Json.Formatting.Indented);

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        var ModConfigJsonEntry = archive.CreateEntry("ModConfig.json");

                        using (var entryStream = ModConfigJsonEntry.Open())
                        {
                            using (var streamWriter = new StreamWriter(entryStream))
                            {
                                streamWriter.Write(PackJson);
                            }
                        }

                        for (int ModFileIdx = 0; ModFileIdx < IncludedModFilesList.Count; ModFileIdx++)
                        {
                            var ModFile = IncludedModFilesList.ElementAt(ModFileIdx);
                            string RelativeFilePath = ModFile.Key.Remove(0, BaseModPathDirectory.Length + 1);

                            // Only include referenced files in the final package unless the user requested otherwise
                            bool HasReferences = !string.IsNullOrEmpty(ModFile.Value);
                            if (HasReferences || IncludeUnreferencedFiles)
                            {
                                archive.CreateEntryFromFile(ModFile.Key, RelativeFilePath);
                            }
                        }
                    }

                    using (var fileStream = new FileStream(OutputArchivePath, FileMode.Create))
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        memoryStream.CopyTo(fileStream);
                    }

                    Console.WriteLine($"Finished writting mod package to {OutputArchivePath}");
                }
            }
            catch (Exception ex)
            {
                ModCreationError = ex.ToString();
            }
        }
    }
}
