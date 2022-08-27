// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

#nullable enable

using System;

/// <summary>A simple enum using a bit per cube face.</summary>
[Flags]
public enum PerFaceFlag : sbyte
{
    None =  0,
    Front = 1 << 0,
    Back  = 1 << 1,
    Right = 1 << 2,
    Left  = 1 << 3,
    Up    = 1 << 4,
    Down  = 1 << 5,
    All   = Front | Back | Right | Left | Up | Down
}
