using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Graph
{
    public class Edge<T>
    {
        public float Weight;

        public Node<T> Origin;
        public Node<T> Destination;

        public Edge(Node<T> origin, Node<T> destination, float weight)
        {
            Origin = origin;
            Destination = destination;
            Weight = weight;
        }

        public bool IsBackwards(Node<T> origin)
        {
            return origin == Destination;
        }

        public Node<T> GetOrigin(ref Node<T> origin)
        {
            return IsBackwards(origin) ? Destination : Origin;
        }

        public Node<T> GetDestination(ref Node<T> origin)
        {
            return IsBackwards(origin) ? Origin : Destination;
        }
    }
}