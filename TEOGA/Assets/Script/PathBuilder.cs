using ConvexHull;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBuilder : MonoBehaviour
{

    public GraphScreen GraphScreen;
    public GameObject PointsContainer;
    public GameObject Map;

    public List<MapLocation> Locations;

    CHGraham Graham;

    GK.VoronoiDiagram Diagram;

    Graph.Graph<MapLocation> LocationGraph;

    #region Visual

    public Material AnglesMeshMaterial;
    public Material HullMeshMaterial;
    public Material VoronoiMeshMaterial;
    public Material DelaunayMeshMaterial;
    public Material PathMeshMaterial;

    #endregion

    #region Result

    private Matrix4x4 MeshMatrix;

    private Mesh AnglesMesh;
    public bool ShowAnglesMesh;

    private Mesh HullMesh;
    public bool ShowHullMesh;

    private Mesh VoronoiMesh;
    public bool ShowVoronoiMesh;

    private Mesh DelaunayMesh;
    public bool ShowDelaunayMesh;

    private Mesh PathMesh;
    public bool ShowPathMesh;

    private GameObject FirstPathObject;
    private GameObject SecondPathObject;

    #endregion


    void Start()
    {

        var points = new List<GameObject>();
        Locations = new List<MapLocation>();
        foreach (Transform item in PointsContainer.transform)
        {
            points.Add(item.gameObject);
            Locations.Add(item.GetComponent<MapLocation>());
        }
        Graham = new CHGraham(points);
        MeshMatrix = CreateMeshMatrix();

        LocationGraph = new Graph.Graph<MapLocation>();
    }

    void Update()
    {
        if (AnglesMesh != null && ShowAnglesMesh)
        {
            Graphics.DrawMesh(AnglesMesh, MeshMatrix, AnglesMeshMaterial, 0);
        }

        if (VoronoiMesh != null && ShowVoronoiMesh)
        {
            Graphics.DrawMesh(VoronoiMesh, MeshMatrix, VoronoiMeshMaterial, 0);
        }

        if (DelaunayMesh != null && ShowDelaunayMesh)
        {
            Graphics.DrawMesh(DelaunayMesh, MeshMatrix, DelaunayMeshMaterial, 0);
        }

        if (HullMesh != null && ShowHullMesh)
        {
            Graphics.DrawMesh(HullMesh, MeshMatrix, HullMeshMaterial, 0);
        }


        if (PathMesh != null && ShowPathMesh)
        {
            Graphics.DrawMesh(PathMesh, MeshMatrix, PathMeshMaterial, 0);
        }

    }

    // calcula o primeiro passo do graham scan
    // no "void Start()" é feito uma lista de pontos que é passado para o objeto
    // "CHGraham Graham" que acha o menor y e calcula o angulo polar de todos os outros pontos
    public void BeginProcessing()
    {
        Graham.CalculateConvexHull();

        // aqui é um passo adicional para pegar uma lista de arestas formadas pelo ponto de menor y até todos os outros pontos e mostrar na tela
        var points = Graham.GetAnglesVectorList();

        AnglesMesh = CreateAngleLineMesh(ref points);

        BuildHull();
    }

    // aqui é feito o calculo do hull em si
    // mais detalhes nos comentários do CHGraham.CalculateHull
    public void BuildHull()
    {
        //Debug.Log(CHGraham.AngleBetweenVectors(new Vector3(3, -4, 5), new Vector3(2, 7, -3))); // resultado deve ser 131.647
        //Debug.Log(CHGraham.ComputeOrientation(new Vector3(1, 3, 10), new Vector3(9, 7, 4), new Vector3(5, 8, 2))); // resultado deve ser 358


        Graham.CalculateHull();

        var points = Graham.GetHullVectorList();

        HullMesh = CreateHullLineMesh(ref points);


        BuildVoronoi();
    }

    // calcula o diagrama de voronoi                                                      link para a implementação utilizada: https://github.com/OskarSigvardsson/unity-delaunay
    // serve mais para ser usar a triangulação de delaunay do resultado do diagrama
    public void BuildVoronoi()
    {
        var points2d = Graham.GetPointVectorList();
        GK.VoronoiCalculator voronoiCalculator = new GK.VoronoiCalculator();
        Diagram = voronoiCalculator.CalculateDiagram(Graham.GetPointVectorList());

        //var extends = Map.GetComponent<MeshFilter>().mesh.bounds.size;
        //extends.Scale(Map.transform.localScale);

        VoronoiMesh = CreateVoronoiMesh(Diagram);


        BuildGraph();
    }

    // cria a representação visual do diagrama de delaunay
    // cria um grafo a partir do resultado da triangulação
    public void BuildGraph()
    {
        var orderedLocations = new List<MapLocation>();
        foreach (var item in Diagram.Triangulation.Vertices)
        {
            orderedLocations.Add(Locations.Find(i => i.IsEqualTo(item)));
        }

        LocationGraph.AddNodes(orderedLocations);

        int[] indices = new int[Diagram.Triangulation.Triangles.Count * 3];

        // separa cada triangulo em 3 segmentos de reta
        int j = 0;
        for (int i = 0; i < Diagram.Triangulation.Triangles.Count; i += 3)
        {
            indices[j] = Diagram.Triangulation.Triangles[i];
            indices[j + 1] = Diagram.Triangulation.Triangles[i + 1];

            indices[j + 2] = Diagram.Triangulation.Triangles[i + 1];
            indices[j + 3] = Diagram.Triangulation.Triangles[i + 2];

            indices[j + 4] = Diagram.Triangulation.Triangles[i + 2];
            indices[j + 5] = Diagram.Triangulation.Triangles[i];

            j += 6;
        }

        LocationGraph.CreateEdges(indices, (p, q) => Vector3.Distance(p.transform.position, q.transform.position));
        DelaunayMesh = CreateTriangulationMesh(Diagram, indices);

        GraphScreen.CreateDistances(LocationGraph);

    }
    // calcula o A* 
    public void BuildPath()
    {
        var originObj = FirstPathObject.GetComponent<MapLocation>();
        var destinationObj = SecondPathObject.GetComponent<MapLocation>();

        var origin = LocationGraph.FindNode((i) => i.Item == originObj);
        var destination = LocationGraph.FindNode((i) => i.Item == destinationObj);

        LocationGraph.CalculateHeuristics(destination.Item, (c, d) => Vector3.Distance(c.transform.position, d.transform.position));
        var path = LocationGraph.AStar(origin, destination);

        PathMesh = CreatePathMesh(path);
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

    public static Mesh CreateVoronoiMesh(GK.VoronoiDiagram diagram)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[diagram.Vertices.Count];
        Vector2[] uv = new Vector2[diagram.Vertices.Count];
        int[] indices = new int[diagram.Edges.Count * 2];


        for (int i = 0; i < diagram.Vertices.Count; i++)
        {
            vertices[i] = diagram.Vertices[i];
            uv[i] = diagram.Vertices[i];
        }

        int j = 0;
        for (int i = 0; i < diagram.Edges.Count; i++)
        {
            if (diagram.Edges[i].Type == GK.VoronoiDiagram.EdgeType.Segment)
            {
                indices[j] = diagram.Edges[i].Vert0;
                indices[j + 1] = diagram.Edges[i].Vert1;

                j += 2;
            }
        }

        mesh.SetVertices(vertices);
        mesh.uv = uv;
        mesh.SetIndices(indices, MeshTopology.Lines, 0);

        return mesh;
    }

    public static Mesh CreateTriangulationMesh(GK.VoronoiDiagram diagram, int[] indices)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[diagram.Triangulation.Vertices.Count];
        Vector2[] uv = new Vector2[diagram.Triangulation.Vertices.Count];



        for (int i = 0; i < diagram.Triangulation.Vertices.Count; i++)
        {
            vertices[i] = diagram.Triangulation.Vertices[i];
            uv[i] = diagram.Triangulation.Vertices[i];
        }

        mesh.SetVertices(vertices);
        mesh.uv = uv;
        mesh.SetIndices( indices, MeshTopology.Lines, 0);

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

    public static Mesh CreatePathMesh(List<Graph.Edge<MapLocation>> path)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[path.Count * 2];
        Vector2[] uv = new Vector2[path.Count * 2];
        int[] indices = new int[path.Count * 2];

        int j = 0;
        // roda ao contrario por causa do jeito que monta o a *
        for (int i = 0; i < path.Count; i++)
        {
            vertices[j] = path[i].Origin.Item.transform.position;
            vertices[j + 1] = path[i].Destination.Item.transform.position;
            uv[j] = path[i].Origin.Item.transform.position;
            uv[j + 1] = path[i].Destination.Item.transform.position;
            indices[j] = j;
            indices[j + 1] = j + 1;
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

    #region Setters

    public void SetShowAnglesMesh(bool value)
    {
        ShowAnglesMesh = value;
    }

    public void SetShowHullMesh(bool value)
    {
        ShowHullMesh = value;
    }

    public void SetShowVoronoiMesh(bool value)
    {
        ShowVoronoiMesh = value;
    }

    public void SetShowDelaunayMesh(bool value)
    {
        ShowDelaunayMesh = value;
    }

    public void SetShowPathMesh(bool value)
    {
        ShowPathMesh = value;
    }

    public void SetPathObject(GameObject value)
    {
        if (DelaunayMesh == null)
            return;

        if (FirstPathObject == null || SecondPathObject != null)
        {
            FirstPathObject = value;
            SecondPathObject = null;
            if (PathMesh != null)
            {
                PathMesh.Clear();
                Destroy(PathMesh);
                PathMesh = null;
            }
        }
        else if (FirstPathObject != null || SecondPathObject == null)
        {
            SecondPathObject = value;
        }

        if (FirstPathObject != null && SecondPathObject != null)
        {
            BuildPath();
        }

    }

    #endregion
}
