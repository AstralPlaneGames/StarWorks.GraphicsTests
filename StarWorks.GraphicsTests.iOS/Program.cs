using System;
using Foundation;
using UIKit;
using System.Runtime.InteropServices;
using SDL3;
using MoonWorks;

namespace MoonWorksGraphicsTests;

class Program
{
    internal static void RunGame()
    {
		var windowCreateInfo = new WindowCreateInfo(
			"MoonWorksGraphicsTests",
			640,
			480,
			ScreenMode.Windowed
		);

		var framePacingSettings = FramePacingSettings.CreateCapped(60, 120);

		var game = new Game1(
			new AppInfo("MoonsideGames", "MoonWorksGraphicsTests"),
			windowCreateInfo,
			framePacingSettings,
			false
		);
        game.Run();
    }

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static void Main(string[] args)
    {
        // Keep mouse and touch input separate.
        SDL.SDL_SetHint(SDL.SDL_HINT_MOUSE_TOUCH_EVENTS, "0");
        SDL.SDL_SetHint(SDL.SDL_HINT_TOUCH_MOUSE_EVENTS, "0");

        realArgs = args;
        SDL.SDL_RunApp(0, IntPtr.Zero, FakeMain, IntPtr.Zero);
    }

    static string[] realArgs;

    [ObjCRuntime.MonoPInvokeCallback(typeof(SDL.SDL_main_func))]
    static int FakeMain(int argc, IntPtr argv)
    {
        RealMain(realArgs);
        return 0;
    }
    static void RealMain(string[] args)
    {
        RunGame();
    }
}
