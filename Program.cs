using MoonWorks;
using SDL3;
using System;
using System.Diagnostics;

namespace MoonWorksGraphicsTests
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static unsafe class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            realArgs = args;
            SDL.SDL_SetHint("SDL_WINRT_HANDLE_BACK_BUTTON", "1");

            SDL.SDL_RunApp(0, IntPtr.Zero, FakeMain, IntPtr.Zero);
        }

        static string[] realArgs;

        static int FakeMain(int argc, IntPtr argv)
        {
            RealMain(realArgs);
            return 0;
        }

        static void RealMain(string[] args)
        {
            Logger.LogInfo = (str) =>
            {
                Debug.Write("INFO: ");
                Debug.WriteLine(str);
            };

            var scale = Windows.Graphics.Display.DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var window = Windows.UI.Core.CoreWindow.GetForCurrentThread();
            var coreWindowWidth = (uint)(0.5f + scale * window.Bounds.Width);
            var coreWindowHeight = (uint)(0.5f + scale * window.Bounds.Height);

            var windowCreateInfo = new WindowCreateInfo(
                "MoonWorksGraphicsTests",
                coreWindowWidth,
                coreWindowHeight,
                ScreenMode.Windowed
            );

            var framePacingSettings = FramePacingSettings.CreateCapped(60, 120);

            var debugMode = false;

            #if DEBUG
            debugMode = true;
            #endif

            var game = new Game1(
                new AppInfo("MoonsideGames", "MoonWorksGraphicsTests"),
                windowCreateInfo,
                framePacingSettings,
                debugMode
            );
            game.Run();
        }
    }
}
