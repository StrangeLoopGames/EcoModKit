// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

#nullable enable

using System;
using UnityEngine;

/// <summary>
/// Returns a prefab based on a weighted list and a random number used to select from the list.
/// </summary>
[Serializable]
public partial class WeightedPrefabBlockBuilder : PrefabBlockBuilder
{
    [Serializable]
    public struct WeightedPrefab
    {
        public GameObject? prefab;
        public float weight;
    }

    public WeightedPrefab[] prefabs = Array.Empty<WeightedPrefab>();
}