using System;
using System.Linq;

namespace Clustering
{
    public static class Criteria
    {
        public static double EuclideanDistance(Vector a, Vector b)
        {
            return (a - b).Length;
        }

        public static double EuclideanDistanceSquared(Vector a, Vector b)
        {
            return (a - b).LengthSquared;
        }

        // Also known as Manhattan Distance
        public static double TaxicabDistance(Vector a, Vector b)
        {
            return (a - b).Components.Sum();
        }

        // Also known as Chessboard Distance
        public static double ChebyshevDistance(Vector a, Vector b)
        {
            return (a - b).Components.Max();
        }

        // If p=2 and r=2 - Euclidean distance 
        // Not use, 'cause don't match Func<Vector, Vector, double>
        // Use GetExponentialDistanceFunction instead
        public static double ExponentialDistance(Vector a, Vector b, double p, double r)
        {
            if (a.Dimension != b.Dimension)
            {
                throw new ArgumentException("Dimensions must be equal", "b");
            }
            Vector v = a - b;
            double result = 0;
            for (int i = 0; i < v.Dimension; i++)
            {
                result += Math.Pow(v[i], p);
            }
            return Math.Pow(result, 1 / r);
        }

        public static Func<Vector, Vector, double> GetExponentialDistanceFunction(double p, double r)
        {
            return (a, b) => ExponentialDistance(a, b, p, r);
        }
    }
}
