using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

/// <summary>
/// A usage case for PrefabRuleBlockBuilder. When the conditions are met the prefab will be used.
/// </summary>
[Serializable]
public partial class PrefabUsageCase
{
    public GameObject? prefab;
    public List<OffsetCondition> conditions = new List<OffsetCondition>();

    public override string ToString()
    {
        return String.Format("{0} ({1} conditions)", prefab != null ? prefab.name : "None", conditions.Count);
    }
}
