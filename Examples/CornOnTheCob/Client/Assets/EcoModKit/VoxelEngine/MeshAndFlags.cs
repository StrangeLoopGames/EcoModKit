// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using UnityEngine;

#nullable enable

[Serializable]
public struct MeshAndFlags
#if UNITY_EDITOR
    : IEquatable<MeshAndFlags>
#endif
{
    public Mesh? mesh;

    public PerFaceFlag concaveFaces;

#if UNITY_EDITOR
    public bool Equals(MeshAndFlags other)
    {
        return this.mesh == other.mesh && this.concaveFaces == other.concaveFaces;
    }
#endif
}