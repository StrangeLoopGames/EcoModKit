using UnityEngine;

// We send different ints from the animation events to change the behaviour and effect using the switch statement below
public enum FoodEffectType
{
    None  = 0,
    InteractionVFX  = 1,
    MouthVFX = 2,
    ChewSound = 3,
    BiteVFX = 4,
    InteractionSound = 5
}

/// <summary>
/// Configurable food effects to use with BiteableFoodObject
/// Will help to subscribe to all the actions and tie them to particles and sounds
/// </summary>
public class FoodEffects : MonoBehaviour
{
    [Tooltip("Should the interaction effects play all at once or in a sequence with each bite?")]
    public bool                 sequenceInteractionEffects;
    [Tooltip("Interaction Particles are for when the player interacts with the food before taking a bite. eg. spoon entering soup")] 
    public ParticleSystem[]     interactionParticleSystems;
    [Space(20f)][Tooltip("Should the bite effects play all at once or in a sequence with each bite?")]
    public bool                 sequenceBiteEffects;
    [Tooltip("Bite Particles are for when the player actually takes a bite. if the anim event triggering this is passed a 4 it is spawned on the original position in the prefab, 2 will use the mouth offset")]
    public ParticleSystem[]     biteEffectParticleSystems;
    [Space(20f)][Tooltip("Select an audio category for the chewing sounds for each bite")]
    public FoodSoundType        foodSoundType;
    [Tooltip("Sound to play when the player interacts with the food")]
    public InteractionSoundType interactionSoundType;
    [Tooltip("Optional: If sequencing mouth effects then you can parent the individual effects to this transform and it will be positioned to the mouth instead of the parent effect")]
    public Transform            mouthEffectTransform;
    [Tooltip("When passing a 2 to the anim event, the bite effects are moved to the camera plus this offset to simulate mouth position")]
    public Vector3              mouthOffset = new Vector3(0f,-0.1f,0.1f);
    [Tooltip("How much variety is there in the angle of juicy mouth effects")]
    public float                mouthSprayRandomAngle = 15;

    // Used to send the food type to Wwise
    public enum FoodSoundType
    {
        Liquid,
        Wet,
        Hard,
        Soft,
    }
    
    // Used to send the interaction sound type to Wwise 
    public enum InteractionSoundType
    {
        None,
        Spoon,
        Fork,
        Grab,
        Nibbles
    }
}