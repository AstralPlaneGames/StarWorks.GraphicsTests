using System;
using System.Collections.Generic;
using MoonWorks;
using MoonWorks.Graphics;
using SDL3;

namespace MoonWorksGraphicsTests;

public class Game1 : Game
{
	List<Example> Examples = new();

	int ExampleIndex = 0;
	DateTime LastIndexChange = DateTime.Now.AddSeconds(3);

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
		Examples.Add(new ClearScreenExample());

		switch (SDL.SDL_GetPlatform())
		{
			case "WinRT":
			case "Android":
			case "iOS":
				break;
			default:
				Examples.Add(new ClearScreen_MultiWindowExample());
				break;
		}
		Examples.Add(new BasicStencilExample());
		Examples.Add(new BasicTriangleExample());

		switch (SDL.SDL_GetPlatform())
		{
			case "Android":
			case "iOS":
				break;
			default:
				Examples.Add(new CompressedTexturesExample());
				break;
		}
		Examples.Add(new FontExample());
		Examples.Add(new BasicComputeExample());
		Examples.Add(new ComputeUniformsExample());
		Examples.Add(new CopyTextureExample());
		Examples.Add(new CullFaceExample());
		Examples.Add(new DrawIndirectExample());
		Examples.Add(new GetBufferDataExample());
		Examples.Add(new InstancingAndOffsetsExample());
		switch (SDL.SDL_GetPlatform())
		{
			case "Android":
			case "iOS":
				break;
			default:
				Examples.Add(new MSAAExample());
				Examples.Add(new DepthMSAAExample());
				break;
		}
		Examples.Add(new RenderTexture2DArrayExample());
		Examples.Add(new RenderTexture2DExample());
		Examples.Add(new RenderTextureCubeExample());
		Examples.Add(new RenderTextureMipmapsExample());
		Examples.Add(new StoreLoadExample());
		Examples.Add(new Texture3DCopyExample());
		Examples.Add(new Texture3DExample());
		Examples.Add(new TexturedQuadExample());
		Examples.Add(new TexturedAnimatedQuadExample());
		Examples.Add(new TextureMipmapsExample());
		Examples.Add(new TriangleVertexBufferExample());
		Examples.Add(new VertexSamplerExample());
		Examples.Add(new VideoPlayerExample());
		Examples.Add(new WindowResizingExample());
		Examples.Add(new CPUSpriteBatchExample());
		Examples.Add(new ComputeSpriteBatchExample());
		Examples.Add(new PullSpriteBatchExample());
		Examples.Add(new CubeExample());
		Examples.Add(new HotReloadShaderExample());


		Logger.LogInfo("Welcome to the MoonWorks Graphics Tests program!");
		Examples[ExampleIndex].Start(this);
    }

    protected override void Update(TimeSpan delta)
    {
		if (TestUtils.CheckButtonPressed(Inputs, TestUtils.ButtonType.Previous))
		{
			Examples[ExampleIndex].Destroy();

			ExampleIndex -= 1;
			if (ExampleIndex < 0)
			{
				ExampleIndex = Examples.Count - 1;
			}

			MainWindow.SetSize(640, 480);
			MainWindow.SetPositionCentered();
			Examples[ExampleIndex].Start(this);
		}
		else if (TestUtils.CheckButtonPressed(Inputs, TestUtils.ButtonType.Next))
		{
			Examples[ExampleIndex].Destroy();

			ExampleIndex = (ExampleIndex + 1) % Examples.Count;

			MainWindow.SetSize(640, 480);
			MainWindow.SetPositionCentered();
			Examples[ExampleIndex].Start(this);
		}

		if (SDL.SDL_GetPlatform() == "Android" || SDL.SDL_GetPlatform() == "iOS")
		{
			if (LastIndexChange < DateTime.Now)
			{
				LastIndexChange = DateTime.Now.AddSeconds(3);

				ExampleIndex = (ExampleIndex + 1) % Examples.Count;
				if (Examples[ExampleIndex] is CubeExample)
					LastIndexChange = DateTime.Now.AddSeconds(10);
				Examples[ExampleIndex].Start(this);
			}
		}

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
