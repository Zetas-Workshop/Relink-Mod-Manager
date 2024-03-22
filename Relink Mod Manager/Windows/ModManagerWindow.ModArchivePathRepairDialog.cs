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
        void ShowModArchivePathRepairDialog()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(new Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.SetNextWindowSize(new Vector2(900, 450), ImGuiCond.Appearing);
            ImGui.SetNextWindowSizeConstraints(new Vector2(650, 450), new Vector2(float.PositiveInfinity, float.PositiveInfinity));

            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, Colors.DarkRed);

            if (ImGui.BeginPopupModal("Error Attempting to Repair Mod Archive Paths###ModArchivePathRepair"))
            {
                var style = ImGui.GetStyle();

                ImGui.TextWrapped("One or more Mod Packs were found to have an incorrect Mod Archive Path instead of pointing to your Mod Archive Storage. If the Mod no longer exists, please delete it from your Imported Mods menu.");
                ImGui.TextWrapped("Please ensure Mod Packs are correctly placed in your selected Mod Archive Storage directory listed below:");

                string ModArchivesDir = Settings.ModArchivesDirectory;
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - Util.BUTTON_ITEM_WIDTH_BASE - style.ItemSpacing.X);
                ImGui.InputText("##ModArchiveStoragePath", ref ModArchivesDir, 1024, ImGuiInputTextFlags.ReadOnly);
                ImGui.SameLine();
                if (ImGui.Button("Open Folder", new Vector2(Util.BUTTON_ITEM_WIDTH_BASE, 0)))
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

                ImGui.Text("Mods with incorrect Archive Path:");
                if (ImGui.BeginListBox("##ModInvalidArchivePathList", new Vector2(-1, 0)))
                {
                    foreach (var ModItem in ModInvalidArchivePathList)
                    {
                        ImGui.Selectable(ModItem);
                    }
                    ImGui.EndListBox();
                }

                ImGui.TextWrapped("All mods have been uninstalled. Once you have ensured mods are in their correct locations, attempt to install again.");
                ImGui.TextDisabled("Note: The Metadata tab on a Mod Pack will show the current Archive Path for it.");
                ImGui.Separator();

                if (ImGui.Button("OK", new Vector2(Util.BUTTON_ITEM_WIDTH_BASE, 0)))
                {
                    ModInvalidArchivePathList.Clear();
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }

            ImGui.PopStyleColor();
        }
    }
}
