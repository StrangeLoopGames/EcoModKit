using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Renderer))]
public class MaterialEvents : MonoBehaviour
{
    [Tooltip("Shader name (eg '_Cutoff') of property to modify. Specify names for the types you modify. (Color if you use SetColor, etc)")]
    public string ShaderColorName, ShaderFloatName, ShaderIntName, ShaderVectorName;

    [Tooltip("Colors for use with SetColor action.")]
    [ColorUsage(true, true)]
    public Color[] indexedColors;

    [Tooltip("Vectors for use with SetVector action.")]
    public Vector4[] indexedVectors;

    [ColorUsage(true, true)]
    public Color tweenFrom, tweenTo;
    private Color tweenColor; // this will be used by the Tween to store the current color
    public float tweenLength = 2f;
    public Ease easeType = Ease.InOutCubic;
    public LoopType loopType = LoopType.Yoyo;
    public bool tweenOnEnable = false;

    private Renderer r;
    private bool instanced = false;

    public void SetColor(int colorIndex)
    {
        if (!instanced) ForceInstanceMats();
        if (indexedColors.Length == 0) return;
        foreach(Material m in r.sharedMaterials)
            m.SetColor(ShaderColorName, indexedColors[colorIndex]);
    }

    public void TweenColor()
    {
        if (!instanced) ForceInstanceMats();
        DOTween.To(() => tweenColor, (c) => SetColor(c), tweenTo, tweenLength).SetId("tweenColor");
    }

    public void StopTweens()
    {
        DOTween.Kill("tweenColor");
        DOTween.Kill("tweenFloat");
    }

    public void TweenFloat(float target) => DOTween.To(() => r.sharedMaterial.GetFloat(this.ShaderFloatName), (value) => SetFloat(value), target, tweenLength).SetId("tweenFloat");

    public void SetFloat(float value)
    {
        if (!instanced) ForceInstanceMats();
        foreach(Material m in r.sharedMaterials)
            m.SetFloat(ShaderFloatName, value);
    }

    public void SetVector(int index)
    {
        if (!instanced) ForceInstanceMats();
        foreach(Material m in r.sharedMaterials)
            m.SetVector(ShaderVectorName, indexedVectors[index]);
    }

    public void SetInt(int value)
    {
        if (!instanced) ForceInstanceMats();
        foreach(Material m in r.sharedMaterials)
            m.SetInt(ShaderIntName, value);
    }

    public void OnEnable()
    {
        if (tweenOnEnable)
            this.TweenColor();
    }

    public void OnDisable()
    {
        if (tweenOnEnable)
            this.StopTweens();
    }

    #region internal
    void Awake()
    {
        r = GetComponent<Renderer>();
        instanced = false;
	}

    // Instance the material(s) so we don't have to worry about conflicts w/ highlighting
    // This script will break batching, should only be used on rare objects like crafting stations
    private void ForceInstanceMats()
    {
        bool allInstanced = true;

#if ECO_DEV
        //HACK: skip fading, alternatively we need to have fader use an instanced material when it finishes
        ObjectFader f = GetComponent<ObjectFader>();
        if(f != null)
            f.FinishFade();
#endif

        foreach (Material m in r.sharedMaterials)
            if (!m.name.Contains("Instance"))
                allInstanced = false;

        if(!allInstanced)
        {
            Material[] mats = r.materials;
            r.materials = mats;
        }
        instanced = true;
    }

    // called by TweenColor
    public void SetColor(Color c)
    {
        tweenColor = c;
        foreach(Material m in r.sharedMaterials)
            m.SetColor(ShaderColorName, c);
    }
    #endregion
}