using System;
using UnityEngine;

[Serializable]
public partial class WeightedPrefabBlockBuilder : PrefabBlockBuilder
{
    [Serializable]
    public struct WeightedPrefab
    {
        public GameObject prefab;
        public float weight;
    }

    public WeightedPrefab[] prefabs;
}