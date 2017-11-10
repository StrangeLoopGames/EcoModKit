// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class BlockFace
{
    public static int Front = 0;
    public static int Back = 1;
    public static int Right = 2;
    public static int Left = 3;
    public static int Top = 4;
    public static int Bottom = 5;
}

[Serializable]
public class Block 
{
    public string Name;
    public BlockBuilder Builder;
    public Material Material;
    public Material[] Materials = new Material[0];

    public Color MinimapColor = Color.green;
    public bool IsWater = false;
    public bool Solid = true;
    public bool BuildCollider = true;
    public bool Rendered = true;
    public UnityEngine.Rendering.ShadowCastingMode ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    public string Layer = "Terrain";
    public string Category = "Default";

    // post init, these are set to the same value for faster compares
    public int categoryID;
    public int nameID;

    public bool GenerateMeshCollider = false;

    public bool IsEmpty;
    public float PrefabHeightOffset = -.5f;

    public GameObject walkOnPrefab;

    public string AudioCategory; //This is used for tool interactions

    public string MusicCategory; //This is used for the music system

    public GameObject[] Effects = new GameObject[0];
    public string[] EffectNames = new string[0];
    private Dictionary<string, List<GameObject>> effectsDictionary;

    public bool HasEffect(string effectName)
    {
        if (effectsDictionary == null)
            this.BuildEffectsDictionary();
        return effectsDictionary.ContainsKey(effectName) && effectsDictionary[effectName] != null;
    }

    public GameObject GetEffect(string effectName, int index = 0)
    {
        if (effectsDictionary == null)
            this.BuildEffectsDictionary();
        return effectsDictionary[effectName][index];
    }

    public List<GameObject> GetEffects(string effectName)
    {
        if (effectsDictionary == null)
            this.BuildEffectsDictionary();
        return effectsDictionary[effectName];
    }

    private void BuildEffectsDictionary()
    {
        effectsDictionary = new Dictionary<string, List<GameObject>>();
        for (int i = 0; i < this.Effects.Length; i++)
        {
            string currentEffectName = EffectNames[i];
            GameObject currentEffect = Effects[i];
            if (effectsDictionary.ContainsKey(currentEffectName))
            {
                var effectList = effectsDictionary[currentEffectName];
                effectList.Add(currentEffect);
            }
            else
            {
                var effectList = new List<GameObject>();
                effectList.Add(currentEffect);
                effectsDictionary.Add(currentEffectName, effectList);
            }
        }
    }

#if UNITY_EDITOR
    public Editor Editor;
#endif
}