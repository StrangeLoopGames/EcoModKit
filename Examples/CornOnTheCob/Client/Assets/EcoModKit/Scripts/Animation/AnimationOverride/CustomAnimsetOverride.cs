using System;
using System.Collections.Generic;
using UnityEngine;

// A data class that hold State names and animation clips that will be applied to animator in runtime.
// This data holder is universal and can be used for every animator, just with specifying correct state names
// "Presets" can be found in CustomAnimsetOverrideEditor, that is used to predefine states for modders/devs.
[CreateAssetMenu(menuName = "Eco/Animation/Animset Override", fileName = "Animset_")]
public class CustomAnimsetOverride : ScriptableObject
{
    [Tooltip("List of states that will have an overridden animation. State name - Animation clip")]
    public List<AnimsetOverrideItem> OverrideStates = new List<AnimsetOverrideItem>();
}

// A struct that used to hold setup data for everrides
[Serializable]
public struct AnimsetOverrideItem
{
    public string State;                 // State name in animator (actually its source clip name that used in state)
    public AnimationClip Clip;           // Target animation clip that will be applied in runtime to State

    public AnimsetOverrideItem(string state)
    {
        this.State = state;
        this.Clip = null;
    }
    
    public AnimsetOverrideItem(string state, AnimationClip clip)
    {
        this.State = state;
        this.Clip = clip;
    }
}