// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public static class EditorGUIUtils {
	
	public static Color SELECT_COLOR {
		get {
			return new Color( 61/255f, 128/255f, 223/255f );
		}
	}
	
	public static void DrawRect(Rect rect, Color color) {
		Vector3 a = new Vector3(rect.xMin, rect.yMin, 0);
		Vector3 b = new Vector3(rect.xMax, rect.yMin, 0);
		Vector3 c = new Vector3(rect.xMax, rect.yMax, 0);
		Vector3 d = new Vector3(rect.xMin, rect.yMax, 0);
		
		Handles.color = color;
		Handles.DrawLine(a, b);
		Handles.DrawLine(b, c);
		Handles.DrawLine(c, d);
		Handles.DrawLine(d, a);
	}
	
	public static void FillRect(Rect rect, Color color) {
		Color oldColor = GUI.color;
		GUI.color = color;
		GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
		GUI.color = oldColor;
	}
	
	public static int Popup(string label, int selected, object[] items) {
		string[] strings = new string[items.Length];
		for(int i=0; i<items.Length; i++) {
			if(items[i] != null) strings[i] = items[i].ToString();
		}
		return EditorGUILayout.Popup(label, selected, strings);
	}
	
	public static Enum Toolbar(Enum selected) {
		string[] names = Enum.GetNames(selected.GetType());
		int index = Array.IndexOf<string>(names, Enum.GetName(selected.GetType(), selected));
		index = GUILayout.Toolbar(index, names);
		return (Enum) Enum.Parse(selected.GetType(), names[index]);
	}
	
	public static int DrawList(int selected, IList list) {
		float labelHeight = GUI.skin.label.CalcHeight( GUIContent.none, 0 );
		Rect rect = GUILayoutUtility.GetRect(0, labelHeight*list.Count, GUILayout.ExpandWidth(true));
		Rect[] rects = GUIUtils.Separate(rect, 1, list.Count);
		for(int i=0; i<list.Count; i++) {
			Rect position = rects[i];
			object item = list[i];
			
			if(i == selected) FillRect(position, SELECT_COLOR);
			string name = item != null ? item.ToString() : "Null";
			GUI.Label(position, name);
		}
		
		
		GUI.BeginGroup(rect);
		if(Event.current.type == EventType.MouseDown) {
			float mouseY = Event.current.mousePosition.y;
			selected = Mathf.FloorToInt( mouseY / labelHeight );
			if(selected < 0 || selected >= list.Count) selected = -1;
			Event.current.Use();
		}
		GUI.EndGroup();
		
		return selected;
	}

    public static void Draw2Arrays<A, B>(ref A[] arrayA, ref B[] arrayB, Func<A, A> drawItemA, Func<B, B> drawItemB)
    {
        int newLength = EditorGUILayout.IntField("Length", arrayA.Length);
        if (newLength != arrayA.Length)
        {
            SetArrayLength(ref arrayA, newLength);
            SetArrayLength(ref arrayB, newLength);
        }

        for (int i = 0; i < arrayA.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            arrayA[i] = drawItemA(arrayA[i]);
            arrayB[i] = drawItemB(arrayB[i]);
            EditorGUILayout.EndHorizontal();
        }
    }

    public static T GetStateObject<T>(int controlID) {
		return (T) GUIUtility.GetStateObject(typeof(T), controlID);
	}
    
    public static void SetArrayLength<T>(ref T[] array, int length)
    {
        T[] newArray = new T[length];
        Array.Copy(array, newArray, Math.Min(array.Length, length));
        array = newArray;
    }
}

class Container<T> where T : struct {
	public T value;
	
	public Container() {
	}
	public Container(T value) {
		this.value = value;
	}
		
	public static implicit operator T(Container<T> c) {
		return c.value;
	}
}