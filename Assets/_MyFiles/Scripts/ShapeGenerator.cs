using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        if (GameplayUI.Instance.IsAnswerInDegrees())
        {
            return targetAngle * Mathf.Rad2Deg;
        }
        else
        {
            return targetAngle;
        }
    }

    #region Shape Creation

    public void ClearMesh()
    {
        meshFilter.mesh.Clear();
        meshFilter.mesh = null;
    }

    public void SetLine(Vector3 a, Vector3 b)
    {
        Vector3 midPoint = FindMidpoint(a, b);

        float lineLength = Vector3.Distance(a, b);

        SetTargetDistanceAndAngle((b - a), lineLength);

        GameplayUI.Instance.SetLineLength(lineLength, midPoint);
    }

    public void GenerateTriangleFromLine(Vector2 startPos, Vector2 endPosition)
    {
        meshFilter.mesh.Clear();

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        Vector3[] vertices = new Vector3[3];
        vertices[0] = new Vector3(startPos.x, startPos.y, 0); // start point
        vertices[1] = new Vector3(endPosition.x, endPosition.y, 0); // end point
        vertices[2] = new Vector3(endPosition.x, startPos.y, 0); // right angle point
        
        mesh.vertices = vertices;
        mesh.triangles = GetTriangle(vertices);
        mesh.uv = GetUVS();
        mesh.normals = GetNormals();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        UpdateGUISideLengths(vertices);
        SetTargetDistanceAndAngle(vertices);

        Vector3 direction = vertices[1] - vertices[0];
        float xDirection = direction.x >= 0 ? 1 : -1;
        float yDirection = direction.y >= 0 ? 1 : -1;

        float targetAngle = Mathf.Atan2(Mathf.Abs(direction.y), Mathf.Abs(direction.x)) * Mathf.Rad2Deg;
        GameplayUI.Instance.SetHypTextZAngle(targetAngle * xDirection * yDirection);
    }

    private void SetTargetDistanceAndAngle(Vector3[] verts)
    {
        float adjacentLength = Vector2.Distance(verts[0], verts[2]);
        float oppositeLength = Vector2.Distance(verts[2], verts[1]);

        targetDistance = Mathf.Sqrt(Mathf.Pow(adjacentLength, 2) + Mathf.Pow(oppositeLength, 2));
        
        targetAngle = Mathf.Atan(oppositeLength / adjacentLength);

        Debug.Log("Distance: " + targetDistance + " Angle: " + targetAngle);
    }

    private void SetTargetDistanceAndAngle(Vector3 direction, float distance)
    {
        targetDistance = distance;

        if (Mathf.Approximately(direction.y, 0))
        {
            targetAngle = direction.x >= 0 ? 0 : Mathf.PI;
        }

        if (Mathf.Approximately(direction.x, 0))
        {
            targetAngle = direction.y >= 0 ? Mathf.PI / 2 : (3 * Mathf.PI) / 2;
        }

        Debug.Log("Distance: " + targetDistance + " Angle: " + targetAngle);
    }

    private int[] GetTriangle(Vector3[] verts)
    {
        byte p1, p2, p3;
        SetTriangleVertPointValue(verts, out p1, out p2, out p3);

        int[] triangles = new int[3];
        triangles.SetValue(0, p1);
        triangles.SetValue(1, p2);
        triangles.SetValue(2, p3);

        return triangles;
    }

    private void SetTriangleVertPointValue(Vector3[] verts, out byte p1, out byte p2, out byte p3)
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

    private void UpdateGUISideLengths(Vector3[] verts)
    {
        Vector3 adjMidpoint = FindMidpoint(verts[0], verts[2]);

        Vector3 oppMidpoint = FindMidpoint(verts[1], verts[2]);

        Vector3 hypMidpoint = FindMidpoint(verts[0], verts[1]);

        Vector3 direction = verts[1] - verts[0];
        float xDirection = direction.x >= 0 ? 1 : -1;
        float yDirection = direction.y >= 0 ? 1 : -1;

        GameplayUI.Instance.SetTriangleSideLengths
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

    public static Vector3 FindMidpoint(Vector3 p1, Vector3 p2)
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
