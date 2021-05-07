using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ConvexHull
{
    class GrahamPoint
    {
        public Vector3 Position { get => pointObject.transform.position; }

        public float angle;

        public GameObject pointObject;

        public GrahamPoint(GameObject pointObject)
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
}
