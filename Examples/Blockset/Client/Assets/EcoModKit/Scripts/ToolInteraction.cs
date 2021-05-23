using UnityEngine;
using System.Collections;
using System;

public class ToolInteraction : MonoBehaviour
{
    public bool HasTakeAnimation       = true;
    public bool HasPlaceAnimation      = true;
    public bool MustContainCarriedItem = false;
    public bool TakeRequiresCalories   = true;
    public bool PlaceRequiresCalories  = true;

    // This tool has a custom controller that controls how the tool work when player click the Tool Button.
    public bool HasCustomInputHandler     = false;
    public ToolInputState inputState      = ToolInputState.None;

    public Transform[] Multiples;

    /// <summary>
    /// State of the tool button. (it usually is the left mouse button).
    /// Tool controller can get the input state from here and controls how the tool works
    /// </summary>
    public enum ToolInputState
    {
        /// <summary>This initial state is needed. it's the default state of the very first frame. <see cref="FishingController"></summary>
        None,
        ButtonDown,
        ButtonUp
    }
}