using UnityEngine;

// Bite sequence for food items like tomatoes, when we have FULL food model, and them next state will be like next separate bite object
[CreateAssetMenu(menuName = "Eco/Food Sequences/FullToNone", fileName = "BiteSequence_FullToNone")]
public class FullToNoneSequence : FoodBiteSequence
{
    public override void ProcessBiteAtIndex(GameObject[] parts, int index)
    {
        // Double check other food pieces scaled properly and disabled
        for (var i = 0; i < parts.Length; i++)
        {
            parts[i].SetActive(i == index); // Show only current bite part
            parts[i].transform.localScale = Vector3.one;
        }
    }
    
    public override GameObject GetCurrentBitePiece(GameObject[] parts) => null; // All parts here are just next object state, so this not not valid for current bite sequence
}