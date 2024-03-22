using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Relink_Mod_Manager.Dialogs
{
    public static class NoModsInstalledDialog
    {
        public static void Draw()
        {
            SubmitContent();
        }

        static void SubmitContent()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(new Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.SetNextWindowSize(new Vector2(800, 150), ImGuiCond.Appearing);

            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, Colors.Goldenrod_Transparent);

            if (ImGui.BeginPopupModal("No Mods Installed!###NoModsInstalled"))
            {
                ImGui.Text("There are no registered mods installed that can be uninstalled.");
                ImGui.Text("If mods were installed outside of the Relink Mod Manager they cannot be uninstalled from within here.");
                ImGui.Text("Please uninstall mods through the original method used to install them, or verify your game file integrity through Steam.");

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
