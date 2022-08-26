using GameProgrammingExercises.Maths;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace GameProgrammingExercises;

public class Game
{
    // All the actors in the game
    private readonly List<Actor> _actors = new();

    // Any pending actors
    private readonly List<Actor> _pendingActors = new();

    private Renderer _renderer;
    private AudioSystem _audioSystem;
    private InputSystem _inputSystem;

    private bool _updatingActors;

    // Game specific
    private CameraActor _cameraActor;
    private AudioActor _audioActor;

    public Renderer Renderer => _renderer;

    public AudioSystem AudioSystem => _audioSystem;

    public IWindow Window => _renderer.Window;

    public CameraActor Camera => _cameraActor;

    public IWindow Initialize()
    {
        // Create the renderer
        _renderer = new Renderer(this);
        var window = _renderer.Initialize(1024.0f, 768.0f, "Game Programming in C++ (Chapter 9)"); 

        // Create the audio system
        _audioSystem = new AudioSystem(this);
        _audioSystem.Initialize();

        window.Load += () =>
        {
            // Initialize input system
            _inputSystem = new InputSystem(this);
            _inputSystem.Initialize();

            LoadData();
        };

        window.Update += deltaTime =>
        {
            ProcessInput();
            UpdateGame((float)deltaTime);
        };

        window.Render += _ =>
        {
            GenerateOutput();
        };

        window.Closing += () =>
        {
            UnloadData();

            _inputSystem.Dispose();
            _renderer.Dispose();
            _audioSystem.Dispose();
        };

        return window;
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

    private void ProcessInput()
    {
        _inputSystem.Update();
        var state = _inputSystem.State;
    
        if (state.Keyboard.GetKeyState(Key.Escape) == ButtonState.Released)
        {
            _renderer.Window.Close();
        }
    
        // Process input for all actors
        _updatingActors = true;
        foreach (var actor in _actors)
        {
            actor.ProcessInput(state);
        }
        _updatingActors = false;
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
        var deadActors = _actors.Where(a => a.State == ActorState.Dead).ToArray();
        foreach (var actor in deadActors)
        {
            actor.Dispose();
        }

        // Update audio system
        _audioSystem.Update(deltaTime);
    }

    private void GenerateOutput()
    {
        _renderer.Draw();
    }

    private void LoadData()
    {
        // Create actors
        var a = new Actor(this)
        {
            Position = new Vector3D<float>(200.0f, 75.0f, 0.0f),
            Scale = 100.0f
        };
        var q = GameMath.CreateQuaternion(Vector3D<float>.UnitY, -1 * Scalar<float>.PiOver2);
        q = Quaternion<float>.Concatenate(q, GameMath.CreateQuaternion(Vector3D<float>.UnitZ, (float)(Math.PI + Math.PI / 4.0f)));
        a.Rotation = q;
        _ = new MeshComponent(a)
        {
            Mesh = _renderer.GetMesh("Assets/Cube.gpmesh")
        };

        a = new Actor(this)
        {
            Position = new Vector3D<float>(200.0f, -75.0f, 0.0f),
            Scale = 3.0f
        };
        _ = new MeshComponent(a)
        {
            Mesh = _renderer.GetMesh("Assets/Sphere.gpmesh")
        };
        
        // Setup floor
        var start = -1250.0f;
        var size = 250.0f;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                a = new PlaneActor(this)
                {
                    Position = new Vector3D<float>(start + i * size, start + j * size, -100.0f),
                };
            }
        }

        // Left/right walls
        q = GameMath.CreateQuaternion(Vector3D<float>.UnitX, Scalar<float>.PiOver2);
        for (int i = 0; i < 10; i++)
        {
            a = new PlaneActor(this);
            a.Position = new Vector3D<float>(start + i * size, start - size, 0.0f);
            a.Rotation = q;
        
            a = new PlaneActor(this);
            a.Position = new Vector3D<float>(start + i * size, -start + size, 0.0f);
            a.Rotation = q;
        }
        
        q = Quaternion<float>.Concatenate(q, GameMath.CreateQuaternion(Vector3D<float>.UnitZ, Scalar<float>.PiOver2));
        // Forward/back walls
        for (int i = 0; i < 10; i++)
        {
            a = new PlaneActor(this);
            a.Position = new Vector3D<float>(start - size, start + i * size, 0.0f);
            a.Rotation = q;
        
            a = new PlaneActor(this);
            a.Position = new Vector3D<float>(-start + size, start + i * size, 0.0f);
            a.Rotation = q;
        }

        // Setup lights
        _renderer.AmbientLight = new Vector3D<float>(0.2f, 0.2f, 0.2f);
        _renderer.DirectionalLight = new DirectionalLight(
            direction: new Vector3D<float>(0.0f, -0.707f, -0.707f),
            diffuseColor: new Vector3D<float>(0.78f, 0.88f, 1.0f),
            specularColor: new Vector3D<float>(0.8f, 0.8f, 0.8f)
        );

        // Camera actor
        _cameraActor = new CameraActor(this);

        // UI elements
        a = new Actor(this)
        {
            Position = new Vector3D<float>(-350.0f, -350.0f, 0.0f)
        };
        _ = new SpriteComponent(a)
        {
            Texture = _renderer.GetTexture("Assets/HealthBar.png")
        };
        
        a = new Actor(this)
        {
            Position = new Vector3D<float>(375.0f, -275.0f, 0.0f),
            Scale = 0.75f
        };
        _ = new SpriteComponent(a)
        {
            Texture = _renderer.GetTexture("Assets/Radar.png")
        };

        // Create spheres with audio components playing different sounds
        a = new Actor(this)
        {
            Position = new Vector3D<float>(500.0f, -75.0f, 0.0f),
            Scale = 1.0f
        };
        var mc = new MeshComponent(a);
        mc.Mesh = _renderer.GetMesh("Assets/Sphere.gpmesh");
        var ac = new AudioComponent(a);
        ac.PlayEvent("event:/FireLoop");

        // Audio actor
        _audioActor = new AudioActor(this);
    }

    private void UnloadData()
    {
        // Delete actors
        // Because ~Actor calls RemoveActor, have to use a different style loop
        foreach (var actor in _actors.ToArray())
        {
            actor.Dispose();
        }
    }
}