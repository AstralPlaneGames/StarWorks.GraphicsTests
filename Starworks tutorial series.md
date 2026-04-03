# 🎮 StarWorks 完整教程系列

**StarWorks**（基于 MoonWorks）是一款现代跨平台游戏开发框架，提供高性能的 C# 游戏开发工具。本教程涵盖从入门到精通的全套内容。

---

## 📚 教程目录

### 第一部分：基础入门
1. [环境搭建与项目创建](#环境搭建与项目创建)
2. [Game 类基础](#game-类基础)
3. [第一个三角形](#第一个三角形)
4. [理解游戏循环](#理解游戏循环)

### 第二部分：图形系统
5. [顶点缓冲与索引](#顶点缓冲与索引)
6. [纹理系统入门](#纹理系统入门)
7. [着色器编程](#着色器编程)
8. [管道状态管理](#管道状态管理)
9. [2D 精灵渲染](#2d-精灵渲染)

### 第三部分：高级图形
10. [3D 几何体渲染](#3d-几何体渲染)
11. [深度测试与模板缓冲](#深度测试与模板缓冲)
12. [多采样反走样（MSAA）](#多采样反走样msaa)
13. [计算着色器](#计算着色器)
14. [渲染目标](#渲染目标)

### 第四部分：交互与输入
15. [键盘输入处理](#键盘输入处理)
16. [鼠标与游戏手柄](#鼠标与游戏手柄)
17. [虚拟按钮系统](#虚拟按钮系统)

### 第五部分：音视频系统
18. [音频系统基础](#音频系统基础)
19. [3D 空间音效](#3d-空间音效)
20. [视频播放与解码](#视频播放与解码)

### 第六部分：高级主题
21. [多窗口管理](#多窗口管理)
22. [文件存储系统](#文件存储系统)
23. [性能优化](#性能优化)
24. [跨平台部署](#跨平台部署)

---

## 第一部分：基础入门

### 环境搭建与项目创建

#### 系统要求
- **.NET SDK**: 9.0 或更高版本
- **操作系统**: Windows、Linux、macOS、iOS、Android
- **IDE**: Visual Studio、Visual Studio Code 或 JetBrains Rider

#### 创建新项目

```bash
# 克隆或获取 StarWorks 框架
git clone https://github.com/AstralPlaneGames/StarWorks.git

# 创建新的控制台应用项目
dotnet new console -n MyGameProject
cd MyGameProject

# 添加 StarWorks NuGet 包（如果可用）
# 或直接添加项目引用
```

#### 项目文件配置 (.csproj)

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

### Game 类基础

#### 理解 Game 基类

StarWorks 中所有游戏必须继承 `Game` 类。这是游戏的主入口点。

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
            PresentMode = PresentMode.Mailbox  // 或 Immediate、Fifo
        },
        new FramePacingSettings(),
        ShaderFormat.SPIRV | ShaderFormat.DXIL | ShaderFormat.MSL | ShaderFormat.DXBC
    )
    {
    }

    public override void Init()
    {
        // 初始化资源：加载着色器、纹理、创建缓冲区等
        Window.SetTitle("My First Game");
    }

    public override void Update(TimeSpan delta)
    {
        // 更新游戏逻辑
    }

    public override void Draw(double alpha)
    {
        // 绘制游戏：提交渲染命令
    }

    public override void Destroy()
    {
        // 清理资源
    }

    static void Main(string[] args)
    {
        var game = new MyFirstGame();
        game.Run();
    }
}
```

#### Game 类核心属性

| 属性 | 类型 | 用途 |
|------|------|------|
| `GraphicsDevice` | GraphicsDevice | 图形设备管理器，用于创建和管理图形资源 |
| `AudioDevice` | AudioDevice | 音频设备，用于播放声音 |
| `VideoDevice` | VideoDevice | 视频解码设备 |
| `Inputs` | Inputs | 输入管理系统（键盘、鼠标、手柄） |
| `MainWindow` | Window | 主游戏窗口 |
| `RootTitleStorage` | TitleStorage | 应用目录（只读资源） |
| `UserStorage` | UserStorage | 用户文档目录（可读写） |

---

### 第一个三角形

#### 最小化渲染示例

要渲染三角形，需要：
1. 着色器程序（顶点 + 片段）
2. 图形管道
3. 顶点数据或缓冲区

#### 编写着色器

**顶点着色器** (triangle.vert, 使用 HLSL)：
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

**片段着色器** (triangle.frag)：
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

#### C# 代码实现

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
        // 加载着色器
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

        // 创建管道
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

        // 创建顶点数据
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

        // 上传顶点数据到 GPU
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

        // 索引缓冲
        uint[] indices = new uint[] { 0, 1, 2 };
        IndexBuffer = Buffer.Create<uint>(
            GraphicsDevice,
            BufferUsageFlags.Index,
            3
        );

        // 上传索引数据（过程同上）
        // 为简洁省略...
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

### 理解游戏循环

在 StarWorks 中，游戏循环由 `Run()` 方法自动管理。

```
Run()
  ├─ Init() - 初始化一次
  └─ Loop:
      ├─ IsRunning 检查
      ├─ Update(deltaTime) - 逻辑更新
      ├─ Draw(alpha) - 渲染
      └─ 帧同步 (使用 PresentMode)
```

#### 关键参数解析

**PresentMode** - 屏幕刷新方式：
- `Mailbox`: 不等待垂直同步，最小延迟（推荐全屏游戏）
- `Fifo`: 锁定到屏幕刷新率（60Hz、120Hz 等）
- `Immediate`: 立即呈现，可能撕裂（基准测试用）

**FrameLimiterSettings** - 帧率限制：
```csharp
new FramePacingSettings
{
    Mode = FramePacingMode.Capped,
    TargetFrameRate = 60  // 限制到 60FPS
}
```

---

## 第二部分：图形系统

### 顶点缓冲与索引

#### 为什么需要缓冲区

在 GPU 渲染中，顶点数据必须存储在显存中，而不是 CPU 内存。缓冲区是 GPU 显存的抽象。

#### 顶点结构定义

```csharp
using System.Numerics;

struct PositionColorVertex : IVertexType
{
    public Vector3 Position;      // 坐标
    public Vector4 Color;         // RGBA 颜色

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

#### 创建顶点缓冲

```csharp
// 定义顶点数据
PositionColorVertex[] vertices = new PositionColorVertex[]
{
    new() { Position = new Vector3(0, 1, 0), Color = new Vector4(1, 0, 0, 1) },
    new() { Position = new Vector3(1, -1, 0), Color = new Vector4(0, 1, 0, 1) },
    new() { Position = new Vector3(-1, -1, 0), Color = new Vector4(0, 0, 1, 1) }
};

// 步骤 1：创建 TransferBuffer（CPU→GPU 中转）
TransferBuffer uploadBuffer = TransferBuffer.Create<PositionColorVertex>(
    GraphicsDevice,
    TransferBufferUsage.Upload,
    vertices.Length
);

// 步骤 2：映射内存并拷贝数据
var span = uploadBuffer.Map<PositionColorVertex>(false);
vertices.CopyTo(span);
uploadBuffer.Unmap();

// 步骤 3：创建实际的 GPU 缓冲区
Buffer vertexBuffer = Buffer.Create<PositionColorVertex>(
    GraphicsDevice,
    BufferUsageFlags.Vertex,
    vertices.Length
);

// 步骤 4：提交拷贝命令到 GPU
CommandBuffer cmdbuf = GraphicsDevice.AcquireCommandBuffer();
CopyPass copyPass = cmdbuf.BeginCopyPass();
copyPass.UploadToBuffer(uploadBuffer, vertexBuffer, false);
cmdbuf.EndCopyPass(copyPass);
GraphicsDevice.Submit(cmdbuf);

uploadBuffer.Dispose();
```

#### 索引缓冲

索引缓冲允许重新使用顶点数据，减少内存占用。

```csharp
// 16 个顶点定义的立方体
PositionColorVertex[] cubeVertices = new PositionColorVertex[24];

// 索引：6 个面，每个面 2 个三角形（6 个索引）
uint[] cubeIndices = new uint[]
{
    // 前面
    0, 1, 2,    2, 3, 0,
    // 背面
    4, 6, 5,    6, 7, 5,
    // 左面
    8, 9, 10,   10, 11, 8,
    // 右面
    12, 14, 13, 14, 15, 13,
    // 顶面
    16, 18, 17, 18, 19, 17,
    // 底面
    20, 21, 22, 22, 23, 20
};

// 创建索引缓冲
Buffer indexBuffer = Buffer.Create<uint>(
    GraphicsDevice,
    BufferUsageFlags.Index,
    cubeIndices.Length
);

// 上传索引（过程同顶点缓冲）
```

#### 使用缓冲区发送渲染命令

```csharp
RenderPass renderPass = cmdbuf.BeginRenderPass(
    new ColorTargetInfo(swapchainTexture, Color.Black)
);

renderPass.BindGraphicsPipeline(pipeline);
renderPass.BindVertexBuffers(vertexBuffer, 0);  // 绑定顶点缓冲
renderPass.BindIndexBuffer(indexBuffer, IndexElementSize.ThirtyTwo);  // 绑定索引缓冲

// 绘制命令
// 参数：(索引数量, 实例数, 索引偏移, 顶点偏移, 实例偏移)
renderPass.DrawIndexedPrimitives(36, 1, 0, 0, 0);

cmdbuf.EndRenderPass(renderPass);
```

---

### 纹理系统入门

#### 纹理基础

纹理是存储在 GPU 上的图像数据，用于表面细节、光照等。

#### 加载纹理

```csharp
// 方法 1：从文件加载
ResourceUploader uploader = new ResourceUploader(GraphicsDevice);

Texture textureFromPNG = uploader.CreateTexture2DFromCompressed(
    RootTitleStorage,
    "Assets/Textures/wood.png",
    TextureFormat.R8G8B8A8Unorm,
    TextureUsageFlags.Sampler
);

uploader.Upload();
uploader.Dispose();

// 方法 2：创建空白纹理
Texture renderTarget = Texture.Create2D(
    GraphicsDevice,
    1024,
    768,
    TextureFormat.R8G8B8A8Unorm,
    TextureUsageFlags.ColorTarget
);
```

#### 采样器

采样器定义纹理在 GPU 上的采样行为。

```csharp
// 点采样（最近邻）
Sampler pointSampler = Sampler.Create(
    GraphicsDevice,
    SamplerCreateInfo.PointClamp
);

// 线性采样（双线性）
Sampler linearSampler = Sampler.Create(
    GraphicsDevice,
    SamplerCreateInfo.LinearClamp
);

// 各向异性采样
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

#### 绑定纹理和采样器

```csharp
// 在渲染通道中
renderPass.BindFragmentSamplers(
    new TextureSamplerBinding(myTexture, mySampler)
);
```

#### 纹理寻址模式

```csharp
// 各种寻址模式
SamplerAddressMode.Clamp,        // 边界夹紧
SamplerAddressMode.Wrap,         // 重复
SamplerAddressMode.MirroredWrap, // 镜像重复
SamplerAddressMode.ClampToEdge,  // 边界颜色
```

---

### 着色器编程

#### 着色器类型

StarWorks 支持 GLSL（SPIR-V）和 HLSL。

#### 基础顶点着色器

```glsl
// 顶点着色器示例（GLSL）
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

#### 基础片段着色器

```glsl
// 片段着色器示例
#version 450

layout(location = 0) in vec2 inTexCoord;
layout(set = 1, binding = 0) uniform sampler2D MainTexture;

layout(location = 0) out vec4 FragColor;

void main()
{
    FragColor = texture(MainTexture, inTexCoord);
}
```

#### 加载着色器

```csharp
Shader vertShader = Shader.Create(
    GraphicsDevice,
    RootTitleStorage,
    "BasicTexturedQuad.vert",  // 文件路径
    "main",                     // 入口点
    ShaderCross.ShaderFormat.SPIRV,  // 格式
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

#### Uniform 数据传递

```csharp
// 顶点着色器 Uniform
Matrix4x4 viewOrthogonal = Matrix4x4.CreateOrthographicOffCenter(
    0, 1280, 720, 0, 0, -1
);

Texture swapchainTexture = cmdbuf.AcquireSwapchainTexture(MainWindow);
RenderPass renderPass = cmdbuf.BeginRenderPass(
    new ColorTargetInfo(swapchainTexture, Color.Black)
);
renderPass.BindGraphicsPipeline(pipeline);
cmdbuf.PushVertexUniformData(viewOrthogonal);  // 推送数据
renderPass.DrawPrimitives(3, 1, 0, 0);

// 片段着色器 Uniform（需要设置特定的 binding slot）
```

---

### 管道状态管理

#### GraphicsPipeline 创建

```csharp
GraphicsPipelineCreateInfo pipelineInfo = new GraphicsPipelineCreateInfo
{
    // 输出纹理格式
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

    // 着色器
    VertexShader = vertShader,
    FragmentShader = fragShader,

    // 顶点输入布局
    VertexInputState = VertexInputState.CreateSingleBinding<PositionColorVertex>(),

    // 拓扑结构
    PrimitiveType = PrimitiveType.TriangleList,

    // 光栅化状态
    RasterizerState = new RasterizerState
    {
        CullMode = CullMode.Back,              // 背面剔除
        FrontFace = FrontFace.CounterClockwise,
        FillMode = FillMode.Fill               // 填充模式
    },

    // 深度/模板测试
    DepthStencilState = new DepthStencilState
    {
        EnableDepthTest = true,
        EnableDepthWrite = true,
        CompareOp = CompareOp.LessOrEqual
    },

    // 多采样
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

#### 常见管道配置

```csharp
// 不透明物体管道
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

// 透明物体管道（关闭深度写入）
var transparentPipelineInfo = new GraphicsPipelineCreateInfo
{
    DepthStencilState = new DepthStencilState
    {
        EnableDepthTest = true,
        EnableDepthWrite = false,  // 不写入深度
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

### 2D 精灵渲染

#### 精灵结构

```csharp
struct Sprite
{
    public Vector2 Position;
    public float Rotation;
    public Vector2 Scale;
    public Vector4 Color;
    public Rectangle SourceRect;  // 纹理坐标
}
```

#### CPU 精灵批处理

这是最灵活但最慢的方法。参考 `CPUSpriteBatchExample.cs`。

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

// 在每帧 Draw 中：
for (int i = 0; i < SPRITE_COUNT; i++)
{
    // 更新精灵变换矩阵
    Matrix4x4 transform =
        Matrix4x4.CreateScale(spriteData[i].Size.X, spriteData[i].Size.Y, 1) *
        Matrix4x4.CreateRotationZ(spriteData[i].Rotation) *
        Matrix4x4.CreateTranslation(spriteData[i].Position);

    // 根据变换生成顶点
    // ...
}
```

#### GPU 精灵批处理（使用计算着色器）

更高效的方法是使用计算着色器处理。参考 `ComputeSpriteBatchExample.cs`。

```csharp
// 使用 ComputePipeline 在 GPU 上处理精灵变换
ComputePipeline computePipeline = ShaderCross.Create(
    GraphicsDevice,
    RootTitleStorage,
    "SpriteTransform.comp",
    "main",
    ShaderCross.ShaderFormat.SPIRV
);
ComputePass computePass = cmdbuf.BeginComputePass();
computePass.BindComputePipeline(computePipeline);
// 绑定数据和执行计算...
cmdbuf.EndComputePass(computePass);
```

---

## 第三部分：高级图形

### 3D 几何体渲染

#### 立方体生成

```csharp
void GenerateCubeVertices(
    out PositionColorVertex[] vertices,
    out uint[] indices)
{
    vertices = new PositionColorVertex[24];
    indices = new uint[36];

    // 前面（z=1）
    vertices[0] = new PositionColorVertex(-1, -1, 1);
    vertices[1] = new PositionColorVertex(1, -1, 1);
    vertices[2] = new PositionColorVertex(1, 1, 1);
    vertices[3] = new PositionColorVertex(-1, 1, 1);

    // 背面（z=-1）
    vertices[4] = new PositionColorVertex(-1, -1, -1);
    vertices[5] = new PositionColorVertex(-1, 1, -1);
    vertices[6] = new PositionColorVertex(1, 1, -1);
    vertices[7] = new PositionColorVertex(1, -1, -1);

    // ... 其他 4 个面

    // 索引定义三角形
    indices = new uint[]
    {
        // 前面
        0, 1, 2, 2, 3, 0,
        // 背面
        4, 6, 5, 6, 7, 4,
        // ... 其他面
    };
}
```

#### 矩阵变换

```csharp
// 模型矩阵（模型空间→世界空间）
Matrix4x4 modelMatrix = Matrix4x4.CreateTranslation(new Vector3(0, 0, 5)) *
                        Matrix4x4.CreateRotationY((float)gameTime.TotalGameTime.TotalSeconds);

// 视矩阵（世界空间→摄像机空间）
Vector3 cameraPosition = new Vector3(0, 2, -5);
Vector3 targetPosition = Vector3.Zero;
Vector3 upDirection = Vector3.UnitY;
Matrix4x4 viewMatrix = Matrix4x4.CreateLookAt(
    cameraPosition,
    targetPosition,
    upDirection
);

// 投影矩阵（摄像机空间→裁剪空间）
float fov = MathHelper.PiOver4;
float aspectRatio = 1280f / 720f;
Matrix4x4 projMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
    fov,
    aspectRatio,
    0.1f,      // 近平面
    1000f      // 远平面
);

// 组合到 ViewProj
Matrix4x4 viewProj = modelMatrix * viewMatrix * projMatrix;
cmdbuf.PushVertexUniformData(viewProj);
```

#### 立方体贴图（包括天空盒）

```csharp
// 加载立方体贴图
Texture cubemap = resourceUploader.CreateTextureCubeFromCompressed(
    RootTitleStorage,
    "Assets/Textures/skybox",  // 目录包含 posx.png, negx.png, posy.png 等
    TextureFormat.R8G8B8A8Unorm,
    TextureUsageFlags.Sampler
);

// 在片段着色器中采样
// vec3 direction = normalize(vertexPosition);
// vec4 color = texture(cubemapSampler, direction);
```

---

### 深度测试与模板缓冲

#### 深度缓冲

```csharp
// 创建带深度的渲染目标
Texture depthTexture = Texture.Create2D(
    GraphicsDevice,
    1280,
    720,
    TextureFormat.D32Float,
    TextureUsageFlags.DepthStencilTarget
);

// 在管道中指定深度格式
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
        CompareOp = CompareOp.LessOrEqual  // 通常用 Less 或 LessOrEqual
    }
};

// 渲染时指定深度附件
RenderPass renderPass = cmdbuf.BeginRenderPass(
    new DepthStencilTargetInfo(depthTexture, 1f),
    new ColorTargetInfo(swapchainTexture, Color.Black)
);
```

#### 模板测试（模板缓冲）

模板缓冲用于实现阴影、剪影等效果。

```csharp
// 管道中启用模板操作
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

#### 深度采样

在着色器中读取深度值：

```glsl
#version 450

layout(set = 1, binding = 0) uniform sampler2D DepthTexture;

void main()
{
    float depth = texture(DepthTexture, uv).r;
    // 使用深度值进行后期处理
}
```

---

### 多采样反走样（MSAA）

#### MSAA 配置

```csharp
// MSAA 纹理
Texture msaaTexture = Texture.Create2D(
    GraphicsDevice,
    1280,
    720,
    TextureFormat.R8G8B8A8Unorm,
    TextureUsageFlags.ColorTarget,
    sampleCount: SampleCount.Eight  // 8 倍采样
);

// 管道配置
GraphicsPipelineCreateInfo pipelineInfo = new GraphicsPipelineCreateInfo
{
    MultisampleState = new MultisampleState
    {
        SampleCount = SampleCount.Eight,
        SampleMask = 0xffffffff
    },
    // ...
};

// 渲染
RenderPass renderPass = cmdbuf.BeginRenderPass(
    new ColorTargetInfo(msaaTexture, Color.Black)
);
```

#### MSAA 与深度

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

参考示例：`DepthMSAAExample.cs`

---

### 计算着色器

#### 计算管道基础

计算着色器在 GPU 上执行并行计算任务。

```glsl
#version 450

layout(local_size_x = 16, local_size_y = 16) in;

layout(set = 0, binding = 0, rgba8) uniform image2D TargetTexture;

void main()
{
    ivec2 pixelCoord = ivec2(gl_GlobalInvocationID.xy);
    vec4 pixelColor = vec4(1.0, 0.0, 0.0, 1.0);  // 红色
    imageStore(TargetTexture, pixelCoord, pixelColor);
}
```

#### 在 C# 中创建计算管道

```csharp
ComputePipeline computePipeline = ShaderCross.Create(
    GraphicsDevice,
    RootTitleStorage,
    "ComputeShader.comp",
    "main",
    ShaderCross.ShaderFormat.SPIRV
);

// 创建目标纹理
Texture targetTexture = Texture.Create2D(
    GraphicsDevice,
    1024,
    1024,
    TextureFormat.R8G8B8A8Unorm,
    TextureUsageFlags.ComputeStorageWrite
);

// 执行计算
CommandBuffer cmdbuf = GraphicsDevice.AcquireCommandBuffer();
ComputePass computePass = cmdbuf.BeginComputePass();

computePass.BindComputePipeline(computePipeline);
computePass.BindStorageTextures(targetTexture);
computePass.Dispatch(1024 / 16, 1024 / 16, 1);  // 分组数

cmdbuf.EndComputePass(computePass);
GraphicsDevice.Submit(cmdbuf);
```

参考示例：`BasicComputeExample.cs`、`ComputeSpriteBatchExample.cs`

---

### 渲染目标

#### 离屏渲染

```csharp
// 创建渲染纹理（2D）
Texture renderTexture2D = Texture.Create2D(
    GraphicsDevice,
    512,
    512,
    TextureFormat.R8G8B8A8Unorm,
    TextureUsageFlags.ColorTarget | TextureUsageFlags.Sampler
);

// 渲染到纹理
RenderPass renderPass = cmdbuf.BeginRenderPass(
    new ColorTargetInfo(renderTexture2D, Color.Black)
);

renderPass.BindGraphicsPipeline(pipeline);
renderPass.DrawPrimitives(3, 1, 0, 0);

cmdbuf.EndRenderPass(renderPass);

// 后续可以采样 renderTexture2D 作为普通纹理使用
```

#### 立方体贴图渲染

生成程序化的立方体贴图（如环境贴图）：

```csharp
// 创建立方体贴图
Texture cubemap = Texture.CreateCube(
    GraphicsDevice,
    512,
    TextureFormat.R8G8B8A8Unorm,
    TextureUsageFlags.ColorTarget | TextureUsageFlags.Sampler
);

// 为立方体的 6 个面分别渲染
for (int i = 0; i < 6; i++)
{
    var colorTargetInfo = new ColorTargetInfo(cubemap, Color.Black)
    {
        LayerOrDepthPlane = (uint)i
    };

    RenderPass renderPass = cmdbuf.BeginRenderPass(
        colorTargetInfo
    );
    // 渲染该面...
    cmdbuf.EndRenderPass(renderPass);
}
```

参考示例：`RenderTextureCubeExample.cs`

---

## 第四部分：交互与输入

### 键盘输入处理

#### 基础按键检测

```csharp
public override void Update(TimeSpan delta)
{
    // 检查特定按键
    if (Inputs.Keyboard.IsPressed(KeyCode.Escape))
    {
        Exit();  // 退出游戏
    }

    // WASD 移动
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

#### 文本输入事件

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

### 鼠标与游戏手柄

#### 鼠标输入

```csharp
public override void Update(TimeSpan delta)
{
    // 获取鼠标位置（相对于窗口）
    int x = Inputs.Mouse.X;
    int y = Inputs.Mouse.Y;

    // 滚轮
    int wheelDelta = Inputs.Mouse.Wheel;
    if (wheelDelta > 0)
    {
        cameraDistance -= zoomSpeed * (float)delta.TotalSeconds;
    }
    else if (wheelDelta < 0)
    {
        cameraDistance += zoomSpeed * (float)delta.TotalSeconds;
    }

    // 按钮
    if (Inputs.Mouse.LeftButton.IsPressed)
    {
        // 左键按下
    }

    if (Inputs.Mouse.RightButton.IsPressed)
    {
        // 右键按下
    }

    // 相对鼠标模式（FPS 游戏）
    Inputs.Mouse.SetRelativeMode(MainWindow, true);
    int relX = Inputs.Mouse.DeltaX;
    int relY = Inputs.Mouse.DeltaY;
}
```

#### 游戏手柄输入

```csharp
public override void Update(TimeSpan delta)
{
    // 检查连接的游戏手柄
    if (!Inputs.GamepadExists(0))
    {
        return;  // 手柄 0 未连接
    }

    var gamepad = Inputs.GetGamepad(0);

    // 摇杆
    float leftStickX = gamepad.LeftX.Value;
    float leftStickY = gamepad.LeftY.Value;
    
    // 应用死区
    if (MathF.Abs(leftStickX) < 0.1f) leftStickX = 0;
    if (MathF.Abs(leftStickY) < 0.1f) leftStickY = 0;

    // 触发器
    float leftTrigger = gamepad.TriggerLeft.Value;
    float rightTrigger = gamepad.TriggerRight.Value;

    // 按钮
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

### 虚拟按钮系统

虚拟按钮允许键盘、鼠标和手柄按钮映射到逻辑操作。

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

// 使用
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

## 第五部分：音视频系统

### 音频系统基础

#### 初始化音频

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

#### 加载和播放声音

```csharp
// 加载 WAV 音频
AudioBuffer soundEffect = AudioDataWav.CreateBuffer(
    AudioDevice,
    RootTitleStorage,
    "Assets/Sounds/jump.wav"
);

PersistentVoice sourceVoice = PersistentVoice.Create(AudioDevice, soundEffect.Format);
sourceVoice.Submit(soundEffect);
sourceVoice.Play();
```

#### 音频缓冲和流式播放

```csharp
AudioDataOgg musicData = AudioDataOgg.Create(AudioDevice);
musicData.Open(RootTitleStorage, "Assets/Music/background.ogg");

PersistentVoice musicVoice = PersistentVoice.Create(AudioDevice, musicData.Format);
musicData.SendTo(musicVoice);
musicVoice.Play();

// 音量控制
musicVoice.SetVolume(0.8f);
```

#### 音效混音

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

#### 音频淡入淡出

```csharp
// 使用 Voice 内建的补间
sourceVoice.SetVolume(
    targetValue: 1f,
    duration: 2f,
    easingFunction: MoonWorks.Math.Easing.Function.Linear
);
```

---

### 3D 空间音效

#### AudioListener 和 AudioEmitter

```csharp
// 创建监听器（玩家/摄像机位置）
AudioListener listener = new AudioListener
{
    Position = cameraPosition,
    Forward = cameraForward,
    Up = Vector3.UnitY,
    Velocity = cameraVelocity
};

// 创建声源（游戏中的物体）
AudioEmitter emitter = new AudioEmitter
{
    Position = enemyPosition,
    Forward = Vector3.UnitZ,
    Up = Vector3.UnitY,
    Velocity = enemyVelocity
};

// 计算并应用 3D 音效参数
AudioBuffer monoSfx = AudioDataWav.CreateBuffer(AudioDevice, RootTitleStorage, "Assets/Sounds/enemy_mono.wav");
PersistentVoice enemySound = PersistentVoice.Create(AudioDevice, monoSfx.Format);
enemySound.Submit(monoSfx, loop: true);
enemySound.Apply3D(listener, emitter);
```

---

### 视频播放与解码

#### Ogg Theora 视频

```csharp
// 加载视频文件
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

#### 视频同步

```csharp
private double videoTime = 0;
private VideoAV1 video;

public override void Update(TimeSpan delta)
{
    video.Update(delta);
}
```

---

## 第六部分：高级主题

### 多窗口管理

#### 声明和取消声明窗口

```csharp
// 在应用程序中创建额外窗口
Window primaryWindow;
Window secondaryWindow;

public override void Init()
{
    primaryWindow = MainWindow;

    // 创建并声明额外窗口
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
    // 渲染到主窗口
    CommandBuffer cmdbuf1 = GraphicsDevice.AcquireCommandBuffer();
    Texture swapchain1 = cmdbuf1.AcquireSwapchainTexture(primaryWindow);
    // 渲染...
    GraphicsDevice.Submit(cmdbuf1);

    // 渲染到辅窗口
    CommandBuffer cmdbuf2 = GraphicsDevice.AcquireCommandBuffer();
    Texture swapchain2 = cmdbuf2.AcquireSwapchainTexture(secondaryWindow);
    // 渲染...
    GraphicsDevice.Submit(cmdbuf2);
}

public override void Destroy()
{
    GraphicsDevice.UnclaimWindow(secondaryWindow);
    secondaryWindow.Dispose();
}
```

---

### 文件存储系统

#### TitleStorage 和 UserStorage

```csharp
// TitleStorage：只读文件读取
if (RootTitleStorage.GetFileSize("Assets/Shaders/basic.vert", out ulong shaderSize))
{
    byte[] shaderBytes = new byte[(int)shaderSize];
    RootTitleStorage.ReadFile("Assets/Shaders/basic.vert", shaderBytes);
}
```

#### 异步文件操作

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

### 性能优化

#### 批处理

最小化 Draw Call：

```csharp
// 不好：每个对象一个 Draw Call
foreach (var obj in gameObjects)
{
    renderPass.BindGraphicsPipeline(pipeline);
    renderPass.BindVertexBuffers(obj.VertexBuffer);
    renderPass.DrawPrimitives(obj.VertexCount, 1, 0, 0);  // 多次调用
}

// 好：使用实例化或批处理
// 将多个对象的数据打包到单个缓冲区，使用一个 Draw Call
List<Matrix4x4> transforms = new();
foreach (var obj in gameObjects)
{
    transforms.Add(obj.Transform);
}

// 使用 InstanceCount > 1 和 Uniform Buffer
renderPass.DrawPrimitives(vertexCount, (uint)transforms.Count, 0, 0)
```

#### 缓冲区复用

```csharp
// 使用对象池减少分配
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

#### GPU 命令优化

```csharp
// 最小化命令缓冲提交
instead of:
  cmdbuf1.Submit();
  cmdbuf2.Submit();
  cmdbuf3.Submit();

do:
  CommandBuffer cmdbuf = GraphicsDevice.AcquireCommandBuffer();
  // 所有操作
  GraphicsDevice.Submit(cmdbuf);  // 一次提交
```

---

### 跨平台部署

#### Windows 部署

```bash
# 发布版本
dotnet publish -c Release -r win-x64

# 输出位置：bin/Release/net9.0/win-x64/publish/
```

#### 其他平台

StarWorks 支持以下平台特定项目：

- **Android**: `StarWorks.GraphicsTests.Android`
- **iOS**: `StarWorks.GraphicsTests.iOS`
- **UWP**: `StarWorks.GraphicsTests.Uwp`

#### 平台相关配置

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

## 附录 A：常用类速查表

| 类 | 用途 |
|----|------|
| `Game` | 游戏基类 |
| `GraphicsDevice` | 图形设备管理 |
| `CommandBuffer` | GPU 命令缓冲 |
| `RenderPass` / `ComputePass` | 渲染/计算通道 |
| `Shader` | 着色器程序 |
| `GraphicsPipeline` | 图形管道状态 |
| `Texture` | 纹理资源 |
| `Buffer` | GPU 缓冲区 |
| `Sampler` | 纹理采样器 |
| `Window` | 窗口管理 |
| `AudioDevice` | 音频设备 |
| `SourceVoice` / `SubmixVoice` | 音源和混音 |
| `Inputs` | 输入管理 |
| `VideoDevice` | 视频解码 |
| `TitleStorage` / `UserStorage` | 文件存储 |

---

## 附录 B：示例代码参考

本教程基于以下示例代码：

- **ClearScreenExample** - 基础窗口和清屏
- **BasicTriangleExample** - 第一个三角形
- **TexturedQuadExample** - 纹理和采样
- **CubeExample** - 3D 几何和深度
- **BasicStencilExample** - 模板缓冲
- **ComputeSpriteBatchExample** - 计算着色器应用
- **CPUSpriteBatchExample** - CPU 端精灵渲染
- **VideoPlayerExample** - 视频播放
- **FontExample** - 文本渲染

---

## 附录 C：常见问题 (FAQ)

### Q1: 如何提高渲染性能？
**A:** 
- 使用实例化而不是多个 Draw Call
- 启用背面剔除
- 使用计算着色器代替 CPU 端处理
- 参考：性能优化部分

### Q2: 如何在着色器中访问纹理？
**A:** 使用 `sampler2D`（2D）或 `samplerCube`（立方体）和 `texture()` 函数：
```glsl
vec4 color = texture(MainTexture, uv);
```

### Q3: 如何处理窗口大小变化？
**A:** 监听窗口事件或在每帧检查 `Window.Width` 和 `Window.Height`。

### Q4: StarWorks 支持哪些音频格式？
**A:** OGG (Vorbis)、WAV、QOA、流式音频。

### Q5: 如何保存和加载游戏数据？
**A:** 使用 `UserStorage` 进行文件读写操作。

---

**最后更新**: 2026年4月1日  
**版本**: 1.0  
**维护者**: StarWorks 社区

