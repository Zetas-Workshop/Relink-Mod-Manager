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
        void ShowModArchivesMissingDuringInstallDialog()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(new Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.SetNextWindowSize(new Vector2(900, 300), ImGuiCond.Appearing);

            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, Colors.Goldenrod_Transparent);

            if (ImGui.BeginPopupModal("Mods Installed With Errors###ModArchivesMissingDuringInstall"))
            {
                ImGui.TextWrapped("Mods were successfully installed.");
                ImGui.Text("However, the following Mod Archives were unable to be found and not were installed:");

                if (ImGui.BeginListBox("##ModArchivesMissingDuringInstallList", new Vector2(-1, 0)))
                {
                    foreach (var ModItem in ModArchivesMissingDuringInstallList)
                    {
                        ImGui.Selectable(ModItem);
                    }
                    ImGui.EndListBox();
                }

                if (ImGui.Button("OK", new Vector2(Util.BUTTON_ITEM_WIDTH_BASE, 0)))
                {
                    ModArchivesMissingDuringInstallList.Clear();
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }

            ImGui.PopStyleColor();
        }
    }
}
