// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;

[CustomEditor(typeof(PrefabRuleBlockBuilder))]
class PrefabRuleBlockBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Editor"))
            PrefabRuleBlockBuilderEditorWindow.Open(target as PrefabRuleBlockBuilder);

        base.OnInspectorGUI();
    }
}

public class PrefabUsageCaseEditorComponent : MonoBehaviour
{
    new Renderer renderer;

    bool firstSelected = false;
    public Action OnSelected;

    public OffsetCondition condition;
    public Color color;

    static void DrawString(string text, UnityEngine.Vector3 worldPos, Color? colour = null)
    {
        UnityEditor.Handles.BeginGUI();
        if (colour.HasValue)
            GUI.color = colour.Value;
        var view = UnityEditor.SceneView.currentDrawingSceneView;
        UnityEngine.Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
        UnityEngine.Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
        GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height - (size.y / 2), size.x, size.y), text);
        UnityEditor.Handles.EndGUI();
    }

    void OnDrawGizmos()
    {
        Color color = Color.white;
        if (renderer == null)
            renderer = this.GetComponent<Renderer>();

        if (condition != null)
            DrawString(condition.ToString(), this.transform.position);

        bool selected = (Selection.activeGameObject == this.gameObject);

        if (!firstSelected && selected)
        {
            renderer.sharedMaterial.color = new Color(0, 1, 0, .125f);
            firstSelected = true;
            if (OnSelected != null)
                OnSelected();
        }
        else if (!selected && firstSelected == true)
        {
            firstSelected = false;
        }
        else
        {
            color = new Color(1, 1, 1, .125f);

            if (condition != null && condition.rules.Count > 0)
            {
                var rule = condition.rules[0];
                switch (rule.ruleType)
                {
                    case BlockRule.RuleType.EqualsType: color = new Color(0, 1, 0, .125f); break;
                    case BlockRule.RuleType.NotEqualsType: color = new Color(1, 0, 0, .125f); break;
                    case BlockRule.RuleType.NotSolidType: color = new Color(0, 0, 1, .125f); break;
                    case BlockRule.RuleType.IsSolidType: color = new Color(1, 1, 1, .125f); break;
                    case BlockRule.RuleType.EqualsCategory: color = new Color(0, 1, .5f, .125f); break;
                    case BlockRule.RuleType.NotEqualsCategory: color = new Color(1, 0, .5f, .125f); break;
                    case BlockRule.RuleType.EqualsThisType: color = new Color(0, 1, 0, .125f); break;
                    case BlockRule.RuleType.NotEqualsThisType: color = new Color(1, 0, 0, .125f); break;
                    case BlockRule.RuleType.SameOrHigherPriority: color = new Color(1, .5f, 0, .125f); break;
                    case BlockRule.RuleType.LowerPriorirty: color = new Color(1, 0, .5f, .125f); break;
                }
            }

            renderer.sharedMaterial.color = color;
        }

        color.a = 1.0f;
        Gizmos.color = color;
        Gizmos.DrawWireCube(this.transform.position, UnityEngine.Vector3.one);
    }
}

public class PrefabRuleBlockBuilderEditorWindow : EditorWindow
{
    private PrefabRuleBlockBuilder target;
    private PrefabUsageCase selectedUsageCase;

    static public void Open(PrefabRuleBlockBuilder bldr)
    {
        PrefabRuleBlockBuilderEditorWindow editor = GetWindow<PrefabRuleBlockBuilderEditorWindow>();

        editor.SetTarget(bldr);
    }

    public void SetTarget(PrefabRuleBlockBuilder bldr)
    {
        target = bldr;
    }

    void OnDestroy()
    {
        GameObject.DestroyImmediate(this.previewObject);
    }

    void OnGUI()
    {
        CreateRuleObjects();

        if (target != null)
        {
            EditorUtility.SetDirty(target);
            this.titleContent.text = target.name;
        }

        if (GUILayout.Button("Export builder"))
        {
            string filename = target.name + ".eco";
            string json = JsonUtility.ToJson(target, true);

            File.WriteAllText(filename, json);
        }

        if (GUILayout.Button("Import builder"))
        {
            string filename = EditorUtility.OpenFilePanel("Overwrite Builder", "", "eco");
            string json = File.ReadAllText(filename);

            JsonUtility.FromJsonOverwrite(json, target);
        }

        EditorGUILayout.BeginHorizontal();
        {
            ShowMeshSet();
            ShowNeighborGrids();
            ShowConditions();
        }
        EditorGUILayout.EndHorizontal();
    }

    void OnChange()
    {
        if (target != null)
        {
            UpdateRuleObjects();

            EditorUtility.SetDirty(target);
        }
    }

    GameObject previewObject;
    GameObject topLayer;
    GameObject middleLayer;
    GameObject bottomLayer;

    Dictionary<OffsetCondition.Offset, GameObject> offsetObjects;

    void CreateRuleObjects()
    {
        if (previewObject == null)
        {
            previewObject = new GameObject("PreviewPrefabBuilder", typeof(MeshFilter), typeof(MeshRenderer));
            topLayer = new GameObject("TopLayer");
            topLayer.transform.SetParent(previewObject.transform);
            topLayer.transform.localPosition = UnityEngine.Vector3.zero;

            middleLayer = new GameObject("MiddleLayer");
            middleLayer.transform.SetParent(previewObject.transform);
            middleLayer.transform.localPosition = UnityEngine.Vector3.zero;

            bottomLayer = new GameObject("BottomLayer");
            bottomLayer.transform.SetParent(previewObject.transform);
            bottomLayer.transform.localPosition = UnityEngine.Vector3.zero;

            var view = SceneView.currentDrawingSceneView;
            if (view != null)
            {
                view.pivot = previewObject.transform.position;
            }
        }

        if (offsetObjects == null)
        {
            offsetObjects = new Dictionary<OffsetCondition.Offset, GameObject>();

            for (int y = -1; y <= 1; y++)
                for (int x = -1; x <= 1; x++)
                    for (int z = -1; z <= 1; z++)
                    {
                        if (x == 0 && y == 0 && z == 0)
                            continue;

                        GameObject offsetObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        offsetObj.name = string.Format("OFFSET {0},{1},{2}", x, y, z);
                        var usageComponent = offsetObj.AddComponent<PrefabUsageCaseEditorComponent>();

                        if (y == -1)
                            offsetObj.transform.SetParent(bottomLayer.transform);
                        else if (y == 0)
                            offsetObj.transform.SetParent(middleLayer.transform);
                        else if (y == 1)
                            offsetObj.transform.SetParent(topLayer.transform);

                        offsetObj.transform.localPosition = new UnityEngine.Vector3(x, y, z);

                        var renderer = offsetObj.GetComponent<MeshRenderer>();
                        var material = AssetDatabase.LoadAssetAtPath("Assets/EcoModKit/Materials/CustomBuilderPreview.mat", typeof(Material)) as Material;
                        if (material != null)
                            renderer.sharedMaterial = new Material(material);

                        OffsetCondition.Offset condition = OffsetCondition.GetFromVector(new Vector3(x, y, z));

                        offsetObjects[condition] = offsetObj;

                        int index = (x + 1) + ((1 - z) * 3);
                        if (y == 1)
                            usageComponent.OnSelected = () => { this.ResetSelectedOffset(); this.topLayerSelection = index; this.Repaint(); };
                        if (y == 0)
                            usageComponent.OnSelected = () => { this.ResetSelectedOffset(); this.midLayerSelection = index; this.Repaint(); };
                        if (y == -1)
                            usageComponent.OnSelected = () => { this.ResetSelectedOffset(); this.bottomLayerSelection = index; this.Repaint(); };

                    }
        }

        UpdateRuleObjects();
    }

    void UpdateRuleObjects()
    {
        foreach (var offset in offsetObjects.Keys)
        {
            var obj = offsetObjects[offset];

            if (obj == null)
                continue;

            var usageComponent = obj.GetComponent<PrefabUsageCaseEditorComponent>();

            usageComponent.condition = selectedUsageCase != null ? selectedUsageCase.conditions.Find(cond => cond.offsetType == offset) : null;
            obj.SetActive(usageComponent.condition != null);
        }
    }

    // show the list of meshes, allowing selection of one
    void ShowMeshSet()
    {
        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(400));
        {
            GUILayout.Label("CUBES", EditorStyles.boldLabel);

            int i = 0;
            int removeIndex = -1;
            int moveUpIndex = -1;
            int moveDownIndex = -1;
            foreach (var usageCase in target.usageCases)
            {
                GUIStyle style = new GUIStyle(EditorStyles.miniButton);
                GUIStyle area = new GUIStyle(EditorStyles.miniButton);

                if (selectedUsageCase == usageCase)
                {
                    style.fontStyle = FontStyle.Bold;

                    area.normal.background = new Texture2D(1, 1);
                    area.normal.background.SetPixel(0, 0, Color.green);
                    area.normal.background.Apply();
                }

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.BeginHorizontal(area);
                    {
                        if (GUILayout.Button("X", style, GUILayout.Width(20)))
                            removeIndex = i;

                        if (GUILayout.Button("▲", style, GUILayout.Width(20)))
                            moveUpIndex = i;
                        if (GUILayout.Button("▼", style, GUILayout.Width(20)))
                            moveDownIndex = i;

                        GUILayout.Space(5);

                        usageCase.prefab = EditorGUILayout.ObjectField(usageCase.prefab, typeof(GameObject), false) as GameObject;

                        if (GUILayout.Button("Edit", style))
                        {
                            selectedUsageCase = usageCase;

                            ResetSelectedOffset();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndHorizontal();                

                i++;
            }

            if (removeIndex != -1)
            {
                target.usageCases.RemoveAt(removeIndex);
                OnChange();
            }

            if (moveUpIndex != -1 && moveUpIndex > 0)
            {
                var src = target.usageCases[moveUpIndex];
                var dst = target.usageCases[moveUpIndex - 1];
                target.usageCases[moveUpIndex - 1] = src;
                target.usageCases[moveUpIndex] = dst;
                OnChange();
            }

            if (moveDownIndex != -1 && moveDownIndex < target.usageCases.Count - 1)
            {
                var src = target.usageCases[moveDownIndex];
                var dst = target.usageCases[moveDownIndex + 1];
                target.usageCases[moveDownIndex + 1] = src;
                target.usageCases[moveDownIndex] = dst;
                OnChange();
            }

            if (GUILayout.Button("Add New Cube"))
            {
                target.usageCases.Add(new PrefabUsageCase());
                OnChange();
            }
        }
        EditorGUILayout.EndVertical();
    }

    void ResetSelectedOffset()
    {
        topLayerSelection = -1;
        midLayerSelection = -1;
        bottomLayerSelection = -1;
    }

    int topLayerSelection = -1;
    int midLayerSelection = -1;
    int bottomLayerSelection = -1;
    OffsetCondition.Offset selectedOffset = OffsetCondition.Offset.Offset_Null;

    // show the grid of neighbors to add conditions for, defaulting to all null (don't care)
    void ShowNeighborGrids()
    {
        if (selectedUsageCase == null)
            return;

        GUILayout.Space(10);

        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(400));
        {
            GUILayout.Label("Usage Conditions", EditorStyles.largeLabel);
            GUILayout.Label(selectedUsageCase.ToString(), EditorStyles.label);

            topLayer.SetActive(ShowLayer(Vector3.up, "Top", ref topLayerSelection, topLayer.activeSelf));
            middleLayer.SetActive(ShowLayer(Vector3.zero, "Middle", ref midLayerSelection, middleLayer.activeSelf));
            bottomLayer.SetActive(ShowLayer(Vector3.down, "Bottom", ref bottomLayerSelection, bottomLayer.activeSelf));
        }
        EditorGUILayout.EndVertical();
    }

    bool ShowLayer(Vector3 layerOffset, string name, ref int selection, bool enabled)
    {
        string[] layers = new string[9];
        Vector3[] offsets = new Vector3[9];

        int i = 0;

        for (int z = 1; z >= -1; z--)
            for (int x = -1; x <= 1; x++)
            {
                var pos = new Vector3(x, 0, z);

                Vector3 layerPos = layerOffset + pos;
                offsets[i] = layerPos;

                if (layerPos == Vector3.zero)
                    layers[i] = "[  FREE  ]\r\n[ SPACE ]";
                else
                {
                    var condition = OffsetCondition.GetFromVector(layerPos);

                    var usage = selectedUsageCase.conditions.Find(cond => cond.offsetType == condition);

                    if (usage != null)
                        layers[i] = layerPos.ToString() + "\r\n" + usage.ToString();
                    else
                        layers[i] = layerPos.ToString() + "\r\n" + "DontCare";
                }

                i++;
            }

        bool result = GUILayout.Toggle(enabled, name);

        // so dumb, selection grid steals focus so have to do this crazy button layout
        i = 0;
        GUILayout.BeginVertical();
        int newSelection = selection;
        for (int r1 = 0; r1 < 3; r1++)
        {
            GUILayout.BeginHorizontal();
            for (int r2 = 0; r2 < 3; r2++)
            {
                bool hit = selection == i;
                bool before = hit;
                hit = GUILayout.Toggle(hit, layers[i], "Button");

                if (before != hit)
                    newSelection = i;
                i++;
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        if (newSelection != selection)
        {
            ResetSelectedOffset();
            selectedOffset = OffsetCondition.GetFromVector(offsets[newSelection]);

            selection = newSelection;
        }

        return result;
    }

    // when selecting a location, show the list of conditions for that
    void ShowConditions()
    {
        if (selectedUsageCase == null)
            return;

        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(400));
        {
            GUILayout.Label("Conditions for " + selectedOffset, EditorStyles.largeLabel);

            var condition = selectedUsageCase.conditions.Find(x => x.offsetType == selectedOffset);

            if (condition != null)
            {
                if (condition.rules == null)
                    condition.rules = new List<BlockRule>();

                int i = 0;
                int removeIndex = -1;
                foreach (var rule in condition.rules)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Remove"))
                            removeIndex = i;

                        rule.ruleType = (BlockRule.RuleType)EditorGUILayout.EnumPopup(rule.ruleType);
                        rule.ruleString = EditorGUILayout.TextField(rule.ruleString);
                    }
                    EditorGUILayout.EndHorizontal();

                    i++;
                }

                if (removeIndex != -1)
                {
                    condition.rules.RemoveAt(removeIndex);

                    if (condition.rules.Count == 0)
                        selectedUsageCase.conditions.Remove(condition);

                    OnChange();
                }
            }

            if (GUILayout.Button("Add New Rule"))
            {
                if (condition == null)
                {
                    OffsetCondition newCondition = new OffsetCondition(selectedOffset);
                    selectedUsageCase.conditions.Add(newCondition);

                    condition = newCondition;
                }

                condition.rules.Add(new BlockRule());

                OnChange();
            }
        }
        EditorGUILayout.EndVertical();
    }
}