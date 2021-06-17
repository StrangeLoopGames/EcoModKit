// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public static class BlockEditor
{
    public static void DrawBlockEditor(Block block) 
    {
        try
        {
            GUILayout.BeginVertical(GUI.skin.box);
            block.Name = EditorGUILayout.TextField("Name", block.Name);
            block.Layer = EditorGUILayout.TextField("Layer", block.Layer);
            block.Tier = EditorGUILayout.IntField("Tier", block.Tier);
            block.Category = EditorGUILayout.TextField("Category", block.Category);

            block.Builder = EditorGUILayout.ObjectField("Builder", block.Builder, typeof(ScriptableObject), false) as BlockBuilder;
            block.Material = EditorGUILayout.ObjectField("Material", block.Material, typeof(Material), false) as Material;

            if (block.Materials == null)
                block.Materials = new Material[0];
            int numSubMaterials = EditorGUILayout.IntField("Sub Material Count", block.Materials.Length);
            if (numSubMaterials != block.Materials.Length)
                Array.Resize(ref block.Materials, numSubMaterials);
            for (int subMaterial = 0; subMaterial < block.Materials.Length; subMaterial++)
                block.Materials[subMaterial] = EditorGUILayout.ObjectField("Sub Material " + subMaterial, block.Materials[subMaterial], typeof(Material), false) as Material;

            block.MinimapColor = EditorGUILayout.ColorField("Minimap Color", block.MinimapColor);

            block.IsDiggable         = EditorGUILayout.Toggle("Is Diggable", block.IsDiggable);
            block.Solid              = EditorGUILayout.Toggle("Solid", block.Solid);
            block.BuildCollider      = EditorGUILayout.Toggle("Build Collider?", block.BuildCollider);
            block.Rendered           = EditorGUILayout.Toggle("Rendered", block.Rendered);
            block.ShadowCastingMode  = (UnityEngine.Rendering.ShadowCastingMode)EditorGUILayout.EnumPopup("Shadow Casting Mode", block.ShadowCastingMode);
            block.IsWater            = EditorGUILayout.Toggle("Water?", block.IsWater);
            block.IsEmpty            = EditorGUILayout.Toggle("Empty?", block.IsEmpty);
            block.IsLadder           = EditorGUILayout.Toggle("Ladder?", block.IsLadder);
            block.IsSlope            = EditorGUILayout.Toggle("Slope?", block.IsSlope);
            block.BlendingPriority   = EditorGUILayout.IntField("Blend Priority", block.BlendingPriority);
            block.PrefabHeightOffset = EditorGUILayout.FloatField("Prefab Height Offset", block.PrefabHeightOffset);

            block.AudioCategory = EditorGUILayout.TextField("Audio Category", block.AudioCategory);
            block.MusicCategory = EditorGUILayout.TextField("Music Category", block.MusicCategory);

            GUILayout.Label("Effects");
            EditorGUI.indentLevel++;
            EditorGUIUtils.Draw2Arrays(
                ref block.EffectNames,
                ref block.Effects,
                DrawString,
                DrawGameObject);
            EditorGUI.indentLevel--;
        }
        finally
        {
            GUILayout.EndVertical();
        }
	}

    private static string DrawString(string val) { return EditorGUILayout.TextField(val); }

    private static GameObject DrawGameObject(GameObject val) { return EditorGUILayout.ObjectField(val, typeof(GameObject), false) as GameObject; }
}