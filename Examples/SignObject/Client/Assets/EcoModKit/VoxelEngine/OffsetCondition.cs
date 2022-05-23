// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A reference to a block surrounding the target block with rules used for choosing a mesh usage case.
/// </summary>
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
