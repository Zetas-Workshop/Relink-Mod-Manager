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
        void ShowModsAlreadyInstalledDialog()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(new Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.SetNextWindowSize(new Vector2(750, 175), ImGuiCond.Appearing);

            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, Colors.DarkRed);

            if (ImGui.BeginPopupModal("Mods Are Already Installed!###ModsAlreadyInstalled"))
            {
                ImGui.Text("There mods already registered as installed but the data.i file in your game installation is unmodified.");
                ImGui.Text("Please uninstall your existing mods prior to installing new mods and mod updates.");
                ImGui.NewLine();
                ImGui.Text("If you have performed a Steam Game Files Integrity Verification, select 'I Restored My Files' below.");

                ImGui.Separator();

                if (ImGui.Button("OK", new Vector2(Util.BUTTON_ITEM_WIDTH_SECOND, 0)))
                {
                    ImGui.CloseCurrentPopup();
                }

                ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - Util.BUTTON_ITEM_WIDTH_SECOND);
                if (ImGui.Button("I Restored My Files", new Vector2(Util.BUTTON_ITEM_WIDTH_SECOND, 0)))
                {
                    WriteInstalledPathsFile(new List<string>(), Settings.ManagerAppDataDirectory);
                    WriteVolatileBackupsFile(new List<string>(), Settings.ManagerAppDataDirectory);

                    // Try to clear out potential stale data_volatile_backups but only silently fail if it errors
                    try
                    {
                        string GameDirectory = Path.GetDirectoryName(Settings.GameExecutableFilePath);
                        string VolatileBackupsDir = Path.Combine(GameDirectory, "data_volatile_backups");
                        if (Directory.Exists(VolatileBackupsDir))
                        {
                            foreach (string directory in Directory.GetDirectories(VolatileBackupsDir))
                            {
                                Directory.Delete(directory, true);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting data_volatile_backups contents: {ex.Message}");
                    }

                    ImGui.CloseCurrentPopup();
                }
                ImGui.SetItemTooltip("Clears list of registered mods.\nOnly to be used if mods were removed outside of the manager or files have been restored via Steam.");

                ImGui.EndPopup();
            }

            ImGui.PopStyleColor();
        }
    }
}
