// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BlockSet))]
public class BlockSetEditor : Editor
{
    private BlockSet blockSet;
	
    private static Block selectedBlock;

    private MaterialEditor materialEditor;
	
	[MenuItem ("Assets/Create/VoxelEngine/BlockSet")]
	private static void CreateBlockSet() 
    {
		string path = "Assets/";
		if(Selection.activeObject != null) 
        {
			path = AssetDatabase.GetAssetPath(Selection.activeObject)+"/";
		}
		AssetDatabase.CreateAsset(CreateInstance<BlockSet>(), path+"NewBlockSet.asset");
	}

    [MenuItem("Assets/Create/VoxelEngine/CustomBuilder")]
    private static void CreateCustomBuilder()
    {
        string path = "Assets/";
        if (Selection.activeObject != null)
        {
            path = AssetDatabase.GetAssetPath(Selection.activeObject) + "/";
        }
        AssetDatabase.CreateAsset(CreateInstance<CustomBuilder>(), path + "NewBuilder.asset");
    }

    [MenuItem("Assets/Create/VoxelEngine/PrefabBuilder")]
    private static void CreatePrefabBuilder()
    {
        string path = "Assets/";
        if (Selection.activeObject != null)
        {
            path = AssetDatabase.GetAssetPath(Selection.activeObject) + "/";
        }
        AssetDatabase.CreateAsset(CreateInstance<PrefabBlockBuilder>(), path + "NewPrefabBuilder.asset");
    }

    void OnEnable() 
    {
		blockSet = (BlockSet)target;		
	}	
	
	public override void OnInspectorGUI() 
    {		
		DrawBlockSet( blockSet );
		EditorGUILayout.Separator();
            
        if (selectedBlock != null) 
        {
            BlockEditor.DrawBlockEditor(selectedBlock, blockSet);

            if (materialEditor == null || materialEditor.target != selectedBlock.Material)
                materialEditor = (MaterialEditor)CreateEditor(selectedBlock.Material);

            if (materialEditor != null)
            {
                materialEditor.DrawHeader();
                materialEditor.OnInspectorGUI();
            }
		}
		
		if (GUI.changed) 
        {
			EditorUtility.SetDirty(blockSet);
		}
	}
	
	private void DrawBlockSet(BlockSet blockSet) 
    {
		GUILayout.BeginVertical(GUI.skin.box);

        Block oldSelectedBlock = selectedBlock;
		selectedBlock = BlockSetViewer.SelectionGrid(blockSet, selectedBlock, GUILayout.MinHeight(200), GUILayout.MaxHeight(300));
		
        if(selectedBlock != oldSelectedBlock) 
            GUIUtility.keyboardControl = 0;
		
		EditorGUILayout.Separator();
		
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("New Block")) 
        {
			Block newBlock = (Block) Activator.CreateInstance(typeof(Block));
            newBlock.Name = "NewBlock";
            blockSet.Blocks.Add(newBlock);

			EditorGUIUtility.keyboardControl = 0;
			GUI.changed = true;
		}
		GUILayout.EndHorizontal();
		
		if( GUILayout.Button("Remove") ) 
        {
            blockSet.Blocks.Remove(selectedBlock);
			GUI.changed = true;
		}

        if (GUILayout.Button("Force Save"))
        {
            EditorUtility.SetDirty(blockSet);
        }

        GUILayout.EndVertical();
	}	
}
