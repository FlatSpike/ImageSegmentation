using System;
using System.Collections.Generic;
using System.Linq;

namespace Clustering
{
    public static class Clustering
    {

        public static void ApplyClustering(List<Cluster> clusters, List<Vector> vectors)
        {
            List<Vector> averageVectors = clusters.Select(cluster => cluster.AverageVector).ToList();
            for (int i = 0; i < vectors.Count; i++)
            {
                vectors[i] = averageVectors[ clusters.FindIndex(cluster => cluster.Contains(vectors[i])) ];
            }
        }

        public static List<Cluster> MeanShiftClustering(List<Vector> vectors, Func<Vector, Vector, double> criteria, double scale)
        {
            List<Cluster> clusters = new List<Cluster>();
            foreach (Vector vector in vectors)
            {
                Vector centroid = new Vector(vector);
                while (true)
                {
                    Vector averageVector = new Vector();
                    int vectorCount = 0;
                    foreach (Vector innerVector in vectors)
                    {
                        if (criteria(innerVector, vector) <= scale)
                        {
                            averageVector += innerVector;
                            vectorCount++;
                        }
                    }
                    if (vectorCount > 0)
                    {
                        averageVector /= vectorCount;
                    }
                    if (!averageVector.Equals(centroid))
                    {
                        centroid = averageVector;
                    }
                    else
                    {
                        break;
                    }
                }
                Cluster cluster = new Cluster(centroid);
                if (!clusters.Contains(cluster))
                {
                    clusters.Add(cluster);
                }
                else
                {
                    cluster = clusters.Find(c => c.Equals(cluster));
                }
                cluster.Add(vector);
            }
            return clusters;
        }

        // TODO: Implement k-means algorithm
         
    }
}
