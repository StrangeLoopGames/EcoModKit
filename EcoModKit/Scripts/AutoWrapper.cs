// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using Eco.Shared.Voxel;
using UnityEngine;

public class AutoWrapper : MonoBehaviour 
{
	void LateUpdate() 
    {
        if (WorldObserver.obj != null)
            this.transform.position = World.ClosestWrappedLocation(WorldObserver.obj.transform.position.Convert(), this.transform.position.Convert()).Convert();
    }

    public static Vector3 Wrap(Vector3 pos)
    {
        if (WorldObserver.obj != null)
            return World.ClosestWrappedLocation(WorldObserver.obj.transform.position.Convert(), pos.Convert()).Convert();
        else
            return pos;
    }
}
