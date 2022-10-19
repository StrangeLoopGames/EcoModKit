using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;

///<summary> Handles attaching parts to a players avatar </summary>
public partial class AvatarAttachedPart : AvatarPart
{
    [Space] 
    [Header("Attachment Point Settings")]
    [Tooltip("Name of GameObject on avatar to attach to. Eg. AttachTool_Hammer")]                                               public string AttachPointName;
    [Tooltip("Do we need custom offsets to get it to fit on the avatar?")]                                                      public bool useCustomOffsets = false;
    [Tooltip("Adjusting offsets is easiest by modifying in play mode and then copying values in here")]                         public UnityEngine.Vector3 positionOffset;                
    [Tooltip("Adjusting offsets is easiest by modifying in play mode and then copying values in here")]     [QuaternionToEuler] public Quaternion rotationOffset;                        // Rotation is displayed as eulers in inspector 
}
