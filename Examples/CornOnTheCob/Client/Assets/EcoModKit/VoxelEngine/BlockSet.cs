// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using UnityEngine;

#nullable enable

[AddComponentMenu("VoxelEngine/BlockSet")]
[ExecuteAlways]
public class BlockSet : ScriptableObject
{
    public List<Block?> Blocks = new List<Block?>();
}
