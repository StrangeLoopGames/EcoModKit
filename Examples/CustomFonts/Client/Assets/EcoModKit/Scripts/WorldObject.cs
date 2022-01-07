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
    // Enabled/Disabled is the status for describing if the object can be used (Ex: A workbench that fails to meet it's RoomTier requirement would call OnInitiallyDisabled or OnDisabled).
    // These elements trigger ONLY during ReceiveInitialState.
    [Tooltip("Use a separate set of events for initially enabled (otherwise enabled events will get called)")]
    public bool              OverrideInitEnabled;                   // If this is set to false in the WorldObject script in the prefab, then OnEnabled/OnDisabled is used instead.
    public ChangedStateEvent OnInitialState;
    public SetStateEvent     OnInitiallyEnabled;
    public SetStateEvent     OnInitiallyDisabled;

    // These elements trigger on any UpdateEnabled call.
    public ChangedStateEvent OnEnabledChanged;
    public SetStateEvent     OnEnabled;
    public SetStateEvent     OnDisabled;

    // EnableOperating/DisableOperating is the status for describing if the object is currently in use (Ex: A workbench that is working on a crafting task will call OnInitiallyEnabledOperating or OnEnableOperating).
    // These elements trigger ONLY during ReceiveInitialState
    [Tooltip("Use a separate set of events for initially operating (otherwise operating events will get called)")]
    public bool              OverrideInitOperating;                 // If this is set to false in the WorldObject script in the prefab, then OnEnabledOperating/OnDisabledOperating is used instead.
    public ChangedStateEvent OnInitialOperatingState;
    public SetStateEvent     OnInitiallyEnabledOperating;
    public SetStateEvent     OnInitiallyDisabledOperating;

    // These elements trigger when the Operating status is changed on a view update from the server.
    public ChangedStateEvent OnOperatingChanged;
    public SetStateEvent     OnEnableOperating;
    public SetStateEvent     OnDisableOperating;

    // EnableUsing/DisableUsing is the status for describing if the object is currently being interacted with by a player.
    // These elements trigger when the Operating status is changed on a view update from the server.
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