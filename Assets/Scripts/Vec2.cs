using System.Collections.Generic;
using UnityEngine;

public class Vec2
{
    public int R;
    public int C;

    public Vec2 parent;
    public float g, h, f;
    public void CalculateF()
    {
        f = g + h;
    }
    public Vec2()
    {
        R = 0;
        C = 0;
    }

    public Vec2(int r, int c)
    {
        R = r;
        C = c;
    }

    public static Vec2 GetLowestFNode(List<Vec2> node)
    {
        Vec2 lowest = node[0];

        for (int i = 1; i < node.Count; i++)
        {
            if (node[i].f < lowest.f)
            {
                lowest = node[i];
            }
        }

        return lowest;
    }

    public static List<Vec2> GeneratePath(Vec2 node)
    {
        List<Vec2> path = new List<Vec2>();
        path.Add(node);

        while (node.parent != null)
        {
            path.Add(node.parent);
            node = node.parent;
        }

        path.Reverse();
        return path;
    }
    public static float Heuristic(Vec2 a, Vec2 b)
    {
        // Manhattan
        float distance = Mathf.Abs(a.R - b.R) + Mathf.Abs(a.C - b.C);
        return distance;
    }

    public static int r, c;
    public static int count = 0;
    
    public string Print()
    {
        string r = "(" + R + "," + C + ")";
        return r;
    }
}
