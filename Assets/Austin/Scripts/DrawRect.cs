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
    private List<Vector2> uv;
    private List<Vector3> vertices;
    private List<int> triangles;

    private int pointCount;

    // Start is called before the first frame update
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        points = new List<Vector2>();
    }

    public void SetPoints(List<Vector2> newPoints)
    {
        pointCount = newPoints.Count;
        this.points = newPoints;
    }

    // Given a set of points, draws parallelograms connecting the points.
    public void Draw()
    {
        CalculateVertices();
        CalculateTris();
        CalculateUV();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = vertices.ConvertAll<Vector3>( x => -Vector3.forward ).ToArray();
        mesh.uv = uv.ToArray();

        meshFilter.mesh = mesh;

        PhysicsShapeGroup2D shapes = new PhysicsShapeGroup2D();
        shapes.AddPolygon(vertices.ConvertAll<Vector2>( x => x ));
        GetComponent<CustomCollider2D>().SetCustomShapes(shapes);
    }

    private void CalculateVertices()
    {
        vertices = new List<Vector3>(pointCount * 2);
        for (int i = 0; i < pointCount; i++)
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

    private void CalculateTris()
    {
        triangles = new List<int>(pointCount * 3);
        for (int i = 0; i < pointCount - 1; i++) {
            triangles.Add(i * 2);
            triangles.Add(i * 2 + 2);
            triangles.Add(i * 2 + 1);
            triangles.Add(i * 2 + 1);
            triangles.Add(i * 2 + 2);
            triangles.Add(i * 2 + 3);
        };
    }

    private void CalculateUV()
    {
        uv = new List<Vector2>(pointCount * 2);
        for (int i = 0; i < pointCount; i++) {
            if (i % 2 == 0)
            {
                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(1, 0));
            }
            else
            {
                uv.Add(new Vector2(0, 1));
                uv.Add(new Vector2(1, 1));
            }
        };
    }
}
