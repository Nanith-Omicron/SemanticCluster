
using System;

public class SemanticClusterSystem<T>
{
    private SemanticClusterManager<T> manager;

    public SemanticClusterSystem(Func<T, T, float> distanceMetric, int datasetSize, float entropyThreshold = 0.5f)
    {
        manager = new SemanticClusterManager<T>(distanceMetric, datasetSize, entropyThreshold);
    }

    public void Add(T item) => manager.Insert(item);

    public T Query(T item) => manager.QueryNearest(item);
}
