// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;
using UnityEditor;

public class BlockSetViewer 
{
    public static Block SelectionGrid(BlockSet blockSet, Block selected, params GUILayoutOption[] options) 
    {
		Container<Vector2> scroll = EditorGUIUtils.GetStateObject<Container<Vector2>>(blockSet.GetHashCode());
		
		scroll.value = GUILayout.BeginScrollView(scroll, options);
        selected = SelectionGrid(blockSet, selected);
		GUILayout.EndScrollView();

        return selected;
	}

    private static Block SelectionGrid(BlockSet blockSet, Block selected)
    {
		int xCount = Mathf.FloorToInt( Screen.width / 66f );
		int yCount = Mathf.CeilToInt( (float) blockSet.Blocks.Count / xCount );
		
		Rect rect = GUILayoutUtility.GetAspectRect((float)xCount/yCount);
		float labelHeight = GUI.skin.label.CalcHeight(GUIContent.none, 0);
		GUILayout.Space(labelHeight * yCount);
		rect.height += labelHeight * yCount;
		
		Rect[] rects = GUIUtils.Separate(rect, xCount, yCount);
        int i = 0;
        foreach (Block block in blockSet.Blocks)
        {
			Rect position = rects[i];
			position.xMin += 2;
			position.yMin += 2;
				
			bool isSelected = DrawItem(position, block, selected == block, i);
            if (isSelected)
                selected = block;

            i++;
		}
		
		return selected;
	}
	
	private static bool DrawItem(Rect position, Block block, bool selected, int index)
    {
        bool isClicked = false;
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && position.Contains(Event.current.mousePosition))
            isClicked = true;

        Rect texturePosition = position;
		texturePosition.height = texturePosition.width;
		Rect labelPosition = position;
		labelPosition.yMin += texturePosition.height;
        
        Editor.CreateCachedEditor(block.Material, typeof(MaterialEditor), ref block.Editor);
        if (block.Editor != null)
            block.Editor.DrawPreview(texturePosition);

        if (selected)
            EditorGUIUtils.FillRect(labelPosition, new Color(61 / 255f, 128 / 255f, 223 / 255f));

        var style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        GUI.Label(labelPosition, block.Name, style);

		return isClicked;
	}	
}
