using System.Collections.Generic;
using UnityEngine;

public static class BezierUtility {
    public static Vector2 GetInterpolationPosition(List<Vector2> points, float interpolation) {
        return GetInterpolationPosition(points.ToArray(), interpolation);
    }

    public static Vector2 GetInterpolationPosition(Vector2[] points, float interpolation) {
        // NOTE:
        // Reference to wiki "Bézier curve"
        int pointCount = points != null ? points.Length : 0;
        if (pointCount <= 1) {
            return Vector2.zero;
        }

        interpolation = Mathf.Clamp(interpolation, 0.0f, 1.0f);

        // NOTE:
        // If length of 'points' is 4, and interpolation = t (time), then
        //    position =
        //    Mathf.Pow(1 - t, 3) * _controlPoints[0].position +
        //    3 * Mathf.Pow(1 - t, 2) * t * _controlPoints[1].position +
        //    3 * (1 - t) * Mathf.Pow(t, 2) * _controlPoints[2].position +
        //    Mathf.Pow(t, 3) * _controlPoints[3].position;

        Vector2 pos = Vector2.zero;
        int n = pointCount - 1;
        for (int i = 0; i < pointCount; i++) {
            pos += points[i] *
                GetBinomialCoefficientValue(n, i) *
                Mathf.Pow(interpolation, i) *
                Mathf.Pow(1 - interpolation, n - i);
        }

        return pos;
    }

    private static int GetBinomialCoefficientValue(int n, int k) {
        // NOTE:
        // Return the value C(n, k) = n! / (k! * (n - k)!)

        if (n < 0) {
            return 0;
        }

        if (n < k) {
            return 0;
        }

        return GetFactorialValue(n) / (GetFactorialValue(k) * GetFactorialValue(n - k));
    }

    private static int GetFactorialValue(int n) {
        // NOTE:
        // Return the value n!

        if (n <= 1) {
            return 1;
        }

        return n * GetFactorialValue(n - 1);
    }
}
