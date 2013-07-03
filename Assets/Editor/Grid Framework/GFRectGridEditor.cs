using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(GFRectGrid))]
public class GFRectGridEditor : Editor {
	
	private static bool showDrawSettings = true;
	private static bool showRenderRangeSettings = false;

	public override void OnInspectorGUI(){
		GFRectGrid rg = target as GFRectGrid;
		
		rg.spacing = Vector3.Max(EditorGUILayout.Vector3Field("Spacing", rg.spacing), rg.minimumSpacing);
		rg.minimumSpacing = Vector3.Max(EditorGUILayout.Vector3Field("Minimum Spacing", rg.minimumSpacing), new Vector3(0.1f, 0.1f, 0.1f));
		
		rg.size = Vector3.Max(EditorGUILayout.Vector3Field("Size", rg.size), Vector3.zero);
		
		EditorGUILayout.Space();
		
		GUILayout.Label("Axis Colors");
		
		EditorGUILayout.BeginHorizontal();
		++EditorGUI.indentLevel;
		rg.axisColors.x = EditorGUILayout.ColorField(rg.axisColors.x);
		rg.axisColors.y = EditorGUILayout.ColorField(rg.axisColors.y);
		rg.axisColors.z = EditorGUILayout.ColorField(rg.axisColors.z);
		--EditorGUI.indentLevel;
		EditorGUILayout.EndHorizontal();
		
		rg.vertexColor = EditorGUILayout.ColorField("Vertex Color", rg.vertexColor);
		
		EditorGUILayout.Space();
		
		showDrawSettings = EditorGUILayout.Foldout(showDrawSettings, "Draw & Render Settings");
		++EditorGUI.indentLevel;
		if(showDrawSettings){
			rg.renderGrid = EditorGUILayout.Toggle("Render Grid", rg.renderGrid);
			rg.useCustomRenderRange = EditorGUILayout.BeginToggleGroup("Use Custom Rendering Range", rg.useCustomRenderRange);
				showRenderRangeSettings = EditorGUILayout.Foldout(rg.useCustomRenderRange, "Rendering Range");
				if(showRenderRangeSettings){
					rg.renderFrom = Vector3.Min(rg.renderTo, EditorGUILayout.Vector3Field("From", rg.renderFrom));
					rg.renderTo = Vector3.Max(rg.renderFrom, EditorGUILayout.Vector3Field("To", rg.renderTo));
				}
			EditorGUILayout.EndToggleGroup();
			rg.renderMaterial = (Material) EditorGUILayout.ObjectField("Render Material", rg.renderMaterial, typeof(Material), false);
			rg.renderLineWidth = Mathf.Max(1, EditorGUILayout.IntField("Rendered Line Width", rg.renderLineWidth));
			
			rg.hideGrid = EditorGUILayout.Toggle("Hide Drawing", rg.hideGrid);
			rg.hideOnPlay = EditorGUILayout.Toggle("Hide While playing", rg.hideOnPlay);
			++EditorGUI.indentLevel;
			GUILayout.Label("Hide Axis (Render & Draw)t");
			rg.hideAxis.x = EditorGUILayout.Toggle("X", rg.hideAxis.x);
			rg.hideAxis.y = EditorGUILayout.Toggle("Y", rg.hideAxis.y);
			rg.hideAxis.z = EditorGUILayout.Toggle("Z", rg.hideAxis.z);
			--EditorGUI.indentLevel;
			
			rg.drawOrigin = EditorGUILayout.Toggle("Draw Origin", rg.drawOrigin);
		}
		--EditorGUI.indentLevel;
		
		if (GUI.changed)
			EditorUtility.SetDirty (target);
		
	}
}