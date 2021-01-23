// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using UnityEngine;

public static class GUIUtils
{
    public static Rect[] Separate(Rect mainRect, int xCount, int yCount)
    {
        float itemWidth = mainRect.width / xCount;
        float itemHeight = mainRect.height / yCount;
        var list = new List<Rect>();

        for (int y = 0; y < yCount; y++)
        {
            for (int x = 0; x < xCount; x++)
            {
                var rect = new Rect(mainRect.x + itemWidth * x, mainRect.y + itemHeight * y, itemWidth, itemHeight);
                list.Add(rect);
            }
        }

        return list.ToArray();
    }

    public static Rect Shrink(this Rect rect, int size)
    {
        return new Rect(rect.x - size, rect.y + size, rect.width - (2 * size), rect.height - (2 * size));
    }
}