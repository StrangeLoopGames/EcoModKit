// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldObject))]
public partial class WorldObjectEditor : Editor
{
    private static bool showInitiallyEnabled = false;
    private static bool showEnabled = false;
    private static bool showOperating = false;
    private static bool showUsing = false;
    private static Dictionary<string, bool> showState = new Dictionary<string, bool>();

    private WorldObject worldObject;

    void OnEnable()
    {
        worldObject = (WorldObject)target;
        foreach (string state in worldObject.States)
            if (!showState.ContainsKey(state))
                showState[state] = false;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OverrideInitEnabled"));
        if (worldObject.OverrideInitEnabled)
            this.DrawPropertySection("Initial Enabled Overrides", ref showInitiallyEnabled, "OnInitialState", "OnInitiallyEnabled", "OnInitiallyDisabled");
        this.DrawPropertySection("Enabled", ref showEnabled, "OnEnabledChanged", "OnEnabled", "OnDisabled");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OverrideInitOperating"));
        if (worldObject.OverrideInitOperating)
            this.DrawPropertySection("Initial Operating Overrides", ref showInitiallyEnabled, "OnInitialOperatingState", "OnInitiallyEnabledOperating", "OnInitiallyDisabledOperating");
        this.DrawPropertySection("Operating", ref showOperating, "OnOperatingChanged", "OnEnableOperating", "OnDisableOperating");
        this.DrawPropertySection("Using", ref showUsing, "OnUsingChanged", "OnEnableUsing", "OnDisableUsing");

        #region State Toggle Events
        this.DrawDictionary(
            "Boolean State Events",
            ref worldObject.States,
            (int newLength) =>
                {
                    EditorGUIUtils.SetArrayLength(ref worldObject.States, newLength);
                    EditorGUIUtils.SetArrayLength(ref worldObject.OnStateChangedEvents, newLength);
                    EditorGUIUtils.SetArrayLength(ref worldObject.OnStateEnabledEvents, newLength);
                    EditorGUIUtils.SetArrayLength(ref worldObject.OnStateDisabledEvents, newLength);
                }, 
            (int i) =>
            {
                DrawArrayElement("Event Name", "States", i);
                DrawArrayElement("Changed", "OnStateChangedEvents", i);
                DrawArrayElement("Enabled", "OnStateEnabledEvents", i);
                DrawArrayElement("Disabled", "OnStateDisabledEvents", i);
            });
        #endregion

        #region String State Events
        this.DrawDictionary(
            "String State Events",
            ref worldObject.StringStates,
            (int newLength) =>
            {
                EditorGUIUtils.SetArrayLength(ref worldObject.StringStates, newLength);
                EditorGUIUtils.SetArrayLength(ref worldObject.OnStringStateChanged, newLength);
            },
            (int i) =>
            {
                DrawArrayElement("Event Name", "StringStates", i);
                DrawArrayElement("Handler", "OnStringStateChanged", i);
            });
        #endregion

        #region Float State Events
        this.DrawDictionary(
            "Float State Events",
            ref worldObject.FloatStates,
            (int newLength) =>
            {
                EditorGUIUtils.SetArrayLength(ref worldObject.FloatStates, newLength);
                EditorGUIUtils.SetArrayLength(ref worldObject.OnFloatStateChanged, newLength);
            },
            (int i) =>
            {
                DrawArrayElement("Event Name", "FloatStates", i);
                DrawArrayElement("Handler", "OnFloatStateChanged", i);
            });
        #endregion

        #region One Off Events
        this.DrawDictionary(
            "Stateless Events",
            ref worldObject.Events,
            (int newLength) =>
            {
                EditorGUIUtils.SetArrayLength(ref worldObject.Events, newLength);
                EditorGUIUtils.SetArrayLength(ref worldObject.EventHandlers, newLength);
            },
            (int i) =>
            {
                DrawArrayElement("Event Name", "Events", i);
                DrawArrayElement("Handler", "EventHandlers", i);
            });
        #endregion

        worldObject.interactable = EditorGUILayout.Toggle("Interactable", worldObject.interactable);

        if (worldObject.hasOccupancy = EditorGUILayout.Toggle("Has Occupancy", worldObject.hasOccupancy))
        {
            worldObject.occupancyOffset = EditorGUILayout.Vector3Field("Occupancy Offset", worldObject.occupancyOffset);
            if (worldObject.overrideOccupancy = EditorGUILayout.Toggle("Override Occupancy", worldObject.overrideOccupancy))
                worldObject.size = EditorGUILayout.Vector3Field("Override Size", worldObject.size);
        }

#if ECO_DEV
        ShowInternalDebugTools();
#else
        Shader.EnableKeyword("NO_CURVE"); // mod kit doesn't have curve helper
#endif


        serializedObject.ApplyModifiedProperties();
    }

    private void DrawDictionary(string title, ref string[] keys, Action<int> setArrayLengths, Action<int> drawArrayValueForIndex)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(title);

        int newLength = keys.Length;
        if (GUILayout.Button("Add", GUILayout.Width(80)))
            newLength++;
        if (keys.Length > 0 && GUILayout.Button("Remove", GUILayout.Width(80)))
            newLength--;

        GUILayout.EndHorizontal();

        if (newLength != keys.Length)
        {
            EditorGUIUtils.SetArrayLength(ref keys, newLength);
            setArrayLengths(newLength);
        }

        for (int i = 0; i < keys.Length; i++)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;
            {
                if (keys[i] == null)
                    keys[i] = string.Empty;

                string key = keys[i];
                if (!showState.ContainsKey(key))
                    showState[key] = true;

                showState[key] = EditorGUILayout.Foldout(showState[key], key);
                if (showState[key])
                {
                    drawArrayValueForIndex(i);
                }
            }
            EditorGUI.indentLevel--;
            GUILayout.EndVertical();
        }
    }

    private void DrawPropertySection(string title, ref bool isOpen, params string[] properties)
    {
        GUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;
        {
            isOpen = EditorGUILayout.Foldout(isOpen, title);
            if (isOpen)
                foreach (string property in properties)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(property));
        }
        EditorGUI.indentLevel--;
        GUILayout.EndVertical();
    }

    private void DrawArrayElement(string label, string arrayName, int index)
    {
        SerializedProperty property = serializedObject.FindProperty(string.Format("{0}.Array.data[{1}]", arrayName, index));
        if (property != null)
            EditorGUILayout.PropertyField(property, new GUIContent(label));
    }

#if ECO_DEV
    private void ShowInternalDebugTools()
    {
        if (worldObject.hasOccupancy)
        {
            if (GUILayout.Button("Update Occupancy"))
                BuildWorldObjectOccupancy.UpdateOccupancy((WorldObject)target);
        }

        if (EditorApplication.isPlaying)
        {
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("Debug Server Next Tick"))
                worldObject.view.RPC("DebugNextTick");
            if (GUILayout.Button("Rebind Animation Events"))
                worldObject.BindEvents();
            GUI.backgroundColor = oldColor;
        }

        if (GUILayout.Button("UnCurve"))
            Shader.EnableKeyword("NO_CURVE");
    }
#endif
}