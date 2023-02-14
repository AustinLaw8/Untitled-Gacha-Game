using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRect : MonoBehaviour
{
    // Start is called before the first frame update
    public void Start()
    {
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        // meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-.5f, -4f, 0f),
            new Vector3(.5f, -4f, 0f),
            new Vector3(3.5f, 0f, 0f),
            new Vector3(4.5f, 0f, 0f)
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            0, 2, 1,
            2, 3, 1
        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;

        meshFilter.mesh = mesh;
        PhysicsShapeGroup2D shapes = new PhysicsShapeGroup2D();
        List<Vector2> verts = new List<Vector2>()
        {
            new Vector3(-.5f, -4f, 0f),
            new Vector3(.5f, -4f, 0f),
            new Vector3(4.5f, 0f, 0f),
            new Vector3(3.5f, 0f, 0f),
        };
        shapes.AddPolygon(verts);
        GetComponent<CustomCollider2D>().SetCustomShapes(shapes);
        // gameObject.AddComponent<MeshCollider>();
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
