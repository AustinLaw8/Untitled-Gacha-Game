using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRect : MonoBehaviour
{
    private float INNER_BUMP = .01f;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh mesh;

    private List<(float time, int lane)> points;
    private List<Vector2> uv;
    private List<Vector3> vertices;
    private List<int> triangles;

    private float timer;
    private float fallTime;

    // Start is called before the first frame update
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        points = new List<(float time, int lane)>();
        timer = 0f;
        fallTime = Lane.DISTANCE / Note.fallSpeed;
    }

    void Update()
    {
        timer += Time.deltaTime;

        UpdateAll();
    }

    // Given a set of points, draws parallelograms connecting the points.
    public void Init(List<(float, int)> newPoints)
    {
        points = newPoints;
        vertices = new List<Vector3>(points.Count * 2);
        triangles = new List<int>(points.Count * 3);
        uv = new List<Vector2>(points.Count * 2);
        
        UpdateAll();
    }

    private void UpdateAll()
    {
        CalculateVertices();
        CalculateTris();
        CalculateUV();

        if (vertices.Count >= 3)
        {
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.normals = vertices.ConvertAll<Vector3>( x => -Vector3.forward ).ToArray();
            mesh.uv = uv.ToArray();

            meshFilter.mesh = mesh;
            
            SetCollider();
        }
    }

    private void SetCollider()
    {
        List<Vector2> shapeVertices = new List<Vector2>();
        shapeVertices.Add(vertices[0]);
        shapeVertices.Add(vertices[1]);
        for(int i = 3; i < vertices.Count; i += 2)
            shapeVertices.Add(vertices[i]);
        for(int i = vertices.Count - 2; i > 1; i -= 2)
            shapeVertices.Add(vertices[i]);
        GetComponent<PolygonCollider2D>().SetPath(0,shapeVertices.ToArray());
    }

    private void CalculateVertices()
    {
        vertices.Clear();
        bool first = true;
        foreach( (float time, int lane) x in points)
        {
            float y;
            float temp = Mathf.Pow((timer - x.time)/fallTime, 2);
            if (x.time > timer)
            {
                y = BeatManager.SPAWN_POINT + temp  * Lane.DISTANCE;
                if (first)
                {
                    vertices.Add( new Vector3(Lane.LANE_LINES_FOR_OFFSET[x.lane].getX(5f) + INNER_BUMP, 5f, -1f) );
                    vertices.Add( new Vector3(Lane.LANE_LINES_FOR_OFFSET[x.lane + 1].getX(5f) - INNER_BUMP, 5f, -1f) );
                    first = false;
                }
                else
                {
                    vertices.Add( new Vector3(Lane.LANE_LINES_FOR_OFFSET[x.lane].getX(5f) + INNER_BUMP, y, -1f) );
                    vertices.Add( new Vector3(Lane.LANE_LINES_FOR_OFFSET[x.lane + 1].getX(5f) - INNER_BUMP, y, -1f) );
                }
            }
            else
            {
                y = BeatManager.SPAWN_POINT - temp * Lane.DISTANCE;
                vertices.Add( new Vector3(Lane.LANE_LINES_FOR_OFFSET[x.lane].getX(y) + INNER_BUMP, y, -1f) );
                vertices.Add( new Vector3(Lane.LANE_LINES_FOR_OFFSET[x.lane + 1].getX(y) - INNER_BUMP, y, -1f) );
            }
        }
    }

    private void CalculateTris()
    {
        triangles.Clear();
        for (int i = 0; i < vertices.Count / 2 - 1; i++) {
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
        uv.Clear();
        for (int i = 0; i < vertices.Count / 2; i++) {
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
