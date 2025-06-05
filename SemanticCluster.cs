using System;
using System.Collections.Generic;
using System.Linq;

namespace Semantic
{
    public interface IDistanceMetric<T>
    {
        float GetDistance(T a, T b);
    }

    public interface IScalarFilter<T>
    {
        bool Passes(T item);
        void Update(T item);
    }

    public class DefaultScalarFilter<T> : IScalarFilter<T>
    {
        private readonly Func<T, float> scalarProjection;
        private float mean = 0f;
        private float m2 = 0f;
        private int count = 0;
        private float stdDev = 1f;
        private readonly float threshold;

        public DefaultScalarFilter(Func<T, float> projection, float threshold = 1.5f)
        {
            scalarProjection = projection;
            this.threshold = threshold;
        }

        public void Update(T item)
        {
            float value = scalarProjection(item);
            count++;
            float delta = value - mean;
            mean += delta / count;
            m2 += delta * (value - mean);
            stdDev = count > 1 ? (float)Math.Sqrt(m2 / (count - 1)) : 1f;
        }

        public bool Passes(T item)
        {
            float value = scalarProjection(item);
            return Math.Abs(value - mean) <= threshold * stdDev;
        }
    }

    public class SemanticCluster<T>
    {
        public readonly List<T> Items = new();
        public T Centroid;

        private readonly IDistanceMetric<T> distanceMetric;

        public SemanticCluster(IDistanceMetric<T> metric)
        {
            distanceMetric = metric;
        }

        public void Add(T item)
        {
            Items.Add(item);
            Centroid = RecalculateCentroid();
        }

        private T RecalculateCentroid()
        {
            if (Items.Count == 0) return default;
            return FindClosest(Items[^1]);
        }

        public T FindClosest(T query)
        {
            if (Items.Count == 0) return default;
            if (Items.Count == 1) return Items[0];

            int probeIndex = Items.Count - 1;
            T closest = Items[probeIndex];
            float minDist = distanceMetric.GetDistance(query, closest);
            int worseningCount = 0;

            for (int i = probeIndex - 1; i >= 0; i--)
            {
                float dist = distanceMetric.GetDistance(query, Items[i]);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = Items[i];
                    worseningCount = 0;
                }
                else if (++worseningCount >= 5)
                {
                    break;
                }
            }

            return closest;
        }

        public bool Contains(T item, float threshold)
        {
            foreach (var entry in Items)
            {
                if (distanceMetric.GetDistance(item, entry) <= threshold)
                    return true;
            }
            return false;
        }
    }

    public class HierarchicalSemanticCluster<T>
    {
        private readonly List<SemanticCluster<T>> topLevelClusters = new();
        private readonly IDistanceMetric<T> distanceMetric;
        private readonly IScalarFilter<T> prefilter;

        public IReadOnlyList<SemanticCluster<T>> Clusters => topLevelClusters;

        public HierarchicalSemanticCluster(IDistanceMetric<T> metric, IScalarFilter<T> filter)
        {
            distanceMetric = metric;
            prefilter = filter;
        }

        public void Add(T item)
        {
            foreach (var cluster in topLevelClusters)
            {
                if (cluster.Items.Count > 0 && prefilter.Passes(item))
                {
                    cluster.Add(item);
                    prefilter.Update(item);
                    return;
                }
            }

            var newCluster = new SemanticCluster<T>(distanceMetric);
            newCluster.Add(item);
            topLevelClusters.Add(newCluster);
            prefilter.Update(item);
        }

        public T FindClosest(T query)
        {
            var candidateClusters = topLevelClusters
                .Where(c => c.Items.Count > 0)
                .OrderBy(c => distanceMetric.GetDistance(query, c.Centroid))
                .Take(3);

            T best = default;
            float minDist = float.MaxValue;

            foreach (var cluster in candidateClusters)
            {
                T candidate = cluster.FindClosest(query);
                float dist = distanceMetric.GetDistance(query, candidate);
                if (dist < minDist)
                {
                    minDist = dist;
                    best = candidate;
                }
            }
            return best;
        }

        public bool ContainsApproximately(T query, float threshold)
        {
            foreach (var cluster in topLevelClusters)
            {
                if (cluster.Items.Count == 0 || !prefilter.Passes(query))
                    continue;

                T candidate = cluster.FindClosest(query);
                float dist = distanceMetric.GetDistance(query, candidate);
                if (dist <= threshold)
                    return true;
            }
            return false;
        }
    }
}
