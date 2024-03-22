using ImGuiNET;
using Relink_Mod_Manager.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Relink_Mod_Manager.Windows
{
    public class SettingsWindow : Window
    {
        bool ShouldTrackOpenState = false;
        bool PromptModArchiveDirCopy = false;
        string NewModArchiveDir = "";

        public override void Draw()
        {
            SubmitContent();
        }

        void SubmitContent()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(new Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.SetNextWindowSize(new Vector2(850, 500), ImGuiCond.Appearing);
            ImGui.SetNextWindowSizeConstraints(new Vector2(750, 350), new Vector2(float.PositiveInfinity, float.PositiveInfinity));

            bool p_open = true;
            if (ImGui.BeginPopupModal("Settings", ref p_open))
            {
                ShouldTrackOpenState = true;

                var style = ImGui.GetStyle();

                ImGui.Text("Game Executable Path:");
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - Util.BUTTON_ITEM_WIDTH_BASE - style.ItemSpacing.X);
                ImGui.InputText("##GameExePath", ref Settings.GameExecutableFilePath, 512, ImGuiInputTextFlags.ReadOnly);
                ImGui.SameLine();
                if (ImGui.Button("Change Path...##SelectGameExePathBtn", new Vector2(Util.BUTTON_ITEM_WIDTH_BASE, 0)))
                {
                    ImFileBrowser.OpenFile(SelectedFilePath =>
                    {
                        Settings.GameExecutableFilePath = SelectedFilePath;
                    },
                    filter: "granblue_fantasy_relink.exe|granblue_fantasy_relink.exe|All Files (*.*)|*.*",
                    filterIndex: 0,
                    title: "Select the game executable for Granblue Fantasy: Relink...",
                    baseDir: Path.GetDirectoryName(Settings.GameExecutableFilePath));
                }

                ImGui.NewLine();
                ImGui.Text("Mod Package Archives Storage Path:");
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - (Util.BUTTON_ITEM_WIDTH_BASE * 2) - (style.ItemSpacing.X * 2));
                string ModArchivesDir = Settings.ModArchivesDirectory;
                ImGui.InputText("##ModArchivesDir", ref ModArchivesDir, 1024, ImGuiInputTextFlags.ReadOnly);
                ImGui.SameLine();
                if (ImGui.Button("Change Path...##SelectModArchiveStoragePathBtn", new Vector2(Util.BUTTON_ITEM_WIDTH_BASE, 0)))
                {
                    ImFileBrowser.SelectDir(SelectedDir =>
                    {
                        // If user has selected a new location and there were files in the old one, prompt about copying them over
                        if (SelectedDir != Settings.ModArchivesDirectory)
                        {
                            var entries = Directory.EnumerateFileSystemEntries(Settings.ModArchivesDirectory);
                            if (entries.Any())
                            {
                                NewModArchiveDir = SelectedDir;
                                PromptModArchiveDirCopy = true;
                            }
                            else
                            {
                                Settings.ModArchivesDirectory = SelectedDir;
                            }
                        }
                    }, title: "Select the new Mod Archives Storage directory...", baseDir: Settings.ModArchivesDirectory);
                }
                ImGui.SetItemTooltip("Change the Mod Archives Storage directory.\nMods will not be automatically copied to the new location.");

                ImGui.SameLine();
                if (ImGui.Button("Open Folder##OpenModArchivesDirBtn", new Vector2(Util.BUTTON_ITEM_WIDTH_BASE, 0)))
                {
                    if (Directory.Exists(ModArchivesDir))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                        {
                            FileName = $"{ModArchivesDir.TrimEnd('\\')}\\",
                            UseShellExecute = true,
                            Verb = "open"
                        });
                    }
                }
                ImGui.BeginDisabled();
                if (ImGui.Checkbox("Copy Mod Package Archives To Storage", ref Settings.CopyModArchivesToStorage))
                {
                    // TODO: Not copying to Storage was going to be supported but it broke mod already imported checks
                    // So for now we're going to enforce using it and come back to this later on and properly support it
                }
                ImGui.EndDisabled();
                ImGui.SetItemTooltip("Disabling this is not currently supported.");

                ImGui.NewLine();
                ImGui.Text("Mod Manager Settings Path:");
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - Util.BUTTON_ITEM_WIDTH_BASE - style.ItemSpacing.X);
                var AppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var FilePath = Path.Combine(AppData, "Relink Mod Manager");
                ImGui.InputText("##ModManagerSettingsDir", ref FilePath, 1024, ImGuiInputTextFlags.ReadOnly);
                ImGui.SameLine();
                if (ImGui.Button("Open Folder##OpenModManagerSettingsDirBtn", new Vector2(Util.BUTTON_ITEM_WIDTH_BASE, 0)))
                {
                    if (Directory.Exists(FilePath))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                        {
                            FileName = $"{FilePath.TrimEnd('\\')}\\",
                            UseShellExecute = true,
                            Verb = "open"
                        });
                    }
                }

                ImGui.NewLine();
                ImGui.Checkbox("Reduce Volatile Mod Warning Text", ref Settings.ReduceVolatileModWarningText);
                if (ImGui.BeginItemTooltip())
                {
                    ImGui.Text("For users that understand the risks of volatile mods and how to restore their game files after using them if things go wrong.");
                    ImGui.TextDisabled("Volatile mods are ones that replace Sound or UI files which are already external game files.");
                    ImGui.EndTooltip();
                }

                ImGui.NewLine();
                ImGui.Checkbox("Check For Mod Manager Updates on Startup", ref Settings.CheckForUpdateOnStartup);

                if (PromptModArchiveDirCopy)
                {
                    ImGui.OpenPopup("###ModArchiveDirCopyDialog");
                    PromptModArchiveDirCopy = false;
                }
                ShowModArchiveDirCopyDialog();

                ImFileBrowser.Draw();

                ImGui.EndPopup();
            }

            // Fires when window has just closed
            if (!p_open && ShouldTrackOpenState)
            {
                ShouldTrackOpenState = false;

                Settings.Save();
            }
        }

        void ShowModArchiveDirCopyDialog()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(new Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.SetNextWindowSize(new Vector2(650, 270), ImGuiCond.Appearing);
            ImGui.SetNextWindowSizeConstraints(new Vector2(650, 270), new Vector2(float.PositiveInfinity, float.PositiveInfinity));

            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, Colors.Goldenrod_Transparent);

            if (ImGui.BeginPopupModal("Copy Files In Mod Archive Storage###ModArchiveDirCopyDialog"))
            {
                ImGui.Text("There are files present in your current Mod Archive Storage directory.");
                ImGui.Text("Do you want to copy these to your newly selected directory?");
                ImGui.TextWrapped("It is strongly recommended to copy them over as failing to copy them over prior to managing your mods may result in entering a bad state.");
                ImGui.TextWrapped("Note: You may still copy them over manually in File Explorer prior to attempting a mod install/uninstall to prevent issues.");

                ImGui.Separator();
                string OldDirectory = Settings.ModArchivesDirectory;
                ImGui.Text("Old Directory: ");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                ImGui.InputText("##OldDirectoryInput", ref OldDirectory, 1024, ImGuiInputTextFlags.ReadOnly);

                string NewDirectory = NewModArchiveDir;
                ImGui.Text("New Directory:");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                ImGui.InputText("##NewDirectoryInput", ref NewDirectory, 1024, ImGuiInputTextFlags.ReadOnly);

                ImGui.Separator();

                ImGui.PushStyleColor(ImGuiCol.Button, Colors.DarkGreen_Transparent);
                if (ImGui.Button("Copy Now", new Vector2(Util.BUTTON_ITEM_WIDTH_SECOND, 0)))
                {
                    var files = Directory.EnumerateFiles(Settings.ModArchivesDirectory);
                    foreach (var file in files)
                    {
                        File.Copy(file, Path.Combine(NewDirectory, Path.GetFileName(file)), true);
                    }

                    // Update ModEntry's with new location
                    foreach (var entry in Settings.ModList)
                    {
                        if (entry.IsUsingArchiveStorage)
                        {
                            entry.ModArchivePath = Path.Combine(NewModArchiveDir, Path.GetFileName(entry.ModArchivePath));
                        }
                    }

                    Settings.ModArchivesDirectory = NewModArchiveDir;

                    ImGui.CloseCurrentPopup();
                }
                ImGui.PopStyleColor();
                if (ImGui.BeginItemTooltip())
                {
                    ImGui.Text("Applies the directory change and begins copying all existing files.");
                    ImGui.Text("Note: The manager may appear unresponsive while the copy occurs.");
                    ImGui.PushStyleColor(ImGuiCol.Text, Colors.Red);
                    ImGui.Text("Any files and folders in the new directory may be overwritten.");
                    ImGui.PopStyleColor();
                    ImGui.EndTooltip();
                }

                ImGui.SameLine();
                if (ImGui.Button("Do Not Copy", new Vector2(Util.BUTTON_ITEM_WIDTH_SECOND, 0)))
                {
                    Settings.ModArchivesDirectory = NewModArchiveDir;
                    ImGui.CloseCurrentPopup();
                }
                if (ImGui.BeginItemTooltip())
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, Colors.Red);
                    ImGui.Text("[Not Recommended]");
                    ImGui.PopStyleColor();
                    ImGui.Text("The directory path will be changed but no existing files will be copied over to it.");
                    ImGui.Text("You will need to manually copy your mods into the new location for mod installing to function.");
                    ImGui.EndTooltip();
                }

                ImGui.SameLine();
                ImGui.SetCursorPosX(ImGui.GetWindowContentRegionMax().X - Util.BUTTON_ITEM_WIDTH_SECOND);
                if (ImGui.Button("Cancel Change", new Vector2(Util.BUTTON_ITEM_WIDTH_SECOND, 0)))
                {
                    ImGui.CloseCurrentPopup();
                }
                ImGui.SetItemTooltip("The directory will not be changed.");

                ImGui.EndPopup();
            }

            ImGui.PopStyleColor();
        }
    }
}
