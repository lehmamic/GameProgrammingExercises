using Silk.NET.Maths;

namespace GameProgrammingExercises.Maths.Geometry;

public struct LineSegment
{
    public Vector3D<float> Start;
    public Vector3D<float> End;

    public LineSegment(Vector3D<float> start, Vector3D<float> end)
    {
        Start = start;
        End = end;
    }

    /// <summary>
    /// Get point along segment where 0 <= t <= 1
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3D<float> PointOnSegment(float t)
    {
        return Start + (End - Start) * t;
    }

    /// <summary>
    /// Get MinDistSq between two line segments
    /// </summary>
    /// <param name="s1"></param>
    /// <param name="s2"></param>
    /// <returns></returns>
    public static float MinDistSq(LineSegment s1, LineSegment s2)
    {
        Vector3D<float> u = s1.End - s1.Start;
        Vector3D<float> v = s2.End - s2.Start;
        Vector3D<float> w = s1.Start - s2.Start;
        float a = Vector3D.Dot(u, u);         // always >= 0
        float b = Vector3D.Dot(u, v);
        float c = Vector3D.Dot(v, v);         // always >= 0
        float d = Vector3D.Dot(u, w);
        float e = Vector3D.Dot(v, w);
        float D = a * c - b * b;    // always >= 0
        float sc, sN, sD = D;       // sc = sN / sD, default sD = D >= 0
        float tc, tN, tD = D;       // tc = tN / tD, default tD = D >= 0

        // compute the line parameters of the two closest points
        if (D.NearZero()) // the lines are almost parallel
        {
            sN = 0.0f;         // force using point P0 on segment S1
            sD = 1.0f;         // to prevent possible division by 0.0 later
            tN = e;
            tD = c;
        }
        else // get the closest points on the infinite lines
        {
            sN = (b*e - c*d);
            tN = (a*e - b*d);
            if (sN < 0.0f) // sc < 0 => the s=0 edge is visible
            {
                sN = 0.0f;
                tN = e;
                tD = c;
            }
            else if (sN > sD) // sc > 1  => the s=1 edge is visible
            {
                sN = sD;
                tN = e + b;
                tD = c;
            }
        }

        if (tN < 0.0f) // tc < 0 => the t=0 edge is visible
        {
            tN = 0.0f;
            // recompute sc for this edge
            if (-d < 0.0f)
            {
                sN = 0.0f;
            }
            else if (-d > a)
            {
                sN = sD;
            }
            else
            {
                sN = -d;
                sD = a;
            }
        }
        else if (tN > tD) // tc > 1  => the t=1 edge is visible
        {
            tN = tD;
            // recompute sc for this edge
            if ((-d + b) < 0.0)
            {
                sN = 0;
            }
            else if ((-d + b) > a)
            {
                sN = sD;
            }
            else
            {
                sN = (-d + b);
                sD = a;
            }
        }
        // finally do the division to get sc and tc
        sc = (sN.NearZero() ? 0.0f : sN / sD);
        tc = (tN.NearZero() ? 0.0f : tN / tD);

        // get the difference of the two closest points
        Vector3D<float> dP = w + (sc * u) - (tc * v);  // =  S1(sc) - S2(tc)

        return dP.LengthSquared;   // return the closest distance squared
    }

    /// <summary>
    /// Get minimum distance squared between point and line segment
    /// </summary>
    /// <returns></returns>
    public float MinDistSq(Vector3D<float> point)
    {
        // Construct vectors
        Vector3D<float> ab = End - Start;
        Vector3D<float> ba = -1.0f * ab;
        Vector3D<float> ac = point - Start;
        Vector3D<float> bc = point - End;

        // Case 1: C projects prior to A
        if (Vector3D.Dot(ab, ac) < 0.0f)
        {
            return ac.LengthSquared;
        }
        // Case 2: C projects after B
        else if (Vector3D.Dot(ba, bc) < 0.0f)
        {
            return bc.LengthSquared;
        }
        // Case 3: C projects onto line
        else
        {
            // Compute p
            float scalar = Vector3D.Dot(ac, ab)
                           / Vector3D.Dot(ab, ab);
            Vector3D<float> p = scalar * ab;

            // Compute length squared of ac - p
            return (ac - p).LengthSquared;
        }
    }
};