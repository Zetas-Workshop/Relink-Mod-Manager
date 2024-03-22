using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GBFRDataTools.Entities;
using System.IO.Compression;
using Relink_Mod_Manager.Widgets;
using Relink_Mod_Manager.Dialogs;

namespace Relink_Mod_Manager.Windows
{
    public partial class ModManagerWindow : Window
    {
        static bool close_style_editor = false;
        static int SelectedModIndex = -1;
        static Vector2 MainMenuBarSize;
        private static EffectiveChangesWindow _effectiveChangesWindow = new EffectiveChangesWindow();
        private static SettingsWindow _settingsWindow = new SettingsWindow();
        private static FTUEWindow _ftueWindow = new FTUEWindow();
        static bool PromptFTUE = true;
        static List<string> UnsupportedModsList = new List<string>();
        static List<string> NewerFormatModsList = new List<string>();
        static List<string> AlreadyImportedModsList = new List<string>();
        static List<string> ModInvalidArchivePathList = new List<string>();
        static List<string> ModArchivesMissingDuringInstallList = new List<string>();
        static bool PromptImportModErrorWindow = false;
        static bool PromptNoCleanDataBackup = false;
        static bool PromptDataFileNoBackupRestore = false;
        static bool PromptErrorCleanDataRestore = false;
        static bool PromptImportModBrowser = false;
        static bool PromptEditModPackFileBrowser = false;
        static bool PromptModsInstalled = false;
        static bool PromptModsUninstalled = false;
        static bool PromptModFormatVersionNewer = false;
        static bool PromptNoModsInstalled = false;
        static bool PromptModsAlreadyInstalled = false;
        static bool PromptModAlreadyImported = false;
        static bool PromptModArchivePathRepair = false;
        static bool PromptRemoveModDialog = false;
        static bool PromptErrorDuringModInstall = false;
        static bool PromptErrorDuringVolatileRestore = false;
        static bool PromptModArchivesMissingDuringInstall = false;

        public override void Draw()
        {
            SubmitMainMenuBar();
            SubmitContent();

            if (PromptImportModBrowser)
            {
                OpenImportModBrowser();
                PromptImportModBrowser = false;
            }

            if (!ImGui.IsPopupOpen(CreateModPackWindow.CREATE_MOD_PACK_WINDOW_ID) && !ImGui.IsPopupOpen("Settings") && !ImGui.IsPopupOpen("###FirstTimeGameSelectionWindow"))
            {
                ImFileBrowser.Draw();
            }

            if (PromptEditModPackFileBrowser)
            {
                ImGui.OpenPopup(CreateModPackWindow.CREATE_MOD_PACK_WINDOW_ID);
                PromptEditModPackFileBrowser = false;
            }

            if (Settings.GameExecutableFilePath == "" && PromptFTUE)
            {
                ImGui.OpenPopup("###FirstTimeGameSelectionWindow");
                PromptFTUE = false;
            }
            _ftueWindow.Draw();

            if (PromptModAlreadyImported)
            {
                ImGui.OpenPopup("###ModAlreadyImported");
                PromptModAlreadyImported = false;
            }
            ShowModAlreadyImportedDialog();

            if (PromptImportModErrorWindow)
            {
                ImGui.OpenPopup("###UnsupportedModPackage");
                PromptImportModErrorWindow = false;
            }
            if (UnsupportedModsList.Count() > 0)
            {
                ShowUnsupportedModPackageDialog();
            }

            if (PromptModFormatVersionNewer)
            {
                ImGui.OpenPopup("###ModFormatVersionNewer");
                PromptModFormatVersionNewer = false;
            }
            if (NewerFormatModsList.Count() > 0)
            {
                ShowModFormatVersionNewerDialog();
            }

            if (PromptNoCleanDataBackup)
            {
                ImGui.OpenPopup("###NoCleanDataBackup");
                PromptNoCleanDataBackup = false;
            }
            NoCleanDataBackupDialog.Draw();

            if (PromptErrorCleanDataRestore)
            {
                ImGui.OpenPopup("###ErrorCleanDataRestore");
                PromptErrorCleanDataRestore = false;
            }
            ErrorCleanDataRestoreDialog.Draw();

            if (PromptDataFileNoBackupRestore)
            {
                ImGui.OpenPopup("###NoCleanDataRestore");
                PromptDataFileNoBackupRestore = false;
            }
            NoCleanDataRestoreDialog.Draw();

            _settingsWindow.Draw();
            _effectiveChangesWindow.Draw();
            CreateModPackWindow.Draw();

            if (PromptModsInstalled)
            {
                ImGui.OpenPopup("###ModsInstalled");
                PromptModsInstalled = false;
            }
            ModsInstalledDialog.Draw();

            if (PromptModsAlreadyInstalled)
            {
                ImGui.OpenPopup("###ModsAlreadyInstalled");
                PromptModsAlreadyInstalled = false;
            }
            ShowModsAlreadyInstalledDialog();

            if (PromptModsUninstalled)
            {
                ImGui.OpenPopup("###ModsUninstalled");
                PromptModsUninstalled = false;
            }
            ModsUninstalledDialog.Draw();

            if (PromptNoModsInstalled)
            {
                ImGui.OpenPopup("###NoModsInstalled");
                PromptNoModsInstalled = false;
            }
            NoModsInstalledDialog.Draw();

            if (PromptModArchivePathRepair)
            {
                ImGui.OpenPopup("###ModArchivePathRepair");
                PromptModArchivePathRepair = false;
            }
            ShowModArchivePathRepairDialog();

            if (PromptRemoveModDialog)
            {
                ImGui.OpenPopup("###ConfirmRemoveMod");
                PromptRemoveModDialog = false;
            }
            ShowRemoveModDialog();

            if (PromptErrorDuringModInstall)
            {
                ImGui.OpenPopup("###ErrorDuringModInstall");
                PromptErrorDuringModInstall = false;
            }
            ErrorDuringModInstallDialog.Draw();

            if (PromptErrorDuringVolatileRestore)
            {
                ImGui.OpenPopup("###ErrorDuringVolatileRestore");
                PromptErrorDuringVolatileRestore = false;
            }
            ErrorDuringVolatileRestoreDialog.Draw();

            if (PromptModArchivesMissingDuringInstall)
            {
                ImGui.OpenPopup("###ModArchivesMissingDuringInstall");
                PromptModArchivesMissingDuringInstall = false;
            }
            ShowModArchivesMissingDuringInstallDialog();
        }

        static bool content_window_fullscreen = true;
        void SubmitContent()
        {
            var io = ImGui.GetIO();

            ImGui.SetNextWindowPos(new Vector2(0, MainMenuBarSize.Y));
            ImGui.SetNextWindowSize(new Vector2(io.DisplaySize.X, io.DisplaySize.Y - MainMenuBarSize.Y));
            //ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);

            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0f);
            ImGuiWindowFlags flags = !content_window_fullscreen ? ImGuiWindowFlags.None : ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoDocking;
            if (ImGui.Begin("Content", flags))//, ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove);// | ImGuiWindowFlags.MenuBar);
            {
                //ImGui.PushStyleColor(ImGuiCol.Separator, new Vector4(78 / 255f, 78 / 255f, 78 / 255f, 1.0f));
                //ImGui.Separator();
                //ImGui.PopStyleColor();

                if (close_style_editor)
                {
                    ImGui.Begin("Style Editor Window", ref close_style_editor);
                    ImGui.ShowStyleEditor();
                    ImGui.End();
                }

                if (ImGui.BeginTable("##ModManagerPanelTable", 2))
                {
                    ImGui.TableSetupColumn("##SelectionColumn", ImGuiTableColumnFlags.WidthFixed, 350);
                    ImGui.TableSetupColumn("##DetailsColumn", ImGuiTableColumnFlags.WidthStretch);

                    ImGui.TableNextColumn();
                    ShowSelectionWindow();
                    ShowSelectionBottomRow();

                    ImGui.TableNextColumn();
                    ModDetailsPanel.Draw();

                    ImGui.EndTable();
                }

                ImGui.End();
                //ImGui.PopStyleVar(2);
            }
            ImGui.PopStyleVar();
        }

        void ShowSelectionWindow()
        {
            if (ImGui.BeginChild("ModListChild", new Vector2(350, ImGui.GetContentRegionAvail().Y - ImGui.GetFrameHeightWithSpacing()), ImGuiChildFlags.None, ImGuiWindowFlags.HorizontalScrollbar))
            {
                FontManager.PushFont("SegoeUI-SemiBold");
                ImGui.Text("Imported Mods");
                FontManager.PopFont();
                ImGui.Separator();

                ImGui.AlignTextToFramePadding();

                ImGui.PushStyleColor(ImGuiCol.FrameBg, Colors.Transparent);
                // Use a Child instead of a ListBox so we can force the vertical scrollbar to always be displayed. It's not as clean but it's our only workaround
                if (ImGui.BeginChild("##ImportedModsList", new Vector2(-1, -1), ImGuiChildFlags.None, ImGuiWindowFlags.AlwaysVerticalScrollbar | ImGuiWindowFlags.HorizontalScrollbar))
                //if (ImGui.BeginListBox("##ImportedModsList", new Vector2(-1, -1)))
                {
                    ImGui.Spacing();
                    for (int i = 0; i < Settings.ModList.Count; i++)
                    {
                        if (Settings.ModList[i].IsEnabled)
                        {
                            if (Settings.ModList[i].ConflictingMods.Count > 0)
                            {
                                ImGui.PushStyleColor(ImGuiCol.Text, Colors.Yellow);
                            }
                            else
                            {
                                ImGui.PushStyleColor(ImGuiCol.Text, Colors.LightGreen);
                            }
                        }
                        else
                        {
                            ImGui.PushStyleColor(ImGuiCol.Text, Colors.LightRed_Transparent);
                        }

                        FontManager.PushFont("SegoeUI-SemiBold");

                        float indentAmount = 4f;
                        ImGui.Indent(indentAmount);
                        // While duplicate names will function, they should be avoided still
                        if (ImGui.Selectable($"{Settings.ModList[i].Name}##{Settings.ModList[i].Name}_{i}", SelectedModIndex == i))
                        {
                            SelectedModIndex = i;
                            // Update details from selected item
                            UpdateModDetailsPanel(Settings.ModList[i]);
                        }
                        ImGui.Unindent(indentAmount);

                        FontManager.PopFont();
                        ImGui.PopStyleColor();
                    }

                    // TODO: this is dummy code to pad the mod list, delete when done
                    /*
                    for (int i = Settings.ModList.Count; i < 50; i++)
                    {
                        float indentAmount = 4f;
                        ImGui.Indent(indentAmount);
                        if (ImGui.Selectable($"Mod Item {i}", SelectedModIndex == i))
                        {
                            SelectedModIndex = i;
                            UpdateModDetailsPanel(new ModEntry());
                        }
                        ImGui.Unindent(indentAmount);
                    }
                    */
                    //ImGui.EndListBox();
                    ImGui.EndChild();
                }
                ImGui.PopStyleColor();
                ImGui.EndChild();
            }
        }

        void OpenImportModBrowser()
        {
            ImFileBrowser.OpenFiles((SelectedFilePaths) =>
            {
                foreach (var mod in SelectedFilePaths)
                {
                    ModPackage modPackage = ReadModArchive(mod);
                    if (modPackage == null || modPackage.ModFormatVersion == null)
                    {
                        // Inform user that they had an invalid mod package and request they verify validity
                        UnsupportedModsList.Add(Path.GetFileName(mod));
                        PromptImportModErrorWindow = true;
                    }
                    else
                    {
                        if (modPackage.ModFormatVersion == Util.MOD_FORMAT_VERSION_CURRENT)
                        {
                            // TODO: Eventually support not using Mod Storage and perform an already-imported check for updating mods

                            string NewPath = Path.Combine(Settings.ModArchivesDirectory, Path.GetFileName(mod));
                            if (File.Exists(NewPath))
                            {
                                AlreadyImportedModsList.Add(mod);
                                PromptModAlreadyImported = true;
                                continue;
                            }

                            ModEntry modEntry = new ModEntry()
                            {
                                Name = modPackage.Name,
                                ModPack = modPackage,
                                IsEnabled = false,
                                IsUsingArchiveStorage = Settings.CopyModArchivesToStorage,
                                ModArchivePath = (Settings.CopyModArchivesToStorage ? NewPath : mod)
                            };
                            Settings.ModList.Add(modEntry);
                            CopyModArchiveToStorage(mod);

                            Settings.Save();
                        }
                        else if (modPackage.ModFormatVersion < Util.MOD_FORMAT_VERSION_CURRENT)
                        {
                            // Handle older version in-place upgrading
                            // TODO: Implement when this becomes a thing
                        }
                        else
                        {
                            // Package is from a higher than supported version
                            // Prompt that a mod manager update is needed to install this mod
                            NewerFormatModsList.Add(Path.GetFileName(mod));
                            PromptModFormatVersionNewer = true;
                        }
                    }
                }
            }, title: "Select Mod Packages To Import...",
            filter: "Mod Package (*.zip)|*.zip|JSON Files (*.json)|*.json|All Files (*.*)|*.*", filterIndex: 0);
        }

        void OpenEditModPackFileBrowser()
        {
            ImFileBrowser.OpenFile((SelectedFilePath) =>
            {
                ModPackage modPackage = ReadModArchive(SelectedFilePath);

                if (modPackage == null || modPackage.ModFormatVersion == null)
                {
                    // Inform user that they had an invalid mod package and request they verify validity
                    UnsupportedModsList.Add(Path.GetFileName(SelectedFilePath));
                    PromptImportModErrorWindow = true;
                }
                else
                {
                    if (modPackage.ModFormatVersion == Util.MOD_FORMAT_VERSION_CURRENT)
                    {
                        CreateModPackWindow.EditModPackCreation(modPackage, SelectedFilePath);
                        PromptEditModPackFileBrowser = true;
                    }
                    else if (modPackage.ModFormatVersion < Util.MOD_FORMAT_VERSION_CURRENT)
                    {
                        // Handle older version in-place upgrading
                        // TODO: Implement when this becomes a thing
                    }
                    else
                    {
                        // Package is from a higher than supported version
                        // Prompt that a mod manager update is needed to open and edit this mod
                        NewerFormatModsList.Add(Path.GetFileName(SelectedFilePath));
                        PromptModFormatVersionNewer = true;
                    }
                }
            }, title: "Select extracted Mod Package ModConfig.json file to edit...",
            filter: "ModConfig.json|ModConfig.json|JSON Files (*.json)|*.json|All Files (*.*)|*.*", filterIndex: 0);
        }

        void ShowSelectionBottomRow()
        {
            if (ImGui.BeginChild("##SelectionBottomRow"))
            {
                ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(0.0f, 0.0f));

                if (ImGui.BeginTable($"##SelectionBottomRowButtonsTable", 2))
                {
                    ImGui.TableSetupColumn("##ButtonOne", ImGuiTableColumnFlags.WidthStretch, 1);
                    ImGui.TableSetupColumn("##ButtonTwo", ImGuiTableColumnFlags.WidthStretch, 1);

                    ImGui.TableNextColumn();

                    ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 0.0f);
                    FontManager.PushFont("FAS", 18);
                    ImGui.PushStyleColor(ImGuiCol.Text, Colors.LightGreen);
                    if (ImGui.Button($"{FASIcons.Plus}", new Vector2(-1, 0)))
                    {
                        PromptImportModBrowser = true;
                    }
                    ImGui.PopStyleColor();
                    ImGui.PopStyleVar();
                    FontManager.PopFont();
                    ImGui.SetItemTooltip("Import a Mod Pack archive file into the Mod Manager.");

                    ImGui.TableNextColumn();
                    ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 0.0f);
                    FontManager.PushFont("FAS", 18);
                    ImGui.PushStyleColor(ImGuiCol.Text, Colors.LightRed);
                    ImGui.BeginDisabled(SelectedModIndex < 0);
                    if (ImGui.Button($"{FASIcons.TrashCan}", new Vector2(-1, 0)))
                    {
                        PromptRemoveModDialog = true;
                    }
                    ImGui.EndDisabled();
                    ImGui.PopStyleColor();
                    ImGui.PopStyleVar();
                    FontManager.PopFont();
                    ImGui.SetItemTooltip("Removes selected Mod Pack archive from Mod Manager.\nDeletes package from Mod Archive Storage as well.");

                    ImGui.EndTable();
                }

                ImGui.PopStyleVar();

                ImGui.EndChild();
            }
        }

        void SubmitMainMenuBar()
        {
            // One time set variables for opening Popups that will reset themselves
            bool OpenCreateModPackWindow = false;
            bool OpenEditModPackWindow = false;
            bool ShowEffectiveChangesWindow = false;
            bool OpenSettingsWindow = false;

            if (ImGui.BeginMainMenuBar())
            {
                MainMenuBarSize = ImGui.GetWindowSize();
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Import Mods..."))
                    {
                        PromptImportModBrowser = true;
                    }
                    ImGui.SetItemTooltip("Import a Mod Pack archive file into the Mod Manager.");
                    ImGui.Separator();
                    if (ImGui.MenuItem("Create Mod Pack"))
                    {
                        OpenCreateModPackWindow = true;
                    }
                    ImGui.SetItemTooltip("Create a brand new Mod Pack from scratch using loose file assets.");
                    if (ImGui.MenuItem("Edit Mod Pack"))
                    {
                        OpenEditModPackWindow = true;
                    }
                    ImGui.SetItemTooltip("Edit the contents of an existing Mod Pack Archive that has been extracted.\nSelect the JSON file at the root directory of it to begin.");
                    ImGui.Separator();
                    if (ImGui.MenuItem("Settings"))
                    {
                        OpenSettingsWindow = true;
                    }
                    ImGui.EndMenu();
                }

                if (ImGui.Button("Effective Changes##ChangesBtn"))
                {
                    ShowEffectiveChangesWindow = true;
                }
                ImGui.SetItemTooltip("View file changes that will occur with mods installed.");

                int ModsEnabledCount = Settings.ModList.Count(item => item.IsEnabled);
                if (ModsEnabledCount == 0)
                {
                    ImGui.BeginDisabled();
                }
                if (ImGui.Button($"Install{(ModsEnabledCount > 0 ? $" {ModsEnabledCount}" : "")} Mods"))
                {
                    InstallMods();
                }
                if (ModsEnabledCount == 0)
                {
                    ImGui.EndDisabled();
                }

                if (ImGui.Button("Uninstall All Mods"))
                {
                    UninstallMods();
                }

                // TODO: Remove debug menu items
                if (false)
                {
                    ImGui.Text("DEBUG >");

                    ImGui.Checkbox("ContentWindowFullscreen", ref content_window_fullscreen);

                    if (ImGui.Button("Save Settings"))
                    {
                        Settings.Save();
                    }

                    if (ImGui.Button("Load Settings"))
                    {
                        Settings.Load();
                    }

                    ImGui.Checkbox("Show Style Editor", ref close_style_editor);
                }

                ImGui.EndMainMenuBar();
            }

            if (ShowEffectiveChangesWindow)
            {
                ImGui.OpenPopup("Effective Changes");
            }
            if (OpenCreateModPackWindow)
            {
                CreateModPackWindow.InitializeNewCreation();
                ImGui.OpenPopup(CreateModPackWindow.CREATE_MOD_PACK_WINDOW_ID);
            }
            if (OpenEditModPackWindow)
            {
                OpenEditModPackFileBrowser();
            }
            if (OpenSettingsWindow)
            {
                ImGui.OpenPopup("Settings");
            }
        }

        public ModPackage ReadModArchive(string ModFilePath)
        {
            if (Path.Exists(ModFilePath))
            {
                if (Path.GetExtension(ModFilePath) == ".zip")
                {
                    try
                    {
                        using (var archive = ZipFile.OpenRead(ModFilePath))
                        {
                            var ModConfigEntry = archive.Entries.FirstOrDefault(file => file.Name == "ModConfig.json");
                            if (ModConfigEntry != default)
                            {
                                using (var streamReader = new StreamReader(ModConfigEntry.Open()))
                                {
                                    using (var jsonReader = new JsonTextReader(streamReader))
                                    {
                                        ModPackage pack = new JsonSerializer().Deserialize<ModPackage>(jsonReader);
                                        return pack;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }

                }
                else if (Path.GetExtension(ModFilePath) == ".json")
                {
                    ModPackage pack = JsonConvert.DeserializeObject<ModPackage>(File.ReadAllText(ModFilePath));
                    return pack;
                }
            }
            return null;
        }

        public void CopyModArchiveToStorage(string ModFilePath, string NewFileNameWhenStored = "")
        {
            if (Settings.CopyModArchivesToStorage)
            {
                if (File.Exists(ModFilePath))
                {
                    if (string.IsNullOrEmpty(NewFileNameWhenStored))
                    {
                        string NewPath = Path.Combine(Settings.ModArchivesDirectory, Path.GetFileName(ModFilePath));
                        File.Copy(ModFilePath, NewPath, true);
                    }
                    else
                    {
                        string NewPath = Path.Combine(Settings.ModArchivesDirectory, Path.GetFileName(NewFileNameWhenStored));
                        File.Copy(ModFilePath, NewPath, true);
                    }
                }
            }
        }

        public void UpdateModDetailsPanel(ModEntry modEntry)
        {
            ModDetailsPanel.UpdateModDetails(modEntry);
        }

        public static void DeleteEmptySubdirectories(string parentDirectory)
        {
            foreach (string directory in Directory.GetDirectories(parentDirectory))
            {
                DeleteEmptySubdirectories(directory);

                if (!Directory.EnumerateFileSystemEntries(directory).Any())
                {
                    Directory.Delete(directory, false);
                }
            }
        }

        public void InstallMods()
        {
            // Save current app settings before installing mods in case something goes wrong
            Settings.Save();

            string GameDirectory = Path.GetDirectoryName(Settings.GameExecutableFilePath);

            var InstalledPaths = ReadInstalledPathsFile(Settings.ManagerAppDataDirectory);

            Directory.CreateDirectory(Settings.ModArchivesDirectory);

            DataManager dataManager = new DataManager(GameDirectory);
            bool HaveDataIndexBackup = dataManager.BackupDataIndex(Settings.ModArchivesDirectory, GameDirectory);
            // data.i has now been backed up to our manager storage (if it was original)

            // Check first if any mods have been registered as installed before we continue
            // As they are about to all be deleted and if user restored their Steam version it may cause issues
            if (HaveDataIndexBackup && InstalledPaths.Count > 0 && dataManager.IsIndexOriginal(Path.Combine(GameDirectory, "data.i")))
            {
                PromptModsAlreadyInstalled = true;
                return;
            }

            // Restore the orig_data.i back to the game directory to ensure we're working with a clean one
            if (HaveDataIndexBackup)
            {
                if (File.Exists(Path.Combine(Settings.ModArchivesDirectory, "orig_data.i")))
                {
                    File.Copy(Path.Combine(Settings.ModArchivesDirectory, "orig_data.i"), Path.Combine(GameDirectory, "data.i"), true);
                }
            }
            else
            {
                // Prompt to verify integrity in steam to restore original data.i as none exists then try again
                PromptNoCleanDataBackup = true;
                return;
            }

            List<string> InstalledFilePaths = new List<string>();
            List<string> VolatileFilePaths = new List<string>();

            // Need to uninstall mods each time to prevent keeping old mod files in the game directory when mods get disabled
            // Additionally, need to prevent mods from getting their own files copied into volatile backups when installed on top of themselves
            foreach (var FilePath in InstalledPaths)
            {
                string AbsolutePath = Path.Combine(GameDirectory, FilePath);
                if (File.Exists(AbsolutePath))
                {
                    try
                    {
                        File.Delete(AbsolutePath);
                    }
                    catch (Exception ex)
                    {
                        // We're not going to throw any critical errors as the game is likely still in a fine state.
                        // But we will automatically include them again as being installed so the next time maybe they will be cleaned up
                        // Worst case the user can check the installed files json and see what's getting stuck
                        InstalledFilePaths.Add(FilePath);
                        Console.WriteLine($"Failed to delete old mod file [{AbsolutePath}] during install: {ex.Message}");
                    }
                    
                }
            }

            // Clean up empty directories
            DeleteEmptySubdirectories(Path.Combine(GameDirectory, "data"));

            // Attempt to automatically repair mod archive paths after an archive storage path change has happened but mods weren't moved via the manager
            foreach (var entry in Settings.ModList)
            {
                if (entry.IsUsingArchiveStorage && Path.GetDirectoryName(entry.ModArchivePath) != Settings.ModArchivesDirectory)
                {
                    string UpdatedModArchivePath = Path.Combine(Settings.ModArchivesDirectory, Path.GetFileName(entry.ModArchivePath));
                    if (File.Exists(UpdatedModArchivePath))
                    {
                        entry.ModArchivePath = UpdatedModArchivePath;
                    }
                    else
                    {
                        // We couldn't repair a mod path, installing could potentially fail and user already is in a bad imported mod state
                        // Stop the installation process (currently user is in a clean game state now) and inform them of the issues and how to fix
                        WriteInstalledPathsFile(InstalledFilePaths, Settings.ManagerAppDataDirectory);
                        WriteVolatileBackupsFile(VolatileFilePaths, Settings.ManagerAppDataDirectory);

                        ModInvalidArchivePathList.Add(entry.ModArchivePath);

                        PromptModArchivePathRepair = true;
                    }
                }
            }
            if (PromptModArchivePathRepair)
            {
                return;
            }

            // Copy mod files into game data directory
            Dictionary<string, string> FinalEffectiveChanges = new Dictionary<string, string>();
            // Loop through all enabled mods by highest priority to lowest for automatic basic file conflict handling
            foreach (var mod in Settings.ModList.Where(item => item.IsEnabled).OrderByDescending(item => item.Priority))
            {
                if (!string.IsNullOrEmpty(mod.ModArchivePath) && Path.GetExtension(mod.ModArchivePath) == ".zip")
                {
                    if (File.Exists(mod.ModArchivePath))
                    {
                        using (var archive = ZipFile.OpenRead(mod.ModArchivePath))
                        {
                            foreach (var FilePaths in mod.ModifiedPaths)
                            {
                                bool IsAllowed = FinalEffectiveChanges.TryAdd(FilePaths.DestinationPath, FilePaths.SourcePath);
                                if (!IsAllowed)
                                {
                                    continue;
                                }

                                bool IsVolatileBackupCompleted = true;

                                // Find the file in the mod archive and extract to game directory if it's unique
                                var fileEntry = archive.GetEntry(FilePaths.SourcePath);
                                if (fileEntry != null)
                                {
                                    string AbsolutePath = Path.Combine(GameDirectory, FilePaths.DestinationPath);

                                    if (Util.PathContainsVolatileGamePaths(FilePaths.DestinationPath))
                                    {
                                        string NewDestination = FilePaths.DestinationPath;
                                        if (NewDestination.Split(Path.DirectorySeparatorChar).First() == "data")
                                        {
                                            NewDestination = Path.Combine("data_volatile_backups", NewDestination.Remove(0, 5));
                                        }
                                        string BackupPath = Path.Combine(GameDirectory, NewDestination);

                                        if (File.Exists(AbsolutePath))
                                        {
                                            try
                                            {
                                                Directory.CreateDirectory(Path.GetDirectoryName(BackupPath));
                                                File.Move(AbsolutePath, BackupPath, true);
                                                VolatileFilePaths.Add(FilePaths.DestinationPath);
                                            }
                                            catch (Exception ex)
                                            {
                                                IsVolatileBackupCompleted = false;
                                                Console.WriteLine($"Error moving file [{AbsolutePath}] to volatile backup path [{BackupPath}]: {ex.Message}");
                                            }
                                        }
                                    }

                                    if (!IsVolatileBackupCompleted)
                                    {
                                        // We cannot recover the install process if a volatile mod fails to properly install
                                        // Write out the current state so we can rollback the changes that have been made
                                        // Then inform the user of the critical error and let them resolve it
                                        WriteInstalledPathsFile(InstalledFilePaths, Settings.ManagerAppDataDirectory);
                                        WriteVolatileBackupsFile(VolatileFilePaths, Settings.ManagerAppDataDirectory);
                                        PromptErrorDuringModInstall = true;
                                        return;
                                    }

                                    try
                                    {
                                        Directory.CreateDirectory(Path.GetDirectoryName(AbsolutePath));
                                        fileEntry.ExtractToFile(AbsolutePath, true);
                                        // Register the file before processing it as its now on disk and needs to be cleaned up if anything goes wrong
                                        InstalledFilePaths.Add(FilePaths.DestinationPath);
                                        dataManager.ProcessFile(AbsolutePath);
                                    }
                                    catch (Exception ex)
                                    {
                                        // If a mod file fails to extract to the game, we don't want to try and recover
                                        // Mods likely need all of their files otherwise unexpected bugs may occur and no one wants that
                                        // Writing out the current state of installed files will allow another install/uninstall to clean it up
                                        WriteInstalledPathsFile(InstalledFilePaths, Settings.ManagerAppDataDirectory);
                                        WriteVolatileBackupsFile(VolatileFilePaths, Settings.ManagerAppDataDirectory);
                                        PromptErrorDuringModInstall = true;
                                        Console.WriteLine($"Error extracting file [{FilePaths.SourcePath}] to game path [{AbsolutePath}]: {ex.Message}");
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // Mod Archive is missing?
                        ModArchivesMissingDuringInstallList.Add(mod.ModArchivePath);
                        PromptModArchivesMissingDuringInstall = true;
                    }
                }
                else if (!string.IsNullOrEmpty(mod.ModArchivePath) && Path.GetExtension(mod.ModArchivePath) == ".json")
                {
                    // TODO: Support non-archived mods?
                }
                else
                {
                    // No mod archive path was set, will just ignore this as it should never happen
                    // If it does, the user is doing something weird and probably knows what they're doing (they don't)
                }
            }

            WriteInstalledPathsFile(InstalledFilePaths, Settings.ManagerAppDataDirectory);
            WriteVolatileBackupsFile(VolatileFilePaths, Settings.ManagerAppDataDirectory);

            // Apply data.i modifications
            if (!dataManager.UpdateDataIndexFromList(FinalEffectiveChanges.Keys.ToList()))
            {
                PromptErrorDuringModInstall = true;
            }

            Settings.Save();

            // Display install completed prompt
            if (!PromptModArchivesMissingDuringInstall && !PromptErrorDuringModInstall)
            {
                PromptModsInstalled = true;
            }
        }

        void WriteInstalledPathsFile(List<string> InstalledPaths, string DirectoryPath)
        {
            File.WriteAllText(Path.Combine(DirectoryPath, "InstalledModFiles.json"), JsonConvert.SerializeObject(InstalledPaths, Formatting.Indented));
        }

        List<string> ReadInstalledPathsFile(string DirectoryPath)
        {
            List<string> Values = null;
            try
            {
                Values = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Path.Combine(DirectoryPath, "InstalledModFiles.json")));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Values ?? new List<string>();
        }

        void WriteVolatileBackupsFile(List<string> VolatilePaths, string DirectoryPath)
        {
            // When a mod that contains volatile paths is installed, the original game files will be backed up into 'data_volatile_backups' in game dir
            // And those paths will be stored in a new json file. During uninstall the read paths will be moved back into the normal game data folder
            File.WriteAllText(Path.Combine(DirectoryPath, "VolatileModFiles.json"), JsonConvert.SerializeObject(VolatilePaths, Formatting.Indented));
        }

        List<string> ReadVolatileBackupsFile(string DirectoryPath)
        {
            List<string> Values = null;
            try
            {
                Values = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Path.Combine(DirectoryPath, "VolatileModFiles.json")));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Values ?? new List<string>();
        }

        public void UninstallMods()
        {
            Settings.Save();

            string GameDirectory = Path.GetDirectoryName(Settings.GameExecutableFilePath);

            var InstalledPaths = ReadInstalledPathsFile(Settings.ManagerAppDataDirectory);
            var VolatilePaths = ReadVolatileBackupsFile(Settings.ManagerAppDataDirectory);

            // Check first if any mods have been registered as installed before we continue
            if (InstalledPaths.Count == 0)
            {
                PromptNoModsInstalled = true;
                return;
            }

            // Restore original data.i from mod manager storage
            DataManager dataManager = new DataManager(GameDirectory);
            if (!dataManager.IsIndexOriginal(Path.Combine(GameDirectory, "data.i")))
            {
                if (File.Exists(Path.Combine(Settings.ModArchivesDirectory, "orig_data.i")))
                {
                    try
                    {
                        File.Copy(Path.Combine(Settings.ModArchivesDirectory, "orig_data.i"), Path.Combine(GameDirectory, "data.i"), true);
                    }
                    catch (Exception ex)
                    {
                        PromptErrorCleanDataRestore = true;
                        Console.WriteLine($"Error trying to restore the located original data.i to game directory: {ex.Message}");
                    }
                }
                else
                {
                    // Prompt to verify integrity in steam to finish restoration after mod files have been deleted
                    PromptDataFileNoBackupRestore = true;
                }
            }

            List<string> InstalledFilePaths = new List<string>();

            // Remove all mod files from game data directory
            foreach (string ModFilePath in InstalledPaths)
            {
                string AbsolutePath = Path.Combine(GameDirectory, ModFilePath);
                if (File.Exists(AbsolutePath))
                {
                    try
                    {
                        File.Delete(AbsolutePath);
                    }
                    catch (Exception ex)
                    {
                        InstalledFilePaths.Add(ModFilePath);
                        Console.WriteLine($"Failed to delete old mod file [{AbsolutePath}] during uninstall: {ex.Message}");
                    }
                    
                }
            }

            // Restore volatile file backups into the original game location
            foreach (string BackupFilePath in VolatilePaths)
            {
                string AbsolutePath = Path.Combine(GameDirectory, BackupFilePath);
                string BackupDestination = Path.Combine("data_volatile_backups", BackupFilePath.Remove(0, 5));
                if (File.Exists(BackupDestination))
                {
                    try
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(AbsolutePath));
                        File.Move(BackupDestination, AbsolutePath, true);
                    }
                    catch (Exception ex)
                    {
                        PromptErrorDuringVolatileRestore = true;
                        Console.WriteLine($"Error restoring volatile file [{BackupDestination}] backup to game [{AbsolutePath}]: {ex.Message}");
                    }
                }
            }

            // Clean up empty directories
            DeleteEmptySubdirectories(Path.Combine(GameDirectory, "data"));
            if (Directory.Exists(Path.Combine(GameDirectory, "data_volatile_backups")))
            {
                DeleteEmptySubdirectories(Path.Combine(GameDirectory, "data_volatile_backups"));
            }

            // Remove all entries from the installed paths file
            WriteInstalledPathsFile(InstalledFilePaths, Settings.ManagerAppDataDirectory);
            WriteVolatileBackupsFile(new List<string>(), Settings.ManagerAppDataDirectory);

            Settings.Save();

            // Display uninstall completed prompt
            if (!PromptDataFileNoBackupRestore && !PromptErrorCleanDataRestore && !PromptErrorDuringVolatileRestore)
            {
                PromptModsUninstalled = true;
            }
        }
    }
}
