// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BlockEditor 
{
	public static void DrawBlockEditor(Block block, BlockSet blockSet) 
    {
		GUILayout.BeginVertical(GUI.skin.box);
        {
            block.Name = EditorGUILayout.TextField("Name", block.Name);
            block.Layer = EditorGUILayout.TextField("Layer", block.Layer);
            block.Category = EditorGUILayout.TextField("Category", block.Category);

            block.Builder = EditorGUILayout.ObjectField("Builder", block.Builder, typeof(BlockBuilder), false) as BlockBuilder;
            block.Material = EditorGUILayout.ObjectField("Material", block.Material, typeof(Material), false) as Material;

            if (block.Materials == null)
                block.Materials = new Material[0];
            int numSubMaterials = EditorGUILayout.IntField("Sub Material Count", block.Materials.Length);
            if (numSubMaterials != block.Materials.Length)
                Array.Resize(ref block.Materials, numSubMaterials);
            for (int subMaterial = 0; subMaterial < block.Materials.Length; subMaterial++)
                block.Materials[subMaterial] = EditorGUILayout.ObjectField("Sub Material " + subMaterial, block.Materials[subMaterial], typeof(Material), false) as Material;

            block.MinimapColor = EditorGUILayout.ColorField("MinimapColor", block.MinimapColor);

            block.Solid = EditorGUILayout.Toggle("Solid", block.Solid);
            block.BuildCollider = EditorGUILayout.Toggle("BuildCollider?", block.BuildCollider);
            block.Rendered = EditorGUILayout.Toggle("Rendered", block.Rendered);
            block.ShadowCastingMode = (UnityEngine.Rendering.ShadowCastingMode)EditorGUILayout.EnumPopup("Shadow Casting Mode", block.ShadowCastingMode);
            block.IsWater = EditorGUILayout.Toggle("Water?", block.IsWater);
            block.IsEmpty = EditorGUILayout.Toggle("Empty?", block.IsEmpty);
            block.PrefabHeightOffset = EditorGUILayout.FloatField("PrefabHeightOffset", block.PrefabHeightOffset);

            block.walkOnPrefab = EditorGUILayout.ObjectField("WalkPrefab", block.walkOnPrefab, typeof(GameObject), false) as GameObject;
            block.AudioCategory = EditorGUILayout.TextField("AudioCategory", block.AudioCategory);
            block.MusicCategory = EditorGUILayout.TextField("MusicCategory", block.MusicCategory);

            GUILayout.Label("Effects");
            EditorGUI.indentLevel++;
            EditorGUIUtils.Draw2Arrays(
                ref block.EffectNames,
                ref block.Effects,
                DrawString,
                DrawGameObject);
            EditorGUI.indentLevel--;
        }
        GUILayout.EndVertical();
	}

    private static string DrawString(string val) { return EditorGUILayout.TextField(val); }
    private static GameObject DrawGameObject(GameObject val) { return EditorGUILayout.ObjectField(val, typeof(GameObject), false) as GameObject; }
}
