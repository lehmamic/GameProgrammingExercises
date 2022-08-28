using GameProgrammingExercises.Maths.Geometry;
using Silk.NET.Maths;
using Plane = GameProgrammingExercises.Maths.Geometry.Plane;

namespace GameProgrammingExercises.Maths;

public static class Collision
{
    public static bool Intersect(Sphere a, Sphere b)
    {
        float distSq = (a.Center - b.Center).LengthSquared;
        float sumRadii = a.Radius + b.Radius;
        return distSq <= (sumRadii * sumRadii);
    }

    public static bool Intersect(AABB a, AABB b)
    {
        bool no = a.Max.X < b.Min.X ||
            a.Max.Y < b.Min.Y ||
            a.Max.Z < b.Min.Z ||
            b.Max.X < a.Min.X ||
            b.Max.Y < a.Min.Y ||
            b.Max.Z < a.Min.Z;
        // If none of these are true, they must intersect
        return !no;
    }

    public static bool Intersect(Capsule a, Capsule b)
    {
        float distSq = LineSegment.MinDistSq(a.Segment, b.Segment);
        float sumRadii = a.Radius + b.Radius;
        return distSq <= (sumRadii * sumRadii);
    }

    public static bool Intersect(Sphere s, AABB box)
    {
        float distSq = box.MinDistSq(s.Center);
        return distSq <= (s.Radius * s.Radius);
    }

    public static bool Intersect(LineSegment l, Sphere s, out float outT)
    {
        // Compute X, Y, a, b, c as per equations
        Vector3D<float> X = l.Start - s.Center;
        Vector3D<float> Y = l.End - l.Start;
        float a = Vector3D.Dot(Y, Y);
        float b = 2.0f * Vector3D.Dot(X, Y);
        float c = Vector3D.Dot(X, X) - s.Radius * s.Radius;
        // Compute discriminant
        float disc = b * b - 4.0f * a * c;
        if (disc < 0.0f)
        {
            outT = 0.0f;
            return false;
        }
        else
        {
            disc = Scalar.Sqrt(disc);
            // Compute min and max solutions of t
            float tMin = (-b - disc) / (2.0f * a);
            float tMax = (-b + disc) / (2.0f * a);
            // Check whether either t is within bounds of segment
            if (tMin >= 0.0f && tMin <= 1.0f)
            {
                outT = tMin;
                return true;
            }
            else if (tMax >= 0.0f && tMax <= 1.0f)
            {
                outT = tMax;
                return true;
            }
            else
            {
                outT = 0.0f;
                return false;
            }
        }
    }

    public static bool Intersect(LineSegment l, Plane p, out float outT)
    {
        // First test if there's a solution for t
        float denom = Vector3D.Dot(l.End - l.Start, p.Normal);
        if (denom.NearZero())
        {
            outT = 0.0f;
            // The only way they intersect is if start
            // is a point on the plane (P dot N) == d
            if ((Vector3D.Dot(l.Start, p.Normal) - p.D).NearZero())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            float numer = -Vector3D.Dot(l.Start, p.Normal) - p.D;
            outT = numer / denom;
            // Validate t is within bounds of the line segment
            if (outT >= 0.0f && outT <= 1.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public static bool TestSidePlane(float start, float end, float negd, Vector3D<float> norm, List<(float T, Vector3D<float> Norm)> outNorms)
    {
        float denom = end - start;
        if (denom.NearZero())
        {
            outNorms = null;
            return false;
        }
        else
        {
            float numer = -start + negd;
            float t = numer / denom;
            // Test that t is within bounds
            if (t >= 0.0f && t <= 1.0f)
            {
                outNorms.Add((t, norm));
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public static bool Intersect(LineSegment l, AABB b, out float outT, out Vector3D<float> outNorm)
    {
        // Vector to save all possible t values, and normals for those sides
        var values = new List<(float T, Vector3D<float> Norm)>();
        // Test the x planes
        TestSidePlane(l.Start.X, l.End.X, b.Min.X, Vector3D<float>.UnitX * -1, values);
        TestSidePlane(l.Start.X, l.End.X, b.Max.X, Vector3D<float>.UnitX, values);
        // Test the y planes
        TestSidePlane(l.Start.Y, l.End.Y, b.Min.Y, Vector3D<float>.UnitY * -1, values);
        TestSidePlane(l.Start.Y, l.End.Y, b.Max.Y, Vector3D<float>.UnitY, values);
        // Test the z planes
        TestSidePlane(l.Start.Z, l.End.Z, b.Min.Z, Vector3D<float>.UnitZ * -1, values);
        TestSidePlane(l.Start.Z, l.End.Z, b.Max.Z, Vector3D<float>.UnitZ, values);
        
        // Sort the t values in ascending order
        values = values.OrderBy(v => v.T).ToList();

        // Test if the box contains any of these points of intersection
        Vector3D<float> point;
        foreach (var t in values)
        {
            point = l.PointOnSegment(t.T);
            if (b.Contains(point))
            {
                outT = t.T;
                outNorm = t.Norm;
                return true;
            }
        }

        // None of the intersections are within bounds of box
        outT = 0.0f;
        outNorm = Vector3D<float>.Zero;
        return false;
    }

    public static bool SweptSphere(Sphere P0, Sphere P1, Sphere Q0, Sphere Q1, out float outT)
    {
        // Compute X, Y, a, b, and c
        Vector3D<float> X = P0.Center - Q0.Center;
        Vector3D<float> Y = P1.Center - P0.Center - (Q1.Center - Q0.Center);
        float a = Vector3D.Dot(Y, Y);
        float b = 2.0f * Vector3D.Dot(X, Y);
        float sumRadii = P0.Radius + Q0.Radius;
        float c = Vector3D.Dot(X, X) - sumRadii * sumRadii;
        // Solve discriminant
        float disc = b * b - 4.0f * a * c;
        if (disc < 0.0f)
        {
            outT = 0.0f;
            return false;
        }
        else
        {
            disc = Scalar.Sqrt(disc);
            // We only care about the smaller solution
            outT = (-b - disc) / (2.0f * a);
            if (outT >= 0.0f && outT <= 0.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}