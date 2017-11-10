// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using Assets;
using Eco.Shared.Networking;
using Eco.Shared.Serialization;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using Eco.Shared.Utils;
using Eco.Shared;
using System.Linq;

public interface IEmbeddedNetObject
{
    NetObject InitNetObject(int id);
}

public interface INetworkBehaviour
{
    void NetworkInitialize();
}

// please keep this class lean, place extra cruft into other object types
public class NetObjComponent : MonoBehaviour, INetObject, INotifyRemoval, IRPCMethodHandler
{
    public NetObject NetObj { get; private set; }

    public int ID { get { return this.NetObj.ID; } }
    public bool Active { get { return this.NetObj.Active; } set { this.NetObj.Active = value; } }

    public Action<BSONObject> SendUpdate;
    public Action<BSONObject> ReceiveUpdate;
    public Action<BSONObject> ReceiveInitialState;

    public Func<bool> IsUpdated;

    public void OnSendUpdate(BSONObject bsonObj)
    {
        if (SendUpdate != null)
            SendUpdate(bsonObj);
    }

    public void OnReceiveUpdate(BSONObject bsonObj)
    {
        if (ReceiveUpdate != null)
            ReceiveUpdate(bsonObj);
    }

    public void OnReceiveInitialState(BSONObject bsonObj)
    {
        if (ReceiveInitialState != null)
            ReceiveInitialState(bsonObj);
    }

    public void OnRemoved()
    {
        this.gameObject.PoolOrDestroy();
    }

    public void Init(int id)
    {
        var embedded = this.gameObject.GetComponent<IEmbeddedNetObject>();
        if (embedded != null)
            this.NetObj = embedded.InitNetObject(id);
        else
            this.NetObj = new NetObject(this, id);

        INetworkBehaviour[] networkBehaviours = this.GetComponentsInChildren<INetworkBehaviour>(true);
        foreach (INetworkBehaviour behaviour in networkBehaviours)
            behaviour.NetworkInitialize();
    }

    #region NETOBJIMPLEMENTATION
    void INotifyRemoval.OnRemoved()
    {               
        try
        {            
            OnRemoved();
        }
        catch (Exception e)
        {
            Debug.LogError(string.Format("Caught exception on {0} while in OnRemoved.\nException: {1}\nCallstack: {2}", this.name, e.Message, e.StackTrace), this.gameObject);
        }
    }

    bool INetObject.IsRelevant(INetObjectViewer viewer)
    {
        return true;
    }

    bool INetObject.IsNotRelevant(INetObjectViewer viewer)
    {
        return false;
    }

    bool INetObject.IsUpdated(INetObjectViewer viewer)
    {
        if (this.IsUpdated != null)
            return this.IsUpdated.Invoke();
        else
            return true;
    }

    void INetObject.ReceiveInitialState(BSONObject bsonObj)
    {
        try
        { 
            OnReceiveInitialState(bsonObj);
        }
        catch (Exception e)
        {
            while (e is TargetInvocationException)
                e = e.InnerException;
            Debug.LogError(string.Format("Caught exception on {0} while in NetObject ReceiveInitialState.\nException: {1}\nCallstack: {2}\nBSON: {3}", this.name, e.Message, e.StackTrace, bsonObj.ToString()), this.gameObject);
        }
    }

    void INetObject.ReceiveUpdate(BSONObject bsonObj)
    {
        try
        {
            OnReceiveUpdate(bsonObj);
        }
        catch (Exception e)
        {
            Debug.LogError(string.Format("Caught exception on {0} while in NetObject ReceiveUpdate.\nException: {1}\nCallstack: {2}\nBSON: {3}", this.name, e.Message, e.StackTrace, bsonObj.ToString()), this.gameObject);
        }
    }

    void INetObject.SendInitialState(BSONObject bsonObj, INetObjectViewer viewer)
    {
        throw new NotImplementedException();
    }

    void INetObject.SendUpdate(BSONObject bsonObj, INetObjectViewer viewer)
    {
        try
        { 
            OnSendUpdate(bsonObj);
        }
        catch (Exception e)
        {
            Debug.LogError(string.Format("Caught exception on {0} while in NetObject SendUpdate.\nException: {1}\nCallstack: {2}\nBSON: {3}", this.name, e.Message, e.StackTrace, bsonObj.ToString()), this.gameObject);
        }
    }

    public object[] GetSubObjects()
    {
        return this.GetComponentsInChildren<MonoBehaviour>(true);
    }
    #endregion
}