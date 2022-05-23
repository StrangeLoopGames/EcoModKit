using UnityEngine;

/// <summary>
/// State of the tool interact buttons.
/// Tool controller can get the input state from here and controls how the tool works. Available for modkit
/// Those input state already got checks for calories, usage, etc
/// </summary>
public enum ToolInputState
{
    /// <summary>This initial state is needed. it's the default state of the very first frame. Can be cast to working state in animator by INT</summary>
    None = 0,
    LeftButtonDown = 1,
    RightButtonDown = 2,
    LeftButtonUp = 3,
    RightButtonUp = 4,
}

public class ToolInteraction : MonoBehaviour
{
    public bool HasTakeAnimation                    = true;
    public bool HasPlaceAnimation                   = true;
    public bool MustContainCarriedItem              = false;
    public bool TakeRequiresCalories                = true;
    public bool PlaceRequiresCalories               = true;
    public bool CanBeUsedInSyncWithThirdPerson      = true;                 // If set to false, animation manager will call events itself and animator on this object will be disabled
    public ToolHandOrigin TargetHand                = ToolHandOrigin.Right; // Target hand, this tool needs to be placed in
    public AnimatorOverrideController AnimationSetOverride;                 // Set of overriden animation for general actions. Allow to reuse 1 animator for all tools on 1 hands object


    public bool HasCustomInputHandler     = false;                          // This tool has a custom controller that controls how the tool work when player click the Tool Button. Default tool actions won't be executed.
    public ToolInputState inputState      = ToolInputState.None;            // Current input state for tools. Safe to use without extra checks. Not a raw UserInput.

    public Transform[] Multiples;
}

/// <summary>
/// Target hand value for tool/object
/// </summary>
public enum ToolHandOrigin
{
    Right,
    Left,
    Middle
}