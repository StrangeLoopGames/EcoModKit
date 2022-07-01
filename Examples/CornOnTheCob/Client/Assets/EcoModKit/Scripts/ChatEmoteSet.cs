using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>Represents a emote set usable by the chat emote manager.</summary>
[CreateAssetMenu(fileName = "ChatEmoteSet", menuName = "Eco/Chat Emote Set")]
public class ChatEmoteSet : ScriptableObject
{
    /// <summary><see cref="ChatEmote"/> instances registered to this emote set.</summary>
    public ChatEmote[] Emotes;
}
