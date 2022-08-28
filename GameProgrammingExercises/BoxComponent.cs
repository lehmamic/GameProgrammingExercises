using GameProgrammingExercises.Maths.Geometry;

namespace GameProgrammingExercises;

public class BoxComponent : Component
{
    private AABB _objectBox;
    private AABB _worldBox;

    public BoxComponent(Actor owner, int updateOrder = 100)
        : base(owner, updateOrder)
    {
        Owner.Game.PhysWorld.AddBox(this);
    }

    public AABB ObjectBox
    {
        get => _objectBox;
        set => _objectBox = value;
    }

    public AABB WorldBox => _worldBox;

    public bool ShouldRotate { get; set; }

    public override void OnUpdateWorldTransform()
    {
        // Reset to object space box
        _worldBox = ObjectBox;

        // Scale
        _worldBox.Min *= Owner.Scale;
        _worldBox.Max *= Owner.Scale;

        // Rotate (if we want to)
        if (ShouldRotate)
        {
            WorldBox.Rotate(Owner.Rotation);
        }

        // Translate
        _worldBox.Min += Owner.Position;
        _worldBox.Max += Owner.Position;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Owner.Game.PhysWorld.RemoveBox(this);
        }

        base.Dispose(disposing);
    }
}