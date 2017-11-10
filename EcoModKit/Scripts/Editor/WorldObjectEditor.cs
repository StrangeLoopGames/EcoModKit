// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Assets.Editor
{
    using System;
    using System.Collections.Generic;
    using Scripts.WorldObjects;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(WorldObject))]
    public class WorldObjectEditor : Editor
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
            if(worldObject.hasOccupancy = EditorGUILayout.Toggle("Has Occupancy", worldObject.hasOccupancy))
            {
                worldObject.occupancyOffset = EditorGUILayout.Vector3Field("Occupancy Offset", worldObject.occupancyOffset);
                if (worldObject.overrideOccupancy = EditorGUILayout.Toggle("Override Occupancy", worldObject.overrideOccupancy))
                    worldObject.size = EditorGUILayout.Vector3Field("Override Size", worldObject.size);
            }


            this.DrawPropertySection("Initially Enabled", ref showInitiallyEnabled, "OnInitialState", "OnInitiallyEnabled", "OnInitiallyDisabled");
            this.DrawPropertySection("Enabled", ref showEnabled, "OnEnabledChanged", "OnEnabled", "OnDisabled");
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

            if (EditorApplication.isPlaying)
            {
                var oldColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.yellow;
                if (GUILayout.Button("Debug Server Next Tick"))
                    worldObject.view.RPC("DebugNextTick");
                GUI.backgroundColor = oldColor;
            }

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
    }
    
    [CustomEditor(typeof(Sign))]
    public class SignEditor : WorldObjectEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SignText"));
            base.OnInspectorGUI();
        }
    }

    [CustomEditor(typeof(DoorObject))]
    public class DoorEditor : WorldObjectEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("hingeTransform"));
            base.OnInspectorGUI();
        }
    }

    [CustomEditor(typeof(Vehicle))]
    public class VehicleEditor : WorldObjectEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("frontWheels"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("playerCollider"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rotatePlayer"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("syncWheelPos"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("seatedAnimController"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LicensePlate"));
            base.OnInspectorGUI();
        }
    }

    [CustomEditor(typeof(SpaceStation))]
    public class SpaceStationEditor : WorldObjectEditor { }

    [CustomEditor(typeof(LaserObject))]
    public class LaserEditor : WorldObjectEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Target"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LaserBeamPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LaserBeamChargingPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SpinTransform"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TiltTransform"));
            base.OnInspectorGUI();
        }
    }
}
