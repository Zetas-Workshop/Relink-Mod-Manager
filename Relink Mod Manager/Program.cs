using ImGuiNET;
using System.Diagnostics;
using System.Numerics;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Veldrid;
using Relink_Mod_Manager.Windows;
using Newtonsoft.Json;

namespace Relink_Mod_Manager
{
    class Program
    {
        private static Sdl2Window _window;
        private static GraphicsDevice _gd;
        private static CommandList _cl;
        private static ImGuiController _controller;
        private static ModManagerWindow _modManagerWindow;
        private static Settings _AppSettings;
        private static UpdateModManagerWindow _updateModManagerWindow;
        private static bool ModManagerUpdateAvailable = false;
        private static bool ModManagerAlertFound = false;

        // UI state
        private static Vector3 _clearColor = new Vector3(0.45f, 0.55f, 0.6f);

        static void Main(string[] args)
        {
            FontManager.Clear();
            LoadCustomFonts();
            //FontManager.DefaultFont = "UbuntuMono-Regular";

            // Create window, GraphicsDevice, and all resources necessary for the demo.
            VeldridStartup.CreateWindowAndGraphicsDevice(
                new WindowCreateInfo(Sdl2Native.SDL_WINDOWPOS_CENTERED, Sdl2Native.SDL_WINDOWPOS_CENTERED, 1280, 720, WindowState.Normal, "Relink Mod Manager"),
                new GraphicsDeviceOptions(true, null, true, ResourceBindingModel.Improved, true, true),
                GraphicsBackend.Direct3D11,
                out _window,
                out _gd);
            _window.Resized += () =>
            {
                _gd.MainSwapchain.Resize((uint)_window.Width, (uint)_window.Height);
                _controller.WindowResized(_window.Width, _window.Height);
            };
            _cl = _gd.ResourceFactory.CreateCommandList();
            _controller = new ImGuiController(_gd, _gd.MainSwapchain.Framebuffer.OutputDescription, _window.Width, _window.Height);

            _AppSettings = new Settings();
            _AppSettings.Load();

            foreach (var entry in _AppSettings.ModList)
            {
                entry.HasConflicts(_AppSettings.ModList);
            }

            _window.Title = $"Relink Mod Manager - v{_AppSettings.ModManagerVersion}";
            Window.Settings = _AppSettings;
            _modManagerWindow = new ModManagerWindow();
            _updateModManagerWindow = new UpdateModManagerWindow();
            
            var stopwatch = Stopwatch.StartNew();
            float deltaTime = 0f;
            // Main application loop

            // Apply Default Theme
            Theme.RelinkTheme();

            if (_AppSettings.CheckForUpdateOnStartup)
            {
                // Perform Mod Manager update check and prompting if there's a new version
                if (!string.IsNullOrEmpty(_AppSettings.ModManagerLatestVersionCheckURL))
                {
                    try
                    {
                        var client = new HttpClient();
                        // Only allow up to 5 seconds to check for an update since this will be blocking startup
                        client.Timeout = TimeSpan.FromSeconds(5);
                        var task = client.GetStringAsync(_AppSettings.ModManagerLatestVersionCheckURL);
                        var response = task.GetAwaiter().GetResult();
                        if (!string.IsNullOrEmpty(response))
                        {
                            var latestVersion = Version.Parse(response);
                            var IsRunningLatest = _AppSettings.ModManagerVersion.CompareTo(latestVersion);
                            if (IsRunningLatest >= 0)
                            {
                                // Up to date or running newer (dev) version
                                Console.WriteLine("Running latest Mod Manager");
                            }
                            else
                            {
                                ImGui.OpenPopup("###UpdateModManagerWindow");
                                ModManagerUpdateAvailable = true;
                                Console.WriteLine("New Mod Manager Update Available");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to successfully check for updates. Reason:\n {ex.Message}");
                    }
                }
            }

            // Check online if there are 'alerts' for the mod manager such as game update breaking mods
            // Only display alert on start-up and only for specified mod manager versions
            if (!string.IsNullOrEmpty(_AppSettings.ModManagerAlertsURL))
            {
                try
                {
                    var client = new HttpClient();
                    // Only allow up to 5 seconds to check for an alert since this will be blocking startup
                    client.Timeout = TimeSpan.FromSeconds(5);
                    var task = client.GetStringAsync(_AppSettings.ModManagerAlertsURL);
                    var response = task.GetAwaiter().GetResult();
                    if (!string.IsNullOrEmpty(response))
                    {
                        var AlertObj = JsonConvert.DeserializeObject<ModManagerAlert>(response);
                        var MaxVersionShown = Version.Parse(AlertObj.MaxVersionShown);
                        var ShouldShowAlert = _AppSettings.ModManagerVersion.CompareTo(MaxVersionShown);
                        if (ShouldShowAlert <= 0)
                        {
                            ModManagerAlertsWindow.UpdateAlert(AlertObj);
                            ModManagerAlertFound = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to successfully check for alerts. Reason:\n {ex.Message}");
                }
            }

            while (_window.Exists)
            {
                deltaTime = stopwatch.ElapsedTicks / (float)Stopwatch.Frequency;
                stopwatch.Restart();
                InputSnapshot snapshot = _window.PumpEvents();
                if (!_window.Exists) { break; }
                _controller.Update(deltaTime, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

                if (FontManager.DefaultFont != "" && FontManager.Fonts.ContainsKey(FontManager.DefaultFont))
                {
                    FontManager.PushFont(FontManager.DefaultFont);
                }
                else
                {
                    FontManager.PushFont("SegoeUI-SemiBold");
                }

                RenderWindowList();

                ImGui.PopFont();

                _cl.Begin();
                _cl.SetFramebuffer(_gd.MainSwapchain.Framebuffer);
                _cl.ClearColorTarget(0, new RgbaFloat(_clearColor.X, _clearColor.Y, _clearColor.Z, 1f));
                _controller.Render(_gd, _cl);
                _cl.End();
                _gd.SubmitCommands(_cl);
                _gd.SwapBuffers(_gd.MainSwapchain);

                // Prevent excessive GPU usage while app is not visible
                if (_window.WindowState == WindowState.Minimized || _window.WindowState == WindowState.Hidden)
                {
                    Thread.Sleep(100);
                }
            }

            // Save settings on Mod Manager closing
            _AppSettings.Save();

            // Clean up Veldrid resources
            _gd.WaitForIdle();
            _controller.Dispose();
            _cl.Dispose();
            _gd.Dispose();
        }

        private static unsafe void RenderWindowList()
        {
            _modManagerWindow.Draw();
            if (ModManagerAlertFound)
            {
                ImGui.OpenPopup("###ModManagerAlertWindow");
                ModManagerAlertFound = false;
            }
            ModManagerAlertsWindow.Draw();

            if (ModManagerUpdateAvailable && !ImGui.IsPopupOpen("###ModManagerAlertWindow"))
            {
                ImGui.OpenPopup("###UpdateModManagerWindow");
                ModManagerUpdateAvailable = false;
            }
            _updateModManagerWindow.Draw();
        }

        private static unsafe void LoadCustomFonts()
        {
            FontManager.AddFont(new Font("UbuntuMono-Regular", 13, new FontFile("Relink_Mod_Manager.Fonts.UbuntuMono-Regular.ttf")));
            FontManager.AddFont(new Font("UbuntuMono-Bold", [13, 15, 18], new FontFile("Relink_Mod_Manager.Fonts.UbuntuMono-Regular.ttf")));

            FontManager.AddFont(new Font("SegoeUI", 18, new FontFile(@"C:\Windows\Fonts\segoeui.ttf")));
            FontManager.AddFont(new Font("SegoeUI-SemiBold", 18, new FontFile(@"C:\Windows\Fonts\seguisb.ttf")));
            FontManager.AddFont(new Font("SegoeUI-Bold", 18, new FontFile(@"C:\Windows\Fonts\segoeuib.ttf")));
        }
    }
}
