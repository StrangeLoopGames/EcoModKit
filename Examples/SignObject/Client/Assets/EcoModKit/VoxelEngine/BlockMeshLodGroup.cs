// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using UnityEngine;

#nullable enable

/// <summary>
/// A group of meshes for a block, LOD0 contains the highest detail varians, LOD1 contains mid detail and LOD2 contains low detail.
/// </summary>
[Serializable]
[CreateAssetMenu(fileName = "BlockMeshLodGroup", menuName = "Eco/New BlockMeshLodGroup", order = 0)]
public class BlockMeshLodGroup : ScriptableObject
{
    /// <summary> An array of a single mesh, or multiple meshes used as variants.</summary>
    public MeshAndFlags[] LOD0 = new MeshAndFlags[1];

    /// <summary>Mesh collider used for block.</summary>
    public Mesh? Collider;

    /// <summary>A temporary backwards compatible mesh that will work with the existing materials until LODs get rendered separately.</summary>
    public MeshAndFlags LOD1 = new MeshAndFlags();
    public MeshAndFlags LOD2 = new MeshAndFlags();

    public Mesh? GetLodMesh(int lod, int index)
    {
        return lod switch
        {
            2 => LOD2.mesh != null ? LOD2.mesh : LOD1.mesh != null ? LOD1.mesh : LOD0[0].mesh,
            1 => LOD1.mesh != null ? LOD1.mesh : LOD0[0].mesh,
            0 => LOD0[index % LOD0.Length].mesh,
            _ => LOD0[0].mesh,
        };
    }

#if UNITY_EDITOR
    /// <summary>
    /// Validate the meshes so that LOD0 is either a single null mesh, or multiple valid meshes.
    /// </summary>
    public void OnValidate()
    {
        LOD0 = LOD0.Where(l => l.mesh != null).ToArray();

        if (LOD0.Length < 1)
            LOD0 = new MeshAndFlags[1];

        foreach (var lod0 in LOD0)
        {
            if (lod0.mesh != null && !lod0.mesh.isReadable)
                Debug.LogError($"BlockMeshLodGroup {this.name}: LOD0 mesh {lod0.mesh.name} isn't marked as readable");
        }

        if (LOD1.mesh != null)
        {
            if (!LOD1.mesh.isReadable)
                Debug.LogError($"BlockMeshLodGroup {this.name}: LOD1 mesh {LOD1.mesh.name} isn't marked as readable");

            if (LOD1.mesh.subMeshCount > 1)
                Debug.LogError($"BlockMeshLodGroup {this.name}: LOD1 mesh {LOD1.mesh.name} has multiple submeshes");
        }

        if (LOD2.mesh != null)
        {
            if (!LOD2.mesh.isReadable)
                Debug.LogError($"BlockMeshLodGroup {this.name}: LOD2 mesh {LOD2.mesh.name} isn't marked as readable");

            if (LOD2.mesh.subMeshCount > 1)
                Debug.LogError($"BlockMeshLodGroup {this.name}: LOD1 mesh {LOD2.mesh.name} has multiple submeshes");
        }
    }

    public bool Equals(BlockMeshLodGroup rhs)
    {
        return LOD0.SequenceEqual(rhs.LOD0) &&
            LOD1.Equals(rhs.LOD1) &&
            LOD2.Equals(rhs.LOD2);
    }
#endif
}
