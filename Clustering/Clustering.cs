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

        public static void ApplyClusteringColor(List<Cluster> clusters, List<Vector> vectors)
        {
            List<Vector> averageVectors = clusters.Select(cluster => cluster.Color).ToList();
            for (int i = 0; i < vectors.Count; i++)
            {
                vectors[i] = averageVectors[clusters.FindIndex(cluster => cluster.Contains(vectors[i]))];
            }
        }

        public static List<Cluster> MeanShiftClustering(List<Vector> vectors, Func<Vector, Vector, double> criteria, double scale)
        {
            HashSet<Vector> vectorSet = new HashSet<Vector>();
            foreach (Vector vector in vectors)
            {
                vectorSet.Add(vector);
            }

            List<Cluster> clusters = new List<Cluster>();
            foreach (Vector vector in vectorSet)
            {
                Vector centroid = new Vector(vector);
                while (true)
                {
                    Vector averageVector = new Vector(vector.Dimension);
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

        public static List<Cluster> kMeansClustering(List<Vector> vectors, Func<Vector, Vector, double> criteria, List<Vector> centroids)
        {
            HashSet<Vector> vectorSet = new HashSet<Vector>();
            foreach (Vector vector in vectors) {
                vectorSet.Add(vector);
            }

            if (centroids == null || centroids.Count < 2)
            {
                // TODO: Add throw error
                return null;
            }

            List<Cluster> clusters = new List<Cluster>();
            foreach (Vector centroid in centroids)
            {
                Cluster cluster = new Cluster(centroid);
                cluster.Color = centroid;
                clusters.Add(cluster);

            }

            bool centroidChanged = true;

            while (centroidChanged)
            {
                foreach (Vector vector in vectorSet)
                {
                    Cluster currentCluster = clusters[0];
                    double currentDistance = criteria(currentCluster.Centroid, vector);
                    for (int i = 1; i < clusters.Count; i++)
                    {
                        double distanse = criteria(clusters[i].Centroid, vector);
                        if (distanse < currentDistance)
                        {
                            currentDistance = distanse;
                            currentCluster = clusters[i];
                        }
                    }
                    currentCluster.Add(vector);
                }
                centroidChanged = false;
                foreach (Cluster cluster in clusters)
                {
                    Vector centroid = cluster.Centroid;
                    Vector averageVector = cluster.AverageVector;
                    if (!centroid.Equals(averageVector))
                    {
                        centroidChanged = true;
                    }
                    cluster.Centroid = averageVector;
                }
            }
            return clusters;
        }

        // version of rundom clusters 
        public static List<Cluster> kMeansClistering(List<Vector> vectors, Func<Vector, Vector, double> criteria, int clusterNumber)
        {
            if (clusterNumber < 2) 
            {
                // TODO: Add throw error
            }

            return null;
        }
    }
}
