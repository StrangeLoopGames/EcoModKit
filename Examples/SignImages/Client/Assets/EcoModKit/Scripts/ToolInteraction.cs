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

    // This tool has a custom controller that controls how the how work when player click the Tool Button.
    public bool HasCustomInputHandler     = false;
    public Action<ToolInputState> OnPlayerClickToolButton { get; private set; }

    public Transform[] Multiples;

    void Start()
    {
        // Get the custom input handler of this tool
        if (HasCustomInputHandler) OnPlayerClickToolButton = GetComponent<CustomToolInputHandler>()?.OnPlayerClickToolButton;
    }

    
    public enum ToolInputState
    {
        /// <summary>This initial state is needed. it's the default state of the very first frame. <see cref="FishingController"></summary>
        None,
        ButtonDown,
        ButtonUp
    }

    public interface CustomToolInputHandler
    {
        Action<ToolInputState> OnPlayerClickToolButton { get; }
    }
}