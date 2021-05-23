using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;

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

    [MenuItem("ModKit/Build Current Bundle")]
    public static void BuildSceneBundle()
    {
        if (EditorSceneManager.loadedSceneCount > 1)
        {
            EditorUtility.DisplayDialog("Build Error", "Only one scene should be open while building, please close other scenes", "OK");
            return;
        }

        var scene = EditorSceneManager.GetSceneAt(0);
        var importer = AssetImporter.GetAtPath(scene.path);
        if (importer == null || importer.assetPath == string.Empty)
        {
            EditorUtility.DisplayDialog("Build Error", "Please save the current scene before building.", "OK");
            return;
        }

        // deactivate roots for bundle
        Dictionary<string, bool> activeState = new Dictionary<string, bool>();
        foreach (var root in scene.GetRootGameObjects())
        {
            activeState[root.name] = root.activeSelf;
            root.SetActive(false);
        }

        var path = EditorPrefs.GetString("ModKitPath", string.Empty);
        var fileName = string.Empty;
        var dirName = string.Empty;
        if (path != string.Empty)
        {
            fileName = Path.GetFileNameWithoutExtension(path);
            dirName = Path.GetDirectoryName(path);
        }

        path = EditorUtility.SaveFilePanel("Save bundle", dirName, fileName, "unity3d");
        if (path == string.Empty)
            return;
        EditorPrefs.SetString("ModKitPath", path);

        var bundleName = Path.GetFileNameWithoutExtension(path).ToLower();
        importer.SetAssetBundleNameAndVariant(bundleName, string.Empty);

        var manifest = Build();
        foreach (var f in manifest.GetAllAssetBundles())
        {
            if (f == bundleName)
                File.Copy(Path.Combine("AssetBundles", f), path, true);
        }

        // restore active state
        foreach (var root in scene.GetRootGameObjects())
            if (activeState.ContainsKey(root.name))
                root.SetActive(activeState[root.name]);
    }
}