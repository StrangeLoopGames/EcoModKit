// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Eco.Shared.Serialization;
using Eco.Shared.View;
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

[Serializable, RequireComponent(typeof(AutoWrapper), typeof(NetObjComponent), typeof(HighlightableObject))]
public class WorldObject : MonoBehaviour, INetworkBehaviour, IAnonymousSubscriber
{
    public WorldObjectView view;
    public NetObjComponent netComponent { get { return this.GetComponent<NetObjComponent>(); } }
    List<Action> IAnonymousSubscriber.AnonymousActions { get; set; }
    public event System.Action<BSONObject> OnViewInitialized = delegate {};

    #region State Event Data
    public ChangedStateEvent OnInitialState;
    public SetStateEvent OnInitiallyEnabled;
    public SetStateEvent OnInitiallyDisabled;

    public ChangedStateEvent OnEnabledChanged;
    public SetStateEvent OnEnabled;
    public SetStateEvent OnDisabled;

    public ChangedStateEvent OnOperatingChanged;
    public SetStateEvent OnEnableOperating;
    public SetStateEvent OnDisableOperating;

    public ChangedStateEvent OnUsingChanged;
    public SetStateEvent OnEnableUsing;
    public SetStateEvent OnDisableUsing;

    public string[] States = new string[0];
    public SetStateEvent[] OnStateEnabledEvents = new SetStateEvent[0];
    public SetStateEvent[] OnStateDisabledEvents = new SetStateEvent[0];
    public ChangedStateEvent[] OnStateChangedEvents = new ChangedStateEvent[0];
    private Dictionary<string, Dictionary<bool, SetStateEvent>> onStateSet;
    private Dictionary<string, ChangedStateEvent> onStateChanged;

    public string[] StringStates = new string[0];
    public ChangedStringStateEvent[] OnStringStateChanged = new ChangedStringStateEvent[0];
    private Dictionary<string, ChangedStringStateEvent> onStringStateChanged;
    
    public string[] FloatStates = new string[0];
    public ChangedFloatStateEvent[] OnFloatStateChanged = new ChangedFloatStateEvent[0];
    private Dictionary<string, ChangedFloatStateEvent> onFloatStateChanged;

    public string[] Events = new string[0];
    public SetStateEvent[] EventHandlers = new SetStateEvent[0];
    private Dictionary<string, SetStateEvent> onTriggerEvent;
    #endregion

    private Dictionary<string, object> previousAnimatedStates;

    bool firstUpdate = true;

    public bool hasOccupancy = true;

    public bool overrideOccupancy = false;

    public Vector3 size;

    public Vector3 occupancyOffset = Vector3.zero;

    public virtual void NetworkInitialize()
    {
        this.netComponent.ReceiveInitialState += this.ReceiveInitialState;
    }

    void Start()
    {
        this.previousAnimatedStates = new Dictionary<string, object>();
        this.onStateSet = new Dictionary<string, Dictionary<bool, SetStateEvent>>();
        this.onStateChanged = new Dictionary<string, ChangedStateEvent>();
        for (int i = 0; i < States.Length; i++)
        {
            if (!this.onStateSet.ContainsKey(States[i]))
                this.onStateSet[States[i]] = new Dictionary<bool, SetStateEvent>();
            this.onStateSet[States[i]][true] = OnStateEnabledEvents[i];
            this.onStateSet[States[i]][false] = OnStateDisabledEvents[i];
            this.onStateChanged[States[i]] = OnStateChangedEvents[i];
        }

        this.onStringStateChanged = new Dictionary<string, ChangedStringStateEvent>();
        for (int i = 0; i < StringStates.Length; i++)
            this.onStringStateChanged[StringStates[i]] = OnStringStateChanged[i];

        this.onFloatStateChanged = new Dictionary<string, ChangedFloatStateEvent>();
        for (int i = 0; i < FloatStates.Length; i++)
            this.onFloatStateChanged[FloatStates[i]] = OnFloatStateChanged[i];

        this.onTriggerEvent = new Dictionary<string, SetStateEvent>();
        for (int i = 0; i < Events.Length; i++)
            this.onTriggerEvent[Events[i]] = EventHandlers[i];
    }

    void Update()
    {
        if (firstUpdate)
        {
            // invoke initialization states
            OnInitialState.Invoke(this.view.Enabled);
            if (this.view.Enabled)
                OnInitiallyEnabled.Invoke();
            else
                OnInitiallyDisabled.Invoke();

            firstUpdate = false;

            if (this.view.IsUsing)
            {
                OnUsingChanged.Invoke(this.view.IsUsing);
                OnEnableUsing.Invoke();
            }

            this.UpdateAnimatedStates();
        }
    }

    protected virtual void ReceiveInitialState(BSONObject bson)
    {
        this.transform.position = bson["pos"].unityVector3();
        this.transform.rotation = bson["rot"].unityQuaternion();

        if (bson.ContainsKey("view"))
        {
            this.view = ViewManager.UnpackageView<WorldObjectView>(bson["view"].ObjectValue);
            this.OnViewInitialized(bson);

            this.view.Subscribe("Enabled", OnEnableChanged);
            this.view.SubscribeAndCall("Operating", UpdateOperating);
            this.view.SubscribeAndCall("IsUsing", UpdateUsing);
            this.view.Subscribe("AnimatedStates", UpdateAnimatedStates);

            AttachmentManager.SetupAttachments(this);
        }
        else
            GameObject.Destroy(this);
    }

    private void OnEnableChanged()
    {
        OnEnabledChanged.Invoke(this.view.Enabled);
        if (this.view.Enabled)
            OnEnabled.Invoke();
        else
            OnDisabled.Invoke();
    }

    private void UpdateOperating()
    {
        OnOperatingChanged.Invoke(this.view.Operating);
        if (this.view.Operating)
            OnEnableOperating.Invoke();
        else
            OnDisableOperating.Invoke();
    }

    private void UpdateUsing()
    {
        OnUsingChanged.Invoke(this.view.IsUsing);
        if (this.view.IsUsing)
            OnEnableUsing.Invoke();
        else
            OnDisableUsing.Invoke();
    }

    private void UpdateAnimatedStates()
    {
        foreach (KeyValuePair<string, object> kvp in this.view.AnimatedStates)
        {
            string key = kvp.Key;
            object val = kvp.Value;
            if (val is bool)
                UpdateAnimatedState(key, (bool)val);
            else if (val is string)
                UpdateAnimatedState(key, (string)val);
            else if (val is float)
                UpdateAnimatedState(key, (float)val);
            else
                throw new Exception("Unhandled WorldObject event type: " + val.GetType().Name);
        }
        this.CopyAnimatedStates();
    }

    private void CopyAnimatedStates()
    {
        this.previousAnimatedStates.Clear();
        foreach (KeyValuePair<string, object> kvp in this.view.AnimatedStates)
            this.previousAnimatedStates.Add(kvp.Key, kvp.Value);
    }

    private void UpdateAnimatedState(string name, bool isEnabled)
    {
        if (!(this.previousAnimatedStates.ContainsKey(name) && (bool)this.previousAnimatedStates[name] == isEnabled))
        {
            ChangedStateEvent changeEvent;
            if (onStateChanged.TryGetValue(name, out changeEvent))
                changeEvent.Invoke(isEnabled);

            Dictionary<bool, SetStateEvent> setEvents;
            if (onStateSet.TryGetValue(name, out setEvents))
                setEvents[isEnabled].Invoke();
        }
    }

    private void UpdateAnimatedState(string name, float val)
    {
        if (!(this.previousAnimatedStates.ContainsKey(name) && (float)this.previousAnimatedStates[name] == val))
        {
            ChangedFloatStateEvent changeEvent;
            if (onFloatStateChanged.TryGetValue(name, out changeEvent))
                changeEvent.Invoke(val);
        }
    }

    private void UpdateAnimatedState(string name, string val)
    {
        if (!(this.previousAnimatedStates.ContainsKey(name) && (string)this.previousAnimatedStates[name] == val))
        {
            ChangedStringStateEvent changeEvent;
            if (onStringStateChanged.TryGetValue(name, out changeEvent))
                changeEvent.Invoke(val);
        }
    }

    [Eco.Shared.Networking.RPC]
    public void TriggerAnimatedEvent(string name)
    {
        Debug.Log("Got event " + name);
        if (this.onTriggerEvent.ContainsKey(name))
            this.onTriggerEvent[name].Invoke();
    }

    void OnDrawGizmos()
    {
        if (!overrideOccupancy && gameObject.GetComponent<MeshCollider>())
        {
            var boundingBox = gameObject.GetComponent<MeshCollider>();
            var boundSize = boundingBox.bounds.size;
            var sizeXYZi = new Vector3((int)Math.Ceiling(boundSize.x), (int)Math.Ceiling(boundSize.y), (int)Math.Ceiling(boundSize.z));
            size = sizeXYZi;
        }
            
        if (size != Vector3.zero)
        {
            if (hasOccupancy)
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;

            Vector3 cornerOffset = new Vector3(-.5f, -.5f, -.5f);
            Vector3 centerPoint = (size / 2) + cornerOffset + occupancyOffset;

            Gizmos.DrawWireCube(centerPoint, size);
        }
    }
}
