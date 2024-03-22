using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Relink_Mod_Manager.Dialogs
{
    public class ErrorDuringVolatileRestoreDialog
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

            if (ImGui.BeginPopupModal("Error During Volatile Mod File Restoration!###ErrorDuringVolatileRestore"))
            {
                ImGui.Text("There was an error when attempting to restore volatile mod file backups during uninstall.");
                ImGui.TextWrapped("Please check the 'data_volatile_backups' folder in your game installation directory and move any folders/files into your normal game 'data' folder.");
                ImGui.PushStyleColor(ImGuiCol.Text, Colors.Red);
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
