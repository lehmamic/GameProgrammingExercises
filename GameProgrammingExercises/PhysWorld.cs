using System.Diagnostics.CodeAnalysis;
using GameProgrammingExercises.Maths;
using GameProgrammingExercises.Maths.Geometry;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class PhysWorld
{
    private readonly Game _game;
    private readonly List<BoxComponent> _boxes = new();

    public PhysWorld(Game game)
    {
        _game = game;
    }
    
    /// <summary>
    /// Test a line segment against boxes
    /// Returns true if it collides against a box
    /// </summary>
    /// <param name="l"></param>
    /// <param name="outColl"></param>
    /// <returns></returns>
    public bool SegmentCast(LineSegment l, [MaybeNullWhen(false)]out CollisionInfo outColl)
    {
        bool collided = false;
        // Initialize closestT to infinity, so first
        // intersection will always update closestT
        float closestT = Scalar<float>.PositiveInfinity;

        outColl = null;

        // Test against all boxes
        foreach (var box in _boxes)
        {
            // Does the segment intersect with the box?
            if (Collision.Intersect(l, box.WorldBox, out var t, out var norm))
            {
                // Is this closer than previous intersection?
                if (t < closestT)
                {
                    closestT = t;
                    outColl = new CollisionInfo(l.PointOnSegment(t), norm, box, box.Owner);
                    collided = true;
                }
            }
        }
        return collided;
    }

    /// <summary>
    /// Tests collisions using naive pairwise
    /// </summary>
    /// <param name="f"></param>
    public void TestPairwise(Action<Actor, Actor> f)
    {
        // Naive implementation O(n^2)
        for (int i = 0; i < _boxes.Count; i++)
        {
            // Don't need to test vs itself and any previous i values
            for (int j = i + 1; j < _boxes.Count; j++)
            {
                BoxComponent a = _boxes[i];
                BoxComponent b = _boxes[j];
                if (Collision.Intersect(a.WorldBox, b.WorldBox))
                {
                    // Call supplied function to handle intersection
                    f(a.Owner, b.Owner);
                }
            }
        }
    }

    /// <summary>
    /// Test collisions using sweep and prune
    /// </summary>
    /// <param name="f"></param>
    public void TestSweepAndPrune(Action<Actor, Actor> f)
    {
        // Sort by min.x
        var boxes = _boxes
            .OrderBy(b => b.WorldBox.Min.X)
            .ToList();

        for (int i = 0; i < boxes.Count; i++)
        {
            // Get max.x for current box
            BoxComponent a = boxes[i];
            float max = a.WorldBox.Max.X;
            for (int j = i + 1; j < boxes.Count; j++)
            {
                BoxComponent b = boxes[j];
                // If AABB[j] min is past the max bounds of AABB[i],
                // then there aren't any other possible intersections
                // against AABB[i]
                if (b.WorldBox.Min.X > max)
                {
                    break;
                }
                else if (Collision.Intersect(a.WorldBox, b.WorldBox))
                {
                    f(a.Owner, b.Owner);
                }
            }
        }
    }

    public void AddBox(BoxComponent box)
    {
        _boxes.Add(box);
    }

    public void RemoveBox(BoxComponent box)
    {
        _boxes.Remove(box);
    }
}