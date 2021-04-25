using Eco.Shared.Localization;
using Eco.Shared.Utils;
using System;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class EmbeddedLocString
{
    [TextArea(3, 10)]
    public string String;
    // solution for TextComponents text into formattable localization like  "12 plots. 300 sq m." -> {0} plots. {1} sq m. 
    public EmbeddedLocStringParameter[] Params = new EmbeddedLocStringParameter[] { };
    public EmbeddedLocString()                                    => this.String = string.Empty;
    public static implicit operator EmbeddedLocString(string val) => new EmbeddedLocString { String = val};
    public string Localized                                       => Params.Length > 0 ? StringParamReplaced(Localizer.DoStr(this.StringParameterized())) : Localizer.DoStr(this.String);
    public override bool Equals(object obj)                       => obj is EmbeddedLocString other && this.String.Equals(other.String);
    public override int GetHashCode()                             => this.String.GetHashCode();

    public string StringParameterized() // return loc string with manual params e.g: "12 plots. 300 sq m." -> "{0} plots. {1} sq m."
    {
        string stringWithParams = String;
        Params.ForEachIndex((p,i) => stringWithParams = stringWithParams.ReplaceFirst(p.stringParameter, $"{{{i}}}"));
        return stringWithParams;
    }

    private string StringParamReplaced(string s) // adds back loc parameters e.g: "{0} plots. {1} sq m." -> "12 plots. 300 sq m."
    {   
        Params.ForEachIndex((p, i) => s = s.ReplaceFirst($"{{{i}}}", p.localize ? Localizer.DoStr(p.stringParameter) : p.stringParameter));
        return s;
    }
}

// simplification TextComponents to replace text into formattable localization like  "12 plots. 300 sq m." -> {0} plots. {1} sq m.
// this way we dont have to use 4 text components ([12] [plots.] [300] [sq m.]) and use preexisting localizations 
//                                              not loc   loc   not loc   loc
[Serializable]
public class EmbeddedLocStringParameter
{
    public string stringParameter;  // string to look for by (stops at first). e.g.: text = '12 plots. 300 sq m.' we can replace "12" or "300"
    public bool   localize;         // localize parameter or not
}