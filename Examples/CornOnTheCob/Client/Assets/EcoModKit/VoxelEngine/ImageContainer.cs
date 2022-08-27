// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// A container for holding a group of sprites and returning them in a dictionary keyed to their names for later lookup.
/// </summary>
public class ImageContainer : MonoBehaviour
{
    public Sprite[] Sprites = Array.Empty<Sprite>();

    public Dictionary<string, Sprite> GetAll()
    {
        var dict = new Dictionary<string, Sprite>();
        foreach(var sprite in Sprites)
            dict.Add(sprite.name.ToLower(), sprite);
        return dict;
    }
}