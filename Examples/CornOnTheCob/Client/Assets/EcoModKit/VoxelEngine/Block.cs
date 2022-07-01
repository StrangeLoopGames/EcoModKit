// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Eco.Shared.Items;
using Eco.Shared.Localization;
using UnityEngine.Serialization;
using VoxelEngine.Materials;

/// <summary>
/// Block contains all the client side data for a block, including a builder which conains rules and meshes for rendering.
/// </summary>
[Serializable]
[DebuggerDisplay("{Name}")]
public partial class Block 
{
    public string       Name;
    public BlockBuilder Builder;
    public Material     Material;
    [FormerlySerializedAs("Texture")]
    public Texture2D    LODTexture;
    
    /// <summary>
    /// Force set this.<see cref="Material"/> to be transparent. Check comment in
    /// <see cref="IsMaterialTransparent"/> for more details.
    /// </summary>
    public OverrideMaterialTransparency OverrideMainMaterialTransparency;
    public Material[] Materials = new Material[0];
    /// <summary>
    /// Similar to this.<see cref="OverrideMainMaterialTransparency"/> but each of this
    /// corresponding to an item in this.<see cref="Materials"/>.
    /// </summary>
    public OverrideMaterialTransparency[] OverrideSubMaterialsTransparency;

    public Color MinimapColor = Color.green;
    public bool IsDiggable = false;
    public bool IsWater = false;
    public bool Solid = true;
    public bool WaterOccupancy = false;
    public bool BuildCollider = true;
    public bool Rendered = true;
    public UnityEngine.Rendering.ShadowCastingMode ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    public string Layer = "Terrain";
    public string Category = "Default";
    public int Tier = 0;

    public int BlendingPriority = 0;
    
    public bool GenerateMeshCollider = false;

    public bool IsEmpty;
    public float PrefabHeightOffset = -.5f;
    public float ActualHeight       = 1f; // Actual block height, for specific block forms could be more than 1 (1.15 for roofs) or less than 0.5 (0.15 for stacked form)

    public string AudioCategory; //This is used for tool interactions

    public string MusicCategory; //This is used for the music system

    public GameObject[] Effects = new GameObject[0];
    public string[] EffectNames = new string[0];

    public bool IsLadder;
    public bool IsSlope;

#if UNITY_EDITOR
    public UnityEditor.Editor Editor;
#endif
}