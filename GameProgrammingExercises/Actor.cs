using System.Numerics;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class Actor : IDisposable
{
    private readonly List<Component> _components = new();

    // transformation
    private bool _recomputeWorldTransform = true;
    private Vector2D<float> _position = Vector2D<float>.Zero;
    private float _scale = 1.0f;
    private float _rotation = 0.0f;

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

    public Vector2D<float> Position
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

    public float Rotation
    {
        get => _rotation;
        set
        {
            _rotation = value;
            _recomputeWorldTransform = true;
        }
    }

    public Vector2D<float> Forward => new(Scalar.Cos(_rotation), Scalar.Sin(_rotation));

    public Matrix4X4<float> WorldTransform { get; private set; }

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
    /// Updates all the components attached.
    /// </summary>
    /// <param name="deltaTime">The delta time between two frames.</param>
    public void UpdateComponents(float deltaTime)
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
    public virtual void UpdateActor(float deltaTime)
    {
    }

    /// <summary>
    /// ProcessInput function called from Game (not overridable).
    /// </summary>
    /// <param name="keyState"></param>
    public void ProcessInput(int keyState)
    {
        if (State == ActorState.Active)
        {
            // First process input for components
            foreach (var component in _components)
            {
                component.ProcessInput(keyState);
            }

            ActorInput(keyState);
        }
    }

    /// <summary>
    /// Any actor-specific input code (overridable).
    /// </summary>
    /// <param name="keyState"></param>
    public virtual void ActorInput(int keyState)
    {
    }
    
    public void ComputeWorldTransform()
    {
        if (_recomputeWorldTransform)
        {
            _recomputeWorldTransform = false;

            // Scale, then rotate, then translate
            WorldTransform = Matrix4X4.CreateScale(_scale);
            WorldTransform *= Matrix4X4.CreateRotationZ(_rotation);
            WorldTransform *= Matrix4X4.CreateTranslation(new Vector3D<float>(_position.X, _position.Y, 0.0f));

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

    /// <summary>
    /// Disposes the actor which will remove itself from the game and clean up all containing components.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
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
                _components.Last().Dispose();
            }
        }
    }
}