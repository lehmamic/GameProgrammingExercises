using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class Character
{
    public Character(char @char, Texture texture, Vector2D<float> size, Vector2D<float> bearing, int advance)
    {
        Char = @char;
        Texture = texture;
        Size = size;
        Bearing = bearing;
        Advance = advance;
    }

    public char Char { get; }

    public Texture Texture { get; }

    public Vector2D<float> Size { get; }

    public Vector2D<float> Bearing { get; }

    public int Advance { get; }
}