using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graph
{
    public class Node<T>
    {
        public int ID;
        public T Item;

        public List<Edge<T>> Edges;

        #region A* helpers

        public float h;
        public float g;
        public float f;

        public Node<T> Parent;

        #endregion

        public Node(int ID, T item)
        {
            this.Item = item;
            this.ID = ID;

            Edges = new List<Edge<T>>();
        }
       
        public void CalculateF()
        {
            this.f = g + h;
        }

        public override bool Equals(object obj)
        {
            return obj is Node<T> node &&
                   ID == node.ID;
        }
    }
}
