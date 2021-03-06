using UnityEngine;
using UnityEditor;
using System;

public class GFMenuItems : MonoBehaviour {
	
	#region Grid Creation
	[MenuItem("GameObject/Create Grid/Rectangular Grid", false, 0)]
	public static void CreateRectGrid(){
		CreateGrid<GFRectGrid>("Rectangular ");
	}
	
	[MenuItem("GameObject/Create Grid/Hexagonal Grid", false, 0)]
	public static void CreateHexGrid(){
		CreateGrid<GFHexGrid>("Hex ");
	}
	#endregion
	
	#region Grid Component
	[MenuItem("Component/Grid Framework/GFRectGrid", true)]
	public static bool ValidateAddRectGrid(){
		return Selection.gameObjects.Length != 0;
	}
	[MenuItem("Component/Grid Framework/GFRectGrid")]
	public static void AddRectGrid(){
		AddGrid<GFRectGrid>();
	}
	
	[MenuItem("Component/Grid Framework/GFHexGrid", true)]
	public static bool ValidateAddHexGrid(){
		return Selection.gameObjects.Length != 0;
	}
	[MenuItem("Component/Grid Framework/GFHexGrid")]
	public static void AddHexGrid(){
		AddGrid<GFHexGrid>();
	}
	#endregion
	
	#region Camera Scripts
	[MenuItem("Component/Grid Framework/GFGridRenderCamera", true)]
	public static bool ValidateAddGridRenderCamera(){
		return Selection.gameObjects.Length != 0;
	}
	[MenuItem("Component/Grid Framework/GFGridRenderCamera")]
	public static void AddGridRenderCamera(){
		foreach(GameObject go in Selection.gameObjects){
			if(!go.GetComponent<GFGridRenderCamera>() && go.GetComponent<Camera>()) go.AddComponent("GFGridRenderCamera");
		}
	}
	#endregion
	
	#region Helper Functions
	private static void CreateGrid<T>(String name) where T: GFGrid{
		GameObject go = new GameObject(name +  "Grid");
		go.AddComponent<T>();
		//set go's position to the scene view's pivot point, the "centre" of
		// the scene editor. The SceneView class is undocumented, so this could
		// break in the future.
		go.transform.position = SceneView.lastActiveSceneView.pivot;
	}
	
	private static void AddGrid<T>() where T: GFGrid{
		foreach(GameObject go in Selection.gameObjects){
			if(!go.GetComponent<T>()) go.AddComponent<T>();
		}
	}
	#endregion
}
