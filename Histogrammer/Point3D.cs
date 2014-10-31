using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histogrammer
{
    class Point3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }


        internal Point3D difference(Point3D other)
        {
            return new Point3D {
                X = this.X - other.X,
                Y = this.Y - other.Y,
                Z = this.Z - other.Z
            };
        }

        internal double magnitude()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }
    }
}
