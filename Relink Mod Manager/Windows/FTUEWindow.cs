using ImGuiNET;
using Relink_Mod_Manager.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Relink_Mod_Manager.Windows
{
    public class FTUEWindow : Window
    {
        static string SelectedGamePath = "";

        public override void Draw()
        {
            SubmitContent();
        }

        void SubmitContent()
        {
            if (Settings.GameExecutableFilePath != "")
            {
                return;
            }

            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(new Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.SetNextWindowSize(new Vector2(900, 400), ImGuiCond.Appearing);

            if (Util.BeginPopupModal("First Time Setup###FirstTimeGameSelectionWindow", ImGuiWindowFlags.NoResize))
            {
                var style = ImGui.GetStyle();

                ImGui.Text("A Granblue Fantasy: Relink install location has not been set yet.\nPlease select your game installation to continue.");
                ImGui.Text("You can change this later in the Settings menu.");

                ImGui.NewLine();
                ImGui.Text("Game Location:");
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - Util.BUTTON_ITEM_WIDTH_SECOND - style.ItemSpacing.X);
                ImGui.InputText("##GameExecutablePath", ref SelectedGamePath, 1024, ImGuiInputTextFlags.ReadOnly);
                ImGui.SameLine();
                if (ImGui.Button("Select Game Location", new Vector2(Util.BUTTON_ITEM_WIDTH_SECOND, 0)))
                {
                    ImFileBrowser.OpenFile((SelectedFilePath) =>
                    {
                        SelectedGamePath = SelectedFilePath;
                    },
                    filter: "granblue_fantasy_relink.exe|granblue_fantasy_relink.exe|All Files (*.*)|*.*",
                    filterIndex: 0,
                    title: "Select the game executable for Granblue Fantasy: Relink...");
                }
                ImGui.Separator();

                ImGui.NewLine();

                ImGui.Text("Imported Mod Packs will be stored in the following directory.");
                ImGui.Text("You can change this later in the Settings menu.");

                ImGui.NewLine();
                ImGui.Text("Mod Archive Storage:");
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - Util.BUTTON_ITEM_WIDTH_SECOND - style.ItemSpacing.X);
                ImGui.InputText("##ModArchivePath", ref Settings.ModArchivesDirectory, 1024, ImGuiInputTextFlags.ReadOnly);
                ImGui.SameLine();
                if (ImGui.Button("Select Storage Path", new Vector2(Util.BUTTON_ITEM_WIDTH_SECOND, 0)))
                {
                    ImFileBrowser.SelectDir(SelectedDir =>
                    {
                        if (SelectedDir != Settings.ModArchivesDirectory)
                        {
                            Settings.ModArchivesDirectory = SelectedDir;
                        }
                    }, title: "Select the new Mod Archives Storage directory...", baseDir: Settings.ModArchivesDirectory);
                }
                ImGui.Separator();

                ImGui.NewLine();
                ImGui.BeginDisabled(SelectedGamePath == "");
                ImGui.SetCursorPosX(ImGui.GetWindowContentRegionMax().X - Util.BUTTON_ITEM_WIDTH_SECOND);
                if (ImGui.Button("Continue", new Vector2(Util.BUTTON_ITEM_WIDTH_SECOND, 0)))
                {
                    Settings.GameExecutableFilePath = SelectedGamePath;
                    Directory.CreateDirectory(Settings.ModArchivesDirectory);

                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndDisabled();
                ImFileBrowser.Draw();

                ImGui.EndPopup();
            }
        }
    }
}
