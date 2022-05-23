// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

#nullable enable

/// <summary>
/// Simple MonoBehaviour used to contain a list of BlockSets. These are loaded into the modkit at load time.
/// </summary>
public class BlockSetContainer : MonoBehaviour
{
    public BlockSet[] blockSets = new BlockSet[1];
}