using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Relink_Mod_Manager.Windows
{
    public static class ModManagerAlertsWindow
    {
        private static ModManagerAlert _modManagerAlert = new ModManagerAlert();

        public static void Draw()
        {
            SubmitContent();
        }

        public static void UpdateAlert(ModManagerAlert alert)
        {
            _modManagerAlert = alert;
        }

        static void SubmitContent()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(new Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.SetNextWindowSize(new Vector2(650, 150), ImGuiCond.Appearing);
            ImGui.SetNextWindowSizeConstraints(new Vector2(350, 150), new Vector2(float.PositiveInfinity, float.PositiveInfinity));

            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, Colors.DarkRed);

            if (ImGui.BeginPopupModal("Relink Mod Manager Alert!###ModManagerAlertWindow"))
            {
                Window.Settings.WaitingOnStartupPopup = true;

                if (ImGui.BeginChild("##AlertChild", new Vector2(ImGui.GetWindowContentRegionMax().X, ImGui.GetWindowContentRegionMax().Y - ImGui.GetItemRectSize().Y * 3)))
                {
                    ImGui.TextWrapped(_modManagerAlert.Alert);

                    ImGui.EndChild();
                }

                ImGui.Separator();

                if (!string.IsNullOrEmpty(_modManagerAlert.Link) && (_modManagerAlert.Link.StartsWith("http://") || _modManagerAlert.Link.StartsWith("https://")))
                {
                    if (ImGui.Button("Go To Link", new Vector2(Util.BUTTON_ITEM_WIDTH_SECOND, 0)))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                        {
                            FileName = _modManagerAlert.Link,
                            UseShellExecute = true,
                        });
                        Window.Settings.WaitingOnStartupPopup = false;
                        ImGui.CloseCurrentPopup();
                    }
                    ImGui.SetItemTooltip($"Click to open [ {_modManagerAlert.Link} ]");
                    ImGui.SameLine();
                }

                ImGui.SetCursorPosX(ImGui.GetWindowContentRegionMax().X - Util.BUTTON_ITEM_WIDTH_SECOND);
                if (ImGui.Button("Remind Me Later", new Vector2(Util.BUTTON_ITEM_WIDTH_SECOND, 0)))
                {
                    Window.Settings.WaitingOnStartupPopup = false;
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }

            ImGui.PopStyleColor();
        }
    }
}
