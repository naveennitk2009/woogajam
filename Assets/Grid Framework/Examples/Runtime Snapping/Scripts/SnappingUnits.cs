using UnityEngine;
using System.Collections;

/*
	ABOUT THIS SCRIPT
	
This script makes objects stick to the ground of a grid, similar to how
buildings are placed in stategy games. Objects will always be placed
with the bottom touching the grid. In the example scene I rotated the
grid to create a sort of isometric perspective effect, so you are not
limited to a certain view. Just make sure you handle mouse input correctly.

This script demonstrates the snapping feature during runtime and
conversions from world space to grid space and back.
*/

public class SnappingUnits : MonoBehaviour {

	public GFGrid grid;
	
	//cache the transform for performance
	private Transform cachedTransform;
	
	void Awake () {
		cachedTransform = transform;
		
		//always make a sanity check
		if(grid){
			//perform an initial align and snap the objects to the bottom
			grid.AlignTransform(cachedTransform);
			cachedTransform.position = CalculateOffsetZ();
		}
	}
	
	//this function gets called every frame while the object (its collider) is being clicked
	void OnMouseDrag(){
		if(!grid)
			return;
		
		//handle mouse input to convert it to world coordinates
		Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
		Vector3 cursorWorldPoint = Camera.main.ScreenToWorldPoint(cursorScreenPoint);
		
		//we want to keep the Z coordinate, so apply it directly to the new position
		cursorWorldPoint.z = cachedTransform.position.z;
    	
		//change the X and Y coordinates according to the cursor (the Z coordinate stays the same)
		cachedTransform.position = cursorWorldPoint;
		
		//now align the obbject and snap it to the bottom.
		grid.AlignTransform(cachedTransform);
		cachedTransform.position = CalculateOffsetZ();
	}
	
	// makes the object snap to the bottom of the grid, respecting the grid's rotation
	Vector3 CalculateOffsetZ(){
		//first store the objects position in grid coordinates
		Vector3 gridPosition = grid.WorldToGrid(cachedTransform.position);
		//then change only the Z coordinate
		gridPosition.z = -0.5f * cachedTransform.lossyScale.z;
		
		//convert the result back to world coordinates
		return grid.GridToWorld(gridPosition);
	}
}
