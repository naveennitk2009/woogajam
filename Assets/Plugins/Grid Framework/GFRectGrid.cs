using UnityEngine;
using System.Collections;

/* TO DO

- Finding methods need to use WorldToGrid and GridToWorld (all done)
- Same thing for the Get Functions (Vertex done)
*/

[System.Serializable]
public class GFRectGrid: GFGrid{
	public Vector3 spacing = new Vector3(1f, 1f, 1f);
	public Vector3 minimumSpacing = new Vector3(1f, 1f, 1f);
	
//_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-
	
	#region grid to world
	
	public override Vector3 WorldToGrid(Vector3 worldPoint){
		if(!cachedTransform)
			cachedTransform = transform;
		Vector3 gridPoint = cachedTransform.GFInverseTransformPointFixed(worldPoint).GFReverseScale(spacing);
		return gridPoint;
	}
	
	//takes the coordinates of something inside the grid and returns its world coordinates
	public override Vector3 GridToWorld(Vector3 gridPoint){
		if(!cachedTransform)
			cachedTransform = transform;
		Vector3 worldPoint = gridPoint;
		worldPoint.Scale(spacing);
		worldPoint = cachedTransform.GFTransformPointFixed(worldPoint);
		return worldPoint;
	}
	
	#endregion
	
//_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-

	#region get coordinates
	
	//returns XYZ grid coordinates of a vertex close to a given point
	public override Vector3 GetVertexCoordinates(Vector3 fromPoint){
		return WorldToGrid(FindNearestVertex(fromPoint));
	}
	
	/*		A NOTE ABOUT BOXES AND FACES
	 * 
	 * For boxes and faces I need to return the coordinates of the space between two grid
	 * "bars", but there is no central one I could use a zero. That's why I have chosen
	 * the one right from the zero bar (i. e. positive from the centre) as zero.
	 * 
	 * 	EXAMPLE: for the X-axis
	 * 		
	 * 		i.i.i.i:I.i.i.i
	 * 		i.i.i.i:I.i.i.i
	 * 		i.i.i.i:I.i.i.i
	 * 
	 * 	the dots are vertices, the lower case i are the spaces between them and the colon (:)
	 * 	is the central vertex. The space between the central vertex and vertex 1 is the zero
	 * 	space.
	*/  
	
	//returns XYZ grid coordinates of a box close to a given point
	public override Vector3 GetBoxCoordinates(Vector3 fromPoint){
		return WorldToGrid(FindNearestBox(fromPoint)) - 0.5f * Vector3.one;
	}
	
	//returns XYZ grid coordinates of a face close to a given point
	public override Vector3 GetFaceCoordinates(Vector3 fromPoint, GridPlane thePlane){
		//get the grid coordinates of the face
		Vector3 face = GetBoxCoordinates(FindNearestFace(fromPoint, thePlane));
		// two of the face coordinates are in a box, the other is on the vertex closest to the face
		face[(int)thePlane] = FindNearestVertex(fromPoint)[(int)thePlane];
		
		return face;
	}
	
	#endregion
	
//_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-

	#region finding functions
	
	public override Vector3 FindNearestVertex(Vector3 fromPoint, bool doDebug = false){
		//convert fromPoint to grid coordinates first
		Vector3 toPoint = WorldToGrid(fromPoint);
		
		// each coordinate has to be set to a multiple of spacing
		for(int i = 0; i<=2; i++){
			toPoint[i] = Mathf.Round(toPoint[i]);
		}
		
		//back to World coordinates
		toPoint = GridToWorld(toPoint);
		
		if(doDebug){
			Gizmos.DrawSphere(toPoint, 0.3f);
		}
		return toPoint;
	}
	
	public override Vector3 FindNearestFace(Vector3 fromPoint, GridPlane thePlane, bool doDebug = false){
		//get a temporary point (world space)
		Vector3 toPoint = FindNearestBox(fromPoint);
		
		//snap to the plane
		toPoint[(int)thePlane] = FindNearestVertex(fromPoint)[(int)thePlane];
		
		//debugging
		if(doDebug){
			Vector3 debugCube = spacing;
			debugCube[(int)thePlane] = 0.0f;
			
			//store the old matrix and create a new one based on the grid's roation and the point's position
			Matrix4x4 oldRotationMatrix = Gizmos.matrix;
			Matrix4x4 newRotationMatrix = Matrix4x4.TRS(toPoint, transform.rotation, Vector3.one);
			
			Gizmos.matrix = newRotationMatrix;
			Gizmos.DrawCube(Vector3.zero, debugCube);//Position zero because the matrix already contains the point
			Gizmos.matrix = oldRotationMatrix;
		}
		
		return toPoint;
	}
	
	public override Vector3 FindNearestBox(Vector3 fromPoint, bool doDebug = false){
		//convert fromPoint to grid coordinates first
		Vector3 toPoint = WorldToGrid(fromPoint);
		
		//loops through the XYZ components
		for(int i = 0; i<=2; i++){
			toPoint[i] = Mathf.Round(toPoint[i]-0.5f)+0.5f;
		}
		
		toPoint = GridToWorld(toPoint);
		
		if(doDebug){
			//store the old matrix and create a new one based on the grid's roation and the point's position
			Matrix4x4 oldRotationMatrix = Gizmos.matrix;
			Matrix4x4 newRotationMatrix = Matrix4x4.TRS(toPoint, transform.rotation, Vector3.one);
			
			Gizmos.matrix = newRotationMatrix;
			Gizmos.DrawCube(Vector3.zero, spacing);
			Gizmos.matrix = oldRotationMatrix;
		}
		//convert back to world coordinates
		return toPoint;
	}
	
	#endregion
	
//_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-
	
	#region VertexMatrixMethods
	
	//NOTE ABOUT MATRIX ORDER: The matrix starts in the topright most backwards corner, the first component
	//	represents the width, from right to left, the second one the height from top to bottom, the third
	//	the depth from back to front
	
	public override Vector3[,,] BuildVertexMatrix(float width, float height, float depth){
		//prevent negative values
		width = Mathf.Abs(width);
		height = Mathf.Abs(height);
		depth = Mathf.Abs(depth);
		Vector3 matrixSize = new Vector3(width, height, depth);

		if(!cachedTransform)
			cachedTransform = transform;
			
		Vector3 iterationVector = Vector3.zero;
		for(int n=0; n <=2; n++){
			iterationVector[n] = Mathf.Floor(matrixSize[n] / 1.0f);
		}

		Vector3[,,] vertexMatrix = new Vector3[2*(int)Mathf.Floor(width)+1, 2*(int)Mathf.Floor(height)+1, 2*(int)Mathf.Floor(depth)+1];

		for(int i = 0; i <= 2*(int)Mathf.Floor(width); i++){
			for(int j = 0; j <= 2*(int)Mathf.Floor(height); j++){
				for(int k = 0; k <= 2*(int)Mathf.Floor(depth); k++){
					vertexMatrix[i,j,k] = GridToWorld(iterationVector - new Vector3(i,j, k));
				}
			}
		}
//		Debug.Log("Matrix size: " + vertexMatrix.GetUpperBound(0) +"/"+ vertexMatrix.GetUpperBound(1) +"/"+ vertexMatrix.GetUpperBound(2));
		
		return vertexMatrix;
	}
	
	// return an entry from the vertex matrix in a cartesian fashion (central vertex is (0,0,0), first component is the x-axis, second one the y-axis,
	// third one the z-axis
	public override Vector3 ReadVertexMatrix(int x, int y, int z, Vector3[,,] vertexMatrix, bool warning = false){
		if(Mathf.Abs(x)>vertexMatrix.GetUpperBound(0)/2 || Mathf.Abs(y) >vertexMatrix.GetUpperBound(1)/2 || Mathf.Abs(z) >vertexMatrix.GetUpperBound(2)/2){
			if(warning)
				Debug.LogWarning("coordinates too large for this matrix, will default to " + Vector3.zero);
			return vertexMatrix[(vertexMatrix.GetUpperBound(0)/2), (vertexMatrix.GetUpperBound(1)/2), (vertexMatrix.GetUpperBound(2)/2)];
		}
		return vertexMatrix[(vertexMatrix.GetUpperBound(0)/2) - x, (vertexMatrix.GetUpperBound(1)/2) - y, (vertexMatrix.GetUpperBound(2)/2) - z];
	}
	
	#endregion

//_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-

	#region AlignScaleMethods
	
	public override Vector3 AlignVector3(Vector3 position, Vector3 scale, GFBoolVector3 lockAxis, bool beTolerant = true){
		Vector3 currentPosition = WorldToGrid(position);
		Vector3 newPosition = WorldToGrid(FindNearestBox(position));
		bool withinTolerance = true;
		
		for (int i = 0; i <=2; i++){
			if((scale[i] / spacing[i]) % 2f <= 0.5f){ // i.e. the scale is an even multiple of spacing
				newPosition[i] = newPosition[i] + Mathf.Sign (WorldToGrid(position)[i] % 1f - Mathf.Sign(WorldToGrid(position)[i])*0.5f) * 0.5f;
			}
		}
		
		//check tolerance
		for (int j = 0; j <=2; j++){
//			Debug.Log(Mathf.Abs(currenPosition[j] - newPosition[j]));
			if (Mathf.Abs(currentPosition[j] - newPosition[j]) >= 0.01f && Mathf.Abs(currentPosition[j] - newPosition[j]) <0.9f)
				withinTolerance = false;
		}
		
		// don't apply aligning if the axis has been locked+
		for(int i = 0; i < 3; i++){
			if(lockAxis[i])
				newPosition[i] = currentPosition[i];
		}
		
		if(beTolerant && withinTolerance)
			return position;
		return GridToWorld(newPosition);
	}
	
	//scale the Transform to the grid
	public override Vector3 ScaleVector3(Vector3 scale, GFBoolVector3 lockAxis){
		Vector3 relScale = scale.GFModulo3(spacing);
		Vector3 newScale = Vector3.zero;
		
		for (int i = 0; i <= 2; i++){
			newScale[i] = scale[i];
			
			if(relScale[i] >= 0.5f * spacing[i]){
//				Debug.Log ("Grow by " + (spacing.x - relScale.x));
				newScale[i] = newScale[i] - relScale[i] + spacing[i];
			} else{
//				Debug.Log ("Shrink by " + relativeScale.x);
				newScale[i] = newScale[i] - relScale[i];
				//if we went too far default to the spacing
				if(newScale[i] < spacing[i])
					newScale[i] = spacing[i];
			}		
		}
		
		for(int i = 0; i < 3; i++){
			if(lockAxis[i])
				newScale[i] = scale[i];
		}
		
		return  newScale;
	}
	
	#endregion

//_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-
	
	#region DrawingMethods

	public override void DrawGrid(Vector3 from, Vector3 to){
		if(!cachedTransform)
			cachedTransform = transform;
		
		//don't draw if not supposed to
		if(hideGrid)
			return;
			
		Vector3[][][] lines = CalculateDrawPoints(from, to);
		
		for(int i = 0; i < 3; i++){
			if(hideAxis[i])
				continue;
			Gizmos.color = axisColors[i];
			foreach(Vector3[] line in lines[i])
				Gizmos.DrawLine(line[0], line[1]);
		}
		
		//draw a sphere at the centre
		if(drawOrigin){
			Gizmos.color = Color.white;
			Gizmos.DrawSphere(cachedTransform.position, 0.3f);
		}
	}
		
	#endregion
	
//_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-
	
	#region Render Methods
	
	public override void RenderGrid(Vector3 from, Vector3 to, int width = 0, Camera cam = null, Transform camTransform = null){
		if(!renderGrid)
			return;
		
		//don't let the spacing go below the minimum or you get caught in too large loops
		for(int i = 0; i <= 2; i++){
			if(spacing[i] < minimumSpacing[i]) spacing[i] = minimumSpacing[i];
		}
		if(!renderMaterial)
			renderMaterial = defaultRenderMaterial;
		
		Vector3[][][] lines = CalculateDrawPoints(from, to);
				
		renderMaterial.SetPass(0);
		
		if(width <= 1 || !cam || !camTransform){// use simple lines for width 1 or if no camera was passed
			GL.Begin(GL.LINES);
			for(int i = 0; i < 3; i++){
				if(hideAxis[i])
					continue;
				GL.Color(axisColors[i]);
				foreach(Vector3[] line in lines[i]){
					GL.Vertex(line[0]); GL.Vertex(line[1]);
				}
			}
			GL.End();
		} else{// quads for "lines" with width
			GL.Begin(GL.QUADS);
			float mult = Mathf.Max(0, 0.5f * width); //the multiplier, half the desired width
			
			for(int i = 0; i < 3; i++){
				GL.Color(axisColors[i]);
				if(hideAxis[i])
					continue;
				
				//sample a direction vector, one per direction is enough (using the first line of each line set
				Vector3 dir = new Vector3();
				if(lines[i].Length > 0) //can't get a line if the set is empty
					dir = Vector3.Cross(lines[i][0][0] - lines[i][0][1], camTransform.forward).normalized;
				//multiply dir with the world length of one pixel in distance
				if(cam.isOrthoGraphic){
					dir *= (cam.orthographicSize * 2) / cam.pixelHeight;
				} else{// (the 50 below is just there to smooth things out)
					dir *= (cam.ScreenToWorldPoint(new Vector3(0, 0, 50)) - cam.ScreenToWorldPoint(new Vector3(20, 0, 50))).magnitude/20;
				}
						
				foreach(Vector3[] line in lines[i]){
					if(line == null) continue;
					
					Vector3 vec1 = line[0];
					Vector3 vec2 = line[1];
					GL.Vertex(vec1-mult*dir); GL.Vertex(vec1+mult*dir); GL.Vertex(vec2+mult*dir); GL.Vertex(vec2-mult*dir);
					
				}
			}
			GL.End();
		}
	}
	
	#endregion
	
//_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-
	
	#region Draw Gizoms
	
	public void OnDrawGizmos(){
		if(useCustomRenderRange){
			DrawGrid(renderFrom, renderTo);
		} else{
			DrawGrid();
		}
	}
	
	#endregion
	
	#region Calculate draw points
	//This function returns a three-dimensional jagged array. The most inner layer contains
	// a pair of points for one line, the second layer contains the sets of all lines in the
	// same direction and the third layer contains all the sets.
	//The first set of lines are the horizontal X-lines, their amount depends on Y and Z. The
	// second set are the vertical lines (X and Z), the third set the forward lines (X and Y).
	protected override Vector3[][][] CalculateDrawPoints(Vector3 from, Vector3 to){
		Vector3[][][] lineSets = new Vector3[3][][];
		
		float[] length = new float[3];
		for(int i = 0; i < 3; i++){
			length[i] = to[i] - from[i];
		}

		
		//the amount of lines for each direction
		int[] amount = new int[3];
		for(int i = 0; i < 3; i++){
			amount[i] = Mathf.FloorToInt(to[i] / spacing[i]) - Mathf.CeilToInt(from[i] / spacing[i]) + 1;
		}
				
		//the starting point of the first pair (an iteration vector will be added to this)
		Vector3[] startPoint = new Vector3[3]{
			//everything in the right top front
			cachedTransform.GFTransformPointFixed(new Vector3(to.x, spacing.y * Mathf.Floor(to.y / spacing.y), spacing.z * Mathf.Floor(to.z / spacing.z))),
			cachedTransform.GFTransformPointFixed(new Vector3(spacing.x * Mathf.Floor(to.x / spacing.x), to.y, spacing.z * Mathf.Floor(to.z / spacing.z))),
			cachedTransform.GFTransformPointFixed(new Vector3(spacing.x * Mathf.Floor(to.x / spacing.x), spacing.y * Mathf.Floor(to.y / spacing.y), to.z))
		};
		
		//this will be added to each first point in a pair
		Vector3[] endDirection = new Vector3[3]{
			cachedTransform.TransformDirection(new Vector3(-Mathf.Abs(to.x - from.x), 0.0f, 0.0f)),
			cachedTransform.TransformDirection(new Vector3(0.0f, -Mathf.Abs(to.y - from.y), 0.0f)),
			cachedTransform.TransformDirection(new Vector3(0.0f, 0.0f, -Mathf.Abs(to.z - from.z)))
		};
		
		//a multiple of this will be added to the starting point for iteration
		Vector3[] iterationVector = new Vector3[3]{
			cachedTransform.TransformDirection(new Vector3(-spacing.x, 0.0f, 0.0f)),
			cachedTransform.TransformDirection(new Vector3(0.0f, -spacing.y, 0.0f)),
			cachedTransform.TransformDirection(new Vector3(0.0f, 0.0f, -spacing.z))
		};
		
		// assemble the array
		for(int i = 0; i < 3; i++){			
			//when collecting the line sets we need to know the amount of lines, it depends on
			// the other two coordinates that don't affect the line's size. Get them using modulo
			int idx1 = ((i+1)%3);
			int idx2 = ((i+2)%3);
			int iterator = 0;//j+k won't do it if one is larger than zero, use this independent iterator
			
			Vector3[][] lineSet = new Vector3[amount[idx1]*amount[idx2]][];
			
			if(to[i] - from[i] <= 0.01f){// no need for a huge line set no one will see
				lineSet = new Vector3[0][];
			} else{
				for(int j = 0; j < amount[idx1]; ++j){
					for(int k = 0; k < amount[idx2]; ++k){
						Vector3[] line = new Vector3[2];
						line[0] = startPoint[i] + j*iterationVector[idx1] + k*iterationVector[idx2];
						line[1] = line[0] + endDirection[i];
						lineSet[iterator] = line;
						iterator++;
					}
				}
			}
			lineSets[i] = lineSet;
		}
		
		return lineSets;
	}
	#endregion

}