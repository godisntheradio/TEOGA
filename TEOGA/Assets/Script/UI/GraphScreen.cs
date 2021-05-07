using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GraphScreen : MonoBehaviour
{
    public GameObject TextPrefab;
    public GameObject Container;
    private Canvas CurrentCanvas;

    // Start is called before the first frame update
    void Start()
    {
        CurrentCanvas = GetComponentInParent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateDistances(Graph.Graph<MapLocation> graph)
    {
        foreach (var edge in graph.Edges)
        {
            var position = Camera.main.WorldToScreenPoint(Vector3.Lerp(edge.Origin.Item.transform.position, edge.Destination.Item.transform.position, 0.5f));
            var text = Instantiate(TextPrefab, position, CurrentCanvas.transform.rotation, Container.transform);
            text.GetComponent<DynamicText>().UpdateText(edge.Weight.ToString("0.00"));
        }
    }
}
