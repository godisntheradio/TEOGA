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

    public Material AngleLinesMaterial;
    public Material LineHullMaterial;
    public Material LinePathMaterial;

    private Mesh AngleLines;
    private Matrix4x4 AngleLinesMatrix;

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
        if (AngleLines != null)
        {
            Graphics.DrawMesh(AngleLines, AngleLinesMatrix, AngleLinesMaterial, 0);
        }
    }

    public void BeginProcessing()
    {
        Graham.CalculateConvexHull();

        Mesh lines = new Mesh();
        var points = Graham.GetVectorList();

        Vector3[] vertices = new Vector3[points.Count];
        Vector2[] uv = new Vector2[points.Count];
        int[] indices = new int[points.Count * 2];

        int j = 0;
        for (int i = 1; i < points.Count; i++)
        {
            vertices[i] = points[i];
            uv[i] = points[i];
            indices[j] = 0;
            indices[j + 1] = i;
            j += 2;
        }

        lines.SetVertices(vertices);
        lines.uv = uv;
        lines.SetIndices(indices, MeshTopology.Lines, 0);

        AngleLines = lines;
        AngleLinesMatrix = Matrix4x4.TRS(Graham.SmallestY.transform.position, Quaternion.identity, Vector3.one);
        AngleLinesMatrix = PointsContainer.transform.worldToLocalMatrix * AngleLinesMatrix;
    }

    

}
