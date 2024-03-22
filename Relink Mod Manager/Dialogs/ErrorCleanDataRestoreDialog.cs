using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Relink_Mod_Manager.Dialogs
{
    public static class ErrorCleanDataRestoreDialog
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

            if (ImGui.BeginPopupModal("Error Restoring Original data.i from Mod Archive Storage###ErrorCleanDataRestore"))
            {
                ImGui.Text("There was an error when attempting to restore 'orig_data.i' in Archive Storage to your game directory as 'data.i'.");
                ImGui.Text("Mod Files will still be deleted from your installation directory.");
                ImGui.PushStyleColor(ImGuiCol.Text, Colors.Red);
                ImGui.Text("You may manually copy the file from your Mod Archive Storage directory to your game install folder as 'data.i'");
                ImGui.Text("Doing so will finish restoring your game files to a pre-modified state.");
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
