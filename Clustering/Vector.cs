using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace Clustering
{
    // Vector of n-dimensional real space
    public class Vector : IEnumerable, IEquatable<Vector>
    {
        // Construct null vector
        public Vector(int dimension)
        {
            if (dimension <= 0)
            {
                throw new ArgumentOutOfRangeException("dimension", "Dimension must be positive");
            }
            _components = new double[dimension];
        }

        public Vector(Vector v)
        {
            _components = new double[v._components.Length];
            v._components.CopyTo(_components, 0);
        }

        public override int GetHashCode()
        {
            return _components.GetHashCode();
        }


        public Vector(params double[] components)
        {
            _components = new double[components.Length];
            components.CopyTo(_components, 0);
        }

        public double this[int index]
        {
            get { return _components[index]; }
        }

        public int Dimension 
        {
            get { return _components.Length; }
        }

        // Length (magnitude, modulus...) of vector
        public double Length
        {
            get { return Math.Sqrt(this * this); }
        }

        // Squared length (magnitude, modulus...) of vector
        public double LengthSquared
        {
            get { return this * this; }
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
            if (a.Dimension != b.Dimension)
            {
                throw new ArgumentException("Dimensions must be equal", "b");
            }
            double[] result = new double[a.Dimension];
            for (int i = 0; i < a.Dimension; i++)
            {
                result[i] = a[i] + b[i];
            }
            return new Vector(result);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            if (a.Dimension != b.Dimension)
            {
                throw new ArgumentException("Dimensions must be equal", "b");
            }
            return a + (-b);
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
            if (a.Dimension != b.Dimension)
            {
                throw new ArgumentException("Dimensions must be equal", "b");
            }
            double result = 0;
            for (int i = 0; i < a.Dimension; i++)
            {
                result += a[i] * b[i];
            }
            return result;
        }

        // AngleBetweenVectors - not implemented
        // Vector + scalar - not implemented

        public static Vector[] GetVectors(byte[] data, int bytesPerParameter)
        { 
            Vector[] vectors = new Vector[data.Length/bytesPerParameter];

            for (int i = 0; i < vectors.Length; i++) 
            {
                int index = i * bytesPerParameter;
                double[] array = new double[bytesPerParameter];

                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = Convert.ToDouble(data[index + j]);
                }
                vectors[i] = new Vector(array);
            }
            return vectors;
        }

        public bool Equals(Vector other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _components.Equals(other._components);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Vector)obj);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _components.GetEnumerator();
        }

        public ReadOnlyCollection<double> Components
        {
            get { return Array.AsReadOnly(_components); }
        }

        private readonly double[] _components;

    }
}
