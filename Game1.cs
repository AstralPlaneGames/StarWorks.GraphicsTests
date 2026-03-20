using System;
using MoonWorks;
using MoonWorks.Graphics;

namespace MoonWorksGraphicsTests;

class Game1 : Game
{
	Example[] Examples =
	[
		new ClearScreenExample(),
	];

	int ExampleIndex = 0;

    public Game1(
		AppInfo appInfo,
		WindowCreateInfo windowCreateInfo,
		FramePacingSettings framePacingSettings,
		bool debugMode = false
	) : base(
		appInfo,
		windowCreateInfo,
		framePacingSettings,
		ShaderFormat.SPIRV | ShaderFormat.DXIL | ShaderFormat.MSL | ShaderFormat.DXBC,
		debugMode
	) {
		Logger.LogInfo("Welcome to the MoonWorks Graphics Tests program!");
		Examples[ExampleIndex].Start(this);
    }

    protected override void Update(TimeSpan delta)
    {
		Examples[ExampleIndex].Update(delta);
    }

    protected override void Draw(double alpha)
    {
        Examples[ExampleIndex].Draw(alpha);
    }

    protected override void Destroy()
    {
        Examples[ExampleIndex].Destroy();
    }

    protected override void Step()
    {

    }
}
