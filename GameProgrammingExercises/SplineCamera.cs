using GameProgrammingExercises.Maths;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class SplineCamera : CameraComponent
{
    // Current control point index and t
    private int _index = 1;
    private float _t;

    public SplineCamera(Actor owner)
        : base(owner)
    {
    }

    /// <summary>
    /// Spline path camera follows
    /// </summary>
    public Spline Path { get; set; }

    /// <summary>
    /// Amount t changes/sec
    /// </summary>
    public float Speed { get; set; } = 0.5f;

    /// <summary>
    /// Whether to move the camera long the path
    /// </summary>
    public bool Paused { get; set; } = true;

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        // Update t value
        if (!Paused)
        {
            _t += Speed * deltaTime;

            // Advance to the next control point if needed.
            // This assumes speed isn't so fast that you jump past
            // multiple control points in one frame.
            if (_t >= 1.0f)
            {
                // Make sure we have enough points to advance the path
                if (_index < Path.NumPoints - 3)
                {
                    _index++;
                    _t = _t - 1.0f;
                }
                else
                {
                    // Path's done, so pause
                    Paused = true;
                }
            }
        }

        // Camera position is the spline at the current t/index
        Vector3D<float> cameraPos = Path.Compute(_index, _t);

        // Target point is just a small delta ahead on the spline
        Vector3D<float> target = Path.Compute(_index, _t + 0.01f);

        // Assume spline doesn't flip upside-down
        var up = Vector3D<float>.UnitZ;
        Matrix4X4<float> view = GameMath.CreateLookAt(cameraPos, target, up);
        SetViewMatrix(view);
    }

    public void Restart()
    {
        _index = 1;
        _t = 0.0f;
        Paused = false;
    }
}