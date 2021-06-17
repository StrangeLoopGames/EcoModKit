using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public partial class PrefabUsageCase
{
    public GameObject prefab;
    public List<OffsetCondition> conditions = new List<OffsetCondition>();

    public override string ToString()
    {
        return String.Format("{0} ({1} conditions)", prefab != null ? prefab.name : "None", conditions.Count);
    }
}

[Serializable]
public partial class PrefabRuleBlockBuilder : PrefabBlockBuilder
{
    public List<PrefabUsageCase> usageCases = new List<PrefabUsageCase>();
}