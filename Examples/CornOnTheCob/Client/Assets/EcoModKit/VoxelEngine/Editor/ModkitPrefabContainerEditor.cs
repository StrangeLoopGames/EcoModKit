using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ModkitPrefabContainer))]
public class ModkitPrefabContainerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Import Folder"))
            ImportFolder((ModkitPrefabContainer)target);

        if (GUILayout.Button("Check for shared"))
            CheckForShared((ModkitPrefabContainer)target);

        if (GUILayout.Button("Split shared"))
            SplitShared((ModkitPrefabContainer)target);

        base.OnInspectorGUI();
    }

    /// <summary>
    /// Check if any objects in the scene are instances of a shared prefab. If a prefab is shared it means we can't 
    /// apply overrides to as they will get overridden by another instance. Ideally these would be prevab variant 
    /// assets, however that's a bit too complicated for this to set up.
    /// </summary>
    private void CheckForShared(ModkitPrefabContainer target)
    {
        var sb = new StringBuilder(1000);
        sb.AppendLine("Existing prefabs:");

        var children = target.gameObject.transform.Cast<Transform>().Select(t => t.gameObject).ToList();
        var existingPrefabs = new Dictionary<GameObject, GameObject>();
        foreach (var child in children)
        {
            var sourcePrefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(child);
            if (sourcePrefab == null)
            {
                Debug.LogError($"Child {child.GetScenePath()} has no original source");
                continue;
            }

            if (existingPrefabs.ContainsKey(sourcePrefab))
            {
                var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(sourcePrefab);
                sb.AppendLine($"{child.name} => {path}");
            }
            else
            {
                existingPrefabs.Add(sourcePrefab, child);
            }
        }

        Debug.LogWarning(sb.ToString());
    }

    /// <summary>
    /// For any prefab that is shared, create a new copy and use it as the scene object's prefab.
    /// </summary>
    private void SplitShared(ModkitPrefabContainer target)
    {
        var sharedPrefabs = new HashSet<GameObject>();

        var transform = target.gameObject.transform;
        var children = transform.Cast<Transform>().Select(t => t.gameObject).ToList();
        var existingPrefabs = new Dictionary<GameObject, GameObject>();

        // first find every shared prefab
        foreach (var child in children)
        {
            var sourcePrefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(child);
            if (sourcePrefab == null)
            {
                Debug.LogError($"Child {child.GetScenePath()} has no original source");
                continue;
            }

            if (existingPrefabs.ContainsKey(sourcePrefab))
            {
                sharedPrefabs.Add(sourcePrefab);
            }
            else
            {
                existingPrefabs.Add(sourcePrefab, child);
            }
        }

        // go through list again now that we've identified all the shared prefabs
        foreach (var child in children)
        {
            var sourcePrefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(child);
            if (sourcePrefab == null)
            {
                Debug.LogError($"Child {child.GetScenePath()} has no original source");
                continue;
            }

            // if this prefab is a shared one
            if (sharedPrefabs.Contains(sourcePrefab))
            {
                var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(sourcePrefab);

                var fileName = Path.GetFileNameWithoutExtension(path);
                var folder = Path.GetDirectoryName(path);

                var destPath = $@"{folder}\{child.name}.prefab";

                // if we aren't the original version
                if (fileName != child.name)
                {
                    Debug.Log($"Going to copy {fileName} to {child.name} ({destPath})");

                    var i = child.transform.GetSiblingIndex();

                    // check if the replacement has already been created
                    var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(destPath);
                    GameObject newChild;

                    if (existingPrefab == null)
                    {
                        // remove any reference to old prefab but keep all the contents
                        PrefabUtility.UnpackPrefabInstance(child, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

                        // save a copy of the object to a new prefab asset on disk
                        var newPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(child, destPath, InteractionMode.AutomatedAction);

                        // instanciate the new prefab to create a link to our new copy
                        newChild = (GameObject)PrefabUtility.InstantiatePrefab(newPrefab);
                    }
                    else
                    {
                        // already saved new prefabs, just hook up to the new one
                        newChild = (GameObject)PrefabUtility.InstantiatePrefab(existingPrefab);
                    }

                    DestroyImmediate(child);

                    // add back to scene where the existing object was
                    newChild.transform.SetParent(transform, false);
                    newChild.transform.SetSiblingIndex(i);
                }
            }
        }
    }

    /// <summary>
    /// Import all the prefabs in a folder into ModkitPrefabContainers, and remove any existing instances in the scene.
    /// </summary>
    private void ImportFolder(ModkitPrefabContainer target)
    {
        var folder = EditorUtility.OpenFolderPanel("Select folder to import", @"Assets\Art\Player Built Objects\Prefabs", "");
        if (folder == null) return;

        // find all the existing prefabs
        var children = target.gameObject.transform.Cast<Transform>().Select(t => t.gameObject).ToList();
        var existingPrefabs = new Dictionary<GameObject, GameObject>();
        foreach (var child in children)
        {
            // if there are overrides we don't want to lose them
            if (PrefabUtility.HasPrefabInstanceAnyOverrides(child, false))
            {
                Debug.LogError($"Child {child.GetScenePath()} has overrides");
                continue;
            }

            // skip existing MPC children which are for ordering
            if (child.HasComponent<ModkitPrefabContainer>())
                continue;

            // find the source prefab, which will be a reference to a prefab asset on disk
            var sourcePrefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(child);
            if (sourcePrefab == null)
            {
                Debug.LogError($"Child {child.GetScenePath()} has no original source");
                continue;
            }

            // ignore if we already have it (usually from bad scene merges)
            if (existingPrefabs.ContainsKey(sourcePrefab))
            {
                Debug.Log($"Existing duplicate of {sourcePrefab.name}, removing");
                DestroyImmediate(child);
                continue;
            }
            existingPrefabs.Add(sourcePrefab, child);
        }

        // now process the folder with the list of existing prefabs to remove when replaced
        ImportFolder(target.gameObject, folder, existingPrefabs);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="folder"></param>
    /// <param name="existingPrefabs">dictionary of existing prefabs, to be destroyed when replaced</param>
    private void ImportFolder(GameObject target, string folder, Dictionary<GameObject, GameObject> existingPrefabs)
    {
        // get a list of prefab files from the directory and trim their paths to be relative to client dir
        int length = Application.dataPath.IndexOf("Assets");
        var files = Directory.GetFiles(folder, "*.prefab").Select(f => Path.GetFullPath(f)).Select(f => f.Substring(length)).ToList();
        var subfolders = Directory.GetDirectories(folder);
        var container = target.GetOrAddComponent<ModkitPrefabContainer>();

        // create a list of existing prefabs to modify
        var prefabs = container.Prefabs.ToList();
        foreach (var file in files)
        {
            //var asset = AssetDatabase.LoadMainAssetAtPath(file);
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(file);
            var prefab = asset as GameObject;
            //var prefab = PrefabUtility.LoadPrefabContents(file);
            if (!prefab) continue;

            if (!prefabs.Contains(prefab))
            {
                Debug.Log($"Adding prefab '{prefab.name}' to {target.GetScenePath()}");
                prefabs.Add(prefab);
            }

            if (existingPrefabs.TryGetValue(prefab, out var go))
            {
                Debug.Log($"Removing {go.name}");
                DestroyImmediate(go);
            }
        }

        // assign our prefabs
        container.Prefabs = prefabs.ToArray();

        // recurse into folders
        foreach (var subfolder in subfolders)
        {
            var name = new DirectoryInfo(subfolder).Name;
            var child = target.transform.Find(name);
            if (child == null)
            {
                var newGo = new GameObject(name, typeof(ModkitPrefabContainer));
                newGo.transform.SetParent(target.transform, true);
                child = newGo.transform;
            }

            ImportFolder(child.gameObject, subfolder, existingPrefabs);
        }
    }
}
