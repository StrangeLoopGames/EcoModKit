using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ModExporter : EditorWindow
{
    private string bundleName = string.Empty;
    private string sceneName = string.Empty;
    private bool forceTag = false;

    [MenuItem("ModKit/ModKit Tools...")]
    public static void ShowWindow()
    {
        GetWindow<ModExporter>("ModKit Tools");
    }

    public void OnGUI()
    {
        this.bundleName = EditorGUILayout.TextField("Bundle: ", this.bundleName);
        this.sceneName = EditorGUILayout.TextField("Scene: ", this.sceneName);
        this.forceTag = GUILayout.Toggle(this.forceTag, "Replace Existing Tags");
        if (GUILayout.Button("Tag All Dependencies From Scene Into Bundle"))
        {
            var bundlename = this.bundleName.ToLower();

            // everything in this bundle
            var existing = new List<string>(AssetDatabase.GetAssetPathsFromAssetBundle(bundlename));

            var scene = UnityEditor.SceneManagement.EditorSceneManager.GetSceneByName(this.sceneName);
            if (scene.IsValid() && scene.isLoaded)
            {
                var gameObjs = scene.GetRootGameObjects();
                var dependencies = EditorUtility.CollectDependencies(gameObjs);
                foreach (var dep in dependencies)
                {
                    if (dep is MonoScript)
                        continue;
                    var assetpath = AssetDatabase.GetAssetPath(dep);
                    var importer = AssetImporter.GetAtPath(assetpath);
                    if (importer != null)
                    {
                        if (importer.assetBundleName == string.Empty || (forceTag && importer.assetBundleName != bundlename))
                        {
                            Debug.Log("tagging " + dep.ToString(), dep);
                            importer.SetAssetBundleNameAndVariant(bundlename, string.Empty);
                        }
                        existing.Remove(assetpath);
                    }
                }

                foreach (var old in existing)
                {
                    var importer = AssetImporter.GetAtPath(old);
                    if (importer != null)
                    {
                        Debug.Log("removing " + old + " as it is no longer referenced");
                        importer.SetAssetBundleNameAndVariant(string.Empty, string.Empty);
                    }
                }
            }
        }
        
        if (GUILayout.Button("Nuke all bundle tags"))
        {
            var allBundles = AssetDatabase.GetAllAssetBundleNames();
            foreach (var bundleName in allBundles)
            {
                var assets = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
                foreach (var asset in assets)
                {
                    var importer = AssetImporter.GetAtPath(asset);
                    if (importer != null)
                        importer.SetAssetBundleNameAndVariant(string.Empty, string.Empty);
                }
            }
        }
    }

    private static AssetBundleManifest Build()
    {
        var timer = System.Diagnostics.Stopwatch.StartNew();
        if (!Directory.Exists("AssetBundles"))
            Directory.CreateDirectory("AssetBundles");
        var manifest = BuildPipeline.BuildAssetBundles("AssetBundles", BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        Debug.Log(string.Format("Bundling Finished in {0:0.00} seconds", timer.ElapsedMilliseconds / 1000.0));
        return manifest;
    }

    [MenuItem("ModKit/Build Bundles")]
    public static void BuildBundles()
    {
        var manifest = Build();

        // TODO: send specific / all bundles to server dir?
        if (Directory.Exists(Application.streamingAssetsPath))
            Directory.Delete(Application.streamingAssetsPath, true);
        Directory.CreateDirectory(Application.streamingAssetsPath);
        foreach (var f in manifest.GetAllAssetBundles())
            File.Copy(Path.Combine("AssetBundles", f), Path.Combine(Application.streamingAssetsPath, f + ".unity3d"), true);
    }
}