using Eco.Shared.Utils;
using System;
using System.Linq;
using UnityEngine;

/// <summary> Class used by Localization Tool to hold strings intended for localization. </summary>
[Serializable]
public class StringToLocalize
{
    public string   String             { get; set; } // String to localize
    public string   Source             { get; set; } // String extraction localization
    public string   Comment            { get; set; } // The context comment for localization
    public bool     AutoLoc            { get; set; } // localization was automatic (or manual)
    public bool     HasContextConflict { get; set; } // if diff string has context comments from multiple places
    public string[] ContextConflicts   { get; set; } // conflitcting context comments
    public string   Replacement        { get; set; } // instead of adding string, try to find possible replacement first

    public bool     IsPrefab            => Source?.StartsWith("Prefab: ") ?? false; // is string contained in prefab?

    public static readonly char[] InvalidCommentChars = new char[] { '"', ',' };
    public static void ValidadeComment(string comment, string loc, string file) // content comments can't have certain characters
    {
        if(comment.ContainsAny(InvalidCommentChars)) Debug.LogError($"Invalid context comment '{comment}' for '{loc}' at {file}. Can't use [ quotes (' \" ') or commas ' , '] inside comments.");
    }

    public static implicit operator StringToLocalize(EmbeddedLocString locString) => new StringToLocalize { String = locString.String };
    public static implicit operator StringToLocalize(string str)                  => new StringToLocalize { String = str };
}

public static class StringToLocalizeExtensions
{
    public static StringToLocalize[] AsStringsToLocalize(this EmbeddedLocString[] values) => values.Select(x => (StringToLocalize)x).ToArray();
}