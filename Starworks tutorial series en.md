# 🎮 StarWorks Complete Tutorial Series

**StarWorks** (built on MoonWorks) is a modern cross-platform game development framework that provides high-performance C# tooling. This tutorial series covers everything from beginner to advanced topics.

---

## 📚 Tutorial Contents

### Part 1: Foundations
1. [Environment Setup and Project Creation](#environment-setup-and-project-creation)
2. [Game Class Basics](#game-class-basics)
3. [Your First Triangle](#your-first-triangle)
4. [Understanding the Game Loop](#understanding-the-game-loop)

### Part 2: Graphics System
5. [Vertex Buffers and Indices](#vertex-buffers-and-indices)
6. [Texture System Basics](#texture-system-basics)
7. [Shader Programming](#shader-programming)
8. [Pipeline State Management](#pipeline-state-management)
9. [2D Sprite Rendering](#2d-sprite-rendering)

### Part 3: Advanced Graphics
10. [3D Geometry Rendering](#3d-geometry-rendering)
11. [Depth Testing and Stencil Buffer](#depth-testing-and-stencil-buffer)
12. [Multisample Anti-Aliasing (MSAA)](#multisample-anti-aliasing-msaa)
13. [Compute Shaders](#compute-shaders)
14. [Render Targets](#render-targets)

### Part 4: Interaction and Input
15. [Keyboard Input Handling](#keyboard-input-handling)
16. [Mouse and Gamepad](#mouse-and-gamepad)
17. [Virtual Button System](#virtual-button-system)

### Part 5: Audio and Video
18. [Audio System Basics](#audio-system-basics)
19. [3D Spatial Audio](#3d-spatial-audio)
20. [Video Playback and Decoding](#video-playback-and-decoding)

### Part 6: Advanced Topics
21. [Multi-Window Management](#multi-window-management)
22. [File Storage System](#file-storage-system)
23. [Performance Optimization](#performance-optimization)
24. [Cross-Platform Deployment](#cross-platform-deployment)

---

## Part 1: Foundations

### Environment Setup and Project Creation

#### System Requirements
- **.NET SDK**: 9.0 or higher
- **Operating System**: Windows, Linux, macOS, iOS, Android
- **IDE**: Visual Studio, Visual Studio Code, or JetBrains Rider

#### Create a New Project

```bash
# Clone or get the StarWorks framework
git clone https://github.com/AstralPlaneGames/StarWorks.git

# Create a new console app project
dotnet new console -n MyGameProject
cd MyGameProject

# Add the StarWorks NuGet package (if available)
# Or add a direct project reference
```

#### Project File Configuration (.csproj)

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <StartupObject>MyGame.Program</StartupObject>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="../StarWorks/StarWorks.csproj" />
  </ItemGroup>
</Project>
```

---

### Game Class Basics

#### Understanding the Game Base Class

In StarWorks, every game must inherit from the `Game` class. It is the main entry point of your game.

```csharp
using MoonWorks;
using MoonWorks.Graphics;

namespace MyGame;

class MyFirstGame : Game
{
    public MyFirstGame() : base(
        new AppInfo("com.mygame.studio", "MyFirstGame"),
        new WindowCreateInfo
        {
            WindowWidth = 1280,
            WindowHeight = 720,
            WindowTitle = "StarWorks Game",
            ScreenMode = ScreenMode.Windowed,
            PresentMode = PresentMode.Mailbox  // or Immediate, Fifo
        },
        new FramePacingSettings(),
        ShaderFormat.SPIRV | ShaderFormat.DXIL | ShaderFormat.MSL | ShaderFormat.DXBC
    )
    {
    }

    public override void Init()
    {
        // Initialize resources: shaders, textures, buffers, etc.
        Window.SetTitle("My First Game");
    }

    public override void Update(TimeSpan delta)
    {
        // Update game logic
    }

    public override void Draw(double alpha)
    {
        // Draw game: submit render commands
    }

    public override void Destroy()
    {
        // Clean up resources
    }

    static void Main(string[] args)
    {
        var game = new MyFirstGame();
        game.Run();
    }
}
```

#### Core Properties of Game

| Property | Type | Purpose |
|------|------|------|
| `GraphicsDevice` | GraphicsDevice | Manages graphics resources and rendering operations |
| `AudioDevice` | AudioDevice | Audio playback device |
| `VideoDevice` | VideoDevice | Video decoding device |
| `Inputs` | Inputs | Input management (keyboard, mouse, gamepad) |
| `MainWindow` | Window | Main game window |
| `RootTitleStorage` | TitleStorage | Application directory (read-only assets) |
| `UserStorage` | UserStorage | User documents directory (read/write) |

---

### Your First Triangle

#### Minimal Rendering Example

To render a triangle, you need:
1. Shader programs (vertex + fragment)
2. A graphics pipeline
3. Vertex data or buffers

#### Writing Shaders

**Vertex Shader** (triangle.vert, HLSL):
```hlsl
cbuffer UniformBlock : register(b0, space1)
{
    float4x4 ViewProjection : packoffset(c0);
};

struct VS_INPUT
{
    float3 Position : POSITION;
    float4 Color : COLOR;
};

struct VS_OUTPUT
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR;
};

VS_OUTPUT main(VS_INPUT input)
{
    VS_OUTPUT output;
    output.Position = mul(ViewProjection, float4(input.Position, 1.0));
    output.Color = input.Color;
    return output;
}
```

**Fragment Shader** (triangle.frag):
```hlsl
struct PS_INPUT
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR;
};

float4 main(PS_INPUT input) : SV_TARGET
{
    return input.Color;
}
```

#### C# Implementation

```csharp
using MoonWorks;
using MoonWorks.Graphics;
using System.Numerics;

class TriangleGame : Game
{
    GraphicsPipeline Pipeline;
    Buffer VertexBuffer;
    Buffer IndexBuffer;

    struct PositionColorVertex : IVertexType
    {
        public Vector3 Position;
        public Color Color;

        public static VertexElementFormat[] Formats { get; } =
        [
            VertexElementFormat.Float3,
            VertexElementFormat.Ubyte4Norm
        ];

        public static uint[] Offsets { get; } =
        [
            0,
            12
        ];
    }

    public override void Init()
    {
        // Load shaders
        Shader vertShader = ShaderCross.Create(
            GraphicsDevice,
            RootTitleStorage,
            "triangle.vert",
            "main",
            ShaderCross.ShaderFormat.HLSL,
            ShaderStage.Vertex
        );

        Shader fragShader = ShaderCross.Create(
            GraphicsDevice,
            RootTitleStorage,
            "triangle.frag",
            "main",
            ShaderCross.ShaderFormat.HLSL,
            ShaderStage.Fragment
        );

        // Create pipeline
        GraphicsPipelineCreateInfo pipelineInfo = new GraphicsPipelineCreateInfo
        {
            TargetInfo = new GraphicsPipelineTargetInfo
            {
                ColorTargetDescriptions =
                [
                    new ColorTargetDescription
                    {
                        Format = Window.SwapchainFormat,
                        BlendState = ColorTargetBlendState.Opaque
                    }
                ]
            },
            VertexShader = vertShader,
            FragmentShader = fragShader,
            VertexInputState = VertexInputState.CreateSingleBinding<PositionColorVertex>(),
            PrimitiveType = PrimitiveType.TriangleList,
            RasterizerState = RasterizerState.CCW_CullNone,
            MultisampleState = MultisampleState.None,
            DepthStencilState = DepthStencilState.Disable
        };

        Pipeline = GraphicsPipeline.Create(GraphicsDevice, pipelineInfo);

        // Create vertex data
        PositionColorVertex[] vertices = new PositionColorVertex[]
        {
            new PositionColorVertex
            {
                Position = new Vector3(0, -0.5f, 0),
                Color = Color.Red
            },
            new PositionColorVertex
            {
                Position = new Vector3(0.5f, 0.5f, 0),
                Color = Color.Green
            },
            new PositionColorVertex
            {
                Position = new Vector3(-0.5f, 0.5f, 0),
                Color = Color.Blue
            }
        };

        // Upload vertex data to GPU
        VertexBuffer = Buffer.Create<PositionColorVertex>(
            GraphicsDevice,
            BufferUsageFlags.Vertex,
            3
        );

        var uploadBuffer = TransferBuffer.Create<PositionColorVertex>(
            GraphicsDevice,
            TransferBufferUsage.Upload,
            3
        );

        var span = uploadBuffer.Map<PositionColorVertex>(false);
        vertices.CopyTo(span);
        uploadBuffer.Unmap();

        var cmdbuf = GraphicsDevice.AcquireCommandBuffer();
        var copyPass = cmdbuf.BeginCopyPass();
        copyPass.UploadToBuffer(uploadBuffer, VertexBuffer, false);
        cmdbuf.EndCopyPass(copyPass);
        GraphicsDevice.Submit(cmdbuf);

        uploadBuffer.Dispose();

        // Index buffer
        uint[] indices = new uint[] { 0, 1, 2 };
        IndexBuffer = Buffer.Create<uint>(
            GraphicsDevice,
            BufferUsageFlags.Index,
            3
        );

        // Upload index data (same process)
        // Omitted for brevity...
    }

    public override void Update(TimeSpan delta)
    {
    }

    public override void Draw(double alpha)
    {
        Matrix4x4 viewProj = Matrix4x4.CreateOrthographicOffCenter(
            -1, 1, -1, 1, 0, -1
        );

        CommandBuffer cmdbuf = GraphicsDevice.AcquireCommandBuffer();
        Texture swapchainTexture = cmdbuf.AcquireSwapchainTexture(MainWindow);

        if (swapchainTexture != null)
        {
            RenderPass renderPass = cmdbuf.BeginRenderPass(
                new ColorTargetInfo(swapchainTexture, Color.Black)
            );

            renderPass.BindGraphicsPipeline(Pipeline);
            renderPass.BindVertexBuffers(VertexBuffer);
            renderPass.BindIndexBuffer(IndexBuffer, IndexElementSize.ThirtyTwo);
            cmdbuf.PushVertexUniformData(viewProj);
            renderPass.DrawIndexedPrimitives(3, 1, 0, 0, 0);

            cmdbuf.EndRenderPass(renderPass);
        }

        GraphicsDevice.Submit(cmdbuf);
    }

    public override void Destroy()
    {
        Pipeline.Dispose();
        VertexBuffer.Dispose();
        IndexBuffer.Dispose();
    }
}
```

---

### Understanding the Game Loop

In StarWorks, the game loop is managed by the `Run()` method.

```
Run()
  ├─ Init() - called once for initialization
  └─ Loop:
      ├─ IsRunning check
      ├─ Update(deltaTime) - update game logic
      ├─ Draw(alpha) - render
      └─ frame sync (via PresentMode)
```

#### Key Parameters

**PresentMode** - frame presentation mode:
- `Mailbox`: no vertical sync wait, lowest latency (recommended for full-screen games)
- `Fifo`: locked to refresh rate (60Hz, 120Hz, etc.)
- `Immediate`: present immediately, may tear (useful for benchmarks)

**FrameLimiterSettings** - framerate cap:
```csharp
new FramePacingSettings
{
    Mode = FramePacingMode.Capped,
    TargetFrameRate = 60  // cap at 60 FPS
}
```

---

## Part 2: Graphics System

### Vertex Buffers and Indices

#### Why Buffers Are Needed

For GPU rendering, vertex data must live in GPU memory rather than CPU memory. Buffers are the abstraction of GPU memory resources.

#### Vertex Structure Definition

```csharp
using System.Numerics;

struct PositionColorVertex : IVertexType
{
    public Vector3 Position;      // position
    public Vector4 Color;         // RGBA color

    public static VertexElementFormat[] Formats { get; } =
    [
        VertexElementFormat.Float3,
        VertexElementFormat.Float4
    ];

    public static uint[] Offsets { get; } =
    [
        0,
        12
    ];
}
```

#### Creating a Vertex Buffer

```csharp
// Define vertex data
PositionColorVertex[] vertices = new PositionColorVertex[]
{
    new() { Position = new Vector3(0, 1, 0), Color = new Vector4(1, 0, 0, 1) },
    new() { Position = new Vector3(1, -1, 0), Color = new Vector4(0, 1, 0, 1) },
    new() { Position = new Vector3(-1, -1, 0), Color = new Vector4(0, 0, 1, 1) }
};

// Step 1: Create TransferBuffer (CPU->GPU staging)
TransferBuffer uploadBuffer = TransferBuffer.Create<PositionColorVertex>(
    GraphicsDevice,
    TransferBufferUsage.Upload,
    vertices.Length
);

// Step 2: Map memory and copy data
var span = uploadBuffer.Map<PositionColorVertex>(false);
vertices.CopyTo(span);
uploadBuffer.Unmap();

// Step 3: Create actual GPU buffer
Buffer vertexBuffer = Buffer.Create<PositionColorVertex>(
    GraphicsDevice,
    BufferUsageFlags.Vertex,
    vertices.Length
);

// Step 4: Submit copy command to GPU
CommandBuffer cmdbuf = GraphicsDevice.AcquireCommandBuffer();
CopyPass copyPass = cmdbuf.BeginCopyPass();
copyPass.UploadToBuffer(uploadBuffer, vertexBuffer, false);
cmdbuf.EndCopyPass(copyPass);
GraphicsDevice.Submit(cmdbuf);

uploadBuffer.Dispose();
```

#### Index Buffer

Index buffers allow vertex reuse and reduce memory usage.

```csharp
// A cube typically defined with 24 unique vertices
PositionColorVertex[] cubeVertices = new PositionColorVertex[24];

// Indices: 6 faces, each face has 2 triangles (6 indices)
uint[] cubeIndices = new uint[]
{
    // Front
    0, 1, 2,    2, 3, 0,
    // Back
    4, 6, 5,    6, 7, 5,
    // Left
    8, 9, 10,   10, 11, 8,
    // Right
    12, 14, 13, 14, 15, 13,
    // Top
    16, 18, 17, 18, 19, 17,
    // Bottom
    20, 21, 22, 22, 23, 20
};

// Create index buffer
Buffer indexBuffer = Buffer.Create<uint>(
    GraphicsDevice,
    BufferUsageFlags.Index,
    cubeIndices.Length
);

// Upload index data (same process as vertex buffer)
```

#### Rendering with Buffers

```csharp
RenderPass renderPass = cmdbuf.BeginRenderPass(
    new ColorTargetInfo(swapchainTexture, Color.Black)
);

renderPass.BindGraphicsPipeline(pipeline);
renderPass.BindVertexBuffers(vertexBuffer, 0);  // bind vertex buffer
renderPass.BindIndexBuffer(indexBuffer, IndexElementSize.ThirtyTwo);  // bind index buffer

// Draw command
// Parameters: (indexCount, instanceCount, firstIndex, vertexOffset, firstInstance)
renderPass.DrawIndexedPrimitives(36, 1, 0, 0, 0);

cmdbuf.EndRenderPass(renderPass);
```

---

### Texture System Basics

#### Texture Fundamentals

A texture is image data stored on the GPU, used for surface detail, shading, and more.

#### Loading Textures

```csharp
// Method 1: Load from file
ResourceUploader uploader = new ResourceUploader(GraphicsDevice);

Texture textureFromPNG = uploader.CreateTexture2DFromCompressed(
    RootTitleStorage,
    "Assets/Textures/wood.png",
    TextureFormat.R8G8B8A8Unorm,
    TextureUsageFlags.Sampler
);

uploader.Upload();
uploader.Dispose();

// Method 2: Create an empty texture
Texture renderTarget = Texture.Create2D(
    GraphicsDevice,
    1024,
    768,
    TextureFormat.R8G8B8A8Unorm,
    TextureUsageFlags.ColorTarget
);
```

#### Samplers

Samplers define how textures are sampled by the GPU.

```csharp
// Point sampling (nearest)
Sampler pointSampler = Sampler.Create(
    GraphicsDevice,
    SamplerCreateInfo.PointClamp
);

// Linear sampling (bilinear)
Sampler linearSampler = Sampler.Create(
    GraphicsDevice,
    SamplerCreateInfo.LinearClamp
);

// Anisotropic sampling
Sampler anisotropicSampler = Sampler.Create(
    GraphicsDevice,
    new SamplerCreateInfo
    {
        AddressModeU = SamplerAddressMode.Clamp,
        AddressModeV = SamplerAddressMode.Clamp,
        Filter = Filter.Linear,
        AnisotropyEnable = true,
        MaxAnisotropy = 16
    }
);
```

#### Binding Textures and Samplers

```csharp
// In render pass
renderPass.BindFragmentSamplers(
    new TextureSamplerBinding(myTexture, mySampler)
);
```

#### Texture Address Modes

```csharp
// Common address modes
SamplerAddressMode.Clamp,        // clamp to edge
SamplerAddressMode.Wrap,         // repeat
SamplerAddressMode.MirroredWrap, // mirrored repeat
SamplerAddressMode.ClampToEdge,  // edge clamp behavior
```

---

### Shader Programming

#### Shader Types

StarWorks supports GLSL (SPIR-V) and HLSL shader workflows.

#### Basic Vertex Shader

```glsl
// Vertex shader example (GLSL)
#version 450

layout(location = 0) in vec3 Position;
layout(location = 1) in vec2 TexCoord;

layout(set = 0, binding = 0, std140) uniform UBO
{
    mat4 ViewProj;
};

layout(location = 0) out vec2 outTexCoord;

void main()
{
    gl_Position = ViewProj * vec4(Position, 1.0);
    outTexCoord = TexCoord;
}
```

#### Basic Fragment Shader

```glsl
// Fragment shader example
#version 450

layout(location = 0) in vec2 inTexCoord;
layout(set = 1, binding = 0) uniform sampler2D MainTexture;

layout(location = 0) out vec4 FragColor;

void main()
{
    FragColor = texture(MainTexture, inTexCoord);
}
```

#### Loading Shaders

```csharp
Shader vertShader = Shader.Create(
    GraphicsDevice,
    RootTitleStorage,
    "BasicTexturedQuad.vert",  // file path
    "main",                     // entry point
    ShaderCross.ShaderFormat.SPIRV,
    ShaderStage.Vertex
);

Shader fragShader = Shader.Create(
    GraphicsDevice,
    RootTitleStorage,
    "BasicTexturedQuad.frag",
    "main",
    ShaderCross.ShaderFormat.SPIRV,
    ShaderStage.Fragment
);
```

#### Passing Uniform Data

```csharp
// Vertex shader uniform
Matrix4x4 viewOrthogonal = Matrix4x4.CreateOrthographicOffCenter(
    0, 1280, 720, 0, 0, -1
);

Texture swapchainTexture = cmdbuf.AcquireSwapchainTexture(MainWindow);
RenderPass renderPass = cmdbuf.BeginRenderPass(
    new ColorTargetInfo(swapchainTexture, Color.Black)
);
renderPass.BindGraphicsPipeline(pipeline);
cmdbuf.PushVertexUniformData(viewOrthogonal);  // push data
renderPass.DrawPrimitives(3, 1, 0, 0);

// Fragment shader uniform requires the appropriate binding slot setup
```

---

### Pipeline State Management

#### Creating a GraphicsPipeline

```csharp
GraphicsPipelineCreateInfo pipelineInfo = new GraphicsPipelineCreateInfo
{
    // Output attachment formats
    TargetInfo = new GraphicsPipelineTargetInfo
    {
        ColorTargetDescriptions =
        [
            new ColorTargetDescription
            {
                Format = Window.SwapchainFormat,
                BlendState = ColorTargetBlendState.Opaque
            }
        ],
        HasDepthStencilTarget = true,
        DepthStencilFormat = TextureFormat.D32Float
    },

    // Shaders
    VertexShader = vertShader,
    FragmentShader = fragShader,

    // Vertex input layout
    VertexInputState = VertexInputState.CreateSingleBinding<PositionColorVertex>(),

    // Topology
    PrimitiveType = PrimitiveType.TriangleList,

    // Rasterization
    RasterizerState = new RasterizerState
    {
        CullMode = CullMode.Back,              // back-face culling
        FrontFace = FrontFace.CounterClockwise,
        FillMode = FillMode.Fill               // fill mode
    },

    // Depth/stencil
    DepthStencilState = new DepthStencilState
    {
        EnableDepthTest = true,
        EnableDepthWrite = true,
        CompareOp = CompareOp.LessOrEqual
    },

    // Multisampling
    MultisampleState = new MultisampleState
    {
        SampleCount = SampleCount.One,
        SampleMask = 0xffffffff
    }
};

GraphicsPipeline pipeline = GraphicsPipeline.Create(
    GraphicsDevice,
    pipelineInfo
);
```

#### Common Pipeline Configurations

```csharp
// Opaque pipeline
var opaquePipelineInfo = new GraphicsPipelineCreateInfo
{
    RasterizerState = new RasterizerState
    {
        CullMode = CullMode.Back,
        FillMode = FillMode.Fill
    },
    DepthStencilState = new DepthStencilState
    {
        EnableDepthTest = true,
        EnableDepthWrite = true,
        CompareOp = CompareOp.LessOrEqual
    },
    // ...
};

// Transparent pipeline (disable depth write)
var transparentPipelineInfo = new GraphicsPipelineCreateInfo
{
    DepthStencilState = new DepthStencilState
    {
        EnableDepthTest = true,
        EnableDepthWrite = false,
        CompareOp = CompareOp.LessOrEqual
    },
    TargetInfo = new GraphicsPipelineTargetInfo
    {
        ColorTargetDescriptions =
        [
            new ColorTargetDescription
            {
                Format = Window.SwapchainFormat,
                BlendState = ColorTargetBlendState.NonPremultipliedAlphaBlend
            }
        ]
    },
    // ...
};
```

---

### 2D Sprite Rendering

#### Sprite Structure

```csharp
struct Sprite
{
    public Vector2 Position;
    public float Rotation;
    public Vector2 Scale;
    public Vector4 Color;
    public Rectangle SourceRect;  // texture coordinates
}
```

#### CPU Sprite Batching

This is the most flexible but also the slowest method. See `CPUSpriteBatchExample.cs`.

```csharp
const int SPRITE_COUNT = 8192;

struct SpriteInstanceData
{
    public Vector3 Position;
    public float Rotation;
    public Vector2 Size;
    public Vector4 Color;
}

SpriteInstanceData[] spriteData = new SpriteInstanceData[SPRITE_COUNT];

// In Draw each frame:
for (int i = 0; i < SPRITE_COUNT; i++)
{
    // Build sprite transform
    Matrix4x4 transform =
        Matrix4x4.CreateScale(spriteData[i].Size.X, spriteData[i].Size.Y, 1) *
        Matrix4x4.CreateRotationZ(spriteData[i].Rotation) *
        Matrix4x4.CreateTranslation(spriteData[i].Position);

    // Generate vertices from transform
    // ...
}
```

#### GPU Sprite Batching (Compute Shader)

A more efficient approach is to use compute shaders. See `ComputeSpriteBatchExample.cs`.

```csharp
// Use ComputePipeline to process sprite transforms on GPU
ComputePipeline computePipeline = ShaderCross.Create(
    GraphicsDevice,
    RootTitleStorage,
    "SpriteTransform.comp",
    "main",
    ShaderCross.ShaderFormat.SPIRV
);
ComputePass computePass = cmdbuf.BeginComputePass();
computePass.BindComputePipeline(computePipeline);
// Bind data and dispatch...
cmdbuf.EndComputePass(computePass);
```

---

## Part 3: Advanced Graphics

### 3D Geometry Rendering

#### Cube Generation

```csharp
void GenerateCubeVertices(
    out PositionColorVertex[] vertices,
    out uint[] indices)
{
    vertices = new PositionColorVertex[24];
    indices = new uint[36];

    // Front (z=1)
    vertices[0] = new PositionColorVertex(-1, -1, 1);
    vertices[1] = new PositionColorVertex(1, -1, 1);
    vertices[2] = new PositionColorVertex(1, 1, 1);
    vertices[3] = new PositionColorVertex(-1, 1, 1);

    // Back (z=-1)
    vertices[4] = new PositionColorVertex(-1, -1, -1);
    vertices[5] = new PositionColorVertex(-1, 1, -1);
    vertices[6] = new PositionColorVertex(1, 1, -1);
    vertices[7] = new PositionColorVertex(1, -1, -1);

    // ... other 4 faces

    // Triangle indices
    indices = new uint[]
    {
        // Front
        0, 1, 2, 2, 3, 0,
        // Back
        4, 6, 5, 6, 7, 4,
        // ... other faces
    };
}
```

#### Matrix Transformations

```csharp
// Model matrix (model space -> world space)
Matrix4x4 modelMatrix = Matrix4x4.CreateTranslation(new Vector3(0, 0, 5)) *
                        Matrix4x4.CreateRotationY((float)gameTime.TotalGameTime.TotalSeconds);

// View matrix (world space -> camera space)
Vector3 cameraPosition = new Vector3(0, 2, -5);
Vector3 targetPosition = Vector3.Zero;
Vector3 upDirection = Vector3.UnitY;
Matrix4x4 viewMatrix = Matrix4x4.CreateLookAt(
    cameraPosition,
    targetPosition,
    upDirection
);

// Projection matrix (camera space -> clip space)
float fov = MathHelper.PiOver4;
float aspectRatio = 1280f / 720f;
Matrix4x4 projMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
    fov,
    aspectRatio,
    0.1f,
    1000f
);

// Combine to ViewProj
Matrix4x4 viewProj = modelMatrix * viewMatrix * projMatrix;
cmdbuf.PushVertexUniformData(viewProj);
```

#### Cubemaps (Including Skyboxes)

```csharp
// Load cubemap texture
Texture cubemap = resourceUploader.CreateTextureCubeFromCompressed(
    RootTitleStorage,
    "Assets/Textures/skybox",  // contains posx.png, negx.png, posy.png, etc.
    TextureFormat.R8G8B8A8Unorm,
    TextureUsageFlags.Sampler
);

// Sample in fragment shader
// vec3 direction = normalize(vertexPosition);
// vec4 color = texture(cubemapSampler, direction);
```

---

### Depth Testing and Stencil Buffer

#### Depth Buffer

```csharp
// Create a depth render target
Texture depthTexture = Texture.Create2D(
    GraphicsDevice,
    1280,
    720,
    TextureFormat.D32Float,
    TextureUsageFlags.DepthStencilTarget
);

// Specify depth format in pipeline
GraphicsPipelineCreateInfo pipelineInfo = new GraphicsPipelineCreateInfo
{
    TargetInfo = new GraphicsPipelineTargetInfo
    {
        ColorTargetDescriptions =
        [
            new ColorTargetDescription
            {
                Format = Window.SwapchainFormat,
                BlendState = ColorTargetBlendState.Opaque
            }
        ],
        HasDepthStencilTarget = true,
        DepthStencilFormat = depthTexture.Format
    },
    DepthStencilState = new DepthStencilState
    {
        EnableDepthTest = true,
        EnableDepthWrite = true,
        CompareOp = CompareOp.LessOrEqual
    }
};

// Bind depth attachment in render pass
RenderPass renderPass = cmdbuf.BeginRenderPass(
    new DepthStencilTargetInfo(depthTexture, 1f),
    new ColorTargetInfo(swapchainTexture, Color.Black)
);
```

#### Stencil Testing

Stencil buffers are useful for shadows, silhouettes, and masking effects.

```csharp
// Enable stencil operations in pipeline
DepthStencilState = new DepthStencilState
{
    EnableStencilTest = true,
    FrontStencilState = new StencilOpState
    {
        CompareOp = CompareOp.Always,
        PassOp = StencilOp.Replace,
        FailOp = StencilOp.Keep,
        DepthFailOp = StencilOp.Keep
    },
    BackStencilState = new StencilOpState
    {
        CompareOp = CompareOp.Always,
        PassOp = StencilOp.Replace,
        FailOp = StencilOp.Keep,
        DepthFailOp = StencilOp.Keep
    },
    CompareMask = 0xff,
    WriteMask = 0xff
}
```

#### Depth Sampling

Reading depth in shaders:

```glsl
#version 450

layout(set = 1, binding = 0) uniform sampler2D DepthTexture;

void main()
{
    float depth = texture(DepthTexture, uv).r;
    // use depth value for post-processing
}
```

---

### Multisample Anti-Aliasing (MSAA)

#### MSAA Configuration

```csharp
// MSAA texture
Texture msaaTexture = Texture.Create2D(
    GraphicsDevice,
    1280,
    720,
    TextureFormat.R8G8B8A8Unorm,
    TextureUsageFlags.ColorTarget,
    sampleCount: SampleCount.Eight
);

// Pipeline configuration
GraphicsPipelineCreateInfo pipelineInfo = new GraphicsPipelineCreateInfo
{
    MultisampleState = new MultisampleState
    {
        SampleCount = SampleCount.Eight,
        SampleMask = 0xffffffff
    },
    // ...
};

// Render
RenderPass renderPass = cmdbuf.BeginRenderPass(
    new ColorTargetInfo(msaaTexture, Color.Black)
);
```

#### MSAA with Depth

```csharp
Texture msaaDepthTexture = Texture.Create2D(
    GraphicsDevice,
    1280,
    720,
    TextureFormat.D32Float,
    TextureUsageFlags.DepthStencilTarget,
    sampleCount: SampleCount.Eight
);

RenderPass renderPass = cmdbuf.BeginRenderPass(
    new DepthStencilTargetInfo(msaaDepthTexture, 1f),
    new ColorTargetInfo(msaaTexture, Color.Black)
);
```

Reference example: `DepthMSAAExample.cs`

---

### Compute Shaders

#### Compute Pipeline Basics

Compute shaders run parallel workloads on the GPU.

```glsl
#version 450

layout(local_size_x = 16, local_size_y = 16) in;

layout(set = 0, binding = 0, rgba8) uniform image2D TargetTexture;

void main()
{
    ivec2 pixelCoord = ivec2(gl_GlobalInvocationID.xy);
    vec4 pixelColor = vec4(1.0, 0.0, 0.0, 1.0);  // red
    imageStore(TargetTexture, pixelCoord, pixelColor);
}
```

#### Creating a Compute Pipeline in C#

```csharp
ComputePipeline computePipeline = ShaderCross.Create(
    GraphicsDevice,
    RootTitleStorage,
    "ComputeShader.comp",
    "main",
    ShaderCross.ShaderFormat.SPIRV
);

// Create target texture
Texture targetTexture = Texture.Create2D(
    GraphicsDevice,
    1024,
    1024,
    TextureFormat.R8G8B8A8Unorm,
    TextureUsageFlags.ComputeStorageWrite
);

// Dispatch compute work
CommandBuffer cmdbuf = GraphicsDevice.AcquireCommandBuffer();
ComputePass computePass = cmdbuf.BeginComputePass();

computePass.BindComputePipeline(computePipeline);
computePass.BindStorageTextures(targetTexture);
computePass.Dispatch(1024 / 16, 1024 / 16, 1);

cmdbuf.EndComputePass(computePass);
GraphicsDevice.Submit(cmdbuf);
```

Reference examples: `BasicComputeExample.cs`, `ComputeSpriteBatchExample.cs`

---

### Render Targets

#### Off-Screen Rendering

```csharp
// Create a 2D render texture
Texture renderTexture2D = Texture.Create2D(
    GraphicsDevice,
    512,
    512,
    TextureFormat.R8G8B8A8Unorm,
    TextureUsageFlags.ColorTarget | TextureUsageFlags.Sampler
);

// Render into texture
RenderPass renderPass = cmdbuf.BeginRenderPass(
    new ColorTargetInfo(renderTexture2D, Color.Black)
);

renderPass.BindGraphicsPipeline(pipeline);
renderPass.DrawPrimitives(3, 1, 0, 0);

cmdbuf.EndRenderPass(renderPass);

// Then sample renderTexture2D as a regular texture
```

#### Cubemap Render Targets

Generate procedural cubemaps (for example, environment maps):

```csharp
// Create cubemap render target
Texture cubemap = Texture.CreateCube(
    GraphicsDevice,
    512,
    TextureFormat.R8G8B8A8Unorm,
    TextureUsageFlags.ColorTarget | TextureUsageFlags.Sampler
);

// Render each of the 6 cubemap faces
for (int i = 0; i < 6; i++)
{
    var colorTargetInfo = new ColorTargetInfo(cubemap, Color.Black)
    {
        LayerOrDepthPlane = (uint)i
    };

    RenderPass renderPass = cmdbuf.BeginRenderPass(
        colorTargetInfo
    );
    // Render this face...
    cmdbuf.EndRenderPass(renderPass);
}
```

Reference example: `RenderTextureCubeExample.cs`

---

## Part 4: Interaction and Input

### Keyboard Input Handling

#### Basic Key Polling

```csharp
public override void Update(TimeSpan delta)
{
    // Check specific keys
    if (Inputs.Keyboard.IsPressed(KeyCode.Escape))
    {
        Exit();
    }

    // WASD movement
    if (Inputs.Keyboard.IsPressed(KeyCode.W))
    {
        cameraPosition += Vector3.UnitZ * moveSpeed * (float)delta.TotalSeconds;
    }
    if (Inputs.Keyboard.IsPressed(KeyCode.S))
    {
        cameraPosition -= Vector3.UnitZ * moveSpeed * (float)delta.TotalSeconds;
    }
    if (Inputs.Keyboard.IsPressed(KeyCode.A))
    {
        cameraPosition -= Vector3.UnitX * moveSpeed * (float)delta.TotalSeconds;
    }
    if (Inputs.Keyboard.IsPressed(KeyCode.D))
    {
        cameraPosition += Vector3.UnitX * moveSpeed * (float)delta.TotalSeconds;
    }
}
```

#### Text Input Event

```csharp
public override void Init()
{
    Inputs.TextInput += OnTextInput;
}

private void OnTextInput(char inputChar)
{
    Logger.LogInfo($"Text input: {inputChar}");
}

public override void Destroy()
{
    Inputs.TextInput -= OnTextInput;
}
```

---

### Mouse and Gamepad

#### Mouse Input

```csharp
public override void Update(TimeSpan delta)
{
    // Mouse position (relative to window)
    int x = Inputs.Mouse.X;
    int y = Inputs.Mouse.Y;

    // Wheel
    int wheelDelta = Inputs.Mouse.Wheel;
    if (wheelDelta > 0)
    {
        cameraDistance -= zoomSpeed * (float)delta.TotalSeconds;
    }
    else if (wheelDelta < 0)
    {
        cameraDistance += zoomSpeed * (float)delta.TotalSeconds;
    }

    // Buttons
    if (Inputs.Mouse.LeftButton.IsPressed)
    {
        // Left button pressed
    }

    if (Inputs.Mouse.RightButton.IsPressed)
    {
        // Right button pressed
    }

    // Relative mouse mode (FPS style)
    Inputs.Mouse.SetRelativeMode(MainWindow, true);
    int relX = Inputs.Mouse.DeltaX;
    int relY = Inputs.Mouse.DeltaY;
}
```

#### Gamepad Input

```csharp
public override void Update(TimeSpan delta)
{
    // Check connected gamepad
    if (!Inputs.GamepadExists(0))
    {
        return;
    }

    var gamepad = Inputs.GetGamepad(0);

    // Sticks
    float leftStickX = gamepad.LeftX.Value;
    float leftStickY = gamepad.LeftY.Value;

    // Apply deadzone
    if (MathF.Abs(leftStickX) < 0.1f) leftStickX = 0;
    if (MathF.Abs(leftStickY) < 0.1f) leftStickY = 0;

    // Triggers
    float leftTrigger = gamepad.TriggerLeft.Value;
    float rightTrigger = gamepad.TriggerRight.Value;

    // Buttons
    if (gamepad.Start.IsPressed)
    {
        PauseGame();
    }

    if (gamepad.South.IsPressed)
    {
        Jump();
    }
}
```

---

### Virtual Button System

Virtual buttons allow keyboard, mouse, and gamepad mapping to logical actions.

```csharp
public class InputManager
{
    private readonly Dictionary<string, Func<bool>> actions;
    private readonly Inputs inputs;

    public InputManager(Inputs inputs)
    {
        this.inputs = inputs;

        actions = new Dictionary<string, Func<bool>>
        {
            { "Move_Forward", () => inputs.Keyboard.IsDown(KeyCode.W) || inputs.GetGamepad(0).DpadUp.IsDown },
            { "Move_Backward", () => inputs.Keyboard.IsDown(KeyCode.S) || inputs.GetGamepad(0).DpadDown.IsDown },
            { "Move_Left", () => inputs.Keyboard.IsDown(KeyCode.A) || inputs.GetGamepad(0).DpadLeft.IsDown },
            { "Move_Right", () => inputs.Keyboard.IsDown(KeyCode.D) || inputs.GetGamepad(0).DpadRight.IsDown },
            { "Jump", () => inputs.Keyboard.IsPressed(KeyCode.Space) || inputs.GetGamepad(0).South.IsPressed },
        };
    }

    public bool IsPressed(string actionName)
    {
        return actions[actionName]();
    }
}

// Usage
public override void Update(TimeSpan delta)
{
    if (inputManager.IsPressed("Move_Forward"))
    {
        MoveForward();
    }

    if (inputManager.IsPressed("Jump"))
    {
        Jump();
    }
}
```

---

## Part 5: Audio and Video

### Audio System Basics

#### Audio Initialization

```csharp
class MyGame : Game
{
    public MyGame() : base(
        new AppInfo("com.mygame.studio", "MyGame"),
        new WindowCreateInfo { /* ... */ },
        new FramePacingSettings(),
        ShaderFormat.SPIRV | ShaderFormat.DXIL | ShaderFormat.MSL | ShaderFormat.DXBC
    ) { }
}
```

#### Loading and Playing Sound

```csharp
// Load sound file
AudioBuffer soundEffect = AudioDataWav.CreateBuffer(
    AudioDevice,
    RootTitleStorage,
    "Assets/Sounds/jump.wav"
);

PersistentVoice sourceVoice = PersistentVoice.Create(AudioDevice, soundEffect.Format);
sourceVoice.Submit(soundEffect);
sourceVoice.Play();
```

#### Audio Buffers and Streaming Playback

```csharp
AudioDataOgg musicData = AudioDataOgg.Create(AudioDevice);
musicData.Open(RootTitleStorage, "Assets/Music/background.ogg");

PersistentVoice musicVoice = PersistentVoice.Create(AudioDevice, musicData.Format);
musicData.SendTo(musicVoice);
musicVoice.Play();

// Volume control
musicVoice.SetVolume(0.8f);
```

#### Audio Mixing

```csharp
SubmixVoice effectsSubmix = new SubmixVoice(
    AudioDevice,
    sourceChannelCount: 2,
    sampleRate: 44100,
    processingStage: 0
);
effectsSubmix.SetVolume(0.7f);

PersistentVoice jumpSFX = PersistentVoice.Create(AudioDevice, soundEffect.Format);
jumpSFX.SetOutputVoice(effectsSubmix);
```

#### Audio Fade In/Out

```csharp
// Built-in tween on voices
sourceVoice.SetVolume(
    targetValue: 1f,
    duration: 2f,
    easingFunction: MoonWorks.Math.Easing.Function.Linear
);
```

---

### 3D Spatial Audio

#### AudioListener and AudioEmitter

```csharp
// Create listener (player/camera)
AudioListener listener = new AudioListener
{
    Position = cameraPosition,
    Forward = cameraForward,
    Up = Vector3.UnitY,
    Velocity = cameraVelocity
};

// Create emitter (object in world)
AudioEmitter emitter = new AudioEmitter
{
    Position = enemyPosition,
    Forward = Vector3.UnitZ,
    Up = Vector3.UnitY,
    Velocity = enemyVelocity
};

// Compute 3D audio parameters
AudioBuffer monoSfx = AudioDataWav.CreateBuffer(AudioDevice, RootTitleStorage, "Assets/Sounds/enemy_mono.wav");
PersistentVoice enemySound = PersistentVoice.Create(AudioDevice, monoSfx.Format);
enemySound.Submit(monoSfx, loop: true);
enemySound.Apply3D(listener, emitter);
```

---

### Video Playback and Decoding

#### Ogg Theora Video

```csharp
// Load video file
VideoAV1 video = VideoAV1.Create(
    GraphicsDevice,
    VideoDevice,
    RootTitleStorage,
    "Assets/Videos/intro.obu",
    25
);

video.Load(loop: false);
video.Play();
```

#### Video Sync

```csharp
private VideoAV1 video;

public override void Update(TimeSpan delta)
{
    video.Update(delta);
}
```

---

## Part 6: Advanced Topics

### Multi-Window Management

#### Claiming and Releasing Windows

```csharp
// Create additional windows in app
Window primaryWindow;
Window secondaryWindow;

public override void Init()
{
    primaryWindow = MainWindow;

    // Create and claim extra window
    secondaryWindow = new Window(
        new WindowCreateInfo
        {
            WindowWidth = 800,
            WindowHeight = 600,
            WindowTitle = "Editor Window",
            ScreenMode = ScreenMode.Windowed
        },
        0
    );

    GraphicsDevice.ClaimWindow(secondaryWindow);
}

public override void Draw(double alpha)
{
    // Render to main window
    CommandBuffer cmdbuf1 = GraphicsDevice.AcquireCommandBuffer();
    Texture swapchain1 = cmdbuf1.AcquireSwapchainTexture(primaryWindow);
    // render...
    GraphicsDevice.Submit(cmdbuf1);

    // Render to secondary window
    CommandBuffer cmdbuf2 = GraphicsDevice.AcquireCommandBuffer();
    Texture swapchain2 = cmdbuf2.AcquireSwapchainTexture(secondaryWindow);
    // render...
    GraphicsDevice.Submit(cmdbuf2);
}

public override void Destroy()
{
    GraphicsDevice.UnclaimWindow(secondaryWindow);
    secondaryWindow.Dispose();
}
```

---

### File Storage System

#### TitleStorage and UserStorage

```csharp
// TitleStorage: read-only file IO
if (RootTitleStorage.GetFileSize("Assets/Shaders/basic.vert", out ulong shaderSize))
{
    byte[] shaderBytes = new byte[(int)shaderSize];
    RootTitleStorage.ReadFile("Assets/Shaders/basic.vert", shaderBytes);
}
```

#### Asynchronous File Operations

```csharp
using System.Runtime.InteropServices;

CommandBuffer storageCmd = UserStorage.AcquireCommandBuffer();
ResultToken readToken = storageCmd.ReadFile("SaveGames/game.sav");
UserStorage.Submit(storageCmd);

public override void Update(TimeSpan delta)
{
    if (readToken.Result == Result.Success)
    {
        byte[] data = new byte[(int)readToken.Size];
        Marshal.Copy(readToken.Buffer, data, 0, data.Length);
        unsafe { NativeMemory.Free((void*)readToken.Buffer); }
        UserStorage.ReleaseToken(readToken);
        LoadGameData(data);
    }
}
```

---

### Performance Optimization

#### Batching

Minimize draw calls:

```csharp
// Not ideal: one draw call per object
foreach (var obj in gameObjects)
{
    renderPass.BindGraphicsPipeline(pipeline);
    renderPass.BindVertexBuffers(obj.VertexBuffer);
    renderPass.DrawPrimitives(obj.VertexCount, 1, 0, 0);
}

// Better: instancing or batching
List<Matrix4x4> transforms = new();
foreach (var obj in gameObjects)
{
    transforms.Add(obj.Transform);
}

// Use instanceCount > 1 with shared geometry
renderPass.DrawPrimitives(vertexCount, (uint)transforms.Count, 0, 0)
```

#### Buffer Reuse

```csharp
// Use a pool to reduce allocations
public class BufferPool
{
    private Stack<Buffer> availableBuffers;

    public Buffer AcquireBuffer(uint size)
    {
        if (availableBuffers.Count > 0)
        {
            return availableBuffers.Pop();
        }
        return Buffer.Create<byte>(GraphicsDevice, BufferUsageFlags.Vertex, size);
    }

    public void ReleaseBuffer(Buffer buffer)
    {
        availableBuffers.Push(buffer);
    }
}
```

#### GPU Command Optimization

```csharp
// Minimize command buffer submits
instead of:
  cmdbuf1.Submit();
  cmdbuf2.Submit();
  cmdbuf3.Submit();

do:
  CommandBuffer cmdbuf = GraphicsDevice.AcquireCommandBuffer();
  // all operations
  GraphicsDevice.Submit(cmdbuf);  // single submit
```

---

### Cross-Platform Deployment

#### Windows Deployment

```bash
# Publish release
dotnet publish -c Release -r win-x64

# Output path: bin/Release/net9.0/win-x64/publish/
```

#### Other Platforms

StarWorks includes platform-specific projects for:

- **Android**: `StarWorks.GraphicsTests.Android`
- **iOS**: `StarWorks.GraphicsTests.iOS`
- **UWP**: `StarWorks.GraphicsTests.Uwp`

#### Platform-Specific Configuration

```csharp
#if DEBUG
    const bool IsDebug = true;
#else
    const bool IsDebug = false;
#endif

#if ANDROID
    public class AndroidSpecificCode
    {
        // Android specific implementation
    }
#elif IOS
    public class iOSSpecificCode
    {
        // iOS specific implementation
    }
#endif
```

---

## Appendix A: Common Class Quick Reference

| Class | Purpose |
|----|------|
| `Game` | Base game class |
| `GraphicsDevice` | Graphics device management |
| `CommandBuffer` | GPU command buffer |
| `RenderPass` / `ComputePass` | Render/compute pass |
| `Shader` | Shader program |
| `GraphicsPipeline` | Graphics pipeline state |
| `Texture` | Texture resource |
| `Buffer` | GPU buffer |
| `Sampler` | Texture sampler |
| `Window` | Window management |
| `AudioDevice` | Audio device |
| `SourceVoice` / `SubmixVoice` | Sound source and mixing |
| `Inputs` | Input management |
| `VideoDevice` | Video decode device |
| `TitleStorage` / `UserStorage` | File storage |

---

## Appendix B: Example Code References

This tutorial is based on the following example files:

- **ClearScreenExample** - basic window and clear
- **BasicTriangleExample** - first triangle
- **TexturedQuadExample** - textures and sampling
- **CubeExample** - 3D geometry and depth
- **BasicStencilExample** - stencil buffer
- **ComputeSpriteBatchExample** - compute shader application
- **CPUSpriteBatchExample** - CPU-side sprite batching
- **VideoPlayerExample** - video playback
- **FontExample** - text rendering

---

## Appendix C: FAQ

### Q1: How can I improve rendering performance?
**A:**
- Use instancing instead of many draw calls
- Enable back-face culling
- Move heavy CPU-side work to compute shaders
- Refer to the Performance Optimization section

### Q2: How do I access textures in shaders?
**A:** Use `sampler2D` (2D) or `samplerCube` (cube map) with `texture()`:
```glsl
vec4 color = texture(MainTexture, uv);
```

### Q3: How do I handle window resize?
**A:** Listen to window events or check `Window.Width` and `Window.Height` each frame.

### Q4: What audio formats does StarWorks support?
**A:** OGG (Vorbis), WAV, QOA, and streaming audio paths.

### Q5: How do I save and load game data?
**A:** Use `UserStorage` for file read/write operations.

---

**Last Updated**: April 2, 2026
**Version**: 1.0
**Maintainer**: StarWorks Community
