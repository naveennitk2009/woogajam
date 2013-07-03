using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(GFHexGrid))]

public class GFHexGridEditor : Editor {
	
	private static bool showDrawSettings;
	private static bool showRenderRangeSettings;
		
	public override void OnInspectorGUI(){
		GFHexGrid hg = target as GFHexGrid;
		
		hg.radius = Mathf.Max(EditorGUILayout.FloatField("Radius", hg.radius), hg.minimumRadius);
		hg.minimumRadius = Mathf.Max(EditorGUILayout.FloatField("Minimum Radius", hg.minimumRadius), 0.1f);
		hg.depth = Mathf.Max(EditorGUILayout.FloatField("Depth", hg.depth), 0);
		hg.gridPlane = (GFGrid.GridPlane) EditorGUILayout.EnumPopup("Grid Plane", hg.gridPlane);
		hg.hexSideMode = (GFHexGrid.HexOrientation) EditorGUILayout.EnumPopup("Hex Grid Orientation", hg.hexSideMode);
		
		hg.size = Vector3.Max(EditorGUILayout.Vector3Field("Size", hg.size), Vector3.zero);
		
		EditorGUILayout.Space();
		
		GUILayout.Label("Axis Colors");
		
		EditorGUILayout.BeginHorizontal();
		++EditorGUI.indentLevel;
		hg.axisColors.x = EditorGUILayout.ColorField(hg.axisColors.x);
		hg.axisColors.y = EditorGUILayout.ColorField(hg.axisColors.y);
		hg.axisColors.z = EditorGUILayout.ColorField(hg.axisColors.z);
		--EditorGUI.indentLevel;
		EditorGUILayout.EndHorizontal();
		
		hg.vertexColor = EditorGUILayout.ColorField("Vertex Color", hg.vertexColor);
		
		EditorGUILayout.Space();
		
		showDrawSettings = EditorGUILayout.Foldout(showDrawSettings, "Draw & Render Settings");
		++EditorGUI.indentLevel;
		if(showDrawSettings){
			hg.renderGrid = EditorGUILayout.Toggle("Render Grid", hg.renderGrid);
			hg.gridStyle = (GFHexGrid.HexGridShape)EditorGUILayout.EnumPopup("Grid Style", hg.gridStyle);
			hg.useCustomRenderRange = EditorGUILayout.BeginToggleGroup("Use Custom Rendering Range", hg.useCustomRenderRange);
				showRenderRangeSettings = EditorGUILayout.Foldout(hg.useCustomRenderRange, "Rendering Range");
				if(showRenderRangeSettings){
					hg.renderFrom = Vector3.Min(hg.renderTo, EditorGUILayout.Vector3Field("From", hg.renderFrom));
					hg.renderTo = Vector3.Max(hg.renderFrom, EditorGUILayout.Vector3Field("To", hg.renderTo));
				}
			EditorGUILayout.EndToggleGroup();
			hg.renderMaterial = (Material) EditorGUILayout.ObjectField("Render Material", hg.renderMaterial, typeof(Material), false);
			hg.renderLineWidth = Mathf.Max(1, EditorGUILayout.IntField("Rendered Line Width", hg.renderLineWidth));
			
			hg.hideGrid = EditorGUILayout.Toggle("Hide Drawing", hg.hideGrid);
			hg.hideOnPlay = EditorGUILayout.Toggle("Hide While playing", hg.hideOnPlay);
			++EditorGUI.indentLevel;
			GUILayout.Label("Hide Axis (Render & Draw)");
			hg.hideAxis.x = EditorGUILayout.Toggle("X", hg.hideAxis.x);
			hg.hideAxis.y = EditorGUILayout.Toggle("Y", hg.hideAxis.y);
			hg.hideAxis.z = EditorGUILayout.Toggle("Z", hg.hideAxis.z);
			--EditorGUI.indentLevel;
			
			hg.drawOrigin = EditorGUILayout.Toggle("Draw Origin", hg.drawOrigin);
		}
		--EditorGUI.indentLevel;
		
		if (GUI.changed)
			EditorUtility.SetDirty (target);
	}
}
