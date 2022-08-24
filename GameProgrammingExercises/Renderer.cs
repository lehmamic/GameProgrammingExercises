using GameProgrammingExercises.Maths;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace GameProgrammingExercises;

public class Renderer : IDisposable
{
    private readonly Game _game;

    // All the sprite components drawn
    private readonly List<SpriteComponent> _sprites = new();

    // All mesh components drawn
    private readonly List<MeshComponent> _meshComps = new();

    // Map of textures loaded
    private readonly Dictionary<string, Texture> _textures = new();
    
    // Map of meshes loaded
    private readonly Dictionary<string, Mesh> _meshes = new();

    // Mesh Shader
    private Shader _meshShader;

    // Sprite Shader
    private Shader _spriteShader;
    
    // Sprite vertex array
    private VertexArrayObject _spriteVertices;

    public Renderer(Game game)
    {
        _game = game;
    }

    public IWindow Window { get; private set; }

    public GL GL { get; private set; }

    // Width/height of screen
    public float ScreenWidth { get; private set; }

    public float ScreenHeight { get; private set; }


    // View/projection for 3D shaders
    public Matrix4X4<float> ViewMatrix { get; set; }

    public Matrix4X4<float> ProjectionMatrix { get; set; }

    public IWindow Initialize(float screenWidth, float screenHeight, string windowTitle)
    {
        ScreenWidth = screenWidth;
        ScreenHeight = screenHeight;
        
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(1024, 768);
        options.Title = windowTitle;
        options.PreferredDepthBufferBits = 32;

        Window = Silk.NET.Windowing.Window.Create(options);

        Window.Load += () =>
        {
            // Getting the opengl api for drawing to the screen.
            GL = GL.GetApi(Window);

            // Make sure we can load and compile shaders
            LoadShaders();

            // Create quad for drawing sprites
            CreateSpriteVertices();
        };

        return Window;
    }

    public void Draw()
    {
        // Set the clear color to grey
        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

        // Clear the color buffer
        GL.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

        /*
         * Draw all mesh components
         */

        // Enable depth buffer/disable alpha blend
        GL.Enable(EnableCap.DepthTest);
        GL.Disable(EnableCap.Blend);

        // Set the basic mesh shader active
        _meshShader.SetActive();

        // Update view-projection matrix
        _meshShader.SetUniform("uViewProj", ViewMatrix * ProjectionMatrix);

        // Update lighting uniforms
        // SetLightUniforms(mMeshShader);

        // Draw all meshs
        foreach (var mesh in _meshComps)
        {
            mesh.Draw(_meshShader);
        }

        /*
         * Draw all sprite components
         */

        // Enable alpha blending on the color buffer,/disable depth buffering
        GL.Disable(EnableCap.DepthTest);
        GL.Enable(GLEnum.Blend);
        GL.BlendEquationSeparate(BlendEquationModeEXT.FuncAdd, BlendEquationModeEXT.FuncAdd);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        // Set sprite shader and vertex array objects active
        _spriteShader.SetActive();
        _spriteVertices.SetActive();

        // Draw all sprites
        foreach (var sprite in _sprites)
        {
            sprite.Draw(_spriteShader);
        }

        // Swap the buffers (No need Silk.Net does it for us)
        // SDL_GL_SwapWindow(mWindow);
    }
    
    public void AddSprite(SpriteComponent sprite)
    {
        // Find the insertion point in the sorted vector
        // (The first element with a order higher than me)
        int index = 0;
        for (; index < _sprites.Count; index++)
        {
            if (sprite.DrawOrder < _sprites[index].DrawOrder)
            {
                break;
            }
        }

        // Inserts element before position of iterator
        _sprites.Insert(index, sprite);
    }

    public void RemoveSprite(SpriteComponent sprite)
    {
        _sprites.Remove(sprite);
    }
    
    public void AddMeshComp(MeshComponent mesh)
    {
        _meshComps.Add(mesh);
    }

    public void RemoveMeshComp(MeshComponent mesh)
    {
        _meshComps.Remove(mesh);
    }

    public Texture GetTexture(string fileName)
    {
        if (!_textures.ContainsKey(fileName))
        {
            var texture = new Texture(GL, fileName);
            _textures.Add(fileName, texture);
        }

        return _textures[fileName];
    }

    public Mesh GetMesh(string fileName)
    {
        if (!_meshes.ContainsKey(fileName))
        {
            var mesh = Mesh.Load(fileName, _game);
            _meshes.Add(fileName, mesh);
        }

        return _meshes[fileName];
    }

    public void Dispose()
    {
        // Destroy textures
        foreach (var texture in _textures)
        {
            _textures.Remove(texture.Key);
            texture.Value.Dispose();
        }

        // Destroy meshes
        foreach (var mesh in _meshes)
        {
            _meshes.Remove(mesh.Key);
            mesh.Value.Dispose();
        }
        
        _spriteShader.Dispose();
        _spriteVertices.Dispose();
        GL.Dispose();
    }

    private void LoadShaders()
    {
        // Create sprite shader
        _spriteShader = new Shader(GL, "Shaders/Sprite.vert", "Shaders/Sprite.frag");
        _spriteShader.SetActive();

        // Set the view-projection matrix
        Matrix4X4<float> viewProj = GameMath.CreateSimpleViewProj(ScreenWidth, ScreenHeight);
        _spriteShader.SetUniform("uViewProj", viewProj);

        // Create basic mesh shader
        _meshShader = new Shader(GL, "Shaders/BasicMesh.vert", "Shaders/BasicMesh.frag");
        _meshShader.SetActive();

        // Set the view-projection matrix
        ViewMatrix = GameMath.CreateLookAt(
            Vector3D<float>.Zero,      // Camera position
            Vector3D<float>.UnitX,     // target position
            Vector3D<float>.UnitZ);    // Up

        ProjectionMatrix = GameMath.CreatePerspectiveFieldOfView(
            Scalar.DegreesToRadians(70.0f), // Horizontal FOV
            ScreenWidth,
            ScreenHeight,
            25.0f,                  // Near plane
            10000.0f);              // Far plane

        _meshShader.SetUniform("uViewProj", ViewMatrix * ProjectionMatrix);
    }

    private void CreateSpriteVertices()
    {
        var vertices = new[] {
            // vertex(3)/normal(3)/(uv coord)
            -0.5f, 0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, // top left
            0.5f, 0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, // top right
            0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f  // bottom left
        };

        var indices = new uint[] {
            0, 1, 2,
            2, 3, 0
        };

        _spriteVertices = new VertexArrayObject(GL, vertices, indices);
    }
}