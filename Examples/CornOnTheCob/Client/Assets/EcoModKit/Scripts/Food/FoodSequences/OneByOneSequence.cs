using UnityEngine;

// Bite sequence for food items when we have separate parts only, so full food object requires all objects active, and for bites it just hides proper sets of objects
[CreateAssetMenu(menuName = "Eco/Food Sequences/OneByOne", fileName = "BiteSequence_OneByOne")]
public class OneByOneSequence : FoodBiteSequence
{
    public override void ProcessBiteAtIndex(GameObject[] parts, int index)
    {
        // Reset full state of current part
        for (var i = 0; i < parts.Length; i++)
        {
            var needToHidePart = i < index;
            parts[i].SetActive(!needToHidePart);
            parts[i].transform.localScale = Vector3.one;
        }
    }
    
    public override GameObject GetCurrentBitePiece(GameObject[] parts)
    {
        // As parts get deactivated one by one on eating, need to return part before first active
        var prevPart = parts[0]; // For safety 
        foreach (var part in parts)
            if (part.activeSelf) return prevPart;
            else prevPart = part;
        return parts[parts.Length-1];
    }
}