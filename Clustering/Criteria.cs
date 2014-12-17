using System;
using System.Linq;
using System.Windows.Media.Media3D;

namespace Clustering
{
    public static class Criteria
    {
        public static double EuclideanDistance(Point3D a, Point3D b)
        {
            return (a - b).Length;
        }

        public static double EuclideanDistanceSquared(Point3D a, Point3D b)
        {
            return (a - b).LengthSquared;
        }

        // Also known as Manhattan Distance
        public static double TaxicabDistance(Point3D a, Point3D b)
        {
            Vector3D v = a - b;
            return Math.Abs(v.X) + Math.Abs(v.Y) + Math.Abs(v.Z);
        }

        // Also known as Chessboard Distance
        public static double ChebyshevDistance(Point3D a, Point3D b)
        {
            Vector3D v = a - b;
            return (new double[] {v.X, v.Y, v.Z}).Max();
        }
    }
}
