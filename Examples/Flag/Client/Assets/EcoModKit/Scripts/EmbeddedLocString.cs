using Eco.Shared.Localization;
using UnityEngine;

[System.Serializable]
public class EmbeddedLocString
{
    [TextArea(3, 10)]
    public string String;

    public EmbeddedLocString() => this.String = string.Empty;

    public static implicit operator EmbeddedLocString(string val) => new EmbeddedLocString { String = val };

    public string Localized => Localizer.DoStr(this.String);

    public override bool Equals(object obj) => obj is EmbeddedLocString other && this.String.Equals(other.String);

    public override int GetHashCode() => this.String.GetHashCode();
}