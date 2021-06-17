// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

/// <summary> Describes which axis or combination, if any, to synchronize. </summary>
public enum AxisSyncMode
{
    None        = 0,    // Do not sync.
    AxisX       = 1,    // Only x axis.
    AxisY       = 2,    // Only the y axis.
    AxisZ       = 3,    // Only the z axis.
    AxisXY      = 4,    // The x and y axis.
    AxisXZ      = 5,    // The x and z axis.
    AxisYZ      = 6,    // The y and z axis.
    AxisXYZ     = 7     // The x, y and z axis.
}

// Interpolates game object physics over time from network updates
public partial class SyncPhysics : MonoBehaviour
{

    [Header("Position")]
    [Tooltip("Synchronizes the position, but not velocty, of the object.")]                                                 public bool SyncPos;

    [Header("Velocity")]
    [Tooltip("Synchronizes the velocity of the object, in addition to position if that is selected.")]                      public bool SyncVel;

    [Header("Rotation")]
    [Tooltip("Will the object synchronize rotation or not.")]                                                               public bool SyncRot;
    [Tooltip("Which axis, if any, to synchronize on.")]                                                                     public AxisSyncMode RotSyncMode         = AxisSyncMode.None;
    [Tooltip("Which axis, if any, to snap to completion when the snapDistance is exceeded.")]                               public AxisSyncMode RotSnapMode         = AxisSyncMode.AxisXYZ;   // Default to all axis.

    [Header("General")]
    [Tooltip("If true, another script must decide when to issue an update.")]                                               public bool ManuallyUpdated             = false;
    [Tooltip("The distance to the nearest player, beyond which kinematic mode is engaged. Does not update live.")]          public float sleepDistance              = 25f;      // Kinematic mode stops the rigidbody from being affected by collisions, forces like gravity, and joints.

    [Header("Snapping")]
    [Tooltip("The maximum deviation between client/server before jumping to the target pos/rot. Does not update live.")]    public float snapDistanceFar            = 5f;       // If snapDistance is exceeded, interpolation is skipped and the object teleports to its target position/rotation.
    [Tooltip("How near to the intended position to get before snapping to it and ending movement. Does not update live.")]  public float snapDistanceNear           = 1f;       // If the distance to the target space is less or equal to this number, then teleport to the target position/rotation.
    [Tooltip("How many frames to exist within a snap-condition before applying. Can smooth short-distance movement.")]      public int frameCountSmoothing          = 7;        // Set to zero to remove.
}
