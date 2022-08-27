using System;
using System.Collections.Generic;

#nullable enable

/// <summary>
/// PrefabRuleBlockBuilder uses rules similar to CustomBuilder to return a prefab.
/// </summary>
[Serializable]
public partial class PrefabRuleBlockBuilder : PrefabBlockBuilder
{
    public List<PrefabUsageCase> usageCases = new List<PrefabUsageCase>();
}