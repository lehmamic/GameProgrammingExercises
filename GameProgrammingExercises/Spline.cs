using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class Spline
{
    /// Control points for spline
    /// (Requires n+2 points where n is number
    /// of points in segment)
    private readonly IReadOnlyList<Vector3D<float>> _controlPoints;
    
    public Spline(IReadOnlyList<Vector3D<float>> controlPoints)
    {
        _controlPoints = controlPoints;
    }

    /// <summary>
    /// Returns number of control points
    /// </summary>
    public int NumPoints => _controlPoints.Count;

    /// <summary>
    /// Given spline segment where startIdx = P1,
    /// compute position based on t value
    /// </summary>
    public Vector3D<float> Compute(int startIdx, float t)
    {
        // Check if startIdx is out of bounds
        if (startIdx >= _controlPoints.Count)
        {
            return _controlPoints.Last();
        }
        else if (startIdx == 0)
        {
            return _controlPoints[startIdx];
        }
        else if (startIdx + 2 >= _controlPoints.Count)
        {
            return _controlPoints[startIdx];
        }

        // Get p0 through p3
        Vector3D<float> p0 = _controlPoints[startIdx - 1];
        Vector3D<float> p1 = _controlPoints[startIdx];
        Vector3D<float> p2 = _controlPoints[startIdx + 1];
        Vector3D<float> p3 = _controlPoints[startIdx + 2];

        // Compute position according to Catmull-Rom equation
        Vector3D<float> position = 0.5f * ((2.0f * p1) + (-1.0f * p0 + p2) * t +
                                           (2.0f * p0 - 5.0f * p1 + 4.0f * p2 - p3) * t * t +
                                           (-1.0f * p0 + 3.0f * p1 - 3.0f * p2 + p3) * t * t * t);
        return position;
    }
}