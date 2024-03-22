using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Relink_Mod_Manager
{
    public static class Theme
    {
        private static void Apply(Dictionary<ImGuiCol, Vector4> NewThemeColors)
        {
            var colors = ImGui.GetStyle().Colors;
            foreach ( var color in NewThemeColors)
            {
                colors[(int)color.Key] = color.Value;
            }
        }

        public static void RelinkTheme()
        {
            Dictionary<ImGuiCol, Vector4> colors = new Dictionary<ImGuiCol, Vector4>();

            Vector4 bgColor = Util.RGBAToVec4(37, 37, 38, 240);
            Vector4 childBgColor = Util.RGBAToVec4(37, 37, 38, 0);
            Vector4 lightBgColor = Util.RGBAToVec4(82, 82, 85, 255);
            Vector4 lightBgColor_fade = Util.RGBAToVec4(82, 82, 85, 90);
            Vector4 veryLightBgColor = Util.RGBAToVec4(90, 90, 95, 255);

            Vector4 panelColor = Util.RGBAToVec4(51, 51, 55, 138);
            Vector4 panelHoverColor = Util.RGBAToVec4(29, 151, 236, 102);
            Vector4 panelActiveColor = Util.RGBAToVec4(0, 119, 200, 171);

            Vector4 textColor = Util.RGBToVec4(255, 255, 255);
            Vector4 textDisabledColor = Util.RGBAToVec4(151, 151, 151, 255);
            Vector4 borderColor = Util.RGBAToVec4(78, 78, 78, 128);
            Vector4 separatorColor = Util.RGBAToVec4(153, 153, 153, 255);

            colors[ImGuiCol.Text] = textColor;
            colors[ImGuiCol.TextDisabled] = textDisabledColor;
            colors[ImGuiCol.TextSelectedBg] = panelActiveColor;
            colors[ImGuiCol.WindowBg] = new Vector4(0.06f, 0.06f, 0.06f, 0.94f);
            colors[ImGuiCol.ChildBg] = new Vector4(0.06f, 0.06f, 0.06f, 0.00f);
            colors[ImGuiCol.PopupBg] = new Vector4(0.06f, 0.06f, 0.06f, 0.94f);
            colors[ImGuiCol.Border] = borderColor;
            colors[ImGuiCol.BorderShadow] = borderColor;
            colors[ImGuiCol.FrameBg] = new Vector4(0.45f, 0.45f, 0.45f, 0.54f);
            colors[ImGuiCol.FrameBgHovered] = new Vector4(0.73f, 0.80f, 0.85f, 0.62f);
            colors[ImGuiCol.FrameBgActive] = new Vector4(0.35f, 0.50f, 0.59f, 0.67f);
            colors[ImGuiCol.TitleBg] = bgColor;
            colors[ImGuiCol.TitleBgActive] = bgColor;
            colors[ImGuiCol.TitleBgCollapsed] = bgColor;
            colors[ImGuiCol.MenuBarBg] = panelColor;
            colors[ImGuiCol.ScrollbarBg] = panelColor;
            colors[ImGuiCol.ScrollbarGrab] = lightBgColor;
            colors[ImGuiCol.ScrollbarGrabHovered] = veryLightBgColor;
            colors[ImGuiCol.ScrollbarGrabActive] = veryLightBgColor;
            colors[ImGuiCol.CheckMark] = panelActiveColor;
            colors[ImGuiCol.SliderGrab] = panelHoverColor;
            colors[ImGuiCol.SliderGrabActive] = panelActiveColor;
            colors[ImGuiCol.Button] = new Vector4(0.32f, 0.32f, 0.32f, 0.90f);
            colors[ImGuiCol.ButtonHovered] = new Vector4(0.90f, 0.90f, 0.90f, 0.40f);
            colors[ImGuiCol.ButtonActive] = new Vector4(0.28f, 0.28f, 0.28f, 0.59f);
            colors[ImGuiCol.Header] = new Vector4(0.22f, 0.34f, 0.53f, 0.54f);
            colors[ImGuiCol.HeaderHovered] = panelHoverColor;
            colors[ImGuiCol.HeaderActive] = panelActiveColor;
            colors[ImGuiCol.Separator] = separatorColor;
            colors[ImGuiCol.SeparatorHovered] = separatorColor;
            colors[ImGuiCol.SeparatorActive] = separatorColor;
            colors[ImGuiCol.ResizeGrip] = bgColor;
            colors[ImGuiCol.ResizeGripHovered] = panelColor;
            colors[ImGuiCol.ResizeGripActive] = lightBgColor;
            colors[ImGuiCol.PlotLines] = panelActiveColor;
            colors[ImGuiCol.PlotLinesHovered] = panelHoverColor;
            colors[ImGuiCol.PlotHistogram] = panelActiveColor;
            colors[ImGuiCol.PlotHistogramHovered] = panelHoverColor;
            colors[ImGuiCol.ModalWindowDimBg] = lightBgColor_fade;
            colors[ImGuiCol.DragDropTarget] = bgColor;
            colors[ImGuiCol.NavHighlight] = bgColor;
            colors[ImGuiCol.NavWindowingHighlight] = new Vector4(1.00f, 1.00f, 1.00f, 0.70f);
            colors[ImGuiCol.NavWindowingDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.20f);
            colors[ImGuiCol.DockingPreview] = panelActiveColor;
            colors[ImGuiCol.Tab] = bgColor;
            colors[ImGuiCol.TabActive] = panelActiveColor;
            colors[ImGuiCol.TabUnfocused] = bgColor;
            colors[ImGuiCol.TabUnfocusedActive] = panelActiveColor;
            colors[ImGuiCol.TabHovered] = panelHoverColor;

            Apply(colors);

            // Apply Styles
            var style = ImGui.GetStyle();

            style.Alpha = 1;
            style.WindowPadding = new Vector2(8, 8);
            style.FramePadding = new Vector2(4, 3);
            style.ItemSpacing = new Vector2(8, 4);
            style.ItemInnerSpacing = new Vector2(4, 4);
            style.TouchExtraPadding = new Vector2(0, 0);
            style.IndentSpacing = 21;
            style.ScrollbarSize = 16; // default: 14
            style.GrabMinSize = 13; // default: 12

            style.WindowBorderSize = 0; // default: 1
            style.ChildBorderSize = 1;
            style.PopupBorderSize = 0; // default: 1
            style.FrameBorderSize = 0;
            style.TabBorderSize = 0;
            style.TabBarBorderSize = 1;

            style.WindowRounding = 4; // default: 0
            style.ChildRounding = 0;
            style.FrameRounding = 4; // default: 0
            style.PopupRounding = 0;
            style.ScrollbarRounding = 9;
            style.GrabRounding = 3; // default: 0
            style.TabRounding = 4;

            style.CellPadding = new Vector2(4, 2);

            style.WindowTitleAlign = new Vector2(0, 0.5f);
            style.WindowMenuButtonPosition = ImGuiDir.Left;
            style.ColorButtonPosition = ImGuiDir.Right;
            style.ButtonTextAlign = new Vector2(0.5f, 0.5f);
            style.SelectableTextAlign = new Vector2(0, 0);
            style.SeparatorTextBorderSize = 3;
            style.SeparatorTextAlign = new Vector2(0, 0.5f);
            style.SeparatorTextPadding = new Vector2(20, 3);
            style.LogSliderDeadzone = 4;
            
            style.DisplaySafeAreaPadding = new Vector2(3, 3);
        }
    }
}
