using UnityEngine;

/// <summary>
/// Defines custom eating sequence for food items
/// Due to virtual/abstract design can support lots of derived variations
/// </summary>
public abstract class FoodBiteSequence : ScriptableObject
{
    /// <summary> Should contain individual part prep code when iteration started (before parts actual switch sequence) </summary>
    public abstract void ProcessBiteAtIndex(GameObject[] parts, int index);

    /// <summary> Gets first valid food piece. Can be used to put that piece on utensils or so. Implementation can vary on different sequence setup </summary>
    public abstract GameObject GetCurrentBitePiece(GameObject[] parts);
}