using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#if MODKITSCRIPTS
public class ScriptExporter : IDisposable
{
    private const string OutputDllName = "Temp_ModKitBuild";

    public void Dispose()
    {
        AssetDatabase.DeleteAsset(Application.dataPath + "/Resources/" + OutputDllName + ".txt");

        // remove stores
        foreach (var store in Object.FindObjectsOfType<EcoScriptStore>())
            Component.DestroyImmediate(store);
    }

    public TextAsset PackageScripts()
    {
        HashSet<string> generatedAssets = new HashSet<string>();
        List<string> textToCompile = new List<string>();

        // pre-remove nascent stores
        foreach (var store in Object.FindObjectsOfType<EcoScriptStore>())
            Component.DestroyImmediate(store);

        // find all EcoScripts in this scene, and generate assemblies for them
        foreach (var ecoscript in Object.FindObjectsOfType<EcoScript>())
        {
            // make store on object
            var store = ecoscript.gameObject.GetComponent<EcoScriptStore>();
            if (store == null)
                store = ecoscript.gameObject.AddComponent<EcoScriptStore>();

            store.ScriptTypes.Add(ecoscript.GetType().Name);

            // mark the assembly to generate, if we haven't already
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(ecoscript));
            if (generatedAssets.Contains(scriptPath))
                continue;

            // load the text
            var scriptAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(scriptPath);
            if (scriptAsset == null)
            {
                Debug.LogWarning("Could not find script asset at: " + scriptPath);
                continue;
            }
            string scriptText = scriptAsset.text;

            textToCompile.Add(scriptText);

            generatedAssets.Add(scriptPath);

            EditorUtility.DisplayProgressBar("Scripts", "Gathering Scripts... " + ecoscript.GetType().Name, 0.0f);
        }

        EditorUtility.DisplayProgressBar("Scripts", "Compiling... ", 0.25f);

        // add in the eco script to compile
        string ecoscripttext = @"
            using UnityEngine; 
            public class EcoScript : MonoBehaviour { }
            ";
        textToCompile.Add(ecoscripttext);

        // run it all through the runtime compiler
        ModKit.RuntimeCompiler rc = new ModKit.RuntimeCompiler();
        rc.SetupCompiler(OutputDllName, false);
        var results = rc.CompileAll(textToCompile);
        if (results == null)
            throw new InvalidOperationException("Failed to compile. Fix script issues first.");

        EditorUtility.DisplayProgressBar("Scripts", "Encoding Compilation Results... ", 0.5f);

        // encode the result in a text asset that can participate in the bundle
        var assemblyBytes = File.ReadAllBytes(results.PathToAssembly);
        var assemblyString = System.Text.Encoding.UTF8.GetString(assemblyBytes);

        EditorUtility.DisplayProgressBar("Scripts", "Converting to Unity Format... ", 0.75f);

        var textAsset = this.ConvertStringToTextAsset(assemblyString, OutputDllName);

        EditorUtility.ClearProgressBar();
        
        return textAsset;
    }

    private TextAsset ConvertStringToTextAsset(string text, string filename)
    {
        File.WriteAllText(Application.dataPath + "/Resources/" + filename + ".txt", text);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        TextAsset textAsset = Resources.Load(filename) as TextAsset;
        return textAsset;
    }
}
#endif