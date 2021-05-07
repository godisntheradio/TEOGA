using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace ConvexHull
{
    class CHGraham
    {
        public GrahamPoint SmallestY;
        private List<GrahamPoint> Points;
        public List<GrahamPoint> Hull;

        public CHGraham(List<GameObject> points)
        {
            Hull = new List<GrahamPoint>();
            Points = new List<GrahamPoint>();
            foreach (var item in points)
            {
                Points.Add(new GrahamPoint(item));
            }
        }

        public void FindSmallestY()
        {
            SmallestY = Points.OrderBy((i) => i.Position.y).FirstOrDefault();
        }

        public void CalculateConvexHull()
        {
            FindSmallestY();

            Points.Remove(SmallestY);

            foreach (var item in Points)
            {
                item.SetPolarAngle(item.Position - SmallestY.Position);
            }
            // ordena no sentido anti horário
            Points = Points.OrderBy(p => p.angle < 0.0f).ThenBy(p => p.angle).ToList();
        }

        public List<Vector3> GetAnglesVectorList()
        {
            var list = new List<Vector3>();
            list.Add(SmallestY.Position);
            foreach (var item in Points)
            {
                list.Add(item.Position);
            }
            return list;
        }

        public List<Vector2> GetPointVectorList()
        {
            var list = new List<Vector2>();
            list.Add(SmallestY.Position);
            foreach (var item in Points)
            {
                list.Add(item.Position);
            }
            return list;
        }

        public List<Vector3> GetHullVectorList()
        {
            var list = new List<Vector3>();
            foreach (var item in Hull)
            {
                list.Add(item.Position);
            }
            return list;
        }

        internal void CalculateHull()
        {
            // usa uma lista encadeada para facilitar a remoção de itens no algorítmo
            LinkedList<GrahamPoint> lPoints = new LinkedList<GrahamPoint>(Points);
            // esse é um passo adicional por que na função CalculateConvexHull eu removo o ponto com menor Y da lista de pontos desse objeto CHGraham
            lPoints.AddFirst(SmallestY);
            // o algorítmo mantém duas listas, a lista de pontos a serem analisados como uma lista encadeada
            // e outra lista normal para manter os pontos do hull (poderia ter sido uma pilha)
            Hull.Add(SmallestY);
            Hull.Add(Points[0]);
            // começar a partir do 3º ponto da lista
            var i = lPoints.First.Next.Next;
            while (i != null)
            {
                var p1 = i.Previous.Value.Position;
                var p2 = i.Value.Position;
                var p3 = i.Next ==  null ? Hull.First().Position : i.Next.Value.Position; // se o pivô (p2) for o ultimo da lista, o p3 seria o primeiro da lista
                var orientation = ComputeOrientation(p1, p2, p3);
                if (orientation <= 0) // calcula a orientação de 3 pontos
                {
                    i = i.Previous;                     // se o resultado for menor ou igual a zero, remover o pivô da lista de pontos a serem analisados e da lista do hull
                    lPoints.Remove(i.Next);             // pois os 3 pontos formam um angulo concavo
                    Hull.RemoveAt(Hull.Count - 1);      // o pivô passa a ser o anterior para o proximo calculo ser feito com o próximo depois do removido
                }
                else
                {
                    Hull.Add(i.Value);                  // adiciona no hull e vai pro próximo
                    i = i.Next;
                }
            }
        }

        public static float AngleBetweenVectors(Vector3 v1, Vector3 v2)
        {
            float dot = Vector3.Dot(v1, v2);
            var result = Mathf.Acos(dot / (v1.magnitude * v2.magnitude));
            return result * Mathf.Rad2Deg;
        }

        public static float ComputeOrientation(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return (p2.x - p1.x) * (p3.y - p1.y) - (p2.y - p1.y) * (p3.x - p1.x);
            //Matrix4x4 mat = new Matrix4x4(new Vector4(p1.x, p1.y, p1.z, 0), new Vector4(p2.x, p2.y, p2.z, 0), new Vector4(p3.x, p3.y, p3.z, 0), new Vector4(0, 0, 0, 1));
            //return Matrix4x4.Determinant(mat);
        }
    }
}
