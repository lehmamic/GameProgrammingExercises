using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace GameProgrammingExercises;

public class Game
{
    // All the actors in the game
    private readonly List<Actor> _actors = new();

    // Any pending actors
    private readonly List<Actor> _pendingActors = new();

    // All the sprite components drawn
    private readonly List<SpriteComponent> _sprites = new();
    
    // Map of textures loaded
    private readonly Dictionary<string, Texture> _textures = new();

    private IWindow _window;
    private IInputContext _input;
    private GL _gl;

    private bool _updatingActors;
    private IKeyboard _primaryKeyboard;

    // Sprite Shader
    private Shader _spriteShader;
    
    // Sprite vertex array
    private VertexArrayObject _spriteVertices;

    // Game specific
    private List<Asteroid> _asteroids = new();

    public IWindow Initialize()
    {
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(1024, 768);
        options.Title = "Game Programming in C++ (Chapter 5)";

        _window = Window.Create(options);

        _window.Load += () =>
        {
            // Set-up input context.
            _input = _window.CreateInput();
            _primaryKeyboard = _input.Keyboards.First();
            _primaryKeyboard.KeyDown += (_, key, _) =>
            {
                if (key == Key.Escape)
                {
                    _window.Close();
                }
            };

            // Getting the opengl api for drawing to the screen.
            _gl = GL.GetApi(_window);

            // Make sure we can load and compile shaders
            LoadShaders();

            CreateSpriteVertices();

            LoadData();

            _spriteShader.SetActive();
            _spriteShader.SetUniform("uWorldTransform", Matrix4X4<float>.Identity);
        };

        _window.Update += deltaTime =>
        {
            ProcessInput();
            UpdateGame((float)deltaTime);
        };

        _window.Render +=deltaTime =>
        {
            GenerateOutput((float)deltaTime);
        };

        _window.Closing += () =>
        {
            UnloadData();

            _spriteShader.Dispose();
            _spriteVertices.Dispose();
            _window.Dispose();
            _input.Dispose();

            _gl.Dispose();
        };

        return _window;
    }

    public void AddActor(Actor actor)
    {
        // If updating actors, need to add to pending
        if (_updatingActors)
        {
            _pendingActors.Add(actor);
        }
        else
        {
            _actors.Add(actor);
        }
    }
    
    public void RemoveActor(Actor actor)
    {
        // Is it in pending actors?
        _pendingActors.Remove(actor);

        // Is it in actors?
        _actors.Remove(actor);
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
    
    public void AddAsteroid(Asteroid asteroid)
    {
        _asteroids.Add(asteroid);
    }

    public void RemoveAsteroid(Asteroid asteroid)
    {
        _asteroids.Remove(asteroid);
    }

    public Texture GetTexture(string fileName)
    {
        if (!_textures.ContainsKey(fileName))
        {
            var texture = new Texture(_gl, fileName);
            _textures.Add(fileName, texture);
        }

        return _textures[fileName];
    }

    private void ProcessInput()
    {
        
    }

    private void UpdateGame(float deltaTime)
    {
        // Update all actors
        _updatingActors = true;
        foreach (var actor in _actors)
        {
            actor.Update(deltaTime);
        }
        _updatingActors = false;

        // Move any pending actors to _actors
        foreach (var pending in _pendingActors)
        {
            pending.ComputeWorldTransform();
            _actors.Add(pending);
        }

        _pendingActors.Clear();

        // Delete dead actors (which removes them from _actors)
        var deadActors = _actors.Where(a => a.State == ActorState.Dead);
        foreach (var actor in deadActors)
        {
            actor.Dispose();
        }
    }

    private void GenerateOutput(float deltaTime)
    {
        // Set the clear color to grey
        _gl.ClearColor(0.86f, 0.86f, 0.86f, 1.0f);

        // Clear the color buffer
        _gl.Clear((uint) ClearBufferMask.ColorBufferBit);

        // Draw all sprite components
        
        // Enable alpha blending on the color buffer
        _gl.Enable(GLEnum.Blend);
        _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        // Set shader/vao as active
        _spriteShader.SetActive();
        _spriteVertices.SetActive();

        foreach (var sprite in _sprites)
        {
            sprite.Draw(_spriteShader);
        }

        // Swap the buffers (No need Silk.Net does it for us)
        // SDL_GL_SwapWindow(mWindow);
    }

    private void LoadShaders()
    {
        _spriteShader = new Shader(_gl, "Shaders/Sprite.vert", "Shaders/Sprite.frag");
        _spriteShader.SetActive();

        // Set the view-projection matrix
        Matrix4X4<float> viewProj = CreateSimpleViewProj(1024.0f, 768.0f);
        _spriteShader.SetUniform("uViewProj", viewProj);
    }

    private static Matrix4X4<float> CreateSimpleViewProj(float width, float height)
    {
        return Matrix4X4<float>.Identity with { M11 = 2.0f/width, M22 = 2.0f/height, M33 = 1.0f, M43 = 1.0f, M44 = 1.0f };
    }

    private void CreateSpriteVertices()
    {
        var vertices = new[] {
            -0.5f, 0.5f, 0.0f, 0.0f, 0.0f, // top left
            0.5f, 0.5f, 0.0f, 1.0f, 0.0f, // top right
            0.5f, -0.5f, 0.0f, 1.0f, 1.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 1.0f  // bottom left
        };

        var indices = new uint[] {
            0, 1, 2,
            2, 3, 0
        };

        var vbo = new BufferObject<float>(_gl, vertices, BufferTargetARB.ArrayBuffer);
        var ebo = new BufferObject<uint>(_gl, indices, BufferTargetARB.ElementArrayBuffer);
        _spriteVertices = new VertexArrayObject(_gl, vbo, ebo);
    }
    
    private void LoadData()
    {
        // Create player's ship
        // mShip = new Ship(this);
        // mShip->SetRotation(Math::PiOver2);

        // Create asteroids
        const int numAsteroids = 20;
        for (int i = 0; i < numAsteroids; i++)
        {
            new Asteroid(_gl, this);
        }
    }

    private void UnloadData()
    {
        // Delete actors
        // Because ~Actor calls RemoveActor, have to use a different style loop
        foreach (var actor in _actors.ToArray())
        {
            actor.Dispose();
        }

        // Destroy textures
        foreach (var texture in _textures.ToArray())
        {
            _textures.Remove(texture.Key);
            texture.Value.Dispose();
        }
    }
}