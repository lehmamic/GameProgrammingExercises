using GameProgrammingExercises.Maths;
using Silk.NET.Input;
using Silk.NET.OpenGL;

namespace GameProgrammingExercises;

public class Ship : Actor
{
    private readonly GL _gl;
    private float _laserCooldown = 0.0f;

    public Ship(GL gl, Game game) : base(game)
    {
        _gl = gl;

        // Create a sprite component
        var sprite = new SpriteComponent(gl, this, 150);
        sprite.Texture = Game.GetTexture("Assets/Ship.png");

        // Create an input component and set keys/speed
        var input = new InputComponent(this)
        {
            ForwardKey = Key.W,
            BackKey = Key.S,
            ClockwiseKey = Key.A,
            CounterClockwiseKey = Key.D,
            MaxForwardSpeed = 300.0f,
            MaxAngularSpeed = GameMath.TwoPi,
        };
    }

    protected override void UpdateActor(float deltaTime)
    {
        _laserCooldown -= deltaTime;
    }

    protected override void ActorInput(IKeyboard keyboard)
    {
        if (keyboard.IsKeyPressed(Key.Space) && _laserCooldown <= 0.0f)
        {
            // Create a laser and set its position/rotation to mine
            var laser = new Laser(_gl, Game)
            {
                Position = Position,
                Rotation = Rotation,
            };

            // Reset laser cooldown (half second)
            _laserCooldown = 0.5f;
        }
    }
}