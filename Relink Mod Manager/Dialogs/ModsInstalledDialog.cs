using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Relink_Mod_Manager.Dialogs
{
    public static class ModsInstalledDialog
    {
        public static void Draw()
        {
            SubmitContent();
        }

        static void SubmitContent()
        {
            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, Colors.DarkGreen);

            if (Util.BeginPopupModal("Mods Installed!###ModsInstalled", ImGuiWindowFlags.NoResize))
            {
                ImGui.Text("Mods have finished installing.");

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
