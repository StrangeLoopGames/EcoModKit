using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
/// <summary>
/// Use this class for scrolling texture on conveyor belts.
/// </summary>
public class TextureScroll : MonoBehaviour
{
    public int materialIndex; //Index of the material which is to be scrolled. Assign from inspector.
    public UnityEngine.Vector2 direction = new UnityEngine.Vector2(1, 0);

    private UnityEngine.Vector2 currentOffset;
    private Material _material;
    private Coroutine scrollRoutine;

    private void Init()
    {
        Renderer r = GetComponent<Renderer>();
        _material = r.materials[materialIndex];
    }

    public void StartScrolling()
    {
        if (_material == null)
            Init();
        
        StopScrolling();

        currentOffset = _material.GetTextureOffset("_MainTex");
        scrollRoutine = StartCoroutine("ScrollTexture");
    }

    public void StopScrolling()
    {
        if (scrollRoutine != null)
            StopCoroutine(scrollRoutine);

        scrollRoutine = null;
    }

    IEnumerator ScrollTexture()
    {
        while(true)
        {
            currentOffset += direction * Time.deltaTime;
            currentOffset.x = Mathf.Repeat(currentOffset.x, 1f);
            currentOffset.y = Mathf.Repeat(currentOffset.y, 1f);

            _material.SetTextureOffset("_MainTex", currentOffset);

            yield return null;
        }
    }

    private void OnDisable()
    {
        StopScrolling();
    }
}
