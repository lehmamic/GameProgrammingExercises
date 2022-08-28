namespace GameProgrammingExercises;

public class PlaneActor : Actor
{
    private readonly BoxComponent _box;

    public PlaneActor(Game game)
        : base(game)
    {
        Scale = 10.0f;
        var mesh = new MeshComponent(this);
        mesh.Mesh = Game.Renderer.GetMesh("Assets/Plane.gpmesh");

        // Add collision box
        _box = new BoxComponent(this);
        _box.ObjectBox = mesh.Mesh.Box;

        Game.AddPlane(this);
    }

    public BoxComponent Box => _box;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Game.RemovePlane(this);
        }

        base.Dispose(disposing);
    }
}