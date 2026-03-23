using UnityEngine;
using Vector3 = System.Numerics.Vector3;

public class LineGen : MonoBehaviour
{
    public Material material;

    public float size = 1.5f;

    public Vector2 pyramidPos;
    public float pyramidZ;

    public Vector2 spherePos;
    public float sphereZ;

    public float xRot = 0;
    public float yRot = 0;
    public float zRot = 0;

    public int sphereSegments = 16;

    private void OnPostRender()
    {
        if (material == null)
        {
            Debug.LogError("Assign a material!");
            return;
        }

        material.SetPass(0);
        GL.PushMatrix();
        GL.Begin(GL.LINES);

        DrawPyramid(pyramidPos, pyramidZ);
        DrawSphere(spherePos, sphereZ);

        GL.End();
        GL.PopMatrix();
    }

    void DrawPyramid(Vector2 pos, float z)
    {
        float s = size;

        Vector3 top = new Vector3(pos.x, pos.y + s, z);
        Vector3[] basePts = new Vector3[]
        {
            new Vector3(pos.x - s, pos.y - s, z - s),
            new Vector3(pos.x + s, pos.y - s, z - s),
            new Vector3(pos.x + s, pos.y - s, z + s),
            new Vector3(pos.x - s, pos.y - s, z + s),
        };

        top = RotatePoint(top);
        for (int i = 0; i < 4; i++)
            basePts[i] = RotatePoint(basePts[i]);

        for (int i = 0; i < 4; i++)
            DrawEdge(basePts[i], basePts[(i + 1) % 4]);

        for (int i = 0; i < 4; i++)
            DrawEdge(basePts[i], top);
    }

    void DrawSphere(Vector2 pos, float z)
    {
        float r = size;

        for (int lat = -sphereSegments / 2; lat <= sphereSegments / 2; lat++)
        {
            float y = r * lat / (sphereSegments / 2);
            float ringRadius = Mathf.Sqrt(r * r - y * y);

            for (int i = 0; i < sphereSegments; i++)
            {
                float a1 = 2 * Mathf.PI * i / sphereSegments;
                float a2 = 2 * Mathf.PI * (i + 1) / sphereSegments;

                Vector3 p1 = new Vector3(
                    pos.x + ringRadius * Mathf.Cos(a1),
                    pos.y + y,
                    z + ringRadius * Mathf.Sin(a1)
                );

                Vector3 p2 = new Vector3(
                    pos.x + ringRadius * Mathf.Cos(a2),
                    pos.y + y,
                    z + ringRadius * Mathf.Sin(a2)
                );

                p1 = RotatePoint(p1);
                p2 = RotatePoint(p2);

                DrawEdge(p1, p2);
            }
        }
    }

    void DrawEdge(Vector3 p1, Vector3 p2)
    {
        float p1Persp = PerspectiveCamera.Instance.GetPerspective(p1.Z);
        float p2Persp = PerspectiveCamera.Instance.GetPerspective(p2.Z);

        Vector2 v1 = new Vector2(p1.X, p1.Y) * p1Persp;
        Vector2 v2 = new Vector2(p2.X, p2.Y) * p2Persp;

        GL.Vertex(v1);
        GL.Vertex(v2);
    }

    Vector3 RotatePoint(Vector3 p)
    {
        float xRad = xRot * Mathf.Deg2Rad;
        float yRad = yRot * Mathf.Deg2Rad;
        float zRad = zRot * Mathf.Deg2Rad;

        float x1 = p.X * Mathf.Cos(zRad) - p.Y * Mathf.Sin(zRad);
        float y1 = p.X * Mathf.Sin(zRad) + p.Y * Mathf.Cos(zRad);
        float z1 = p.Z;

        float y2 = y1 * Mathf.Cos(xRad) - z1 * Mathf.Sin(xRad);
        float z2 = y1 * Mathf.Sin(xRad) + z1 * Mathf.Cos(xRad);
        float x2 = x1;

        float x3 = x2 * Mathf.Cos(yRad) + z2 * Mathf.Sin(yRad);
        float z3 = -x2 * Mathf.Sin(yRad) + z2 * Mathf.Cos(yRad);
        float y3 = y2;

        return new Vector3(x3, y3, z3);
    }
}
