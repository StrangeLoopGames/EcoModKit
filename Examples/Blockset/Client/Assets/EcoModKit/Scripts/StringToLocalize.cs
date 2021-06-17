using System;
using System.Linq;

/// <summary> Class used by Localization Tool to hold strings intended for localization. </summary>
[Serializable]
public class StringToLocalize
{
    public string String  { get; set; } // String to localize
    public string Source  { get; set; } // String extraction localization
    public string Comment { get; set; } // The context comment for localization
    public bool   AutoLoc { get; set; } // localization was automatic (or manual)

    public static implicit operator StringToLocalize(EmbeddedLocString locString) => new StringToLocalize { String = locString.String };
    public static implicit operator StringToLocalize(string str) => new StringToLocalize { String = str };
}

public static class StringToLocalizeExtensions
{
    public static StringToLocalize[] AsStringsToLocalize(this EmbeddedLocString[] values) => values.Select(x => (StringToLocalize)x).ToArray();
}