using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class SplineActor : Actor
{
    private readonly SplineCamera _cameraComp;

    public SplineActor(Game game)
        : base(game)
    {
        //MeshComponent* mc = new MeshComponent(this);
        //mc->SetMesh(game->GetRenderer()->GetMesh("Assets/RacingCar.gpmesh"));
        //SetPosition(Vector3(0.0f, 0.0f, -100.0f));

        _cameraComp = new SplineCamera(this);

        // Create a spline
        var path = new List<Vector3D<float>> { Vector3D<float>.Zero };

        for (int i = 0; i < 5; i++)
        {
            if (i % 2 == 0)
            {
                path.Add(new Vector3D<float>(300.0f * (i + 1), 300.0f, 300.0f));
            }
            else
            {
                path.Add(new Vector3D<float>(300.0f * (i + 1), 0.0f, 0.0f));
            }
        }

        _cameraComp.Path = new Spline(path);
        _cameraComp.Paused = false;
    }

    public void RestartSpline()
    {
        _cameraComp.Restart();
    }

    protected override void ActorInput(InputState state)
    {
    }
}