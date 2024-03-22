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
        void ShowModFormatVersionNewerDialog()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(new Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.SetNextWindowSize(new Vector2(900, 300), ImGuiCond.Appearing);

            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, Colors.Goldenrod_Transparent);

            if (ImGui.BeginPopupModal("Mod Package From Newer Relink Mod Manager Version###ModFormatVersionNewer"))
            {
                ImGui.Text("You attempted to import a Mod Package made in a newer version of Relink Mod Manager.");
                ImGui.Text("Please update your version of Relink Mod Manager to sucessfully import the following mods.");

                if (ImGui.BeginListBox("##NewerFormatModsList", new Vector2(-1, 0)))
                {
                    foreach (var ModItem in NewerFormatModsList)
                    {
                        ImGui.Selectable(ModItem);
                    }
                    ImGui.EndListBox();
                }

                if (ImGui.Button("OK", new Vector2(Util.BUTTON_ITEM_WIDTH_BASE, 0)))
                {
                    NewerFormatModsList.Clear();
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }

            ImGui.PopStyleColor();
        }
    }
}
