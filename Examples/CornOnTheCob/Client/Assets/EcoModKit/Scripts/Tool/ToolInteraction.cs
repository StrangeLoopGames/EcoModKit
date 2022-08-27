using UnityEngine;

/// <summary>
/// Basic component to setup tool/food animation and logic states
/// </summary>
public class ToolInteraction : MonoBehaviour
{
    public bool HasTakeAnimation                    = false;
    public bool HasPlaceAnimation                   = false;
    public bool TakeRequiresCalories                = true;
    public bool PlaceRequiresCalories               = true;
    
    [Tooltip("Custom animation set for animation states. Override this to get custom animation and behaviours with this item." +
             " If this is used on tpv tool - avatar animator will be used, and hands animator for fpv tool prefab")]
    public CustomAnimsetOverride CustomAnimset; // Set of overriden animation for general actions. Allows to reuse 1 animator for all tools/items
    
    public Transform[] Multiples;
}