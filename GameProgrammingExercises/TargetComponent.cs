namespace GameProgrammingExercises;

public class TargetComponent : Component
{
    public TargetComponent(Actor owner)
        : base(owner)
    {
        Owner.Game.HUD.AddTargetComponent(this);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Owner.Game.HUD.RemoveTargetComponent(this);
        }
        base.Dispose(disposing);
    }
}