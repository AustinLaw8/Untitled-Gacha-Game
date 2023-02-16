using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRect : MonoBehaviour
{
    private static float X_OFFSET=.5f;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh mesh;

    private List<Vector2> points;
    private List<Vector3> vertices;
    private List<Vector3> triangles;

    // Start is called before the first frame update
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        points = new List<Vector2>();
    }

    public void SetPoints(List<Vector2> points)
    {
        this.points = points;
    }

    public void Draw()
    {
        Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };

        CalculateVertices();
        // CalculateTris();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = CalculateTris();
        mesh.normals = normals;
        mesh.uv = uv;

        meshFilter.mesh = mesh;

        PhysicsShapeGroup2D shapes = new PhysicsShapeGroup2D();
        // List<Vector2> verts = new List<Vector2>()
        // {
        //     mesh.vertices[0],
        //     mesh.vertices[1],
        //     mesh.vertices[3],
        //     mesh.vertices[2],
        // };
        shapes.AddPolygon(vertices.ConvertAll<Vector2>( x => x ));
        GetComponent<CustomCollider2D>().SetCustomShapes(shapes);
    }

    private void CalculateVertices()
    {
        vertices = new List<Vector3>(points.Count * 2);
        for (int i = 0; i < points.Count; i++)
        {
            vertices.Add(new Vector3(
                points[i].x - X_OFFSET,
                points[i].y,
                0f
            ));
            vertices.Add(new Vector3(
                points[i].x + X_OFFSET,
                points[i].y,
                0f
            ));
        }
    }

    private int[] CalculateTris()
    {
        int[] tris = new int[6]{
            0, 2, 1,
            1, 2, 3,
        };
        // int[] tris = new int[points.Count * 3];
        // for (int i = 0; i < points.Count; i++);
        // {
        //     0, 2, 1,
        //     2, 3, 1
        // };
        return tris;
    }
}
