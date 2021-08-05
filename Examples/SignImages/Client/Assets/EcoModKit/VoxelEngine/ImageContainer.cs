// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ImageContainer : MonoBehaviour
{
    public Sprite[] Sprites;

    public Dictionary<string, Sprite> GetAll()
    {
        var dict = new Dictionary<string, Sprite>();
        foreach(var sprite in Sprites)
            dict.Add(sprite.name.ToLower(), sprite);
        return dict;
    }
}