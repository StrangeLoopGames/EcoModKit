using Assets.EcoModKit.VoxelEngine.Editor.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Assets.Editor.Blocks
{
    public class BlockEditorWindow : EditorWindow
    {
        VisualTreeAsset visualTree;

        BlockEditorModel model;
        private ListView blockList;
        Button retrieve;

        [MenuItem("Eco/BlockEditorWindow")]
        public static void OpenWindow()
        {
            var window = ScriptableObject.CreateInstance<BlockEditorWindow>();
            window.model = ScriptableObject.CreateInstance<BlockEditorModel>();
            window.Show();
        }

        void CreateGUI()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/EcoModKit/VoxelEngine/Editor/BlockEditorWindow.uxml");
            var rootFromUxml = visualTree.Instantiate();
            rootVisualElement.Add(rootFromUxml);

            // Styles
            var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/EcoModKit/VoxelEngine/Editor/BlockEditorWindow.uss");
            rootVisualElement.styleSheets.Add(stylesheet);

            // Find elements
            blockList = rootVisualElement.Q("BlockList") as ListView;

            retrieve = rootVisualElement.Q("retrieve") as Button;


            blockList.itemHeight = 42;
            blockList.makeItem = BlockList_MakeItem;
            blockList.bindItem = BlockList_BindItem;

            retrieve.clickable.clicked += Retrieve_clicked;

            if (model != null)
            {
                var so = new SerializedObject(model);
                rootVisualElement.Bind(so);
            }
        }

        private void BlockList_BindItem(VisualElement e, int index)
        {
            var block = model.Blocks[index];
            ((BlockElement)e).value = block;
        }

        private VisualElement BlockList_MakeItem()
        {
            return new BlockElement();
        }

        void Retrieve_clicked()
        {
            model.BlockSets = new List<BlockSet>();

            LoadObjectsFromScene("Eco");

            model.Blocks = model.BlockSets.SelectMany(blockSet => blockSet.Blocks).ToList();
        }

        void LoadObjectsFromScene(string sceneName)
        {
            var sceneObjects = SceneManager.GetSceneByName(sceneName).GetRootGameObjects();
            foreach (var sceneObject in sceneObjects)
            {
                var containers = sceneObject.GetComponentsInChildren<BlockSetContainer>(true);

                model.BlockSets.AddRange(containers.SelectMany(c => c.blockSets));
            }
        }
    }
}
