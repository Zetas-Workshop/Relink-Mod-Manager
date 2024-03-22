using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Relink_Mod_Manager.Windows
{
    public class UpdateModManagerWindow : Window
    {
        public override void Draw()
        {
            SubmitContent();
        }

        void SubmitContent()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(new Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.SetNextWindowSize(new Vector2(650, 150), ImGuiCond.Appearing);
            ImGui.SetNextWindowSizeConstraints(new Vector2(550, 150), new Vector2(float.PositiveInfinity, float.PositiveInfinity));

            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, Colors.DarkGreen);

            if (ImGui.BeginPopupModal("Relink Mod Manager Update Available!###UpdateModManagerWindow"))
            {
                if (ImGui.BeginChild("##AlertChild", new Vector2(ImGui.GetWindowContentRegionMax().X, ImGui.GetWindowContentRegionMax().Y - ImGui.GetItemRectSize().Y * 3)))
                {
                    ImGui.Text("There is a new update available for Relink Mod Manager!");
                    ImGui.Text("It is strongly recommended to always update to the latest version available.");

                    ImGui.EndChild();
                }

                ImGui.Separator();

                ImGui.BeginDisabled(string.IsNullOrEmpty(Settings.ModManagerUpdateURL));
                if (ImGui.Button("Go To Update Website", new Vector2(Util.BUTTON_ITEM_WIDTH_SECOND, 0)))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = Settings.ModManagerUpdateURL,
                        UseShellExecute = true,
                    });
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndDisabled();
                if (string.IsNullOrEmpty(Settings.ModManagerUpdateURL))
                {
                    ImGui.SetItemTooltip("No website to open because ModManagerUpdateURL was not set in Config.json for Relink Mod Manager.");
                }
                else
                {
                    ImGui.SetItemTooltip($"Click to open [ {Settings.ModManagerUpdateURL} ]");
                }

                ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - Util.BUTTON_ITEM_WIDTH_SECOND);
                if (ImGui.Button("Remind Me Later", new Vector2(Util.BUTTON_ITEM_WIDTH_SECOND, 0)))
                {
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }

            ImGui.PopStyleColor();
        }
    }
}
