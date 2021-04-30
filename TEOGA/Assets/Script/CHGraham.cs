using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace ConvexHull
{
    

    class Point
    {
        public Vector3 Position { get => pointObject.transform.position; }

        public float angle;

        public GameObject pointObject;

        public Point(GameObject pointObject)
        {
            this.pointObject = pointObject;
        }

        public float PolarAngle(Vector3 p)
        {
            return Mathf.Atan(p.y / p.x);
        }

        public void SetPolarAngle(Vector3 vector)
        {
            this.angle = PolarAngle(vector);
        }
    }


    class CHGraham
    {
        public Point SmallestY;
        private List<Point> Points;
        public List<Point> Hull;

        public CHGraham(List<GameObject> points)
        {
            Hull = new List<Point>();
            Points = new List<Point>();
            foreach (var item in points)
            {
                Points.Add(new Point(item));
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
            Hull.Add(SmallestY);
            Hull.Add(Points[0]);
            for (int i = 1; i < Points.Count; i++)
            {
                Hull.Add(Points[i]);
                var orientation = ComputeOrientation(Hull[Hull.Count - 2].Position, Points[i].Position, i + 1 == Points.Count ? Hull.First().Position : Points[i + 1].Position);
                if (orientation < 0)
                    Hull.RemoveAt(Hull.Count - 1);
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
