// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ChangedStringStateEvent : UnityEvent<string>
{ }

[Serializable]
public class ChangedFloatStateEvent : UnityEvent<float>
{ }

[Serializable]
public class ChangedStateEvent : UnityEvent<bool>
{ }

[Serializable]
public class SetStateEvent : UnityEvent
{ }

// Do not remove base class as it is needed for modkit
[RequireComponent(typeof(AutoWrapper), typeof(NetObjComponent), typeof(HighlightableObject))]
public partial class WorldObject : SubscribableBehavior
{
    #region State Event Data
    [Tooltip("Use a separate set of events for initially enabled (otherwise enabled events will get called)")]
    public bool              OverrideInitEnabled;
    public ChangedStateEvent OnInitialState;
    public SetStateEvent     OnInitiallyEnabled;
    public SetStateEvent     OnInitiallyDisabled;

    public ChangedStateEvent OnEnabledChanged;
    public SetStateEvent     OnEnabled;
    public SetStateEvent     OnDisabled;

    [Tooltip("Use a separate set of events for initially operating (otherwise operating events will get called)")]
    public bool              OverrideInitOperating;
    public ChangedStateEvent OnInitialOperatingState;
    public SetStateEvent     OnInitiallyEnabledOperating;
    public SetStateEvent     OnInitiallyDisabledOperating;

    public ChangedStateEvent OnOperatingChanged;
    public SetStateEvent     OnEnableOperating;
    public SetStateEvent     OnDisableOperating;

    public ChangedStateEvent OnUsingChanged;
    public SetStateEvent     OnEnableUsing;
    public SetStateEvent     OnDisableUsing;

    public string[] States                                                 = new string[0];
    public SetStateEvent[] OnStateEnabledEvents                            = new SetStateEvent[0];
    public SetStateEvent[] OnStateDisabledEvents                           = new SetStateEvent[0];
    public ChangedStateEvent[] OnStateChangedEvents                        = new ChangedStateEvent[0];
    private Dictionary<string, ChangedStateEvent> onStateChanged           = new Dictionary<string, ChangedStateEvent>();

    public string[] StringStates                          = new string[0];
    public ChangedStringStateEvent[] OnStringStateChanged = new ChangedStringStateEvent[0];
    private Dictionary<string, ChangedStringStateEvent> onStringStateChanged;
    
    public string[] FloatStates                           = new string[0];
    public ChangedFloatStateEvent[] OnFloatStateChanged   = new ChangedFloatStateEvent[0];
    private Dictionary<string, ChangedFloatStateEvent> onFloatStateChanged;

    public string[] Events                                = new string[0];
    public SetStateEvent[] EventHandlers                  = new SetStateEvent[0];
    private Dictionary<string, SetStateEvent> onTriggerEvent;
    #endregion

    #region Occupancy
    public bool hasOccupancy = true;
    public bool overrideOccupancy = false;
    public Vector3 size;
    public Vector3 occupancyOffset = Vector3.zero;
    #endregion

    #region Interactable
    public bool interactable = true;
    #endregion
}