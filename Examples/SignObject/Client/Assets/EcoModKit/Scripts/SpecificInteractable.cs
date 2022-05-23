using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(HighlightableObject))]
public partial class SpecificInteractable : MonoBehaviour
{
    [Tooltip("Tells the server which part of an object you interacted with. Check for it server-side in OnInteract with context.Parameters.ContainsKey")]
    public string interactionTargetName;
}