using System;
using MoonWorks;

namespace MoonWorksGraphicsTests;

class Program
{
    static void Main(string[] args)
	{
		var debugMode = false;

		#if DEBUG
		debugMode = true;
		#endif

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
			debugMode
		);

		game.Run();
	}
}
