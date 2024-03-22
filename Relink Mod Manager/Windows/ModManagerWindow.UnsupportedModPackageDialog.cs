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
        void ShowUnsupportedModPackageDialog()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(new Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.SetNextWindowSize(new Vector2(900, 300), ImGuiCond.Appearing);

            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, Colors.DarkRed);

            if (ImGui.BeginPopupModal("Unsupported Mod Package###UnsupportedModPackage"))
            {
                ImGui.Text("You attempted to import an unsupported Mod Package.\nPlease verify what are you trying to import is a valid Mod and try again.");

                if (ImGui.BeginListBox("##UnsupportedModsList", new Vector2(-1, 0)))
                {
                    foreach (var ModItem in UnsupportedModsList)
                    {
                        ImGui.Selectable(ModItem);
                    }
                    ImGui.EndListBox();
                }

                if (ImGui.Button("OK", new Vector2(Util.BUTTON_ITEM_WIDTH_BASE, 0)))
                {
                    UnsupportedModsList.Clear();
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }

            ImGui.PopStyleColor();
        }
    }
}
