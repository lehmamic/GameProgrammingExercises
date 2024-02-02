using GameProgrammingExercises.Maths;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class Actor : IDisposable
{
    private readonly List<Component> _components = new();

    // transformation
    private bool _recomputeWorldTransform = true;
    private Vector3D<float> _position = Vector3D<float>.Zero;
    private float _scale = 1.0f;
    private Quaternion<float> _rotation = Quaternion<float>.Identity;

    /// <summary>
    /// Constructor. Creates an instance of the Actor.
    /// </summary>
    /// <param name="game">The owning game.</param>
    public Actor(Game game)
    {
        Game = game;
        Game.AddActor(this);
    }

    ~Actor()
    {
        Dispose(false);
    }

    /// <summary>
    /// The owning game.
    /// </summary>
    public Game Game { get; }

    /// <summary>
    /// Actor's state.
    /// </summary>
    public ActorState State { get; set; } = ActorState.Active;

    public Vector3D<float> Position
    {
        get => _position;
        set
        {
            _position = value;
            _recomputeWorldTransform = true;
        }
    }

    public float Scale
    {
        get => _scale;
        set
        {
            _scale = value;
            _recomputeWorldTransform = true;
        }
    }

    public Quaternion<float> Rotation
    {
        get => _rotation;
        set
        {
            _rotation = value;
            _recomputeWorldTransform = true;
        }
    }

    public Vector3D<float> Forward => Vector3D.Transform(Vector3D<float>.UnitX, _rotation);

    public Vector3D<float> Right => Vector3D.Transform(Vector3D<float>.UnitY, _rotation);

    public Matrix4X4<float> WorldTransform { get; private set; }

    public static Actor Create<T>(Game game, LevelLoader.RawActor rawActor) where T : Actor
    {
        var actor = (T)Activator.CreateInstance(typeof(T), game)!;
        actor.LoadProperties(rawActor.Properties);

        return actor;
    }

    /// <summary>
    /// Update function called from Game (not overridable).
    /// </summary>
    /// <param name="deltaTime">The delta time between two frames.</param>
    public void Update(float deltaTime)
    {
        if (State == ActorState.Active)
        {
            ComputeWorldTransform();

            UpdateComponents(deltaTime);
            UpdateActor(deltaTime);

            ComputeWorldTransform();
        }
    }

    /// <summary>
    /// ProcessInput function called from Game (not overridable).
    /// </summary>
    /// <param name="state"></param>
    public void ProcessInput(InputState state)
    {
        if (State == ActorState.Active)
        {
            // First process input for components
            foreach (var component in _components)
            {
                component.ProcessInput(state);
            }

            ActorInput(state);
        }
    }
    
    public void RotateToNewForward(Vector3D<float> forward)
    {
        // Figure out difference between original (unit x) and new
        float dot = Vector3D.Dot(Vector3D<float>.UnitX, forward);
        float angle = Scalar.Acos(dot);
        // Facing down X
        if (dot > 0.9999f)
        {
            Rotation = Quaternion<float>.Identity;
        }
        // Facing down -X
        else if (dot < -0.9999f)
        {
            Rotation = GameMath.CreateQuaternion(Vector3D<float>.UnitZ, Scalar<float>.Pi);
        }
        else
        {
            // Rotate about axis from cross product
            Vector3D<float> axis = Vector3D.Cross(Vector3D<float>.UnitX, forward);
            axis = Vector3D.Normalize(axis);
            Rotation = GameMath.CreateQuaternion(axis, angle);
        }
    }

    public void ComputeWorldTransform()
    {
        if (_recomputeWorldTransform)
        {
            _recomputeWorldTransform = false;

            // Scale, then rotate, then translate
            WorldTransform = Matrix4X4.CreateScale(_scale);
            WorldTransform *= Matrix4X4.CreateFromQuaternion(_rotation);
            WorldTransform *= Matrix4X4.CreateTranslation(_position);

            // Inform components world transform updated
            foreach (var component in _components)
            {
                component.OnUpdateWorldTransform();
            }
        }
    }

    public void AddComponent(Component component)
    {
        // Find the insertion point in the sorted vector
        // (The first element with a order higher than me)
        int index = 0;
        for (; index < _components.Count; index++)
        {
            if (component.UpdateOrder < _components[index].UpdateOrder)
            {
                break;
            }
        }

        // Inserts element before position of iterator
        _components.Insert(index, component);
    }
    
    public void RemoveComponent(Component component)
    {
        _components.Remove(component);
    }
    
    // Search through component vector for one of type
    public Component? GetComponentOfType(string type)
    {
        return _components.Find(c => string.Equals(c.GetType().Name, type, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Disposes the actor which will remove itself from the game and clean up all containing components.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Updates all the components attached.
    /// </summary>
    /// <param name="deltaTime">The delta time between two frames.</param>
    protected void UpdateComponents(float deltaTime)
    {
        foreach (var component in _components)
        {
            component.Update(deltaTime);
        }
    }

    /// <summary>
    /// Any actor-specific update code (overridable).
    /// </summary>
    /// <param name="deltaTime">The delta time between two frames.</param>
    protected virtual void UpdateActor(float deltaTime)
    {
    }

    /// <summary>
    /// Any actor-specific input code (overridable).
    /// </summary>
    /// <param name="state"></param>
    protected virtual void ActorInput(InputState state)
    {
    }

    protected virtual void LoadProperties(LevelLoader.ActorProperties properties)
    {
        State = properties.State;

        // Load position, rotation, and scale, and compute transform
        JsonHelper.TryGetVector3D(properties.Position, out _position);
        JsonHelper.TryGetQuaternion(properties.Rotation, out _rotation);
        _scale = properties.Scale.GetValueOrDefault(1.0f);
        ComputeWorldTransform();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Game.RemoveActor(this);

            // Need to delete components
            // Because ~Component calls RemoveComponent, need a different style loop
            while (_components.Any())
            {
                var component = _components.Last();
                component.Dispose();
                _components.Remove(component);
            }
        }
    }
}