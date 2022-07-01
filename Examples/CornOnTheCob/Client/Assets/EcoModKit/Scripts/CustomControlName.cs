using System;

using UnityEngine;
using UnityEngine.Serialization;

/// <summary> Component that is read by the ControlPopupUI to give custom text for object's controls when it is targeted. </summary>
public partial class CustomControlName : MonoBehaviour
{
    // Default values assigned by the inspector
    [SerializeField, FormerlySerializedAs("LeftMouseName")]   string leftMouseName;
    [SerializeField, FormerlySerializedAs("RightMouseName")]  string rightMouseName;
    [SerializeField, FormerlySerializedAs("InteractionName")] string interactionName;
    [SerializeField]                                          bool isDynamicPosition;//ControlPopupUI will update its position following the object
    // Funcs that return custom controls' text.
    // Assign any of these from another script to further customize the text when it should change based on the object's state (e.g.: doors)
    public Func<string> LeftMouseNameFunc;
    public Func<string> RightMouseNameFunc;
    public Func<string> InteractionNameFunc;

    // Exposed properties as shorthand for invoking the related Func or using the default value
    public string LeftMouseName   => this.LeftMouseNameFunc  ?.Invoke() ?? leftMouseName;
    public string RightMouseName  => this.RightMouseNameFunc ?.Invoke() ?? rightMouseName;
    public string InteractionName => this.InteractionNameFunc?.Invoke() ?? interactionName;
    public bool IsDynamicPosition => this.isDynamicPosition;
}