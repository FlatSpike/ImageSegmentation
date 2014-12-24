using System;
using System.Collections;
using System.Collections.Generic;

namespace Clustering
{
    public class Cluster : IEnumerable, IEquatable<Cluster>
    {
        public Cluster(Vector centroid)
        {
            Centroid = centroid;
        }

        public bool Add(Vector v)
        {
            return _vectors.Add(v);
        }

        public bool Remove(Vector v)
        {
            return _vectors.Remove(v);
        }

        public bool Contains(Vector v)
        {
            return _vectors.Contains(v);
        }

        public bool Equals(Cluster other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Centroid, other.Centroid);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Cluster) obj);
        }

        public override int GetHashCode()
        {
            return (Centroid != null ? Centroid.GetHashCode() : 0);
        }

        public IEnumerator GetEnumerator()
        {
            return _vectors.GetEnumerator();
        }

        public Vector AverageVector
        {
            get
            {
                Vector averageVector = new Vector(Centroid.Dimension);
                foreach (Vector vector in _vectors)
                {
                    averageVector += vector;
                }
                averageVector /= _vectors.Count;
                return averageVector;
            }
        }

        public Vector Centroid { get; set; }

        private readonly HashSet<Vector> _vectors = new HashSet<Vector>();
    }
}
