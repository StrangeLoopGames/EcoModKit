// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Eco.Shared.Math;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using System.Linq;

[Serializable]
public class BlockRule
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
    }

    public RuleType ruleType;
    public string ruleString;
    public int ruleID;
    public Dictionary<int, bool> ruleIDs;

    public bool Evaluate(Block thisBlock, Block otherBlock)
    {
        switch (ruleType)
        {
            case RuleType.EqualsType:
                return ruleIDs == null ? otherBlock.nameID == ruleID : ruleIDs.ContainsKey(otherBlock.nameID);
            case RuleType.NotEqualsType:
                return ruleIDs == null ? otherBlock.nameID != ruleID : !ruleIDs.ContainsKey(otherBlock.nameID);
            case RuleType.NotSolidType:
				return !otherBlock.Solid;
            case RuleType.IsSolidType:
                return otherBlock.Solid;
            case RuleType.EqualsCategory:
                return otherBlock.categoryID == ruleID;
            case RuleType.NotEqualsCategory:
                return otherBlock.categoryID != ruleID;
            case RuleType.EqualsThisType:
                return otherBlock.nameID == thisBlock.nameID;
            case RuleType.NotEqualsThisType:
                return otherBlock.nameID != thisBlock.nameID;
        }

        return false;
    }

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
        }

        return String.Empty;
    }
}

[Serializable]
public class OffsetCondition
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

    public static Vector3i[] OffsetMapping =
    {
        new Vector3i(-1, 1, -1),  // Offset_020
        new Vector3i( 0, 1, -1),
        new Vector3i( 1, 1, -1),
        new Vector3i(-1, 1, 0),
        new Vector3i( 0, 1, 0),
        new Vector3i( 1, 1, 0),
        new Vector3i(-1, 1, 1),
        new Vector3i( 0, 1, 1),
        new Vector3i( 1, 1, 1),

        new Vector3i(-1, 0, -1),  // Offset_010
        new Vector3i( 0, 0, -1),
        new Vector3i( 1, 0, -1),
        new Vector3i(-1, 0, 0),
        //new Vector3i( 0, 0, 0),
        new Vector3i( 1, 0, 0),
        new Vector3i(-1, 0, 1),
        new Vector3i( 0, 0, 1),
        new Vector3i( 1, 0, 1),

        new Vector3i(-1, -1, -1),  // Offset_000
        new Vector3i( 0, -1, -1),
        new Vector3i( 1, -1, -1),
        new Vector3i(-1, -1, 0),
        new Vector3i( 0, -1, 0),
        new Vector3i( 1, -1, 0),
        new Vector3i(-1, -1, 1),
        new Vector3i( 0, -1, 1),
        new Vector3i( 1, -1, 1),
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

    static public Offset GetFromVector(Vector3i v)
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

    public bool Evaluate(BlockKey key)
    {
        var block = MapConfig.obj.blockMap[key.blocks[IndexMapping[(int)this.offsetType]]];
        var thisBlock = MapConfig.obj.blockMap[key.blocks[13]];
        for (int i = 0; i < rules.Count; i++)
        {
            if (!rules[i].Evaluate(thisBlock, block))
                return false;
        }

        return true;
    }

    public override string ToString()
    {
        if (rules.Count == 1)
            return rules[0].ToString();

        return string.Join("\n", rules.Select(rule => rule.ToString()).ToArray());
    }
}

[Serializable]
public class MeshUsageCase
{
    public bool enabled = true;

    [NonSerialized]
    public bool foldout = false;

    public GameObject mesh; // unity gameobject (mesh) to use, not accessible on other threads

    public GameObject[] meshAlternates = new GameObject[0];                          // alternate meshes to use
    public ThreadSafeMeshCopy[] storedMeshAlternates = new ThreadSafeMeshCopy[0];    // thread safe versions (1*)
    public ThreadSafeMeshCopy[] rotationMeshAlternates = new ThreadSafeMeshCopy[0];  // rotated variations (3*)
    
    [NonSerialized]
    public ThreadSafeMeshCopy storedMesh;   // copied mesh data, usable on other threads

    [NonSerialized]
    private MeshUsageCase[] alternates;     // 3 rotation alternate meshes to use

    public List<OffsetCondition> conditions = new List<OffsetCondition>();

    public bool applyConditionsToAllRotations = true;
    public bool dontRotateBaseMesh = false;

    public Vector3 importRotation = new Vector3(0, 0, 0);

    // decorative mesh builder.  When this usage case is chosen, these rules are then additionally evaluated to determine
    // the resulting mesh decorations added
    // this is primarily used to blend or obscure one type of terrain or block with another type nearby
    public CustomBuilder decorativeBuilder;

    public void BuildStoredMesh(CustomBuilder bldr, Matrix4x4 xform)
    {
        Matrix4x4 rotxForm = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 90, 0), Vector3.one);
        Matrix4x4 rotxForm2 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 180, 0), Vector3.one);
        Matrix4x4 rotxForm3 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 270, 0), Vector3.one);

        storedMesh = ThreadSafeMeshCopy.GetCopyOf(bldr, mesh, xform);

        // generate alternate mesh options
        if (meshAlternates.Length > 0)
        {
            if (applyConditionsToAllRotations)
                rotationMeshAlternates = new ThreadSafeMeshCopy[meshAlternates.Length * 3];

            storedMeshAlternates = new ThreadSafeMeshCopy[meshAlternates.Length];
            for (int baseAlt = 0; baseAlt < meshAlternates.Length; baseAlt++)
            {
                storedMeshAlternates[baseAlt] = ThreadSafeMeshCopy.GetCopyOf(bldr, meshAlternates[baseAlt], xform);

                if (applyConditionsToAllRotations)
                {
                    rotationMeshAlternates[baseAlt * 3 + 0] = ThreadSafeMeshCopy.GetCopyOf(bldr, meshAlternates[baseAlt], rotxForm * xform);
                    rotationMeshAlternates[baseAlt * 3 + 1] = ThreadSafeMeshCopy.GetCopyOf(bldr, meshAlternates[baseAlt], rotxForm2 * xform);
                    rotationMeshAlternates[baseAlt * 3 + 2] = ThreadSafeMeshCopy.GetCopyOf(bldr, meshAlternates[baseAlt], rotxForm3 * xform);
                }
            }
        }

        // generate base rotation options
        if (applyConditionsToAllRotations)
        {
            alternates = new MeshUsageCase[3];

            alternates[0] = GenerateRotationCase(bldr, ref xform, ref rotxForm, 0);
            alternates[1] = GenerateRotationCase(bldr, ref xform, ref rotxForm2, 1);
            alternates[2] = GenerateRotationCase(bldr, ref xform, ref rotxForm3, 2);
        }
        else
        {
            alternates = null;
        }
    }

    private MeshUsageCase GenerateRotationCase(CustomBuilder bldr, ref Matrix4x4 xform, ref Matrix4x4 rotxForm, int rotIdx)
    {
        var rot = new MeshUsageCase();
        rot.applyConditionsToAllRotations = false;

        if (dontRotateBaseMesh)
            rot.storedMesh = this.storedMesh; // just use same mesh, but rotated rules
        else
            rot.storedMesh = ThreadSafeMeshCopy.GetCopyOf(bldr, mesh, rotxForm * xform);

        foreach (var condition in conditions)
        {
            // rotate offset vectors
            // 0, 0, 1
            // -1, 0, 0
            // 0, 0, -1
            // 1, 0, 0

            Vector3i offset = OffsetCondition.OffsetMapping[(int)condition.offsetType];
            Vector3i rotOff = new Vector3i(offset.z, offset.y, -offset.x);
            for (int r = 0; r < rotIdx; r++ )
                rotOff = new Vector3i(rotOff.z, rotOff.y, -rotOff.x);

            // TODO: fix
            int idx = 0;
            foreach (var mapping in OffsetCondition.OffsetMapping)
            {
                if (mapping == rotOff)
                    break;
                idx++;
            }

            OffsetCondition rotCondition = new OffsetCondition((OffsetCondition.Offset)idx);
            rotCondition.rules = condition.rules;
            
            rot.conditions.Add(rotCondition);
        }

        return rot;
    }

    public ThreadSafeMeshCopy Evaluate(Vector3i posLocal, BlockKey key)
    {
        ThreadSafeMeshCopy result = storedMesh;

        int variant = 0;
        if (storedMeshAlternates.Length > 0)
        {
            var r = new System.Random(key.worldPos.GetHashCode()); // base random off the world pos, so its the same variant each time we build the same world pos
            variant = r.Next(storedMeshAlternates.Length + 1);
            if (variant != 0)
                result = storedMeshAlternates[variant - 1];
        }
        
        for (int i = 0; i < conditions.Count; i++)
        {
            if (!conditions[i].Evaluate(key))
            {
                result = null;
                break;
            }
        }

        if (result == null && applyConditionsToAllRotations)
        {
            for (int alt = 0; alt < alternates.Length; alt++)
            {
                result = alternates[alt].Evaluate(posLocal, key);
                if (result != null)
                {
                    if (rotationMeshAlternates.Length > 0 && variant != 0)
                        result = rotationMeshAlternates[(variant - 1) * 3 + alt];
                    break;
                }
            }
        }
        
        return result;
    }

    public override string ToString()
    {
        return String.Format("{0} ({1} conditions)", mesh != null ? mesh.name : "None", conditions.Count);
    }
}

public abstract class MeshFace
{
    public const int Forward = 0;
    public const int Back = 1;
    public const int Left = 2;
    public const int Right = 3;
    public const int Up = 4;
    public const int Down = 5;

    public static string[] Names = new string[] { "Forward [+Z]", "Back [-Z]", "Left [-X]", "Right [+X]", "Up [+Y]", "Down [-Y]" };
}

public class BlockBuilder : ScriptableObject
{ }

public class CustomBuilder : BlockBuilder
{
    public List<MeshUsageCase> usageCases = new List<MeshUsageCase>();

    // material used when using preview editor
    public Material previewMaterial;

    void RebuildUsageCases()
    {        
        foreach (var usageCase in usageCases)
        {
            Matrix4x4 importXForm = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(usageCase.importRotation), Vector3.one);

            if (usageCase.importRotation == Vector3.zero)
                usageCase.BuildStoredMesh(this, importXForm);
            else
                usageCase.BuildStoredMesh(this, Matrix4x4.identity);
        }
    }

    // map the rule strings to IDs for faster matching when performing evaluation of the rule set
    public void Initialize()
    {
        int caseIdx = 0;
        foreach (var usageCase in usageCases)
        {
            foreach (var condition in usageCase.conditions)
            {
                foreach (var rule in condition.rules)
                {
                    if (rule.ruleType == BlockRule.RuleType.EqualsCategory ||
                        rule.ruleType == BlockRule.RuleType.NotEqualsCategory)
                    {
                        if (!MapConfig.obj.categoryIDs.TryGetValue(rule.ruleString, out rule.ruleID))
                        {
                            if (string.IsNullOrEmpty(rule.ruleString))
                                Debug.Log("Bad rule type, string is null or empty on: " + this.name + " offset: " + condition.offsetType.ToString());
                            else
                                Debug.Log("Bad rule ID: could not find matching category type for: " + rule.ruleString + " on: " + this.name + " case: " + caseIdx + " offset: " + condition.offsetType.ToString());
                            rule.ruleID = -1;
                        }
                    }
                    else if (rule.ruleType == BlockRule.RuleType.EqualsType ||
                        rule.ruleType == BlockRule.RuleType.NotEqualsType)
                    {
                        if (rule.ruleString.Contains(","))
                        {
                            rule.ruleIDs = new Dictionary<int, bool>();
                            foreach (var type in rule.ruleString.Split(','))
                            {
                                if (MapConfig.obj.typeIDs.TryGetValue(type, out rule.ruleID))
                                    rule.ruleIDs.Add(rule.ruleID, true);
                                else
                                    Debug.Log("Bad rule ID: could not find matching block type for: " + type + " in " + rule.ruleString + " on: " + this.name + " case: " + caseIdx + " offset: " + condition.offsetType.ToString());
                            }
                            rule.ruleID = -1;
                        }
                        else if (!MapConfig.obj.typeIDs.TryGetValue(rule.ruleString, out rule.ruleID))
                        {
                            if (string.IsNullOrEmpty(rule.ruleString))
                                Debug.Log("Bad rule type, string is null or empty on: " + this.name + " offset: " + condition.offsetType.ToString());
                            else
                                Debug.Log("Bad rule ID: could not find matching block type for: " + rule.ruleString + " on: " + this.name + " case: " + caseIdx + " offset: " + condition.offsetType.ToString());
                            rule.ruleID = -1;
                        }
                    }
                }
            }
            caseIdx++;
        }

        RebuildUsageCases();
    }
}
