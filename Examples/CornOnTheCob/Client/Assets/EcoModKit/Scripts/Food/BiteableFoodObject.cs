using UnityEngine;

/// <summary>
/// Component to turn any object to biteable thing. Can be reused for any object that can have sub-states, so not only for food.
/// Has basic logic for object states, applies sequences from FoodBiteSequence classes for animation and process control
/// </summary>
public class BiteableFoodObject : MonoBehaviour
{
    [Header("Core config")]
    [Tooltip("Collection of food parts. All the bites by size descending")]
    public GameObject[] FoodParts;
    
    [Tooltip("Animation sequence for food sequence during eating. Can be created from asset creation menu")]
    public FoodBiteSequence BiteSequence;

    [Tooltip("Plate/container object that will be used by default for this food object. Can be overwritten in runtime if needed.")]
    public GameObject PlateTemplate;

    [Tooltip("Default Hands FPV animations/ If not set to custom, BiteableFoodObjectController will set [ToolInteraction.CustomAnimset] to an CustomAnimsetOverride queried from FoodManager according to the this field's value")]
    public PredefinedFoodAnimations DefaultAnimationOverride = PredefinedFoodAnimations.Custom;

    [Header("Utensil config")]
    [Tooltip("Spoon/fork object that will be used by default for this food object. Can be overwritten in runtime if needed.")]
    public GameObject UtensilTemplate;

    [Tooltip("If true food parts will be placed inside utensil object automatically, using current bite piece in sequence")]
    public bool UseAutoUtensilPartPlacement = true; 

    [Tooltip("Custom food chunk to use on every bite for utensils")]
    public GameObject CustomFoodChunkForUtensils; 
    
    [Tooltip("Position offset for parts to use with Utensils (when part will be spawned on utensil)")]
    public Vector3 UtensilPartPositionOffset;
    
    [Tooltip("Rotation offset for parts to use with Utensils (when part will be spawned on utensil)")]
    public Vector3 UtensilPartRotationOffset;
    
    [Tooltip("Scale multiplier on each side for parts to use with Utensils (when part will be spawned on utensil)")]
    public Vector3 UtensilScaleMultForBitePiece = new Vector3(1f, 1f, 1f);

    [Header("Misc config")]
    // Its better to keep clear transform on main biteable object class (for artists easier edits). So those will be applied in runtime
    public Vector3 PositionOffset;
    public Vector3 RotationOffset;
}
