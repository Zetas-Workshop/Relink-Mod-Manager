using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Relink_Mod_Manager.Dialogs
{
    public static class NoCleanDataBackupDialog
    {
        public static void Draw()
        {
            SubmitContent();
        }

        static void SubmitContent()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(new Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.SetNextWindowSize(new Vector2(700, 330), ImGuiCond.Appearing);

            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, Colors.DarkRed);

            if (ImGui.BeginPopupModal("No Original data.i File To Backup###NoCleanDataBackup"))
            {
                ImGui.Text("Unable to install mods. The data.i in your game installation is not original.");
                ImGui.Text("Additionally, there was no original data.i file in your Relink Mod Archives directory to restore from.");
                ImGui.PushStyleColor(ImGuiCol.Text, Colors.Red);
                ImGui.Text("You will need to restore the original data.i file manually from Steam File Integrity Verification.");
                ImGui.PopStyleColor();

                ImGui.Separator();

                ImGui.Text("Steps:");
                ImGui.Indent();
                ImGui.Text("1. Right Click 'GranBlue Fantasty: Relink' in Steam.");
                ImGui.Text("2. Select 'Properties'.");
                ImGui.Text("3. Select the 'Installed Files' tab.");
                ImGui.Text("4. Click the 'Verify integrity of game files' button.");
                ImGui.Unindent();
                ImGui.Text("Once the process is complete the original data.i file will be restored and no mods will be installed.");

                ImGui.NewLine();
                ImGui.Text("You may attempt the mod installation process again once an original data.i file is present.");

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
