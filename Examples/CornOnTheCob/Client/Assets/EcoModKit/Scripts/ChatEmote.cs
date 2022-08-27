// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;
using System;

/// <summary>Represents a chat emote inside a <see cref="ChatEmoteSet"/></summary>
[Serializable]
public class ChatEmote
{
    /// <summary>Primary name of the emote. This will be used to access it via the <c>:name:</c> text pattern.</summary>
    public string Name;

    /// <summary>Potential aliases of this emote. These do not need to follow the <c>:name:</c> pattern.</summary>
    public string[] Aliases;

    /// <summary>Sprite image to use for this emote.</summary>
    public Sprite Emote;

    /// <summary>Name used to display this chat emote in chat.</summary>
    public string KeywordName => $":{Name}:";
}

