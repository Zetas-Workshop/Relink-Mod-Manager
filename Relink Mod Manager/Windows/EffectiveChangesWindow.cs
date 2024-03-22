using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Relink_Mod_Manager.Windows
{
    public class EffectiveChangesWindow : Window
    {
        public override void Draw()
        {
            SubmitContent();
        }

        void SubmitContent()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(new Vector2(io.DisplaySize.X * 0.5f, io.DisplaySize.Y * 0.5f), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.SetNextWindowSize(new Vector2(1100, 500), ImGuiCond.Appearing);
            ImGui.SetNextWindowSizeConstraints(new Vector2(600, 350), new Vector2(float.PositiveInfinity, float.PositiveInfinity));

            bool p_open = true;
            if (ImGui.BeginPopupModal("Effective Changes", ref p_open))
            {
                ImGui.Text("Below is a list of changes that will be applied when you install the enabled mods:");

                if (ImGui.BeginTable("Effective Changes Table", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
                {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    FontManager.PushFont("SegoeUI-Bold");
                    ImGui.Text(" [ Game File Path ] ");
                    ImGui.TableNextColumn();
                    ImGui.Text(" [ Mod File Path ] ");
                    FontManager.PopFont();

                    Dictionary<string, string> FinalEffectiveChanges = new Dictionary<string, string>();
                    foreach (var mod in Settings.ModList.Where(item => item.IsEnabled).OrderByDescending(item => item.Priority))
                    {
                        foreach (var FilePaths in mod.ModifiedPaths)
                        {
                            bool IsAllowed = FinalEffectiveChanges.TryAdd(FilePaths.DestinationPath, FilePaths.SourcePath);
                            if (!IsAllowed)
                            {
                                continue;
                            }

                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.Text("");
                            ImGui.Text(FilePaths.DestinationPath); // Game File Path
                            ImGui.SetItemTooltip("Click to copy path to clipboard.");
                            if (ImGui.IsItemClicked())
                            {
                                ImGui.SetClipboardText(FilePaths.DestinationPath);
                            }

                            ImGui.TableNextColumn();
                            ImGui.TextDisabled(mod.Name);
                            ImGui.Text(FilePaths.SourcePath); // Mod File Path
                            if (ImGui.BeginItemTooltip())
                            {
                                ImGui.Text($"File is being modified from enabled mod: {mod.Name}");
                                ImGui.Text("Click to copy path to clipboard.");
                                ImGui.EndTooltip();
                            }
                            if (ImGui.IsItemClicked())
                            {
                                ImGui.SetClipboardText(FilePaths.SourcePath);
                            }
                        }
                    }
                    // TODO: this is dummy code to pad the list, delete when done
                    /*for (int i = 0; i < 50; i++)
                    {
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.Text($"Game File Path {i}"); // Game File Path
                        ImGui.SetItemTooltip("Click to copy path to clipboard.");

                        ImGui.TableNextColumn();
                        ImGui.Text($"Mod File Path {i}"); // Mod File Path
                        if (ImGui.BeginItemTooltip())
                        {
                            ImGui.Text($"File is being modified from enabled mod: Mod {i}");
                            ImGui.Text("Click to copy path to clipboard.");
                            ImGui.EndTooltip();
                        }
                    }*/
                    ImGui.EndTable();
                }

                ImGui.EndPopup();
            }
        }
    }
}
