    #if UNITY_EDITOR
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Experimental.SceneManagement;
    using UnityEngine;

    // Custom editor for BiteableFoodObject. Contains some sliders and bulk inspector actions to speedup setup and test (food items)
    [CustomEditor(typeof(BiteableFoodObject))]
    public class BiteableFoodObjectEditor : Editor
    {
        BiteableFoodObject data;
        GameObject platePreview;
        
        int sequenceProgressPreviewStep = 0;

        void OnEnable()
        {
            this.data = this.target as BiteableFoodObject;
            PrefabStage.prefabSaved += OnPrefabSaved;
            PrefabStage.prefabStageClosing += OnFinishEdit;
        }
        
        void OnDisable()
        {
            PrefabStage.prefabSaved -= OnPrefabSaved;
            PrefabStage.prefabStageClosing -= OnFinishEdit;
        }

        public override void OnInspectorGUI()
        {
            // Draw initial inspector without changes
            this.DrawDefaultInspector();
            this.DrawSetupButton();

            // Early quit if setup went wrong
            if (this.data.FoodParts == null || this.data.FoodParts.Length == 0) return;
            
            // Draw info and preview button for plate
            if (this.data.PlateTemplate != null) this.DrawPlatePreview();
            
            // Draw slider to preview bite states in editor
            this.DrawPreviewSlider();
            
            // Draw button, that will add components for FPV (boilerplate speedup)
            this.DrawComponentSetupButton();
            this.DrawEffectsSetupButton();
            
            if (!Application.isPlaying)
                EditorGUILayout.HelpBox("Enter play mode to unlock animation button", MessageType.Info);
            else
            { 
                // Allow to trigger animation sequence in editor
                this.DrawNextBiteButton();
                this.DrawNexFoodButton();
            }
            this.ShowCurrentState();
        }

        // Allows to preview selected model (temp spawn) for plate object. Plate ref should be kept only to spawn at runtime, no need to pre-spawn it here
        void DrawPlatePreview()
        {
            if (this.platePreview != null)
            {
                EditorGUILayout.HelpBox("Preview will be deleted when selection changes or when you will exit the prefab mode! Also remove it if its not. Do not need to keep plate object here.", MessageType.Info);
                if (GUILayout.Button("Remove plate preview")) // same button with different text if we have preview and not
                {
                    DestroyImmediate(platePreview);
                }
            }
            else if (GUILayout.Button("Preview plate"))
            {
                // Spawn plate as it should do in runtime, but temporary (just for preview)
                this.platePreview = Instantiate(this.data.PlateTemplate, this.data.transform);
                this.platePreview.name = "PLATE PREVIEW. Delete this.";
                
                // Add selection changed event subscription safely when we added preview object to scene
                Selection.selectionChanged -= OnSelectionChanged;
                Selection.selectionChanged += OnSelectionChanged;
            }        
        }

        // Simplifies setup for food with adding interaction components
        void DrawComponentSetupButton()
        {
            void AddInteraction()
            {
                var interaction = this.data.gameObject.GetOrAddComponent<ToolInteraction>();
                interaction.HasTakeAnimation = true;
                interaction.HasPlaceAnimation = true;
                interaction.PlaceRequiresCalories = false;
                interaction.TakeRequiresCalories = false;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add FPV components"))
            {
                AddInteraction();
                this.data.gameObject.GetOrAddComponent<PerformInteraction>();
                EditorUtility.SetDirty(this.data);
            }
            
            if (GUILayout.Button("Add TPV components"))
            {
                AddInteraction();
                EditorUtility.SetDirty(this.data);
            }
            EditorGUILayout.EndHorizontal();
        }
        
        // Simplifies setup for food with adding effects components
        void DrawEffectsSetupButton()
        {
            if (GUILayout.Button("Add Effects component"))
            {
                this.data.gameObject.GetOrAddComponent<FoodEffects>();
                EditorUtility.SetDirty(this.data);
            }
        }

        // Allows to preview each bite state (needed to test part setup right in editor) Uses no animations, just simeple snapshots
        void DrawPreviewSlider()
        {
            if (this.sequenceProgressPreviewStep > this.data.FoodParts.Length)
                this.sequenceProgressPreviewStep = this.data.FoodParts.Length;

            var newSequenceProgress = EditorGUILayout.IntSlider("Preview Sequence", this.sequenceProgressPreviewStep, 0, this.data.FoodParts.Length);
            if (newSequenceProgress != this.sequenceProgressPreviewStep)
            {
                this.sequenceProgressPreviewStep = newSequenceProgress;
                this.OnSequenceProgressChanged();
            }
        }

        void DrawNextBiteButton()
        {
            if (GUILayout.Button("Next Bite"))
            {
                var nextIndex = this.sequenceProgressPreviewStep + 1;
                if (nextIndex > this.data.FoodParts.Length) nextIndex = 0;
                this.data.BiteSequence.ProcessBiteAtIndex(this.data.FoodParts, nextIndex);
                this.sequenceProgressPreviewStep = nextIndex;
            }
        }
        
        // Tries to pool food parts automatically and put them in same order as in fbx file. Should help in 95% of setups
        void DrawSetupButton()
        {
            if (GUILayout.Button("Try auto-setup parts"))
            {
                if (this.data.transform.childCount == 0) return;   //early quit of no child objects
                var root = this.data.transform.GetChild(0); // by default we have model under prefab root, and only model then contains parts, so we assume its a root
                if (this.data.transform.childCount > 1) root = this.data.transform; // if we have more objects under prefab root then we can assume parts are directly under it
                if (root.childCount == 0) return;
                this.data.FoodParts = new GameObject[root.childCount];
                for (var i = 0; i < root.childCount; i++) this.data.FoodParts[i] = root.GetChild(i).gameObject;
                EditorUtility.SetDirty(this.data);
            }
        }
        
        // This one is only for food test scene to switch between active game objects faster ion one click
        void DrawNexFoodButton()
        {
            if (GUILayout.Button("Next Food"))
            {
                var mainObject = this.data.transform.parent;
                var objects = new List<Transform>();
                for (var i = 0; i < mainObject.childCount; i++) objects.Add(mainObject.GetChild(i));
                
                var activeFood = objects.FirstOrDefault(x => x.gameObject.activeInHierarchy);
                var activeIndex = 0;
                if (activeFood != null) activeIndex = objects.IndexOf(activeFood);
                
                foreach (var obj in objects) obj.gameObject.SetActive(false);
                activeIndex++;
                if (activeIndex >= objects.Count) activeIndex = 0;

                var nextObject = objects[activeIndex].gameObject;
                nextObject.SetActive(true);
                Selection.activeGameObject = nextObject;
            }
        }

        // Shows warnings if the component is set up incorrectly.
        void ShowCurrentState()
        {
            var tool = this.data.GetComponent<ToolInteraction>();
            if (tool == null) return;
            if (tool.CustomAnimset == null && this.data.DefaultAnimationOverride == PredefinedFoodAnimations.Custom)
                EditorGUILayout.HelpBox("If [DefaultAnimationOverride = Custom] make sure to give [ToolInteraction.CustomAnimset] a value", MessageType.Error);

            if (tool.CustomAnimset != null && this.data.DefaultAnimationOverride != PredefinedFoodAnimations.Custom)
                EditorGUILayout.HelpBox("[ToolInteraction.CustomAnimset] will be prioritized over value set at [DefaultAnimationOverride]", MessageType.Warning);
        }

        // Callback for slider (bite preview states)
        void OnSequenceProgressChanged() => this.data.BiteSequence.ProcessBiteAtIndex(this.data.FoodParts, this.sequenceProgressPreviewStep);

        // list of specific events that helps to build more accurate editor with previews, etc for food
        void OnFinishEdit(PrefabStage obj)
        {
            if (this.platePreview != null) DestroyImmediate(platePreview);
        }

        void OnSelectionChanged()
        {
            if (this.platePreview != null) DestroyImmediate(platePreview);
            Selection.selectionChanged -= OnSelectionChanged;
        }

        void OnPrefabSaved(GameObject prefab)
        {
            if (this.platePreview != null) DestroyImmediate(platePreview);
        }
    }
    #endif
