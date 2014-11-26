using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCI598.Proj3.Histogrammer
{
    /// <summary>
    /// Simple wrapper class for 3D vector/point utility calculations
    /// </summary>
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

        /// <summary>
        /// Calculate the difference between this point and the other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>A Point3D representing the straight-line difference between this point and the other</returns>
        public Point3D difference(Point3D other)
        {
            return new Point3D(
                this.X - other.X,
                this.Y - other.Y,
                this.Z - other.Z
            );
        }

        /// <summary>
        /// Calculate the length of the vector represented by this point.
        /// </summary>
        /// <returns>The magnitude of the vector represented by this point</returns>
        public double magnitude()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        /// <summary>
        /// Calculate the angle formed by this point and 2 others.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>The measure (in radians) formed by the angle with this as the center and left and right defining the two legs. 0 if angle is undefined.</returns>
        public double angleMadeWith(Point3D left, Point3D right)
        {
            Point3D leftLeg = left.difference(this);
            Point3D rightLeg = right.difference(this);
            double angle = Math.Acos(leftLeg.X * rightLeg.X + leftLeg.Y * rightLeg.Y + leftLeg.Z * rightLeg.Z) / (leftLeg.magnitude() * rightLeg.magnitude());
            return (angle == double.NaN ? 0 : angle);
        }
    }
}
