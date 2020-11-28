// Copyright (c) 2009-2012 David Koontz
// Please direct any bugs/comments/suggestions to david@koontzfamily.org
//
// Thanks to Gabriel Gheorghiu (gabison@gmail.com) for his code submission 
// that lead to the integration with the iTween visual path editor.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Linq;

[CustomEditor(typeof(iTweenEvent))]
public class iTweenEventDataEditor : Editor {
	List<string> trueFalseOptions = new List<string>() {"True", "False"};
	Dictionary<string, object> values;
	Dictionary<string, bool> propertiesEnabled = new Dictionary<string, bool>();
	iTweenEvent.TweenType previousType;
	
	[MenuItem("Component/iTween/iTweenEvent")]
    static void AddiTweenEvent () {
		if(Selection.activeGameObject != null) {
			Selection.activeGameObject.AddComponent(typeof(iTweenEvent));
		}
    }
	
	[MenuItem("Component/iTween/Prepare Visual Editor for Javascript Usage")]
	static void CopyFilesForJavascriptUsage() {
		if(Directory.Exists(Application.dataPath + "/iTweenEditor/Helper Classes")) {
			if(!Directory.Exists(Application.dataPath + "/Plugins")) {
				Directory.CreateDirectory(Application.dataPath + "/Plugins");
			}
			
			if(!Directory.Exists(Application.dataPath + "/Plugins/iTweenEditor")) {
				Directory.CreateDirectory(Application.dataPath + "/Plugins/iTweenEditor");
			}
			FileUtil.MoveFileOrDirectory(Application.dataPath + "/iTweenEditor/Helper Classes", Application.dataPath + "/Plugins/iTweenEditor/Helper Classes");
			FileUtil.MoveFileOrDirectory(Application.dataPath + "/iTweenEditor/iTweenEvent.cs", Application.dataPath + "/Plugins/iTweenEvent.cs");
			FileUtil.MoveFileOrDirectory(Application.dataPath + "/iTweenEditor/iTween.cs", Application.dataPath + "/Plugins/iTween.cs");
			FileUtil.MoveFileOrDirectory(Application.dataPath + "/iTweenEditor/iTweenPath.cs", Application.dataPath + "/Plugins/iTweenPath.cs");
			
			AssetDatabase.Refresh();
		}
		else {
			EditorUtility.DisplayDialog("Can't move files", "Your files have already been moved", "Ok");
		}
	}
	
	[MenuItem("Component/iTween/Donate to support the Visual Editor")]
	static void Donate() {
		Application.OpenURL("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=WD3GQ6HHD257C");
	}
	
	public void OnEnable() {
		var evt = (iTweenEvent)target;
		foreach(var key in EventParamMappings.mappings[evt.type].Keys) {
			propertiesEnabled[key] = false;
		}
		previousType = evt.type;
		
		if(!Directory.Exists(Application.dataPath + "/Gizmos")) {
			Directory.CreateDirectory(Application.dataPath + "/Gizmos");
		}
			
		if(!File.Exists(Application.dataPath + "/Gizmos/iTweenIcon.tif")) {
			FileUtil.CopyFileOrDirectory(Application.dataPath + "/iTweenEditor/Gizmos/iTweenIcon.tif", Application.dataPath + "/Gizmos/iTweenIcon.tif");
		}
	}
	
	public override void OnInspectorGUI() {
		var evt = (iTweenEvent)target;
		values = evt.Values;
		var keys = values.Keys.ToArray();
		
		foreach(var key in keys) {
			propertiesEnabled[key] = true;
			if(typeof(Vector3OrTransform) == EventParamMappings.mappings[evt.type][key]) {
				var val = new Vector3OrTransform();
				
				if(null == values[key] || typeof(Transform) == values[key].GetType()) {
					if(null == values[key]) {
						val.transform = null;
					}
					else {
						val.transform = (Transform)values[key];
					}
					val.selected = Vector3OrTransform.transformSelected;
				}
				else if(typeof(Vector3) == values[key].GetType()) {
					val.vector = (Vector3)values[key];
					val.selected = Vector3OrTransform.vector3Selected;
				}
				
				values[key] = val;
			}
			if(typeof(Vector3OrTransformArray) == EventParamMappings.mappings[evt.type][key]) {
				var val = new Vector3OrTransformArray();
				
				if(null == values[key] || typeof(Transform[]) == values[key].GetType()) {
					if(null == values[key]) {
						val.transformArray = null;
					}
					else {
						val.transformArray = (Transform[])values[key];
					}
					val.selected = Vector3OrTransformArray.transformSelected;
				}
				else if(typeof(Vector3[]) == values[key].GetType()) {
					val.vectorArray = (Vector3[])values[key];
					val.selected = Vector3OrTransformArray.vector3Selected;
				}
				else if(typeof(string) == values[key].GetType()) {
					val.pathName = (string)values[key];
					val.selected = Vector3OrTransformArray.iTweenPathSelected;
				}
				
				values[key] = val;
			}
		}
		
		GUILayout.Label(string.Format("iTween Event Editor v{0}", iTweenEvent.VERSION));
		EditorGUILayout.Separator();
 		
		GUILayout.BeginHorizontal();
			GUILayout.Label("Name");
			evt.tweenName = EditorGUILayout.TextField(evt.tweenName);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
			evt.showIconInInspector = GUILayout.Toggle(evt.showIconInInspector, " Show Icon In Scene");
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
			evt.playAutomatically = GUILayout.Toggle(evt.playAutomatically, " Play Automatically");
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Initial Start Delay (delay begins once the iTweenEvent is played)");
		evt.delay = EditorGUILayout.FloatField(evt.delay);
		GUILayout.EndHorizontal();
		
		EditorGUILayout.Separator();
		
		GUILayout.BeginHorizontal();
			GUILayout.Label("Event Type");
			evt.type = (iTweenEvent.TweenType)EditorGUILayout.EnumPopup(evt.type);
		GUILayout.EndHorizontal();
		
		if(evt.type != previousType) {
			foreach(var key in EventParamMappings.mappings[evt.type].Keys) {
				propertiesEnabled[key] = false;
			}
			evt.Values = new Dictionary<string, object>();
			previousType = evt.type;
			return;
		}
		
		var properties = EventParamMappings.mappings[evt.type];
		foreach(var pair in properties) {
			var key = pair.Key;
			
			GUILayout.BeginHorizontal();
			
			if(EditorGUILayout.BeginToggleGroup(key, propertiesEnabled[key])) {
				propertiesEnabled[key] = true;
				
				GUILayout.BeginVertical();
			
				if(typeof(string) == pair.Value) {
					values[key] = EditorGUILayout.TextField(values.ContainsKey(key) ? (string)values[key] : "");
				}
				else if(typeof(float) == pair.Value) {
					values[key] = EditorGUILayout.FloatField(values.ContainsKey(key) ? (float)values[key] : 0);
				}
				else if(typeof(int) == pair.Value) {
					values[key] = EditorGUILayout.IntField(values.ContainsKey(key) ? (int)values[key] : 0);
				}
				else if(typeof(bool) == pair.Value) {
					GUILayout.BeginHorizontal();
					var currentValueString = (values.ContainsKey(key) ? (bool)values[key] : false).ToString();
					currentValueString = currentValueString.Substring(0, 1).ToUpper() + currentValueString.Substring(1);					
					var index = EditorGUILayout.Popup(trueFalseOptions.IndexOf(currentValueString), trueFalseOptions.ToArray());
					GUILayout.EndHorizontal();
					values[key] = bool.Parse(trueFalseOptions[index]);
				}
				else if(typeof(GameObject) == pair.Value) {
					values[key] = EditorGUILayout.ObjectField(values.ContainsKey(key) ? (GameObject)values[key] : null, typeof(GameObject), true);
				}
				else if(typeof(Vector3) == pair.Value) {
					values[key] = EditorGUILayout.Vector3Field("", values.ContainsKey(key) ? (Vector3)values[key] : Vector3.zero);
				}
				else if(typeof(Vector3OrTransform) == pair.Value) {
					if(!values.ContainsKey(key)) {
						values[key] = new Vector3OrTransform();
					}
					var val = (Vector3OrTransform)values[key];
					
					val.selected = GUILayout.SelectionGrid(val.selected, Vector3OrTransform.choices, 2);
	
					if(Vector3OrTransform.vector3Selected == val.selected) {
						val.vector = EditorGUILayout.Vector3Field("", val.vector);
					}
					else {
						val.transform = (Transform)EditorGUILayout.ObjectField(val.transform, typeof(Transform), true);
					}
					values[key] = val;
				}
				else if(typeof(Vector3OrTransformArray) == pair.Value) {
					if(!values.ContainsKey(key)) {
						values[key] = new Vector3OrTransformArray();
					}
					var val = (Vector3OrTransformArray)values[key];
					val.selected = GUILayout.SelectionGrid(val.selected, Vector3OrTransformArray.choices, Vector3OrTransformArray.choices.Length);

					if(Vector3OrTransformArray.vector3Selected == val.selected) {
						if(null == val.vectorArray) {
							val.vectorArray = new Vector3[0];
						}
						var elements = val.vectorArray.Length;
						GUILayout.BeginHorizontal();
							GUILayout.Label("Number of points");
							elements = EditorGUILayout.IntField(elements);
						GUILayout.EndHorizontal();
						if(elements != val.vectorArray.Length) {
							var resizedArray = new Vector3[elements];
							val.vectorArray.CopyTo(resizedArray, 0);
							val.vectorArray = resizedArray;
						}
						for(var i = 0; i < val.vectorArray.Length; ++i) {
							val.vectorArray[i] = EditorGUILayout.Vector3Field("", val.vectorArray[i]);
						}
					}
					else if(Vector3OrTransformArray.transformSelected == val.selected) {
						if(null == val.transformArray) {
							val.transformArray = new Transform[0];
						}
						var elements = val.transformArray.Length;
						GUILayout.BeginHorizontal();
							GUILayout.Label("Number of points");
							elements = EditorGUILayout.IntField(elements);
						GUILayout.EndHorizontal();
						if(elements != val.transformArray.Length) {
							var resizedArray = new Transform[elements];
							val.transformArray.CopyTo(resizedArray, 0);
							val.transformArray = resizedArray;
						}
						for(var i = 0; i < val.transformArray.Length; ++i) {
							val.transformArray[i] = (Transform)EditorGUILayout.ObjectField(val.transformArray[i], typeof(Transform), true);
						}
					}
					else if(Vector3OrTransformArray.iTweenPathSelected == val.selected) {
						var index = 0;
						var paths = (GameObject.FindObjectsOfType(typeof(iTweenPath)) as iTweenPath[]);
						if(0 == paths.Length) {
							val.pathName = "";
							GUILayout.Label("No paths are defined");
						}
						else {
							for(var i = 0; i < paths.Length; ++i) {
								if(paths[i].pathName == val.pathName) {
									index = i;
								}
							}
							index = EditorGUILayout.Popup(index, (GameObject.FindObjectsOfType(typeof(iTweenPath)) as iTweenPath[]).Select(path => path.pathName).ToArray());	
							
							val.pathName = paths[index].pathName;
						}
					}
					values[key] = val;
				}
				else if(typeof(iTween.LoopType) == pair.Value) {
					values[key] = EditorGUILayout.EnumPopup(values.ContainsKey(key) ? (iTween.LoopType)values[key] : iTween.LoopType.none);
				}
				else if(typeof(iTween.EaseType) == pair.Value) {
					values[key] = EditorGUILayout.EnumPopup(values.ContainsKey(key) ? (iTween.EaseType)values[key] : iTween.EaseType.linear);
				}
				else if(typeof(AudioSource) == pair.Value) {
					values[key] = (AudioSource)EditorGUILayout.ObjectField(values.ContainsKey(key) ? (AudioSource)values[key] : null, typeof(AudioSource), true);
				}
				else if(typeof(AudioClip) == pair.Value) {
					values[key] = (AudioClip)EditorGUILayout.ObjectField(values.ContainsKey(key) ? (AudioClip)values[key] : null, typeof(AudioClip), true);
				}
				else if(typeof(Color) == pair.Value) {
					values[key] = EditorGUILayout.ColorField(values.ContainsKey(key) ? (Color)values[key] : Color.white);
				}
				else if(typeof(Space) == pair.Value) {
					values[key] = EditorGUILayout.EnumPopup(values.ContainsKey(key) ? (Space)values[key] : Space.Self);
				}
				
				GUILayout.EndVertical();
			}
			else {
				propertiesEnabled[key] = false;
				values.Remove(key);
			}
			
			EditorGUILayout.EndToggleGroup();
			GUILayout.EndHorizontal();
			EditorGUILayout.Separator();
		}
		
		keys = values.Keys.ToArray();
		
		foreach(var key in keys) {
			if(values[key] != null && values[key].GetType() == typeof(Vector3OrTransform)) {
				var val = (Vector3OrTransform)values[key];
				if(Vector3OrTransform.vector3Selected == val.selected) {
					values[key] = val.vector;
				}
				else {
					values[key] = val.transform;
				}
			}
			else if(values[key] != null && values[key].GetType() == typeof(Vector3OrTransformArray)) {
				var val = (Vector3OrTransformArray)values[key];
				if(Vector3OrTransformArray.vector3Selected == val.selected) {
					values[key] = val.vectorArray;
				}
				else if(Vector3OrTransformArray.transformSelected == val.selected) {
					values[key] = val.transformArray;
				}
				else if(Vector3OrTransformArray.iTweenPathSelected == val.selected) {
					values[key] = val.pathName;
				}
			}
		}
		
		evt.Values = values;
		previousType = evt.type;
	}
}