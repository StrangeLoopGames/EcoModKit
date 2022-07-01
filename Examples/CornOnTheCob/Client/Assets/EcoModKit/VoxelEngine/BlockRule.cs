// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

#nullable enable

using System;

[Serializable]
public partial class BlockRule
{
    public enum RuleType
    {
        EqualsType,
        NotEqualsType,
        NotSolidType,
        IsSolidType,
        EqualsCategory,
        NotEqualsCategory,
        EqualsThisType,
        NotEqualsThisType,
        SameOrHigherPriority,
        LowerPriorirty,
    }

    public RuleType ruleType;
    public string ruleString = String.Empty;
    public int ruleID;

    public override string ToString()
    {
        switch (ruleType)
        {
            case RuleType.NotSolidType:
                return "NOT Solid";
            case RuleType.IsSolidType:
                return "Solid";
            case RuleType.EqualsType:
            case RuleType.EqualsCategory:
                return ruleString;
            case RuleType.NotEqualsCategory:
            case RuleType.NotEqualsType:
                return "NOT " + ruleString;
            case RuleType.EqualsThisType:
                return "Same Type";
            case RuleType.NotEqualsThisType:
                return "NOT Same Type";
            case RuleType.SameOrHigherPriority:
                return "Same or Higher Priority";
            case RuleType.LowerPriorirty:
                return "Lower Priority";
        }

        return String.Empty;
    }
}
