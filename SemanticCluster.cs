
using System;
using System.Collections.Generic;

public class SemanticCluster<T>
{
    public List<T> Members = new List<T>();
    public T Centroid;

    public float Distance(T a, T b, Func<T, T, float> metric) => metric(a, b);

    public void RecalculateCentroid(Func<T, T, float> metric)
    {
        if (Members.Count == 0) return;

        float minTotalDistance = float.MaxValue;
        T newCentroid = Members[0];

        foreach (var candidate in Members)
        {
            float totalDistance = 0f;
            foreach (var other in Members)
            {
                totalDistance += metric(candidate, other);
            }

            if (totalDistance < minTotalDistance)
            {
                minTotalDistance = totalDistance;
                newCentroid = candidate;
            }
        }

        Centroid = newCentroid;
    }

    public float GetEntropy(Func<T, T, float> metric)
    {
        float entropy = 0f;
        foreach (var member in Members)
        {
            entropy += metric(Centroid, member);
        }

        return entropy / Members.Count;
    }
}

public class SemanticClusterManager<T>
{
    private List<SemanticCluster<T>> clusters = new List<SemanticCluster<T>>();
    private Func<T, T, float> distanceMetric;
    private int optimalClusterSize;
    private float entropyThreshold;

    public SemanticClusterManager(Func<T, T, float> distanceMetric, int datasetSize, float entropyThreshold = 0.5f)
    {
        this.distanceMetric = distanceMetric;
        this.optimalClusterSize = Math.Max(1, (int)Math.Sqrt(datasetSize));
        this.entropyThreshold = entropyThreshold;
    }

    public void Insert(T item)
    {
        if (clusters.Count == 0)
        {
            var cluster = new SemanticCluster<T> { Centroid = item };
            cluster.Members.Add(item);
            clusters.Add(cluster);
            return;
        }

        SemanticCluster<T> bestCluster = null;
        float bestClusterDist = float.MaxValue;

        foreach (var cluster in clusters)
        {
            float d = cluster.Distance(item, cluster.Centroid, distanceMetric);
            if (d < bestClusterDist)
            {
                bestClusterDist = d;
                bestCluster = cluster;
            }
        }

        if (bestCluster.Members.Count >= optimalClusterSize)
        {
            var newCluster = new SemanticCluster<T> { Centroid = item };
            newCluster.Members.Add(item);
            clusters.Add(newCluster);
        }
        else
        {
            bestCluster.Members.Add(item);

            float entropy = bestCluster.GetEntropy(distanceMetric);
            if (entropy > entropyThreshold)
            {
                bestCluster.RecalculateCentroid(distanceMetric);
            }
        }
    }

    public T QueryNearest(T target)
    {
        if (clusters.Count == 0)
            throw new InvalidOperationException("No clusters available.");

        SemanticCluster<T> bestCluster = null;
        float bestClusterDist = float.MaxValue;

        foreach (var cluster in clusters)
        {
            float d = cluster.Distance(target, cluster.Centroid, distanceMetric);
            if (d < bestClusterDist)
            {
                bestClusterDist = d;
                bestCluster = cluster;
            }
        }

        T bestMatch = default;
        float bestMemberDist = float.MaxValue;

        foreach (var member in bestCluster.Members)
        {
            float d = distanceMetric(target, member);
            if (d < bestMemberDist)
            {
                bestMemberDist = d;
                bestMatch = member;
            }
        }

        return bestMatch;
    }
}
