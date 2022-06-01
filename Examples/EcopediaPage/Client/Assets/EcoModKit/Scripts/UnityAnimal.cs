using Assets.Scripts.Networking;
using UnityEngine;

/// <summary>Primary component for registering a game object as an Eco animal.</summary>
[RequireComponent(typeof(WaitForGround))]
[RequireComponent(typeof(AnimalInterpolateTransform))]
[RequireComponent(typeof(HighlightableObject))]
[RequireComponent(typeof(AnimalAnimationManager))]
public partial class UnityAnimal : OptimizedMonobeh
{
    [Tooltip("Should be a child of the first ragdoll rigidbody, somewhere near the center of the animal. While dead this is the position that gets sent to the server.")]
    public Transform ragdollCenter;

    /// <summary>Max range in units to raycast players in front.</summary>
    [Tooltip("Max range in units to raycast players in front")]
    public float playerAvoidanceRange = 4f;
}
