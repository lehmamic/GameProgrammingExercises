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
    private readonly List<SkeletalMeshComponent> _skeletalMeshes = new();

    // Map of textures loaded
    private readonly Dictionary<string, Texture> _textures = new();
    
    // Map of meshes loaded
    private readonly Dictionary<string, Mesh> _meshes = new();

    // Mesh Shader
    private Shader _meshShader;

    // Skinned Shader
    private Shader _skinnedShader;

    // Sprite Shader
    private Shader _spriteShader;

    // Text Shader
    private Shader _textShader;

    // Sprite vertex array
    private VertexArrayObject _spriteVertices;

    // Glyph vertex array
    private VertexArrayObject _glyphVertices;

    // Framebuffer object for the mirror 
    private FrameBufferObject? _mirrorBuffer;

    // Texture for the mirror
    private Texture? _mirrorTexture;
    
    // GBuffer
    private GBuffer _gBuffer;
    private Shader _gBufferGlobalShader;

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

    public Matrix4X4<float> MirrorViewMatrix { get; set; }

    public Matrix4X4<float> ProjectionMatrix { get; set; }


    // Lighting data
    public Vector3D<float> AmbientLight { get; set; }

    public DirectionalLight DirectionalLight { get; set; }

    // Mirror
    public Texture? MirrorTexture => _mirrorTexture;

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

            // Disable 4-byte texture alignment for teh font rendering
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            // Make sure we can load and compile shaders
            LoadShaders();

            // Create quad for drawing sprites
            CreateSpriteVertices();

            // Create quad for text glyphs
            CreateGlyphVertices();

            // Create render target for mirror
            CreateMirrorTarget();
            
            // Create G-buffer
            _gBuffer = GBuffer.Create(GL, (int) ScreenWidth, (int) ScreenHeight);
        };

        return Window;
    }

    public void Draw()
    {
        /*
         * Draw 3d scene to the frame buffers
         */

        // Draw to the mirror texture first
        Draw3DScene(_mirrorBuffer!.FrameBufferId, MirrorViewMatrix, ProjectionMatrix, 0.25f, false);
        // Draw the 3D scene to the G-buffer
        Draw3DScene(_gBuffer.FrameBufferId, ViewMatrix, ProjectionMatrix, 1.0f, false);

        // Set the frame buffer back to zero (screen's frame buffer)
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        // Draw from the GBuffer
        DrawFromGBuffer();

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
            if (sprite.Visible)
            {
                sprite.Draw(_spriteShader);
            }
        }
        
        // Draw any UI screens
        foreach (var ui in _game.UIStack)
        {
            ui.Draw(_spriteShader);
        }

        // Swap the buffers (No need Silk.Net does it for us)
        // SDL_GL_SwapWindow(mWindow);
    }

    private void Draw3DScene(uint framebuffer, Matrix4X4<float> view, Matrix4X4<float> proj, float viewPortScale = 1.0f, bool lit = true)
    {
        // Set the current frame buffer
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);

        // Set viewport size based on scale
        GL.Viewport(0, 0, (uint) (ScreenWidth * viewPortScale), (uint) (ScreenHeight * viewPortScale));

        // Clear color buffer/depth buffer
        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        GL.DepthMask(true);
        GL.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

        // Draw mesh components
        // Enable depth buffer/disable alpha blend
        GL.Enable(EnableCap.DepthTest);
        GL.Disable(EnableCap.Blend);

        // Set the basic mesh shader active
        _meshShader.SetActive();

        // Update view-projection matrix
        _meshShader.SetUniform("uViewProj", view * proj);

        // Update lighting uniforms
        if (lit)
        {
            SetLightUniforms(_meshShader, view);
        }

        // Draw all meshes
        foreach (var mesh in _meshComps)
        {
            if (mesh.Visible)
            {
                mesh.Draw(_meshShader);
            }
        }

        // Draw any skinned meshes now
        _skinnedShader.SetActive();

        // Update view-projection matrix
        _skinnedShader.SetUniform("uViewProj", view * proj);

        // Update lighting uniforms
        if (lit)
        {
            SetLightUniforms(_skinnedShader, view);
        }

        // Draw all meshes
        foreach (var mesh in _skeletalMeshes)
        {
            if (mesh.Visible)
            {
                mesh.Draw(_skinnedShader);
            }
        }
    }
    
    private unsafe void DrawFromGBuffer()
    {
        // Disable depth testing for the global lighting pass
        GL.Disable(GLEnum.DepthTest);
        // Activate global G-buffer shader
        _gBufferGlobalShader.SetActive();
        // Activate sprite verts quad
        _spriteVertices.SetActive();
        // Set the G-buffer textures to sample
        _gBuffer.SetTexturesActive();
        // Set the lighting uniforms
        SetLightUniforms(_gBufferGlobalShader, ViewMatrix);
        // Draw the triangles
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, null);

        // // Copy depth buffer from G-buffer to default frame buffer
        // glBindFramebuffer(GL_READ_FRAMEBUFFER, mGBuffer->GetBufferID());
        // int width = static_cast<int>(mScreenWidth);
        // int height = static_cast<int>(mScreenHeight);
        // glBlitFramebuffer(0, 0, width, height,
        //     0, 0, width, height,
        //     GL_DEPTH_BUFFER_BIT, GL_NEAREST);
        //
        // // Enable depth test, but disable writes to depth buffer
        // glEnable(GL_DEPTH_TEST);
        // glDepthMask(GL_FALSE);
        //
        // // Set the point light shader and mesh as active
        // mGPointLightShader->SetActive();
        // mPointLightMesh->GetVertexArray()->SetActive();
        // // Set the view-projection matrix
        // mGPointLightShader->SetMatrixUniform("uViewProj",
        //     mView * mProjection);
        // // Set the G-buffer textures for sampling
        // mGBuffer->SetTexturesActive();
        //
        // // The point light color should add to existing color
        // glEnable(GL_BLEND);
        // glBlendFunc(GL_ONE, GL_ONE);
        //
        // // Draw the point lights
        // for (PointLightComponent* p : mPointLights)
        // {
        //     p->Draw(mGPointLightShader, mPointLightMesh);
        // }
    }

    public unsafe void DrawTexture(Texture texture, Vector2D<float> offset, float scale = 1.0f, bool flipY = false)
    {
        _spriteVertices.SetActive();
        _spriteShader.SetActive();
        
        // Scale the quad by the width/height of texture
        // and flip the y if we need to
        float yScale = texture.Height * scale;
        if (flipY)
        {
            yScale *= -1.0f;
        }

        Matrix4X4<float> scaleMat = Matrix4X4.CreateScale(
            texture.Width * scale,
            yScale,
            1.0f);
        
        // Translate to position on screen
        Matrix4X4<float> transMat = Matrix4X4.CreateTranslation(
            new Vector3D<float>(offset.X, offset.Y, 0.0f));

        // Set world transform
        Matrix4X4<float> world = scaleMat * transMat;
        _spriteShader.SetUniform("uWorldTransform", world);

        // Set current texture
        texture.SetActive();

        // Draw quad
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, null);
    }

    public unsafe void DrawText(Font font, string text, Vector2D<float> offset, float scale, Vector3D<float> color)
    {
        // Modern text rendering in OpenGL: https://learnopengl.com/In-Practice/Text-Rendering
        // How to render the glyph als signed distance field: https://stackoverflow.com/questions/71185718/how-to-use-ft-render-mode-sdf-in-freetype
        // SDF shader: https://www.youtube.com/watch?v=1b5hIMqz_wM
        _glyphVertices.SetActive();
        _textShader.SetActive();
        _textShader.SetUniform("textColor", color);

        // we need to middle the texture since in the book the position is in the middle of the texture, here we work with multiple texture
        var chars = text.ToCharArray().Select(font.GetCharacter).ToArray();
        var textHigh = chars.Select(c => (c.Bearing.Y) * scale).Max();
        var textWidth = chars.Select(c => (c.Advance >> 6)  * scale).Sum();

        var x = offset.X - textWidth / 2.0f;
        var y = offset.Y - textHigh / 2.0f;

        foreach (var character in chars)
        {
            var texture = character.Texture;

            // Scale the quad by the width/height of texture
            Matrix4X4<float> scaleMat = Matrix4X4.CreateScale(
                texture.Width * scale,
                texture.Height * scale,
                1.0f);

            float xpos = x + character.Bearing.X * scale;
            float ypos = y - (character.Size.Y - character.Bearing.Y) * scale;

            // Translate to position on screen
            Matrix4X4<float> transMat = Matrix4X4.CreateTranslation(
                new Vector3D<float>(xpos, ypos, 0.0f));

            // Set world transform
            Matrix4X4<float> world = scaleMat * transMat;
            _textShader.SetUniform("uWorldTransform", world);

            // Set current texture
            texture.SetActive();

            // Draw quad
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, null);

            // now advance cursors for next glyph (note that advance is number of 1/64 pixels)
            x += (character.Advance >> 6) * scale; // bitshift by 6 to get value in pixels (2^6 = 64)
        }
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
        if (mesh.IsSkeletal)
        {
            _skeletalMeshes.Add((SkeletalMeshComponent)mesh);
        }
        else
        {
            _meshComps.Add(mesh);
        }
    }

    public void RemoveMeshComp(MeshComponent mesh)
    {
        if (mesh.IsSkeletal)
        {
            _skeletalMeshes.Remove((SkeletalMeshComponent) mesh);
        }
        else
        {
            _meshComps.Remove(mesh);
        }
    }

    public Texture GetTexture(string fileName)
    {
        if (!_textures.ContainsKey(fileName))
        {
            var texture = Texture.Load(GL, fileName);
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

    /// <summary>
    /// Given a screen space point, unprojects it into world space,
    /// based on the current 3D view/projection matrices
    /// Expected ranges:
    /// x = [-screenWidth/2, +screenWidth/2]
    /// y = [-screenHeight/2, +screenHeight/2]
    /// z = [0, 1) -- 0 is closer to camera, 1 is further
    /// </summary>
    /// <param name="screenPoint"></param>
    /// <returns></returns>
    public Vector3D<float> Unproject(Vector3D<float> screenPoint)
    {
        // Convert screenPoint to device coordinates (between -1 and +1)
        Vector3D<float> deviceCoord = screenPoint;
        deviceCoord.X /= (ScreenWidth) * 0.5f;
        deviceCoord.Y /= (ScreenHeight) * 0.5f;

        // Transform vector by unprojection matrix
        Matrix4X4<float> unprojection = ViewMatrix * ProjectionMatrix;
        Matrix4X4.Invert(unprojection, out unprojection);
        return GameMath.TransformWithPerspDiv(deviceCoord, unprojection);
    }

    public void GetScreenDirection(out Vector3D<float> outStart, out Vector3D<float> outDir)
    {
        // Get start point (in center of screen on near plane)
        Vector3D<float> screenPoint = new Vector3D<float>(0.0f, 0.0f, 0.0f);
        outStart = Unproject(screenPoint);

        // Get end point (in center of screen, between near and far)
        screenPoint.Z = 0.9f;
        Vector3D<float> end = Unproject(screenPoint);

        // Get direction vector
        outDir = end - outStart;
        outDir = Vector3D.Normalize(outDir);
    }

    public void Dispose()
    {
        // Get rid of any render target textures, if they exist
        if (_mirrorTexture is not null)
        {
            _mirrorBuffer?.Dispose();
            _mirrorTexture?.Dispose();
        }
        
        // Get rid of G-buffer
        if (_gBuffer is not null)
        {
            _gBuffer.Dispose();
        }

        // Destroy textures
        foreach (var texture in _textures.ToArray())
        {
            _textures.Remove(texture.Key);
            texture.Value.Dispose();
        }

        // Destroy meshes
        foreach (var mesh in _meshes.ToArray())
        {
            _meshes.Remove(mesh.Key);
            mesh.Value.Dispose();
        }
        
        _spriteShader.Dispose();
        _spriteVertices.Dispose();
        _textShader.Dispose();
        _skinnedShader.Dispose();
        _gBufferGlobalShader.Dispose();
        GL.Dispose();
    }

    private void CreateMirrorTarget()
    {
        int width = (int) (ScreenWidth / 4);
        int height = (int) (ScreenHeight / 4);

        // Generate a frame buffer for the mirror texture
        GL.GenFramebuffers(1, out uint mirrorBuffer);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, mirrorBuffer);
        _mirrorBuffer = new FrameBufferObject(GL, mirrorBuffer);

        // Create the texture we'll use for rendering
        _mirrorTexture = Texture.CreateForRendering(GL, width, height, InternalFormat.Rgb);

        // Add a depth buffer to this target
        GL.GenRenderbuffers(1, out uint depthBuffer);
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
        GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.DepthComponent, (uint)width, (uint)height);
        GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthBuffer);

        // Attach mirror texture as the output target for the frame buffer
        GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, _mirrorTexture.TextureId, 0);

        // Set the list of buffers to draw to for this frame buffer
        var drawBuffers = new [] { GLEnum.ColorAttachment0 };
        GL.DrawBuffers(1, drawBuffers);

        // Make sure everything worked
        if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
        {
            _mirrorBuffer.Dispose();
            // If it didn't work, delete the framebuffer,
            // unload/delete the texture and return false
            throw new InvalidOperationException("Initializing the framebuffer failed");
        }
    }

    private void LoadShaders()
    {
        // Create sprite shader
        _spriteShader = new Shader(GL, "Shaders/Sprite.vert", "Shaders/Sprite.frag");
        _spriteShader.SetActive();

        // Set the view-projection matrix
        Matrix4X4<float> spriteViewProj = GameMath.CreateSimpleViewProj(ScreenWidth, ScreenHeight);
        _spriteShader.SetUniform("uViewProj", spriteViewProj);
        
        // Create sprite shader
        _textShader = new Shader(GL, "Shaders/Text.vert", "Shaders/Text.frag");
        _textShader.SetActive();

        // Set the view-projection matrix
        _textShader.SetUniform("uViewProj", spriteViewProj);

        // Create basic mesh shader
        _meshShader = new Shader(GL, "Shaders/Pong.vert", "Shaders/GBufferWrite.frag");
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
        
        // Create skinned shader
        _skinnedShader = new Shader(GL, "Shaders/Skinned.vert", "Shaders/GBufferWrite.frag");
        _skinnedShader.SetActive();
        
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

        _skinnedShader.SetUniform("uViewProj", ViewMatrix * ProjectionMatrix);
        
        // Create shader for drawing from GBuffer (global lighting)
        _gBufferGlobalShader =new Shader(GL, "Shaders/GBufferGlobal.vert", "Shaders/GBufferGlobal.frag");

        // For the GBuffer, we need to associate each sampler with an index
        _gBufferGlobalShader.SetActive();
        _gBufferGlobalShader.SetUniform("uGDiffuse", 0);
        _gBufferGlobalShader.SetUniform("uGNormal", 1);
        _gBufferGlobalShader.SetUniform("uGWorldPos", 2);

        // The view projection is just the sprite one
        _gBufferGlobalShader.SetUniform("uViewProj", spriteViewProj);

        // The world transform scales to the screen and flips y
        Matrix4X4<float> gbufferWorld = Matrix4X4.CreateScale(ScreenWidth, -ScreenHeight, 1.0f);
        _gBufferGlobalShader.SetUniform("uWorldTransform", gbufferWorld);
    }

    private void CreateSpriteVertices()
    {
        var vertices = new[] {
            // vertex(3)/normal(3)/(uv coord)
            new VertexPosNormTex(new(-0.5f, 0.5f, 0.0f), new(0.0f, 0.0f, 0.0f), new(0.0f, 0.0f)), // top left
            new VertexPosNormTex(new(0.5f, 0.5f, 0.0f), new(0.0f, 0.0f, 0.0f), new(1.0f, 0.0f)), // top right
            new VertexPosNormTex(new(0.5f, -0.5f, 0.0f), new(0.0f, 0.0f, 0.0f), new(1.0f, 1.0f)), // bottom right
            new VertexPosNormTex(new(-0.5f, -0.5f, 0.0f), new(0.0f, 0.0f, 0.0f), new(0.0f, 1.0f)),  // bottom left
        };

        var indices = new uint[] {
            0, 1, 2,
            2, 3, 0
        };

        _spriteVertices = new VertexArrayObject(GL, vertices, indices);
    }
    
    private void CreateGlyphVertices()
    {
        var vertices = new[] {
            // vertex(3)/normal(3)/(uv coord)
            new VertexPosNormTex(new(0.0f, 1.0f, 0.0f), new(0.0f, 0.0f, 0.0f), new(0.0f, 0.0f)), // top left
            new VertexPosNormTex(new(1.0f, 1.0f, 0.0f), new(0.0f, 0.0f, 0.0f), new(1.0f, 0.0f)), // top right
            new VertexPosNormTex(new(1.0f, 0.0f, 0.0f), new(0.0f, 0.0f, 0.0f), new(1.0f, 1.0f)), // bottom right
            new VertexPosNormTex(new(0.0f, 0.0f, 0.0f), new(0.0f, 0.0f, 0.0f), new(0.0f, 1.0f)),  // bottom left
        };

        var indices = new uint[] {
            0, 1, 2,
            2, 3, 0
        };

        _glyphVertices = new VertexArrayObject(GL, vertices, indices);
    }

    private void SetLightUniforms(Shader shader, Matrix4X4<float> viewMatrix)
    {
        // Camera position is from inverted view
        Matrix4X4.Invert(viewMatrix, out var invView);
        shader.SetUniform("uCameraPos", invView.GetTranslation());

        // Ambient light
        shader.SetUniform("uAmbientLight", AmbientLight);
    
        // Directional light
        shader.SetUniform("uDirLight.mDirection", DirectionalLight.Direction);
        shader.SetUniform("uDirLight.mDiffuseColor", DirectionalLight.DiffuseColor);
        shader.SetUniform("uDirLight.mSpecColor", DirectionalLight.SpecularColor);
    }
}