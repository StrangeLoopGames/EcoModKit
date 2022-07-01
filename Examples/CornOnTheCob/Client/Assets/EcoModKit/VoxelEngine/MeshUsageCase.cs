// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// A single case used in the CustomBuilder. When the conditions are met, the included mesh or alternatives will be used to render a block.
/// </summary>
[Serializable]
public partial class MeshUsageCase
{
    public bool enabled = true;

    [NonSerialized]
    public bool foldout = false;

    /// <summary>Unity gameobject (mesh or prefab) to use, not accessible on other threads.</summary>
    public GameObject? mesh;

    /// <summary>Asset that contains all the meshes for different variations, LODs, and collider.</summary>
    public BlockMeshLodGroup? blockMeshLodGroup;

    /// <summary>
    /// A 6 bits flag corresponding to 6 faces of the cube (down, up, left, right, back, front)
    /// ChunkBuilder.FaceRemover3000 uses a more precise algorithm with the corresponding face.
    /// </summary>
    public PerFaceFlag isMeshFacesConcave;

    /// <summary>Alternate meshes to use based on a per-position noise value. Allows for some variation in blocks.</summary>
    public GameObject[] meshAlternates = new GameObject[0];

    /// <summary>PerFaceFlag for each item in meshAlternates.</summary>
    public PerFaceFlag[]? isMeshAlternatesFacesConcave;

    /// <summary> A list of conditions that must be met for this case to be chosen.</summary>
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

    public GameObject[] GetMeshObjects()
    {
        if (mesh == null) return Array.Empty<GameObject>();

        var meshes = new List<GameObject>();
        meshes.Add(mesh);
        meshes.AddRange(meshAlternates);
        return meshes.ToArray();
    }
}
