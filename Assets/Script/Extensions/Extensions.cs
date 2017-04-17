using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Extensions
{
    public static double NextDouble(this System.Random rand, double max)
    {
        return rand.NextDouble() * max;
    }

    public static double NextDouble(this System.Random rand, double min, double max)
    {
        return min + (rand.NextDouble() * (max - min));
    }

    public static Vector2[] ToVector2(this Vector3[] vec3)
    {
        Vector2[] output = new Vector2[vec3.Length];
        for(int i = 0; i < vec3.Length; i++)
        {
            output[i] = new Vector2(vec3[i].x, vec3[i].z);
        }
        return output;
    }

    public static Vector3 Average(this Vector3 vec3, Vector3 invec3)
    {
        return (vec3 + invec3) / 2f;
    }

    public static float MidX(this Rect rect)
    {
        return (rect.xMin + rect.width / 2f);
    }
    public static float MidY(this Rect rect)
    {
        return (rect.yMin + rect.height / 2);
    }
}

