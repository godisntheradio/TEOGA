using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class GraphTests
    {
        public List<GameObject> CreatePointList()
        {
            List<GameObject> list = new List<GameObject>();
            for (int i = 0; i < 10; i++)
            {
                var g = new GameObject();
                var x = Random.Range(-5.0f, 5.0f);
                var y = Random.Range(-5.0f, 5.0f);
                g.transform.position = new Vector3(x, y, 0.0f);
                list.Add(g);
            }

            return list;
        }
        // A Test behaves as an ordinary method
        [Test]
        public void DoesNotAllowDuplicates()
        {
            var list = CreatePointList();
            list.Add(list[0]);
            Graph.Graph<GameObject> graph = new Graph.Graph<GameObject>();
            graph.AddNodes(list);

            Assert.IsTrue(graph.FindAllNodes(n => n.Item == list[0]).Count == 1);
        }
        [Test]
        public void DoesNotAllowDuplicatesSameCoordinate()
        {
            var list = CreatePointList();
            var g = new GameObject();
            var g2 = new GameObject();
            var x = Random.Range(-5.0f, 5.0f);
            var y = Random.Range(-5.0f, 5.0f);
            g.transform.position = new Vector3(x, y, 0.0f);
            g2.transform.position = new Vector3(x, y, 0.0f);

            list.Add(g);
            list.Add(g2);

            Graph.Graph<GameObject> graph = new Graph.Graph<GameObject>();
            graph.AddNodes(list);

            Assert.IsTrue(graph.FindAllNodes(n => n.Item.transform.position == g.transform.position).Count == 1);
        }
    }
}
