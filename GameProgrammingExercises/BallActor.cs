namespace GameProgrammingExercises;

public class BallActor : Actor
{
    private readonly BallMove _myMove;
    private readonly AudioComponent _audioComp;

    private float _lifeSpan = 2.0f;

    public BallActor(Game game)
        : base(game)
    {
        //SetScale(10.0f);
        var mc = new MeshComponent(this)
        {
            Mesh = Game.Renderer.GetMesh("Assets/Sphere.gpmesh")
        };
        _myMove = new BallMove(this)
        {
            ForwardSpeed = 1500.0f
        };
        _audioComp = new AudioComponent(this);
    }
    
    public Actor Player
    {
        get => _myMove.Player;
        set => _myMove.Player = value;
    }

    public void HitTarget()
    {
        _audioComp.PlayEvent("event:/Ding");
    }

    protected override void UpdateActor(float deltaTime)
    {
        base.UpdateActor(deltaTime);

        _lifeSpan -= deltaTime;
        if (_lifeSpan < 0.0f)
        {
            State = ActorState.Dead;
        }
    }
}