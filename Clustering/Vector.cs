using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Clustering
{
    // Vector of n-dimensional real space
    public class Vector : IEnumerable
    {
        // Construct null vector
        public Vector(int dimension)
        {
            if (dimension > 0)
            {
                _components = new double[dimension];
            }
            else
            {
                throw new ArgumentOutOfRangeException("dimension", "Dimension must be positive");
            }
        }

        public Vector(params double[] components)
        {
            _components = components;
        }

        public double this[int index]
        {
            get { return _components[index]; }
        }

        public int Dimension 
        {
            get { return _components.Length; }
        }

        // Length (magnitude, modulus) of vector
        public double Length
        {
            get { return Math.Sqrt(this * this); }
        }

        public static Vector operator -(Vector a)
        {
            double[] result = new double[a.Dimension];
            for (int i = 0; i < a.Dimension; i++)
            {
                result[i] = -a[i];
            }
            return new Vector(result);
        }

        public static Vector operator +(Vector a, Vector b)
        {
            if (a.Dimension == b.Dimension)
            {
                double[] result = new double[a.Dimension];
                for (int i = 0; i < a.Dimension; i++)
                {
                    result[i] = a[i] + b[i];
                }
                return new Vector(result);
            }
            else
            {
                throw new ArgumentException("Dimensions must be equal", "b");
            }
        }

        public static Vector operator -(Vector a, Vector b)
        {
            if (a.Dimension == b.Dimension)
            {
                return a + (-b);
            }
            else
            {
                throw new ArgumentException("Dimensions must be equal", "b");
            }
        }

        public static Vector operator *(Vector a, double k)
        {
            double[] result = new double[a.Dimension];
            for (int i = 0; i < a.Dimension; i++)
            {
                result[i] = a[i] * k;
            }
            return new Vector(result);            
        }

        public static Vector operator /(Vector a, double k)
        {
            return a*(1/k);
        }

        // Scalar multiplication
        public static double operator *(Vector a, Vector b)
        {
            if (a.Dimension == b.Dimension)
            {
                double result = 0;
                for (int i = 0; i < a.Dimension; i++)
                {
                    result += a[i] * b[i];
                }
                return result;
            }
            else
            {
                throw new ArgumentException("Dimensions must be equal", "b");
            }
        }

        // AngleBetweenVectors - not implemented
        // Vector + scalar - not implemented

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _components.GetEnumerator();
        }

        private readonly double[] _components;

    }
}
