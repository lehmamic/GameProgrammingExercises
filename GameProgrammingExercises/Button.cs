using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class Button
{
    private readonly Action? _onClick;

    public Button(string name, Action onClick, Vector2D<float> position, Vector2D<float> dimensions)
    {
        Name = name;
        _onClick = onClick;
        Position = position;
        Dimensions = dimensions;
    }

    public string Name { get; }

    public Vector2D<float> Position { get; }

    public Vector2D<float> Dimensions { get; }

    public bool Highlighted { get; set; }
    
    public bool ContainsPoint(Vector2D<float> pt)
    {
        bool no = pt.X < (Position.X - Dimensions.X / 2.0f) ||
                  pt.X > (Position.X + Dimensions.X / 2.0f) ||
                  pt.Y < (Position.Y - Dimensions.Y / 2.0f) ||
                  pt.Y > (Position.Y + Dimensions.Y / 2.0f);
        return !no;
    }

    public void Click()
    {
        // Call attached handler, if it exists
        if (_onClick is not null)
        {
            _onClick();
        }
    }
}