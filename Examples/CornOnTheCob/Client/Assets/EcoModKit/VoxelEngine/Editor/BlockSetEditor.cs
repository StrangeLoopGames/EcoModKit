using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using CustomElements;
using System.Collections;
using System.Threading.Tasks;
using System.Text;
using System.IO;

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
    Button newBlock;
    Button removeBlock;
    Button forceSave;
    Button changeAllAudio;
    Button createLodGroups;
    Button validate;
    TextField allAudioCategory;

    readonly Dictionary<string, BlockMeshLodGroup> bmlgCache = new Dictionary<string, BlockMeshLodGroup>(1000);

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
        createLodGroups = rootElement.Q("CreateLodGroups") as Button;
        validate = rootElement.Q("Validate") as Button;

        // Update elements
        blockList.itemHeight = 42;
        blockList.makeItem = ListView_Makeitem;
        blockList.bindItem = ListView_BindItem;
        blockList.onSelectionChange += ListView_onSelectionChange;
        blockList.itemsSource = blockSet.Blocks;
        blockList.Refresh();

        newBlock.clickable.clicked += NewBlock_Clicked;
        removeBlock.clickable.clicked += RemoveBlock_Clicked;
        forceSave.clickable.clicked += ForceSave_Clicked;
        changeAllAudio.clickable.clicked += ChangeAllAudio_Clicked;
        createLodGroups.clickable.clicked += CreateLodGroups_Clicked;
        validate.clickable.clicked += Validate_Clicked;

        return rootElement;
    }

    /// <summary>Validate all contained assets (just the Custombuilders for now)</summary>
    private async void Validate_Clicked()
    {
        bool running = true;
        int progressId = Progress.Start("Validate");
        Progress.RegisterCancelCallback(progressId, () => running = false);

        int max = blockSet.Blocks.Count;
        int current = 0;

        var sb = new StringBuilder(1024);

        foreach (var block in blockSet.Blocks)
        {
            if (!running) break;

            sb.Clear();
            sb.AppendLine($"Block {block.Name} has validation errors:");

            bool hasErrors = false;

            for (int i = 0; i < block.Materials.Length; ++i)
            {
                if (block.Materials[i] == null)
                {
                    sb.AppendLine($"SubMaterial {i} is null");
                    hasErrors = true;
                }
            }

            // include block.Material as first material
            int materialCount = block.Materials.Length + 1;

            if (block.Builder is CustomBuilder builder)
            {
                foreach (var usageCase in builder.usageCases)
                {
                    try
                    {
                        var bmlg = usageCase.blockMeshLodGroup;
                        if (bmlg != null)
                        {
                            bmlg.OnValidate();

                            var meshes = bmlg.LOD0.Select(l => l.mesh).ToList();
                            if (bmlg.LOD1.mesh != null) meshes.Add(bmlg.LOD1.mesh);

                            foreach (var mesh in meshes)
                            {
                                if (mesh.subMeshCount > materialCount)
                                {
                                    sb.AppendLine($"Mesh {mesh.name} has {mesh.subMeshCount} submeshes but block as {materialCount} materials");
                                    hasErrors = true;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to validate: {e.Message}");
                    }

                    Progress.Report(progressId, current, max);
                    await Task.Yield();
                }
            }

            if (hasErrors)
            {
                Debug.LogError(sb.ToString());
            }
            ++current;
        }

        Progress.Remove(progressId);
    }

    /// <summary>Create new BlockMeshLodGroup asset, or update exist ones, for each MeshUsageCase using the existing data.</summary>
    private async void CreateLodGroups_Clicked()
    {
        Debug.Log("===============================================");
        Debug.Log("CreateLodGroups");
        Debug.Log("===============================================");

        bool running = true;
        int progressId = Progress.Start("CreateLodGroups");
        Progress.RegisterCancelCallback(progressId, () => running = false);

        int max = blockSet.Blocks.Count;
        int current = 0;

        this.bmlgCache.Clear();

        foreach (var block in blockSet.Blocks)
        {
            if (!running) break;

            if (block.Builder is CustomBuilder builder)
            {
                Debug.Log($"Processing {block.Name}");
                foreach (var usageCase in builder.usageCases)
                {
                    if (!running) break;

                    try
                    {
                        ProcessUsageCase(builder, usageCase);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to process: {e.Message}");
                        running = false;
                    }

                    Progress.Report(progressId, current, max);
                    await Task.Yield();
                }

                EditorUtility.SetDirty(builder);
            }
            ++current;
        }

        Progress.Remove(progressId);

        ForceSave_Clicked();

        Debug.Log("===============================================");
    }

    private void ProcessUsageCase(CustomBuilder builder, MeshUsageCase usageCase)
    {
        var sourcePrefab = usageCase.mesh;
        var sourceDir = Path.GetDirectoryName(AssetDatabase.GetAssetPath(sourcePrefab));

        var meshesObjects = usageCase.GetMeshObjects();
        var meshes = meshesObjects.Where(mo => mo.TryGetComponent<MeshFilter>(out var _)).Select(mo => mo.GetComponent<MeshFilter>().sharedMesh).ToArray();
        var sourceMeshPaths = meshes.Select(m => AssetDatabase.GetAssetPath(m)).ToArray();

        var names = meshes.Select(m => m.name).OrderBy(m => m).ToList();
        string fileName = MakeFileName(names);

        var destPath = Path.Combine(sourceDir, fileName + ".asset");

        Debug.Log($"Sources: '{fileName}', Destination: '{destPath}'");

        var existingBmlg = usageCase.blockMeshLodGroup;

        var newBmlg = CreateBmlg(usageCase, sourcePrefab);
        
        {
            if (existingBmlg == null)
                existingBmlg = GetBlockMeshLodGroup(destPath);

            int i = 1;
            while (existingBmlg != null && i < 100)
            {
                // if it's the same, use it instead
                if (existingBmlg.Equals(newBmlg))
                    break;

                // try next
                ++i;
                destPath = Path.Combine(sourceDir, $"{fileName}-{i}.asset");
                existingBmlg = GetBlockMeshLodGroup(destPath);
            }

            if (existingBmlg == null)
            {
                existingBmlg = newBmlg;

                this.bmlgCache.Add(destPath.ToLower(), newBmlg);

                AssetDatabase.CreateAsset(existingBmlg, destPath);
            }

            usageCase.blockMeshLodGroup = existingBmlg;
        }

        // SJS not removing existing data for now
        //usageCase.mesh = null;
        //usageCase.isMeshFacesConcave = PerFaceFlag.None;
        //usageCase.meshAlternates = null;
        //usageCase.isMeshAlternatesFacesConcave = null;

    }

    private BlockMeshLodGroup GetBlockMeshLodGroup(string destPath)
    {
        var key = destPath.ToLower();
        if (bmlgCache.TryGetValue(key, out var value))
            return value;

        var bmlg = AssetDatabase.LoadAssetAtPath<BlockMeshLodGroup>(destPath);
        if (bmlg != null)
            bmlgCache.Add(key, bmlg);
        return bmlg;
    }

    private static BlockMeshLodGroup CreateBmlg(MeshUsageCase usageCase, GameObject sourcePrefab)
    {
        var bmlg = ScriptableObject.CreateInstance<BlockMeshLodGroup>();

        bmlg.LOD0 = new MeshAndFlags[usageCase.meshAlternates.Length + 1];

        if (sourcePrefab != null && sourcePrefab.TryGetComponent<MeshFilter>(out var meshFilterLod0))
            bmlg.LOD0[0].mesh = meshFilterLod0.sharedMesh;
        else
            bmlg.LOD0[0].mesh = null;

        bmlg.LOD0[0].concaveFaces = usageCase.isMeshFacesConcave;

        for (int i = 0; i < usageCase.meshAlternates.Length; ++i)
        {
            if (usageCase.meshAlternates[i].TryGetComponent<MeshFilter>(out var meshFilterLod0Alt))
                bmlg.LOD0[i + 1].mesh = meshFilterLod0Alt.sharedMesh;
            else
                bmlg.LOD0[i + 1].mesh = null;

            if (usageCase.isMeshAlternatesFacesConcave != null && i < usageCase.isMeshAlternatesFacesConcave.Length)
                bmlg.LOD0[i + 1].concaveFaces = usageCase.isMeshAlternatesFacesConcave[i];
            else
                bmlg.LOD0[i + 1].concaveFaces = PerFaceFlag.None;
        }


        if (bmlg.LOD0[0].mesh != null)
        {
            //var sourceMeshPrefabRoot = PrefabUtility.GetCorrespondingObjectFromOriginalSource<UnityEngine.Object>(sourcePrefab);
            //var sourceMeshPrefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource<UnityEngine.Object>(bmlg.LOD0[0].mesh);
            //var sourceMeshPrefab2 = PrefabUtility.GetCorrespondingObjectFromOriginalSource<UnityEngine.Object>(sourceMeshPrefab);

            var mesh = FindLod1Mesh(bmlg.LOD0[0].mesh.name);
            bmlg.LOD1 = new MeshAndFlags { mesh = mesh, concaveFaces = PerFaceFlag.None };
        }


        var collider = sourcePrefab.transform.Find("Collider");

        if (collider && collider.TryGetComponent<MeshFilter>(out var meshFilterCollider))
            bmlg.Collider = meshFilterCollider.sharedMesh;

        return bmlg;
    }

    private static Mesh FindLod1Mesh(string name)
    {
        var searchName = name;
        var searchPath = @"Assets\Art\Blocks\Natural Blocks\Primary Blending Set Resources\Terrain Meshes and Prefabs\LOD1\UVed";

        for (int i = 50; i >= 0; --i)
        {
            var postfix = $"_V{i}";
            searchName = RemovePostfix(searchName, postfix);
        }

        searchName = RemovePostfix(searchName, "_Grass");
        searchName = RemovePostfix(searchName, "uved");

        searchName = searchName.Replace("WithCovers1", "WithCovers");
        searchName = searchName.Replace("CornerCube_S_Covers", "CornerCube_S");

        for (int i = 10; i >= 0; --i)
        {
            searchName = searchName.Replace($"S{i}", "S");
        }

        if (name != searchName)
            Debug.Log($"Changed name from '{name}' to '{searchName}'");

        var mesh = AssetDatabase.LoadAssetAtPath<Mesh>($@"{searchPath}\{searchName}_LOD1.fbx");

        if (mesh == null)
            Debug.LogWarning($"Couldn't find LOD1 mesh '{searchName}_LOD1'");

        return mesh;
    }

    private static string RemovePostfix(string name, string postfix)
    {
        if (name.EndsWith(postfix, StringComparison.InvariantCultureIgnoreCase))
        {
            return name.Remove(name.LastIndexOf(postfix, StringComparison.InvariantCultureIgnoreCase));
        }

        return name;
    }

    private static string MakeFileName(List<string> names)
    {
        var start = names[0];

        var prefixLength = names.Min(n => PrefixLength(n, start));

        return start.Substring(0, prefixLength) + String.Join("-", names.Select(n=>n.Substring(prefixLength)));
    }

    private static int PrefixLength(string a, string b)
    {
        int i = 0;
        for (; i < a.Length && i < b.Length; i++)
        {
            if (a[i] != b[i])
                break;
        }

        return i;
    }

    private void ChangeAllAudio_Clicked()
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
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
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
                if (customBuilder.usageCases[0].blockMeshLodGroup != null)
                {
                    var mesh = customBuilder.usageCases[0].blockMeshLodGroup.LOD0[0].mesh;
                    if (mesh != null)
                        preview.image = DrawRenderPreview(new Rect(0, 0, 40, 40), mesh, block.Material);
                }
                else
                {
                    var go = customBuilder.usageCases[0].mesh;
                    var mf = go.GetComponent<MeshFilter>();
                    preview.image = DrawRenderPreview(new Rect(0, 0, 40, 40), mf.sharedMesh, block.Material);
                }
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

        // Create a duplicate material with NO_CURVE enabled for rendering the preview
        Material previewMaterial = Instantiate(material);
        previewMaterial.EnableKeyword("NO_CURVE");

        previewRenderer.BeginStaticPreview(size);
        previewRenderer.DrawMesh(mesh, Matrix4x4.identity, previewMaterial, 0);
        previewRenderer.Render();

        DestroyImmediate(previewMaterial);
        return previewRenderer.EndStaticPreview();
    }
}