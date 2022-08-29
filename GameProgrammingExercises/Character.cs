using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class Character
{
    public Character(char @char, Texture texture, Vector2D<float> site, Vector2D<float> bearing, int advance)
    {
        Char = @char;
        Texture = texture;
        Site = site;
        Bearing = bearing;
        Advance = advance;
    }

    public char Char { get; }

    public Texture Texture { get; }

    public Vector2D<float> Site { get; }

    public Vector2D<float> Bearing { get; }

    public int Advance { get; }
}