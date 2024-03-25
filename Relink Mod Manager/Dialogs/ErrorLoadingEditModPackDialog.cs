using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Relink_Mod_Manager.Dialogs
{
    public class ErrorLoadingEditModPackDialog
    {
        public static void Draw()
        {
            SubmitContent();
        }

        static void SubmitContent()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(new Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.SetNextWindowSize(new Vector2(720, 175), ImGuiCond.Appearing);

            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, Colors.DarkRed);

            if (ImGui.BeginPopupModal("Error Opening Mod Pack To Edit###ErrorLoadingEditModPack"))
            {
                ImGui.Text("An error was encountered while attempting to open the selected Mod Pack for editing.");
                ImGui.TextWrapped("Please ensure all files from the original Mod Pack were extracted. Mod Packs cannot be opened with missing files.");
                ImGui.TextWrapped("Additionally, please verify selected `ModConfig.json` in Mod Pack is from an extracted Relink Mod Manager Mod Pack.");

                ImGui.Separator();

                if (ImGui.Button("OK", new Vector2(Util.BUTTON_ITEM_WIDTH_BASE, 0)))
                {
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }

            ImGui.PopStyleColor();
        }
    }
}
