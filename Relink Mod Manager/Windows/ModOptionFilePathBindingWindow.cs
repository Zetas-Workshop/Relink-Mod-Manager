using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Relink_Mod_Manager.Windows
{
    public static class ModOptionFilePathBindingWindow
    {
        public const string EDITOR_WINDOW_ID = "###ModOptionFilePathBindingEditor";
        static ModOption ModOption;
        static Dictionary<string, string> IncludedModFilesList;
        static string BaseModPathDirectory = "";
        static int GroupIdx = 0;
        static int OptionIdx = 0;
        static HashSet<string> FileDirectoryFilter = new HashSet<string>();
        static string SelectedFilter = "{No Filter}";

        public static void Draw()
        {
            SubmitContent();
        }

        public static void SetOptionToEdit(ModOption modOption, Dictionary<string, string> includedModFilesList, string baseModPathDirectory, int groupIdx, int optionIdx)
        {
            ModOption = modOption;
            IncludedModFilesList = includedModFilesList;
            BaseModPathDirectory = baseModPathDirectory;
            GroupIdx = groupIdx;
            OptionIdx = optionIdx;
            FileDirectoryFilter = new HashSet<string>() { "{No Filter}" };
            SelectedFilter = "{No Filter}";

            foreach (var FileEntry in IncludedModFilesList)
            {
                var fragments = Path.GetDirectoryName(FileEntry.Key).Remove(0, BaseModPathDirectory.Length + 1).Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
                string BuildingPath = BaseModPathDirectory;
                foreach (var Fragment in fragments)
                {
                    BuildingPath = Path.Combine(BuildingPath, Fragment);
                    FileDirectoryFilter.Add(BuildingPath);
                }
            }
        }

        static void SubmitContent()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(new Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.SetNextWindowSize(new Vector2(920, 680), ImGuiCond.Appearing);
            ImGui.SetNextWindowSizeConstraints(new Vector2(780, 480), new Vector2(float.PositiveInfinity, float.PositiveInfinity));

            bool p_open = true;
            if (ImGui.BeginPopupModal($"Mod Option Bindings - [Group {GroupIdx} > Option {OptionIdx}]{EDITOR_WINDOW_ID}", ref p_open))
            {
                ImGui.AlignTextToFramePadding();

                ImGui.Text("Configure the Mod File to Game File replacement bindings for when this option is used.");

                ImGui.PushStyleColor(ImGuiCol.Text, Colors.LightGreen);
                ImGui.Text("Green Mod File Path");
                ImGui.PopStyleColor();
                ImGui.SameLine(0, 0);
                ImGui.Text(" = Mod File is used in at least one other option already.");

                ImGui.PushStyleColor(ImGuiCol.Text, Colors.Red);
                ImGui.Text("Red Mod File Path");
                ImGui.PopStyleColor();
                ImGui.SameLine(0, 0);
                ImGui.Text(" = Mod File has not been used in any options yet.");

                ImGui.Indent();
                ImGui.Text("By default, Mod Files that are never used by an option will not be included in the final Mod Package.");
                ImGui.Unindent();
                ImGui.Text("Bindings without a Game File Path value will be deleted during Mod Pack Creation.");
                ImGui.Text("All changes are immediately saved and applied.");

                ImGui.Separator();

                ImGui.Text("Option Description:");
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                string OptionDescription = ModOption.Description;
                if (ImGui.InputText("##OptionDescriptionInput", ref OptionDescription, 256))
                {
                    ModOption.Description = OptionDescription;
                }
                ImGui.SetItemTooltip("Tooltip text to display when user hovers over option.");

                if (ImGui.Button("Add New Binding", new Vector2(Util.BUTTON_ITEM_WIDTH_SECOND, 0)))
                {
                    ModOption.FilePaths.Add(new ModFilePath());
                }
                ImGui.SetItemTooltip("Create a new binding between a Mod File Path and Game File Path.\nNote: Game File Paths should start with 'data\\'");

                ImGui.SameLine();
                if (ImGui.Button("Trim Game File Paths"))
                {
                    foreach (var FilePath in ModOption.FilePaths)
                    {
                        if (FilePath.DestinationPath.Contains("\\data\\") && !FilePath.DestinationPath.StartsWith("data\\"))
                        {
                            string DestinationPath = FilePath.DestinationPath;
                            FilePath.DestinationPath = DestinationPath.Substring(DestinationPath.IndexOf("\\data\\") + 1);
                        }
                    }
                }
                if (ImGui.BeginItemTooltip())
                {
                    ImGui.Text("Automatically trim every Game File Path to begin at the 'data\\' folder.");
                    ImGui.Text("Useful for when you have a lot of mod files that need trimming.");
                    ImGui.EndTooltip();
                }

                ImGui.SameLine();
                if (ImGui.Button("Add Unreferenced Files As Bindings"))
                {
                    foreach (var FileEntry in IncludedModFilesList)
                    {
                        if (string.IsNullOrEmpty(FileEntry.Value))
                        {
                            if (!string.IsNullOrEmpty(SelectedFilter) && SelectedFilter != "{No Filter}")
                            {
                                if (!FileEntry.Key.StartsWith(SelectedFilter))
                                {
                                    continue;
                                }
                            }

                            ModOption.FilePaths.Add(new ModFilePath());
                            int BindingIdx = ModOption.FilePaths.Count - 1;

                            string SelectedIdentifier = $"G{GroupIdx}O{OptionIdx}B{BindingIdx};";

                            IncludedModFilesList[FileEntry.Key] += SelectedIdentifier;
                            ModOption.FilePaths[BindingIdx].SourcePath = FileEntry.Key;
                            ModOption.FilePaths[BindingIdx].DestinationPath = FileEntry.Key.Remove(0, BaseModPathDirectory.Length + 1);
                        }
                    }
                }
                if (ImGui.BeginItemTooltip())
                {
                    ImGui.Text("Automatically creates a new binding entry for every single unreferenced file and sets the Game File Path.");
                    ImGui.Text("Useful for when you have a lot of mod files and want all of them to be bound to a single option.");
                    ImGui.EndTooltip();
                }

                string FilterFilter = "{No Filter}";
                if (!string.IsNullOrEmpty(SelectedFilter) && SelectedFilter != "{No Filter}")
                {
                    FilterFilter = SelectedFilter.Remove(0, BaseModPathDirectory.Length + 1);
                }
                ImGui.SameLine();
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                if (ImGui.BeginCombo("##UnreferencedFilesFilter", FilterFilter, ImGuiComboFlags.None))
                {
                    foreach (var DirFilter in FileDirectoryFilter)
                    {
                        if (DirFilter != BaseModPathDirectory)
                        {
                            string DisplayText = "{No Filter}";
                            if (DirFilter != "{No Filter}")
                            {
                                DisplayText = $"{DirFilter.Remove(0, BaseModPathDirectory.Length + 1)}\\*";
                            }
                            if (ImGui.Selectable($"{DisplayText}"))
                            {
                                SelectedFilter = DirFilter;
                            }
                        }
                    }
                    
                    ImGui.EndCombo();
                }
                if (ImGui.BeginItemTooltip())
                {
                    ImGui.TextDisabled($"{FilterFilter}");
                    ImGui.Text("Filter for what files to create New Bindings for when 'Add Unreferenced Files' is used.");
                    ImGui.Text("Leaving set to '{No Filter}' will include all files.");
                    ImGui.EndTooltip();
                }

                if (ImGui.BeginChild("##BindingOptionsChild", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - 32)))
                {
                    var style = ImGui.GetStyle();

                    for (int i = 0; i < ModOption.FilePaths.Count; i++)
                    {
                        bool QueuedGroupRemoval = false;

                        ImGui.SeparatorText($"Binding {i}");

                        string PreviewText = "";
                        if (!string.IsNullOrEmpty(ModOption.FilePaths[i].SourcePath))
                        {
                            PreviewText = ModOption.FilePaths[i].SourcePath.Remove(0, BaseModPathDirectory.Length + 1);
                        }

                        if (ImGui.BeginTable($"##BindingOptionsTable_{i}", 4))
                        {
                            var LabelWidth = ImGui.CalcTextSize("Game File Path:").X;
                            ImGui.TableSetupColumn("##LableColumn", ImGuiTableColumnFlags.WidthFixed, LabelWidth);
                            float ButtonCount = 2.0f;
                            var InputBindingColumnWidth = ImGui.GetContentRegionAvail().X - LabelWidth - ImGui.GetFrameHeight() - ImGui.GetItemRectSize().Y * ButtonCount - style.ItemSpacing.X;
                            ImGui.TableSetupColumn("##OptionBinding", ImGuiTableColumnFlags.WidthFixed, InputBindingColumnWidth);
                            ImGui.TableSetupColumn("##BindingCopyDown", ImGuiTableColumnFlags.WidthFixed, ImGui.GetItemRectSize().Y);
                            ImGui.TableSetupColumn("##BindingRemove", ImGuiTableColumnFlags.WidthFixed, ImGui.GetItemRectSize().Y);

                            ImGui.TableNextColumn();

                            ImGui.Text("Mod File Path:");
                            ImGui.TableNextColumn();
                            ImGui.SetNextItemWidth(-1);
                            if (ImGui.BeginCombo($"##BindingCombo_ModFilePath_{i}", PreviewText, ImGuiComboFlags.None))
                            {
                                foreach (var FileEntry in IncludedModFilesList)
                                {
                                    bool is_selected = FileEntry.Key == ModOption.FilePaths[i].SourcePath;

                                    if (!string.IsNullOrEmpty(FileEntry.Value))
                                    {
                                        ImGui.PushStyleColor(ImGuiCol.Text, Colors.LightGreen);
                                    }
                                    else
                                    {
                                        ImGui.PushStyleColor(ImGuiCol.Text, Colors.Red);
                                    }

                                    if (ImGui.Selectable($"{FileEntry.Key.Remove(0, BaseModPathDirectory.Length + 1)}##ModFilePathComboItem_{i}", is_selected))
                                    {
                                        string SelectedIdentifier = $"G{GroupIdx}O{OptionIdx}B{i};";

                                        if (!string.IsNullOrEmpty(ModOption.FilePaths[i].SourcePath))
                                        {
                                            string SourcePathReferences = IncludedModFilesList[ModOption.FilePaths[i].SourcePath];
                                            IncludedModFilesList[ModOption.FilePaths[i].SourcePath] = SourcePathReferences.Replace(SelectedIdentifier, "");
                                        }
                                        IncludedModFilesList[FileEntry.Key] += SelectedIdentifier;
                                        ModOption.FilePaths[i].SourcePath = FileEntry.Key;
                                    }

                                    ImGui.PopStyleColor();

                                    if (is_selected)
                                    {
                                        ImGui.SetItemDefaultFocus();
                                    }
                                }
                                ImGui.EndCombo();
                            }

                            ImGui.TableNextColumn();
                            ImGui.PushStyleColor(ImGuiCol.Text, Colors.SkyBlue);
                            FontManager.PushFont("FAS");
                            if (ImGui.Button($"{FASIcons.TurnDown}##CopyModPathToGamePath_{i}", new Vector2(ImGui.GetItemRectSize().Y, ImGui.GetItemRectSize().Y)))
                            {
                                if (!string.IsNullOrEmpty(ModOption.FilePaths[i].SourcePath))
                                {
                                    ModOption.FilePaths[i].DestinationPath = ModOption.FilePaths[i].SourcePath.Remove(0, BaseModPathDirectory.Length + 1);
                                }
                                else
                                {
                                    ModOption.FilePaths[i].DestinationPath = "";
                                }
                            }
                            FontManager.PopFont();
                            ImGui.PopStyleColor();
                            ImGui.SetItemTooltip("Copy the Mod File Path down to the Game File Path.");


                            ImGui.TableNextColumn();
                            ImGui.PushStyleColor(ImGuiCol.Text, Colors.LightRed);
                            FontManager.PushFont("FAS");
                            if (ImGui.Button($"{FASIcons.Minus}##RemoveOptionBtn_{i}", new Vector2(ImGui.GetItemRectSize().Y, ImGui.GetItemRectSize().Y)))
                            {
                                QueuedGroupRemoval = true;
                            }
                            FontManager.PopFont();
                            ImGui.PopStyleColor();
                            ImGui.SetItemTooltip("Delete this binding entry.");

                            ImGui.TableNextColumn();
                            ImGui.Text("Game File Path:");
                            ImGui.TableNextColumn();
                            ImGui.SetNextItemWidth(-1);
                            string BindingTextGamePathInput = ModOption.FilePaths[i].DestinationPath;
                            if (ImGui.InputText($"##BindingText_GameFilePath_{i}", ref BindingTextGamePathInput, 256))
                            {
                                ModOption.FilePaths[i].DestinationPath = BindingTextGamePathInput;
                            }
                            ImGui.SetItemTooltip("This path should always start with 'data\\'");

                            if (ModOption.FilePaths[i].DestinationPath.Contains("\\data\\") && !ModOption.FilePaths[i].DestinationPath.StartsWith("data\\"))
                            {
                                ImGui.TableNextColumn();
                                ImGui.PushStyleColor(ImGuiCol.Text, Colors.SkyBlue);
                                FontManager.PushFont("FAS");
                                if (ImGui.Button($"{FASIcons.CropSimple}##TrimGamePathToDataStart_{i}", new Vector2(ImGui.GetItemRectSize().Y, ImGui.GetItemRectSize().Y)))
                                {
                                    string DestinationPath = ModOption.FilePaths[i].DestinationPath;
                                    if (!string.IsNullOrEmpty(DestinationPath) && DestinationPath.Contains("\\data\\"))
                                    {
                                        ModOption.FilePaths[i].DestinationPath = DestinationPath.Substring(DestinationPath.IndexOf("\\data\\") + 1);
                                    }
                                }
                                FontManager.PopFont();
                                ImGui.PopStyleColor();
                                ImGui.SetItemTooltip("Trim the Game File Path to begin at the 'data\\' folder.");
                            }

                            ImGui.EndTable();
                        }

                        if (QueuedGroupRemoval)
                        {
                            if (!string.IsNullOrEmpty(ModOption.FilePaths[i].SourcePath))
                            {
                                string SelectedIdentifier = $"G{GroupIdx}O{OptionIdx}B{i};";
                                string SourcePathReferences = IncludedModFilesList[ModOption.FilePaths[i].SourcePath];
                                IncludedModFilesList[ModOption.FilePaths[i].SourcePath] = SourcePathReferences.Replace(SelectedIdentifier, "");
                            }

                            ModOption.FilePaths.RemoveAt(i);
                            UpdateBindingReferences(i);
                        }
                    }

                    ImGui.EndChild();
                }

                if (ImGui.Button("Close", new Vector2(Util.BUTTON_ITEM_WIDTH_SECOND, 0)))
                {
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }
        }

        static void UpdateBindingReferences(int IdxRemoved)
        {
            for (int i = IdxRemoved; i < ModOption.FilePaths.Count; i++)
            {
                if (!string.IsNullOrEmpty(ModOption.FilePaths[i].SourcePath))
                {
                    string OldIdentifier = $"G{GroupIdx}O{OptionIdx}B{i + 1};";
                    string NewIdentifier = $"G{GroupIdx}O{OptionIdx}B{i};";
                    string SourcePathReferences = IncludedModFilesList[ModOption.FilePaths[i].SourcePath];
                    IncludedModFilesList[ModOption.FilePaths[i].SourcePath] = SourcePathReferences.Replace(OldIdentifier, NewIdentifier);
                }
            }
        }
    }
}
