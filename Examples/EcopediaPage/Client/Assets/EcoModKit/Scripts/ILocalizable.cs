using UnityEngine;

public interface ILocalizable
{
    StringToLocalize[] ExtractStrings();
    Component GetComponent();
    void OnApplyChanges();
    void Localize(bool force = false);
    bool IsNewlyAdded { get; set; }
}
