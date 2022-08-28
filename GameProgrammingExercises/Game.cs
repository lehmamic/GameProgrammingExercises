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
    private PhysWorld _physWorld;

    private bool _updatingActors;

    // Game specific
    private FpsActor _fpsActor;
    private SpriteComponent _crosshair;
    private Actor _startSphere;
    private Actor _endSphere;
    private SoundEvent _musicEvent;
    private readonly List<PlaneActor> _planes = new();

    public IWindow Window => _renderer.Window;

    public Renderer Renderer => _renderer;

    public AudioSystem AudioSystem => _audioSystem;

    public PhysWorld PhysWorld => _physWorld;

    public IReadOnlyList<PlaneActor> Planes => _planes;

    public IWindow Initialize()
    {
        // Create the renderer
        _renderer = new Renderer(this);
        var window = _renderer.Initialize(1024.0f, 768.0f, "Game Programming in C++ (Chapter 10)"); 

        // Create the audio system
        _audioSystem = new AudioSystem(this);
        _audioSystem.Initialize();
        
        // Create the physics world
        _physWorld = new PhysWorld(this);

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

    public void AddPlane(PlaneActor plane)
    {
        _planes.Add(plane);
    }

    public void RemovePlane(PlaneActor plane)
    {
        _planes.Remove(plane);
    }

    private void ProcessInput()
    {
        _inputSystem.Update();
        var state = _inputSystem.State;
    
        if (state.Keyboard.GetKeyState(Key.Escape) == ButtonState.Released)
        {
            _renderer.Window.Close();
        }

        HandleKeyPress(state);
    
        // Process input for all actors
        _updatingActors = true;
        foreach (var actor in _actors)
        {
            actor.ProcessInput(state);
        }
        _updatingActors = false;
    }
    
    private void HandleKeyPress(InputState state)
    {
        if (state.Keyboard.GetKeyState(Key.Minus) == ButtonState.Pressed)
        {
            // Reduce master volume
            float volume = _audioSystem.GetBusVolume("bus:/");
            volume = Scalar.Max(0.0f, volume - 0.1f);
            _audioSystem.SetBusVolume("bus:/", volume);
        }

        if (state.Keyboard.GetKeyState(Key.Equal) == ButtonState.Pressed)
        {
            // Increase master volume
            float volume = _audioSystem.GetBusVolume("bus:/");
            volume = Scalar.Min(1.0f, volume + 0.1f);
            _audioSystem.SetBusVolume("bus:/", volume);
        }

        if (state.Mouse.GetButtonState(MouseButton.Left) == ButtonState.Pressed)
        {
            _fpsActor.Shoot();
        }
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
        Actor a;
        Quaternion<float> q;

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

        a = new Actor(this)
        {
            Scale = 2.0f
        };
        _crosshair = new SpriteComponent(a);
        _crosshair.Texture = _renderer.GetTexture("Assets/Crosshair.png");

        // Start music
        _musicEvent = _audioSystem.PlayEvent("event:/Music");

        // Enable relative mouse mode for camera look
        _inputSystem.SetRelativeMouseMode(true);

        // Make an initial call to get relative to clear out
        // SDL_GetRelativeMouseState(nullptr, nullptr);

        // Different camera actors
        _fpsActor = new FpsActor(this);

        // Create target actors
        a = new TargetActor(this);
        a.Position = new Vector3D<float>(1450.0f, 0.0f, 100.0f);
        a = new TargetActor(this);
        a.Position = new Vector3D<float>(1450.0f, 0.0f, 400.0f);
        a = new TargetActor(this);
        a.Position = new Vector3D<float>(1450.0f, -500.0f, 200.0f);
        a = new TargetActor(this);
        a.Position = new Vector3D<float>(1450.0f, 500.0f, 200.0f);
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