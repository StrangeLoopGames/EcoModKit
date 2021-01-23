using UnityEngine;
using System.Collections;

public class ToolInteraction : MonoBehaviour
{
    public bool HasTakeAnimation = true;
    public bool HasPlaceAnimation = true;
    public bool MustContainCarriedItem = false;
    public bool TakeRequiresCalories = true;
    public bool PlaceRequiresCalories = true;

    public Transform[] Multiples;
}