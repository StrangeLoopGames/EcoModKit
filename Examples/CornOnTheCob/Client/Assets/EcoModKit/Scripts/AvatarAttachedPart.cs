using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;

///<summary> Handles attaching parts to a players avatar </summary>
public partial class AvatarAttachedPart : AvatarPart
{
    public string AttachPointName;                            // Name of object to attach tool to
    public bool useCustomOffsets = false;                     // Does this tool need additional offset to fit on the avatar
    public UnityEngine.Vector3 positionOffset;                // Position offset relative to parent
    
    [QuaternionToEuler]
    public Quaternion rotationOffset;                        // Rotation is displayed as eulers in inspector 
}