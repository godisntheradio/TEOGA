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
        public Vector3 position;

        public float angle;

        public GameObject objectRef;

        public Point(Vector3 p, GameObject objectRef)
        {
            this.position = p;
            this.angle = PolarAngle(p);
            this.objectRef = objectRef;
        }

        public float PolarAngle(Vector3 p)
        {
            return Mathf.Atan(p.y / p.x);
        }
    }


    class CHGraham
    {
        private List<GameObject> ObjectList;
        public GameObject SmallestY;
        private List<Point> Points;
        public List<GameObject> Hull;

        public CHGraham(List<GameObject> points)
        {
            ObjectList = points;
            Hull = new List<GameObject>();
            Points = new List<Point>();
        }

        public void FindSmallestY()
        {
            SmallestY = ObjectList.OrderBy((i) => i.transform.position.y).FirstOrDefault();
        }

        public void CalculateConvexHull()
        {
            FindSmallestY();

            ObjectList.Remove(SmallestY);

            foreach (var item in ObjectList)
            {
                Points.Add(new Point(item.transform.position - SmallestY.transform.position, item));
            }

            Points = Points.OrderBy(p => p.angle < 0.0f).ThenBy(p => p.angle).ToList();
        }

        public List<Vector3> GetVectorList()
        {
            var list = new List<Vector3>();
            list.Add(SmallestY.transform.position);
            foreach (var item in Points)
            {
                list.Add(item.position);
            }

            return list;
        }
    }
}
