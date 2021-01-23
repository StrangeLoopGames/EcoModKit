// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    public string ruleString;
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

[Serializable]
public partial class OffsetCondition
{
    public Offset offsetType;
    public List<BlockRule> rules = new List<BlockRule>();

    public enum Offset
    {
        // above
        Offset_020,  // equivalent is vector3(-1, 1, -1)
        Offset_120,
        Offset_220,
        Offset_021,
        Offset_121,
        Offset_221,
        Offset_022,
        Offset_122,
        Offset_222,

        // same level
        Offset_010,
        Offset_110,
        Offset_210,
        Offset_011,
        //Offset_111,  the center block
        Offset_211,
        Offset_012,
        Offset_112,
        Offset_212,

        // below
        Offset_000,
        Offset_100,
        Offset_200,
        Offset_001,
        Offset_101,
        Offset_201,
        Offset_002,
        Offset_102,
        Offset_202,

        Offset_Null
    }

    public static Vector3[] OffsetMapping =
    {
        new Vector3(-1, 1, -1),  // Offset_020
        new Vector3( 0, 1, -1),
        new Vector3( 1, 1, -1),
        new Vector3(-1, 1, 0),
        new Vector3( 0, 1, 0),
        new Vector3( 1, 1, 0),
        new Vector3(-1, 1, 1),
        new Vector3( 0, 1, 1),
        new Vector3( 1, 1, 1),

        new Vector3(-1, 0, -1),  // Offset_010
        new Vector3( 0, 0, -1),
        new Vector3( 1, 0, -1),
        new Vector3(-1, 0, 0),
        //new Vector3( 0, 0, 0),
        new Vector3( 1, 0, 0),
        new Vector3(-1, 0, 1),
        new Vector3( 0, 0, 1),
        new Vector3( 1, 0, 1),

        new Vector3(-1, -1, -1),  // Offset_000
        new Vector3( 0, -1, -1),
        new Vector3( 1, -1, -1),
        new Vector3(-1, -1, 0),
        new Vector3( 0, -1, 0),
        new Vector3( 1, -1, 0),
        new Vector3(-1, -1, 1),
        new Vector3( 0, -1, 1),
        new Vector3( 1, -1, 1),
    };

    // this is a mapping from the offset enum to the block key index
    // of that same position.  For example, offset (-1, -1, -1) is index 0
    // for the purposes of generating the BlockKey
    public static int[] IndexMapping =
    {
        // keys are generated as x/z/y
        18, 19, 20, 21, 22, 23, 24, 25, 26,
         9, 10, 11, 12,     14, 15, 16, 17,
         0,  1,  2,  3,  4,  5,  6,  7,  8,
    };

    static public Offset GetFromVector(Vector3 v)
    {
        int idx = 0;
        foreach (var mapping in OffsetMapping)
        {
            if (mapping == v)
                return (Offset)idx;
            idx++;
        }
        return Offset.Offset_Null;
    }

    public OffsetCondition(Offset offset)
    {
        this.offsetType = offset;
    }

    public override string ToString()
    {
        if (rules.Count == 1)
            return rules[0].ToString();

        return string.Join("\n", rules.Select(rule => rule.ToString()).ToArray());
    }
}

public enum RotationAxis
{
    AroundX,
    AroundY,
    AroundZ
}

[Serializable]
public partial class MeshUsageCase
{
    public bool enabled = true;

    [NonSerialized]
    public bool foldout = false;

    public GameObject mesh;                                                     // unity gameobject (mesh or prefab) to use, not accessible on other threads

    public GameObject[] meshAlternates = new GameObject[0];                     // alternate meshes to use

    public List<OffsetCondition> conditions = new List<OffsetCondition>();

    public bool applyConditionsToAllRotations = true;
    public RotationAxis axis = RotationAxis.AroundY;
    public bool dontRotateBaseMesh = false;

    public Vector3 importRotation = new Vector3(0, 0, 0);

    // decorative mesh builders.  When this usage case is chosen, these rules are then additionally evaluated to determine
    // the resulting mesh decorations added
    // this is primarily used to blend or obscure one type of terrain or block with another type nearby
    public CustomBuilder[] decorativeBuilders = new CustomBuilder[0];

    public override string ToString() => $"{(this.mesh != null ? this.mesh.name : "None")} ({this.conditions.Count} conditions)";
}

public class BlockBuilder : ScriptableObject
{ }

public partial class CustomBuilder : BlockBuilder
{
    public List<MeshUsageCase> usageCases = new List<MeshUsageCase>();

    // material used when using preview editor
    public Material previewMaterial;
}
