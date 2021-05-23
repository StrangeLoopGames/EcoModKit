using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public static class BlockSetMenus
{
    [MenuItem("Assets/Create/VoxelEngine/BlockSet")]
    private static void CreateBlockSet()
    {
        string path = "Assets/";
        if (Selection.activeObject != null)
        {
            path = AssetDatabase.GetAssetPath(Selection.activeObject) + "/";
        }
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<BlockSet>(), path + "NewBlockSet.asset");
    }

    [MenuItem("Assets/Create/VoxelEngine/CustomBuilder")]
    private static void CreateCustomBuilder()
    {
        string path = "Assets/";
        if (Selection.activeObject != null)
        {
            path = AssetDatabase.GetAssetPath(Selection.activeObject) + "/";
        }
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<CustomBuilder>(), path + "NewBuilder.asset");
    }

    [MenuItem("Assets/Create/VoxelEngine/WeightedPrefabBuilder")]
    private static void CreateWeightedPrefabBuilder()
    {
        string path = "Assets/";
        if (Selection.activeObject != null)
        {
            path = AssetDatabase.GetAssetPath(Selection.activeObject) + "/";
        }
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<WeightedPrefabBlockBuilder>(), path + "NewPrefabBuilder.asset");
    }

    [MenuItem("Assets/Create/VoxelEngine/PrefabRuleBuilder")]
    private static void CreatePrefabRuleBuilder()
    {
        string path = "Assets/";
        if (Selection.activeObject != null)
        {
            path = AssetDatabase.GetAssetPath(Selection.activeObject) + "/";
        }
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<PrefabRuleBlockBuilder>(), path + "NewPrefabRuleBuilder.asset");
    }
}
