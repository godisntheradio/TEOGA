using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Graph
{
    public class Graph<T>
    {

        public List<Node<T>> Nodes;
        public List<Edge<T>> Edges;

        public int[] Indices;

        public Graph()
        {
            Nodes = new List<Node<T>>();
            Edges = new List<Edge<T>>();
        }

        public void AddNodes(List<T> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (Nodes.Find(n => ReferenceEquals(n.Item, nodes[i])) == null)
                    this.Nodes.Add(new Node<T>(i, nodes[i]));
            }
        }

        public Node<T> FindNode(System.Predicate<Node<T>> predicate)
        {
            return Nodes.Find(predicate);
        }

        public List<Node<T>> FindAllNodes(System.Predicate<Node<T>> predicate)
        {
            return Nodes.FindAll(predicate);
        }

        public void CreateEdges(int[] indices, System.Func<T, T, float> weightCalculator)
        {
            this.Indices = indices;
            for (int i = 0; i < this.Indices.Length; i+=2)
            {
                var originNode = Nodes[indices[i]];
                var destinationNode = Nodes[indices[i + 1]];
                if (originNode.ID != destinationNode.ID)
                {
                    var weight = weightCalculator(originNode.Item, destinationNode.Item);
                    var edge = new Edge<T>(originNode, destinationNode, weight);
                    Edges.Add(edge);
                    originNode.Edges.Add(edge);
                }

            }
        }

        public void CalculateHeuristics(T destination, System.Func<T, T, float> heuristicsCalculator)
        {
            foreach (var item in Nodes)
            {
                item.h = heuristicsCalculator(item.Item, destination);
            }
        }

        public List<Edge<T>> AStar(int origin, int destination)
        {
            if (origin == destination)
                return null;

            var i = Nodes[origin];
            var j = Nodes[destination];
            return AStar(i, j);
        }

        private void Clear()
        {
            foreach (var item in Nodes)
            {
                item.f = 0;
            }
        }

        public List<Edge<T>> AStar(Node<T> origin, Node<T> destination) // implementação do a* baseada no vídeo https://www.youtube.com/watch?v=eSOJ3ARN5FM
        {
            if (origin == destination)
                return null;

            Clear();
            var openList = new List<Node<T>>();
            var closedList = new List<Node<T>>();
            var path = new List<Edge<T>>();

            Node<T> currentVertex = origin;
            currentVertex.h = 0;
            currentVertex.g = 0;
            currentVertex.f = 0;

            openList.Add(origin);

            while (currentVertex != destination)
            {
                foreach (var edge in currentVertex.Edges)
                {
                    var adjacentVertex = edge.GetDestination(ref currentVertex);
                    if (!openList.Contains(adjacentVertex) && !closedList.Contains(adjacentVertex))
                    {
                        openList.Add(adjacentVertex);
                        
                        adjacentVertex.g = edge.Weight + currentVertex.g;
                        float f = adjacentVertex.g + adjacentVertex.h;

                        if (f < adjacentVertex.f || adjacentVertex.f == 0)
                        {
                            adjacentVertex.f = f;
                            adjacentVertex.Parent = currentVertex;
                        }
                    }
                }
                closedList.Add(currentVertex);
                openList.Remove(currentVertex);
                var lowestF = openList.OrderBy((i) => i.f).FirstOrDefault();
                openList.Remove(lowestF);
                currentVertex = lowestF;
            }

            for (Node<T> i = destination; i != origin; i = i.Parent)
            {
                path.Add(new Edge<T>(i.Parent, i, 0));
            }


            return path;
        }
    }
}
