using System;
using MoonWorks;
using MoonWorks.Graphics;

namespace MoonWorksGraphicsTests;

public class Game1 : Game
{
	Example[] Examples =
	[
		new ClearScreenExample(),
		new ClearScreen_MultiWindowExample(),
		new BasicStencilExample(),
		new BasicTriangleExample(),
		new CompressedTexturesExample(),
		new FontExample(),
		new BasicComputeExample(),
		new ComputeUniformsExample(),
		new CopyTextureExample(),
		new CullFaceExample(),
		new DrawIndirectExample(),
		new GetBufferDataExample(),
		new InstancingAndOffsetsExample(),
		new MSAAExample(),
		new DepthMSAAExample(),
		new RenderTexture2DArrayExample(),
		new RenderTexture2DExample(),
		new RenderTextureCubeExample(),
		new RenderTextureMipmapsExample(),
		new StoreLoadExample(),
		new Texture3DCopyExample(),
		new Texture3DExample(),
		new TexturedQuadExample(),
		new TexturedAnimatedQuadExample(),
		new TextureMipmapsExample(),
		new TriangleVertexBufferExample(),
		new VertexSamplerExample(),
		new VideoPlayerExample(),
		new WindowResizingExample(),
		new CPUSpriteBatchExample(),
		new ComputeSpriteBatchExample(),
        new PullSpriteBatchExample(),
		new CubeExample(),
		new HotReloadShaderExample()
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
		if (TestUtils.CheckButtonPressed(Inputs, TestUtils.ButtonType.Previous))
		{
			Examples[ExampleIndex].Destroy();

			ExampleIndex -= 1;
			if (ExampleIndex < 0)
			{
				ExampleIndex = Examples.Length - 1;
			}

			MainWindow.SetSize(640, 480);
			MainWindow.SetPositionCentered();
			Examples[ExampleIndex].Start(this);
		}
		else if (TestUtils.CheckButtonPressed(Inputs, TestUtils.ButtonType.Next))
		{
			Examples[ExampleIndex].Destroy();

			ExampleIndex = (ExampleIndex + 1) % Examples.Length;

			MainWindow.SetSize(640, 480);
			MainWindow.SetPositionCentered();
			Examples[ExampleIndex].Start(this);
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
