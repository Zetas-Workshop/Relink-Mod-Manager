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
        void ShowRemoveModDialog()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(new Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.SetNextWindowSize(new Vector2(600, 150), ImGuiCond.Appearing);
            ImGui.SetNextWindowSizeConstraints(new Vector2(600, 150), new Vector2(float.PositiveInfinity, float.PositiveInfinity));

            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, Colors.Goldenrod_Transparent);

            if (ImGui.BeginPopupModal("Remove Selected Mod?###ConfirmRemoveMod"))
            {
                var style = ImGui.GetStyle();

                ImGui.TextWrapped("Remove Selected Mod?");

                ImGui.Indent();
                ImGui.PushStyleColor(ImGuiCol.Text, Colors.SkyBlue);
                ImGui.Text($"{Settings.ModList[SelectedModIndex].Name}");
                ImGui.PopStyleColor();
                ImGui.Unindent();

                if (Settings.ModList[SelectedModIndex].IsUsingArchiveStorage)
                {
                    ImGui.Text("Note: Removing this Mod Pack will also delete it from the Mod Archive Storage directory.");
                }
                else
                {
                    ImGui.Text("Note: This Mod Pack is not stored in the Mod Archive Storage and will not be deleted.");
                }

                ImGui.Separator();

                if (ImGui.Button("Yes", new Vector2(Util.BUTTON_ITEM_WIDTH_BASE, 0)))
                {
                    // We don't need to worry about uninstalling as we've already tracked what was installed from this mod
                    string ModPathInStorage = Settings.ModList[SelectedModIndex].ModArchivePath;
                    if (Settings.ModList[SelectedModIndex].IsUsingArchiveStorage && File.Exists(ModPathInStorage))
                    {
                        // Delete the mod pack archive only if it's actually located in our storage location
                        // Files imported, and not stored, from random locations should not be deleted in case the user does not expect that
                        File.Delete(ModPathInStorage);
                    }

                    Settings.ModList[SelectedModIndex].ClearConflicts();
                    Settings.ModList.RemoveAt(SelectedModIndex);
                    SelectedModIndex = -1;
                    UpdateModDetailsPanel(new ModEntry());

                    Settings.Save();

                    ImGui.CloseCurrentPopup();
                }

                ImGui.SameLine();
                ImGui.SetCursorPosX(ImGui.GetWindowContentRegionMax().X - Util.BUTTON_ITEM_WIDTH_BASE);
                if (ImGui.Button("No", new Vector2(Util.BUTTON_ITEM_WIDTH_BASE, 0)))
                {
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }

            ImGui.PopStyleColor();
        }
    }
}
