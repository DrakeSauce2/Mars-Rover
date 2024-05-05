using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator : MonoBehaviour
{
    public static ShapeGenerator Instance;

    private MeshFilter meshFilter;

    private float targetDistance = 0;
    private float targetAngle = 0;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(Instance);

        meshFilter = GetComponent<MeshFilter>();
    }

    public float GetTargetDistance()
    {
        return targetDistance;
    }

    public float GetTargetAngle()
    {
        return targetAngle;
    }

    #region Shape Creation

    public void ClearMesh()
    {
        meshFilter.mesh.Clear();
        meshFilter.mesh = null;
    }

    public void GenerateTriangleFromLine(Vector2 startPos, Vector2 endPosition, GameplayUI gui)
    {
        meshFilter.mesh.Clear();

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        Vector3[] vertices = new Vector3[3];
        vertices[0] = new Vector3(startPos.x, startPos.y, 0); // start point
        vertices[1] = new Vector3(endPosition.x, endPosition.y, 0); // end point
        vertices[2] = new Vector3(endPosition.x, startPos.y, 0); // I want this to be the imaginary
        
        mesh.vertices = vertices;
        mesh.triangles = GetTriangle(vertices);
        mesh.uv = GetUVS();
        mesh.normals = GetNormals();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        UpdateGUISideLengths(vertices, gui);
        SetTargetDistanceAndAngle(vertices);

        Vector3 direction = vertices[1] - vertices[0];
        float xDirection = direction.x >= 0 ? 1 : -1;
        float yDirection = direction.y >= 0 ? 1 : -1;

        gui.SetHypTextZAngle(targetAngle * xDirection * yDirection);
    }

    private void SetTargetDistanceAndAngle(Vector3[] verts)
    {
        float adjacentLength = Vector2.Distance(verts[0], verts[2]);
        float oppositeLength = Vector2.Distance(verts[2], verts[1]);

        targetDistance = Mathf.Sqrt(Mathf.Pow(adjacentLength, 2) + Mathf.Pow(oppositeLength, 2));
        
        targetAngle = Mathf.Atan(oppositeLength / adjacentLength);

        bool IsDegrees = true;
        if (IsDegrees)
        {
            targetAngle *= Mathf.Rad2Deg;
        }

        Debug.Log("Distance: " + targetDistance + " Angle: " + targetAngle);
    }

    private int[] GetTriangle(Vector3[] verts)
    {
        int p1, p2, p3;
        SetTriangleVertPointValue(verts, out p1, out p2, out p3);

        int[] triangles = new int[3];
        triangles.SetValue(0, p1);
        triangles.SetValue(1, p2);
        triangles.SetValue(2, p3);

        return triangles;
    }

    private void SetTriangleVertPointValue(Vector3[] verts, out int p1, out int p2, out int p3)
    {
        Vector3 direction = verts[1] - verts[0];
        if (direction.y >= 0 && direction.x >= 0)
        {
            p1 = 0;
            p2 = 1;
            p3 = 2;
        }
        else if (direction.y < 0 && direction.x >= 0)
        {
            p1 = 0;
            p2 = 2;
            p3 = 1;
        }
        else if (direction.y >= 0 && direction.x < 0)
        {
            p1 = 2;
            p2 = 1;
            p3 = 0;
        }
        else
        {
            p1 = 1;
            p2 = 2;
            p3 = 0;
        }
    }

    private void UpdateGUISideLengths(Vector3[] verts, GameplayUI gui)
    {
        Vector3 adjMidpoint = FindMidpoint(verts[0], verts[2]);

        Vector3 oppMidpoint = FindMidpoint(verts[1], verts[2]);

        Vector3 hypMidpoint = FindMidpoint(verts[0], verts[1]);

        Vector3 direction = verts[1] - verts[0];
        float xDirection = direction.x >= 0 ? 1 : -1;
        float yDirection = direction.y >= 0 ? 1 : -1;

        gui.SetTriangleSideLengths
            (
                Vector2.Distance(verts[2], verts[0]) * xDirection,
                Vector2.Distance(verts[1], verts[2]) * yDirection,
                adjMidpoint,
                oppMidpoint,
                hypMidpoint
            );
    }

    private Vector3[] GetNormals()
    {
        Vector3[] normals = new Vector3[3];
        for (int i = 0; i < 3; i++)
        {
            normals[i] = Vector3.forward;
        }

        return normals;
    }

    private Vector2[] GetUVS()
    {
        Vector2[] uvs = new Vector2[3];
        uvs[0] = new Vector2(0, 0);
        uvs[1] = new Vector2(0, 1);
        uvs[2] = new Vector2(1, 1);

        return uvs;
    }

    #endregion

    private Vector3 FindMidpoint(Vector3 p1, Vector3 p2)
    {
        return (p1 + p2) / 2f;
    }

    private void OnDrawGizmos()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        if (mesh)
        {
            if (mesh.vertices.Length <= 0) return;

            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                Gizmos.DrawSphere(mesh.vertices[i], 0.1f);
            }
        }
        
    }

}
