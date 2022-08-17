using GameProgrammingExercises.Maths;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameProgrammingExercises;

public class Asteroid : Actor
{
    private readonly GL _gl;
    private readonly CircleComponent _circle;

    public Asteroid(GL gl, Game game)
        : base(game)
    {
        // Initialize to random position/orientation
        Position = VectorRandom.GetVector(new Vector2D<float>(-512.0f, -384.0f), new Vector2D<float>(512.0f, 384.0f));
        Rotation = VectorRandom.GetFloatRange(0.0f, (float)Math.PI * 2);

        // Create a sprite component
        var sprite = new SpriteComponent(gl, this);
        sprite.Texture = Game.GetTexture("Assets/Asteroid.png");

        // Create a move component, and set a forward speed
        MoveComponent mc = new MoveComponent(this);
        mc.ForwardSpeed = 150.0f;

        // Create a circle component (for collision)
        _circle = new CircleComponent(this);
        _circle.Radius = 40.0f;

        // Add to mAsteroids in game
        Game.AddAsteroid(this);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Game.RemoveAsteroid(this);
        }

        base.Dispose(disposing);
    }
}