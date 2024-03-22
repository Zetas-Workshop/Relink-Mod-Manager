using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Relink_Mod_Manager.Dialogs
{
    public class ErrorDuringModInstallDialog
    {
        public static void Draw()
        {
            SubmitContent();
        }

        static void SubmitContent()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(new Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.SetNextWindowSize(new Vector2(700, 300), ImGuiCond.Appearing);

            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, Colors.DarkRed);

            if (ImGui.BeginPopupModal("Error During Mod Install!###ErrorDuringModInstall"))
            {
                ImGui.Text("There was an error when attempting to install the enabled mods.");
                ImGui.Text("Some mod files may still have been extracted but the latest mods have not been set as installed in data.i.");
                ImGui.PushStyleColor(ImGuiCol.Text, Colors.Red);
                ImGui.TextWrapped("It is strongly recommended to click 'Uninstall Mods' now and then verify you do not have any game files opened in another program or the game currently running.");
                ImGui.Text("Alternatively, you can use Steam's 'Verify integrity of game files' feature to restore your files that way.");
                ImGui.PopStyleColor();

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
