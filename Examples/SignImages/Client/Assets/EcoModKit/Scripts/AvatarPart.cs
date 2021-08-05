using UnityEngine;

public enum TintType
{
    Clothing,
    Skin,
    Hair
}

public abstract partial class AvatarPart : MonoBehaviour
{
    // Data
    [Header("Objects From FBX")]
    public GameObject MalePrefab;
    public GameObject FemalePrefab;

    [Header("Material Override (leave blank to use default avatar materials)")]
    public Material[] CurvedMaterials;
    public Material[] UIMaterials;
    public bool UseMaterialTexture;
    public TintType firstTint;
}