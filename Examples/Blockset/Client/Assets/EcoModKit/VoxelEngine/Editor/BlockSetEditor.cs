using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using CustomElements;

[CustomEditor(typeof(BlockSet))]
public class BlockSetEditor : Editor
{
    VisualElement rootElement;
    VisualTreeAsset visualTree;

    BlockSet blockSet;
    Block selectedBlock;
    PreviewRenderUtility previewRenderer;

    ListView blockList;
    BlockEditorElement blockElement;
    private Button newBlock;
    private Button removeBlock;
    private Button forceSave;
    private Button changeAllAudio;
    private TextField allAudioCategory;

    public void OnEnable()
    {
        // Hierarchy
        rootElement = new VisualElement();
        visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/EcoModKit/VoxelEngine/Editor/BlockSetEditor.uxml");

        // Styles
        var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/EcoModKit/VoxelEngine/Editor/BlockSetEditor.uss");
        rootElement.styleSheets.Add(stylesheet);

        blockSet = target as BlockSet;
    }

    public void OnDisable()
    {
        previewRenderer?.Cleanup();
    }

    public override VisualElement CreateInspectorGUI()
    {
        // Reset root element and reuse.
        rootElement.Clear();

        // Turn the UXML into a VisualElement hierarchy under root.
        visualTree.CloneTree(rootElement);

        // Find elements
        blockList = rootElement.Q("BlockList") as ListView;
        blockElement = rootElement.Q("BlockEditor") as BlockEditorElement;
        newBlock = rootElement.Q("NewBlock") as Button;
        removeBlock = rootElement.Q("RemoveBlock") as Button;
        forceSave = rootElement.Q("ForceSave") as Button;
        changeAllAudio = rootElement.Q("SetAllAudio") as Button;
        allAudioCategory = rootElement.Q("SetAllAudioField") as TextField;

        // Update elements
        blockList.itemHeight         =  42;
        blockList.makeItem           =  ListView_Makeitem;
        blockList.bindItem           =  ListView_BindItem;

#if !UNITY_2019_4
        blockList.onSelectionChange  += ListView_onSelectionChange;
#else
        blockList.onSelectionChanged += ListView_onSelectionChange;
#endif
        blockList.itemsSource        =  blockSet.Blocks;
        blockList.Refresh();

        newBlock.clickable.clicked += NewBlock_Clicked;
        removeBlock.clickable.clicked += RemoveBlock_Clicked;
        forceSave.clickable.clicked += ForceSave_Clicked;
        changeAllAudio.clickable.clicked += ChangeAllAudio;

        return rootElement;
    }

    private void ChangeAllAudio()
    {
        foreach (var block in blockSet.Blocks)
            block.AudioCategory = allAudioCategory.value;

        ForceSave_Clicked();
    }

    private void NewBlock_Clicked()
    {
        Block newBlock = (Block)Activator.CreateInstance(typeof(Block));
        newBlock.Name = "NewBlock";
        blockSet.Blocks.Add(newBlock);
        blockList.Refresh();
    }

    private void RemoveBlock_Clicked()
    {
        blockSet.Blocks.Remove(selectedBlock);
        blockList.selectedIndex = -1;
        blockList.Refresh();
    }

    private void ForceSave_Clicked()
    {
        EditorUtility.SetDirty(blockSet);
    }

    private void ListView_onSelectionChange(IEnumerable<object> objs) => this.blockElement.Block = this.selectedBlock = objs.FirstOrDefault() as Block;

    private VisualElement ListView_Makeitem()
    {
        var root = new VisualElement() { name = "ListItem" };
        root.Add(new Image() { name = "Preview" });
        root.Add(new Label() { name = "Name" });
        return root;
    }

    private void ListView_BindItem(VisualElement e, int index)
    {
        var block = blockSet.Blocks[index];

        var label = e.Q("Name") as Label;
        var preview = e.Q("Preview") as Image;

        preview.RemoveFromClassList("shader-problem");

        label.text = block.Name;
        preview.image = null;

        bool exception = false;
        bool shaderProblem = false;

        try
        {
            var shaderName = block.Material == null ? null :
                block.Material.shader == null ? null :
                block.Material.shader.name;
            if (shaderName != null && !(shaderName.StartsWith("Curved/") || shaderName.StartsWith("CalmWater")))
                shaderProblem = true;
        }
        catch
        {
            shaderProblem = true;
            exception = true;
        }

        if (block.Builder is CustomBuilder customBuilder)
        {
            if (customBuilder.usageCases.Count > 0)
            {
                var go = customBuilder.usageCases[0].mesh;
                var mf = go.GetComponent<MeshFilter>();
                preview.image = DrawRenderPreview(new Rect(0, 0, 40, 40), mf.sharedMesh, block.Material);
            }
        }
        else if (block.Builder is WeightedPrefabBlockBuilder weightedPrefabBlockBuilder)
        {
            if (weightedPrefabBlockBuilder.prefabs.Length > 0)
            {
                var prefab = weightedPrefabBlockBuilder.prefabs[0];
                preview.image = AssetPreview.GetAssetPreview(prefab.prefab);
            }
        }

        if (shaderProblem || exception)
        {
            preview.AddToClassList("shader-problem");
        }
    }

    public Texture DrawRenderPreview(Rect size, Mesh mesh, Material material)
    {
        if (previewRenderer == null)
        {
            previewRenderer = new PreviewRenderUtility();
            previewRenderer.cameraFieldOfView = 15.0f;
            previewRenderer.ambientColor = (Color.white * 0.6f);
            previewRenderer.camera.transform.position = (Vector3.forward * 5.0f) + (Vector3.up * 5.0f) + (Vector3.right * 5.0f);
            previewRenderer.camera.transform.LookAt(Vector3.zero, Vector3.up);
            previewRenderer.camera.nearClipPlane = 0.01f;
            previewRenderer.camera.farClipPlane = 50.0f;

            previewRenderer.lights[0].enabled = true;
            previewRenderer.lights[0].type = LightType.Directional;
            previewRenderer.lights[0].color = Color.white;
            previewRenderer.lights[0].intensity = 1.5f;
            previewRenderer.lights[0].transform.rotation = Quaternion.Euler(30f, 0f, 0f);
            previewRenderer.lights[1].enabled = true;
            previewRenderer.lights[1].intensity = 0.5f;
        }

        previewRenderer.BeginStaticPreview(size);
        previewRenderer.DrawMesh(mesh, Matrix4x4.identity, material, 0);
        previewRenderer.Render();
        return previewRenderer.EndStaticPreview();
    }
}