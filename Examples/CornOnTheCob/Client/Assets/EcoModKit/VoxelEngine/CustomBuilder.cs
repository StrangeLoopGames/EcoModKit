// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using UnityEngine;

#nullable enable

/// <summary>
/// CustomBuilder is used for choosing a block mesh dependent on the blocks that surround it. This is key to allowing continuous geometry as opposed to plain blocks.
/// </summary>
public partial class CustomBuilder : BlockBuilder
{
    public List<MeshUsageCase> usageCases = new List<MeshUsageCase>();

    /// <summary>Material used when using preview editor</summary>
    public Material? previewMaterial;
}
