using Silk.NET.OpenGL;

namespace GameProgrammingExercises;

public class Laser : Actor
{
    private readonly CircleComponent _circle;

    private float _deathTimer = 1.0f;

    public Laser(GL gl,Game game)
        : base(game)
    {
        // Create a sprite component
        var sprite = new SpriteComponent(gl, this)
        {
            Texture = game.GetTexture("Assets/Laser.png")
        };

        // Create a move component, and set a forward speed
        var move = new MoveComponent(this)
        {
            ForwardSpeed = 800.0f
        };

        // Create a circle component (for collision)
        _circle = new CircleComponent(this)
        {
            Radius = 11.0f
        };
    }

    protected override void UpdateActor(float deltaTime)
    {
        // If we run out of time, laser is dead
        _deathTimer -= deltaTime;
        if (_deathTimer <= 0.0f) {
            State = ActorState.Dead;
        }
        else {
            // Do you intersect with an asteroid?
            foreach (var asteroid in Game.Asteroids) {
                if (CircleComponent.Intersect(_circle, asteroid.Circle)) {
                    // If this laser intersects with an asteroid,
                    // set ourselves an the asteroid to dead
                    State = ActorState.Dead;
                    asteroid.State = ActorState.Dead;
                    break;
                }
            }
        }
    }
}