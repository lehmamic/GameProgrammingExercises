using GameProgrammingExercises.Maths;
using GameProgrammingExercises.Maths.Geometry;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class HUD : UIScreen
{
    private readonly Texture _healthBar;
    private readonly Texture _radar;
    private readonly Texture _crosshair;
    private readonly Texture _crosshairEnemy;
    private readonly Texture _blipTex;
    private readonly Texture _radarArrow;

    // All the target components in the game
    private readonly List<TargetComponent> _targetComps =new();
    // 2D offsets of blips relative to radar
    private readonly List<Vector2D<float>> _blips = new();
    // Adjust range of radar and radius
    private float _radarRange = 2000.0f;
    private float _radarRadius = 92.0f;
    // Whether the crosshair targets an enemy
    private bool _targetEnemy;

    private float _fps = 0.0f;
    private float _lastFpsRefreshTime = 0.0f;

    public HUD(Game game)
        : base(game)
    {
        _healthBar = Game.Renderer.GetTexture("Assets/HealthBar.png");
        _radar = Game.Renderer.GetTexture("Assets/Radar.png");
        _crosshair = Game.Renderer.GetTexture("Assets/Crosshair.png");
        _crosshairEnemy = Game.Renderer.GetTexture("Assets/CrosshairRed.png");
        _blipTex = Game.Renderer.GetTexture("Assets/Blip.png");
        _radarArrow = Game.Renderer.GetTexture("Assets/RadarArrow.png");
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        UpdateCrosshair(deltaTime);
        UpdateRadar(deltaTime);

        if (_lastFpsRefreshTime >= 0.8f)
        {
            _fps = 1.0f / deltaTime;
            _lastFpsRefreshTime = 0.0f;
        }
        _lastFpsRefreshTime += deltaTime;
    }

    public override void Draw(Shader shader)
    {
        // Crosshair
        Texture cross = _targetEnemy ? _crosshairEnemy : _crosshair;
        Game.Renderer.DrawTexture(cross, Vector2D<float>.Zero, 2.0f);

        // Radar
        var radarPos = new Vector2D<float>(-390.0f, 275.0f);
        Game.Renderer.DrawTexture(_radar, radarPos, 1.0f);
        // Blips
        foreach (var blip in _blips)
        {
            Game.Renderer.DrawTexture(_blipTex, radarPos + blip, 1.0f);
        }
        // Radar arrow
        Game.Renderer.DrawTexture(_radarArrow, radarPos);

        Game.Renderer.DrawText(Font, $"{_fps:F1} FPS", new Vector2D<float>(470.0f, 370.0f), 0.4f, Color.Green);
    }

    public void AddTargetComponent(TargetComponent tc)
    {
        _targetComps.Add(tc);
    }

    public void RemoveTargetComponent(TargetComponent tc)
    {
        _targetComps.Remove(tc);
    }

    private void UpdateCrosshair(float deltaTime)
    {
        // Reset to regular cursor
        _targetEnemy = false;
        // Make a line segment
        float cAimDist = 5000.0f;
        Game.Renderer.GetScreenDirection(out var start, out var dir);
        LineSegment l = new(start, start + dir * cAimDist);
        // Segment cast
        if (Game.PhysWorld.SegmentCast(l, out var info))
        {
            // Is this a target?
            foreach (var tc in _targetComps)
            {
                if (tc.Owner == info.Actor)
                {
                    _targetEnemy = true;
                    break;
                }
            }
        }
    }

    protected void UpdateRadar(float deltaTime)
    {
        // Clear blip positions from last frame
        _blips.Clear();

        // Convert player position to radar coordinates (x forward, z up)
        Vector3D<float> playerPos = Game.Player.Position;
        Vector2D<float> playerPos2D = new(playerPos.Y, playerPos.X);
        // Ditto for player forward
        Vector3D<float> playerForward = Game.Player.Forward;
        Vector2D<float> playerForward2D = new(playerForward.Y, playerForward.X);

        // Use atan2 to get rotation of radar
        float angle = Scalar.Atan2(playerForward2D.Y, playerForward2D.X);
        // Make a 2D rotation matrix
        Matrix3X3<float> rotMat = Matrix3X3.CreateRotationZ(angle);

        // Get positions of blips
        foreach (var tc in _targetComps)
        {
            Vector3D<float> targetPos = tc.Owner.Position;
            Vector2D<float> actorPos2D = new(targetPos.Y, targetPos.X);

            // Calculate vector between player and target
            Vector2D<float> playerToTarget = actorPos2D - playerPos2D;

            // See if within range
            if (playerToTarget.LengthSquared <= (_radarRange * _radarRange))
            {
                // Convert playerToTarget into an offset from
                // the center of the on-screen radar
                Vector2D<float> blipPos = playerToTarget;
                blipPos *= _radarRadius/_radarRange;

                // Rotate blipPos
                blipPos = GameMath.Transform(blipPos, rotMat);
                _blips.Add(blipPos);
            }
        }
    }
}