namespace GameProgrammingExercises;

public class SkeletalMeshComponent : MeshComponent
{
    public SkeletalMeshComponent(Actor owner)
        : base(owner, true)
    {
    }
}