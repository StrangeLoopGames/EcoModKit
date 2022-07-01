#if UNITY_EDITOR
using System.Collections.Generic;
using Eco.Shared.Utils;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;

// Custom editor for CustomAnimsetOverride. Contains some buttons to prefill overrides woith data
[CustomEditor(typeof(CustomAnimsetOverride))]
public class CustomAnimsetOverrideEditor : Editor
{
    CustomAnimsetOverride data;
    AnimatorController animatorToGetStates;
    SerializedProperty OverrideStates;
    ReorderableList list;

    void OnEnable()
    {
        this.data = this.target as CustomAnimsetOverride;
        this.SetupOverrideStatesProperty(); // Draw override states with custom drawers
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Here overrides for animator are defined\n" +
                                "(State) on the left is for source state/animation clip name that will be changed\n" +
                                "(Clip) on the right is target animation that will be used for that state\n" +
                                "Preset button below can be used to predefine data.\n" +
                                "New states can be easily added on top/from scratch if needed\n", MessageType.Info);
        
        serializedObject.Update();
        list.DoLayoutList();
        
        // Clears all overrides
        this.DrawResetButton();

        // Food presets
        EditorGUILayout.BeginHorizontal();
        this.DrawStatesFoodButton("FPV");
        this.DrawStatesFoodButton("TPV");
        EditorGUILayout.EndHorizontal();
        
        this.DrawStatesFPVBasicButton();

        // Draw special field to get states from target animator. Just for
        this.DrawStateExtractorField();
        
        serializedObject.ApplyModifiedProperties();
    }

    void DrawStatesFPVBasicButton()
    {
        if (GUILayout.Button("Add FPV Locomotion"))
        {
            this.data.OverrideStates.Add(new AnimsetOverrideItem("FPV_Hands_Idle"));
            this.data.OverrideStates.Add(new AnimsetOverrideItem("FPV_Hands_Walk"));
            this.data.OverrideStates.Add(new AnimsetOverrideItem("FPV_Hands_Jog"));
            this.data.OverrideStates.Add(new AnimsetOverrideItem("FPV_Hands_Run"));
            this.data.OverrideStates.Add(new AnimsetOverrideItem("FPV_Hands_Jump"));
            this.data.OverrideStates.Add(new AnimsetOverrideItem("FPV_Hands_Climb"));
            this.data.OverrideStates.Add(new AnimsetOverrideItem("FPV_Hands_Swimming"));
            this.data.OverrideStates.Add(new AnimsetOverrideItem("FPV_Hands_Stone_Axe_Work"));
        }
    }

    void DrawStateExtractorField()
    {
        EditorGUILayout.BeginHorizontal();
        animatorToGetStates = EditorGUILayout.ObjectField("Animator to get states (optional):", animatorToGetStates, typeof(AnimatorController), false) as AnimatorController;
        if (GUILayout.Button("Extract"))
        {
            if (animatorToGetStates != null)
            {
                this.data.OverrideStates = new List<AnimsetOverrideItem>();
                foreach (var clip in animatorToGetStates.animationClips)
                    this.data.OverrideStates.Add(new AnimsetOverrideItem(clip.name));
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    void DrawStatesFoodButton(string prefix)
    {
        if (GUILayout.Button($"Food: {prefix}"))
        {
            this.data.OverrideStates = new List<AnimsetOverrideItem>()
            {
                new AnimsetOverrideItem($"{prefix}_Eat_Idle_Start"),
                new AnimsetOverrideItem($"{prefix}_Eat_Idle"),
                new AnimsetOverrideItem($"{prefix}_Eat_Idle_Exit"),
                new AnimsetOverrideItem($"{prefix}_Eat_Loop_Start"),
                new AnimsetOverrideItem($"{prefix}_Eat_Loop"),
                new AnimsetOverrideItem($"{prefix}_Eat_Loop_Exit"),
            };
        }
    }

    void DrawResetButton()
    {
        if (GUILayout.Button("Reset states")) this.data.OverrideStates.Clear();
    }
    
    // Drawer for OverrideStates in CustomAnimset, allows to make it the same as AnimatorOverrideController built-in unity
    void SetupOverrideStatesProperty()
    {
        this.OverrideStates = serializedObject.FindProperty("OverrideStates");
        
        list = new ReorderableList(serializedObject, OverrideStates)
        {
            displayAdd = true,
            displayRemove = true,
            draggable = true,

            drawHeaderCallback = rect => { EditorGUI.LabelField(rect, "Animation override set".Capitalize(), EditorStyles.largeLabel); },

            drawElementCallback = (rect, index, a, h) =>
            {
                // get outer element
                var element = OverrideStates.GetArrayElementAtIndex(index);

                var state = element.FindPropertyRelative("State");
                var animation = element.FindPropertyRelative("Clip");
                var hSpace = 25f;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width/2f - hSpace, rect.height), state, new GUIContent(""));
                EditorGUI.PropertyField(new Rect(rect.width/2f + hSpace * 2f, rect.y, rect.width/2f - hSpace, rect.height), animation, new GUIContent(""));
            },

            elementHeightCallback = index => EditorGUIUtility.singleLineHeight
        };
    }
}
#endif