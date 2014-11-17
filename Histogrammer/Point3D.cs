using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCI598.Proj3.Histogrammer
{
    class Point3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3D difference(Point3D other)
        {
            return new Point3D(
                this.X - other.X,
                this.Y - other.Y,
                this.Z - other.Z
            );
        }



        public double magnitude()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public double angleMadeWith(Point3D left, Point3D right)
        {
            Point3D leftLeg = left.difference(this);
            Point3D rightLeg = right.difference(this);
            double angle = Math.Acos(leftLeg.X * rightLeg.X + leftLeg.Y * rightLeg.Y + leftLeg.Z * rightLeg.Z) / (leftLeg.magnitude() * rightLeg.magnitude());
            return (angle == double.NaN ? 0 : angle);
        }
    }
}
