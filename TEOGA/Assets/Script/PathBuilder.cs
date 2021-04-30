using ConvexHull;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBuilder : MonoBehaviour
{
    public GameObject PointsContainer;


    List<GameObject> Points;

    CHGraham Graham;


    #region Visual

    public Material AnglesMeshMaterial;
    public Material HullMeshMaterial;
    public Material PathMeshMaterial;

    #endregion

    #region Result

    private Mesh AnglesMesh;
    private Matrix4x4 AnglesMeshMatrix;

    private Mesh HullMesh;
    private Matrix4x4 HullMeshMatrix;


    #endregion


    void Start()
    {
        Points = new List<GameObject>();
        foreach (Transform item in PointsContainer.transform)
        {
            Points.Add(item.gameObject);
        }
        Graham = new CHGraham(Points);
    }

    // Update is called once per frame
    void Update()
    {
        if (AnglesMesh != null)
        {
            Graphics.DrawMesh(AnglesMesh, AnglesMeshMatrix, AnglesMeshMaterial, 0);
        }

        if (HullMesh != null)
        {
            Graphics.DrawMesh(HullMesh, HullMeshMatrix, HullMeshMaterial, 0);
        }
    }

    public void BeginProcessing()
    {
        Graham.CalculateConvexHull();

        var points = Graham.GetAnglesVectorList();

        AnglesMesh = CreateAngleLineMesh(ref points);
        AnglesMeshMatrix = CreateMeshMatrix();

        BuildHull();
    }

    public void BuildHull()
    {
        //Debug.Log(CHGraham.AngleBetweenVectors(new Vector3(3, -4, 5), new Vector3(2, 7, -3))); // resultado deve ser 131.647
        //Debug.Log(CHGraham.ComputeOrientation(new Vector3(1, 3, 10), new Vector3(9, 7, 4), new Vector3(5, 8, 2))); // resultado deve ser 358
        Graham.CalculateHull();

        var points = Graham.GetHullVectorList();

        HullMesh = CreateHullLineMesh(ref points);
        HullMeshMatrix = CreateMeshMatrix();
    }


    public static Mesh CreateAngleLineMesh(ref List<Vector3> points)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[points.Count];
        Vector2[] uv = new Vector2[points.Count];
        int[] indices = new int[points.Count * 2];

        int j = 0;
        for (int i = 0; i < points.Count; i++)
        {
            vertices[i] = points[i];
            uv[i] = points[i];
            indices[j] = 0;
            indices[j + 1] = i;
            j += 2;
        }

        mesh.SetVertices(vertices);
        mesh.uv = uv;
        mesh.SetIndices(indices, MeshTopology.Lines, 0);

        return mesh;
    }

    public static Mesh CreateHullLineMesh(ref List<Vector3> points)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[points.Count];
        Vector2[] uv = new Vector2[points.Count];
        int[] indices = new int[points.Count * 2];

        int j = 0;
        for (int i = 0; i < points.Count; i++)
        {
            vertices[i] = points[i];
            uv[i] = points[i];
            indices[j] = i;
            indices[j + 1] = i + 1 >= points.Count ? 0 : i + 1;
            j += 2;
        }

        mesh.SetVertices(vertices);
        mesh.uv = uv;
        mesh.SetIndices(indices, MeshTopology.Lines, 0);

        return mesh;
    }

    public Matrix4x4 CreateMeshMatrix()
    {
        return Matrix4x4.TRS(PointsContainer.transform.position, Quaternion.identity, Vector3.one) * PointsContainer.transform.worldToLocalMatrix;
    }
}
