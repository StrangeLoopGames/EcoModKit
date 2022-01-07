// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;
using System;
using System.Collections.Generic;
using Eco.Shared.Items;
using Eco.Shared.Localization;
using VoxelEngine.Materials;

/// <summary>Force set a material to be transparent or opaque.</summary>
public enum OverrideMaterialTransparency : byte
{
    NotOverride,
    ForceTransparent,
    ForceOpaque
}

[Serializable]
public partial class Block 
{
    public const ushort InvalidId = 0xFFFF;

    public string Name;
    public BlockBuilder Builder;
    public Material Material;
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

    // post init, these are set to the same value for faster compares
    [NonSerialized] public int categoryID;
    [NonSerialized] public ushort nameID;
    
    public bool GenerateMeshCollider = false;

    public bool IsEmpty;
    public float PrefabHeightOffset = -.5f;
    public float ActualHeight       = 1f; // Actual block height, for specific block forms could be more than 1 (1.15 for roofs) or less than 0.5 (0.15 for stacked form)

    public string AudioCategory; //This is used for tool interactions

    public string MusicCategory; //This is used for the music system

    public GameObject[] Effects = new GameObject[0];
    public string[] EffectNames = new string[0];

    [NonSerialized] public MaterialInfo   MaterialInfo;
    [NonSerialized] public MaterialInfo[] MaterialInfos = Array.Empty<MaterialInfo>();
    [NonSerialized] public bool           Interactable;
    [NonSerialized] public bool           SticksToWalls;
    [NonSerialized] public float          MoveEfficiency = 1f;
    [NonSerialized] public int            Hardness       = 0;
    [NonSerialized] public bool           IsPlant        = false;
    [NonSerialized] public int            rotation       = 0;
    [NonSerialized] public LocString      LookAtTooltip;
    // If each material is transparent. IsMaterialTransparent[0] is for this.Material.
    // Elements from IsMaterialTransparent[1] to IsMaterialTransparent[IsMaterialTransparent.Count - 1] is for
    // this.Materials. If the material is transparent, it will be skipped from generating shadow hull in
    // ChunkBuilder.BuildWithBuildContext.
    [NonSerialized] public List<bool>     IsMaterialTransparent;

    public bool IsLadder;
    public bool IsSlope;

    /// <summary> True if the block is empty, water, or a non-tree plant. </summary>
    public bool CanBuild => IsEmpty || IsWater || IsPlant;

#if UNITY_EDITOR
    public UnityEditor.Editor Editor;
#endif
}