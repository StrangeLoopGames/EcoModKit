// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

#nullable enable

using System;

/// <summary>
/// A base class for prefab builders which return a prefab given the surrounding set of blocks.
/// </summary>
[Serializable]
public abstract partial class PrefabBlockBuilder : BlockBuilder
{
}