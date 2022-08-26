namespace GameProgrammingExercises;

public class FpsActor : Actor
{
    private readonly MoveComponent _move;
    private readonly AudioComponent _audio;
    private readonly SoundEvent _footstep;

    private float _lastFootstep;

    public FpsActor(Game game)
        : base(game)
    {
        _move = new MoveComponent(this);
        _audio = new AudioComponent(this);
        _lastFootstep = 0.0f;
        _footstep = _audio.PlayEvent("event:/Footstep");
        _footstep.SetPaused(true);
    }

    public void SetFootstepSurface(float value)
    {
        // Pause here because the way I setup the parameter in FMOD
        // changing it will play a footstep
        _footstep.SetPaused(true);
        _footstep.SetParameter("Surface", value);
    }

    protected override void UpdateActor(float deltaTime)
    {
        base.UpdateActor(deltaTime);
    }

    protected override void ActorInput(InputState state)
    {
        base.ActorInput(state);
    }
}