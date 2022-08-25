namespace GameProgrammingExercises;

public class Component : IDisposable
{
    /// <summary>
    /// Constructor. Creates an instance of the Component.
    /// </summary>
    /// <param name="owner">The owning actor.</param>
    /// <param name="updateOrder">The update order of the component. The lower the update order, the earlier the component updates. Defaults to 100.</param>
    public Component(Actor owner, int updateOrder = 100)
    {
        Owner = owner;
        UpdateOrder = updateOrder;

        Owner.AddComponent(this);
    }

    /// <summary>
    /// The owning actor.
    /// </summary>
    public Actor Owner { get; }

    /// <summary>
    /// The update order of the component. The lower the update order, the earlier the component updates.
    /// </summary>
    public int UpdateOrder { get; }

    /// <summary>
    /// Update this component by delta time.
    /// </summary>
    /// <param name="deltaTime">The delta time between two frames.</param>
    public virtual void Update(float deltaTime)
    {
    }

    /// <summary>
    /// Process input for this component.
    /// </summary>
    /// <param name="state"></param>
    public virtual void ProcessInput(InputState state)
    {
    }

    /// <summary>
    /// Called when world transform changes.
    /// </summary>
    public virtual void OnUpdateWorldTransform()
    {
    }

    /// <summary>
    /// Disposes the component which will remove itself from the actor.
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
            Owner.RemoveComponent(this);
        }
    }
}