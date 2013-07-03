using UnityEngine;
using System.Collections;

/*
	ABOUT THIS SCRIPT
	
This script is not a MonoBehaviour, it's a public static class that doesn't inherit from
anything. The class has three functions: it stores a two-dimensional array of bool values
(I will call this array matrix from now on and its entries squares), it sets and return
the value of squares based on which world positions they correspond to.
	Each square represents a face of a rectangular grid on the XY plane and its position in
the matrix mirrors its position in the grid with (0,0) being the lower left face, the first
indes denotes the row and the second index the column of a face in the grid.
	This class works together with two other scripts, SetupForbiddenFiles and BlockSquare.
The matrix is built first by SetupForbiddenFiles in the Awake() function to make sure it is
built before any Start() function gts called. It just calles the matrix-building method and
passes which grid to use fore reference. Then each obstacle fires the Start() function from
its BlockSquare script to set their tiles as forbidden. The palyer's movement script makes use
of the CheckSquare function to find out if a face is forbidden or not, but it doesn't write
anything to the matrix.

This script demonstrates how you can use Grid Framework to store information about individual
tiles apart from the objects they belong to in a format accessible to all objects in the scene.
	The information doesn not have to be limited to just boolean values, you can extend this
approach to store any other information you might have and build more complex game machanics
around it.
*/

public static class ForbiddenTilesExample{
	public static bool[,] allowedTiles; //two-dimensional array of bool values
	public static GFRectGrid movementGrid; //the grid everything is based on
	public static int[] originSquare; //the grid coordinates of thelower left square used for reference (X and Y only)
	
	//builds the matrix and sets everything up, gets called by a script attached to the grid object
	public static void Initialize(GFRectGrid theGrid){
		movementGrid = theGrid;
		BuildMatrix(); //builds a default matrix that has all entries set to tru
		SetOriginSquare(); //stores the X and Y grid coordinates of the lower left square
	}
	
	//takes the grids size or rendering range and builds a magtrix based on that. All entries are set to true
	public static void BuildMatrix(){
		//amount of rows and columns, either based on size or rendering range (first entry row, second one column)
		int[] size = new int[2];
		for(int i = 0; i < 2; i++){
			size[i] = movementGrid.useCustomRenderRange? Mathf.FloorToInt(Mathf.Abs(movementGrid.renderFrom[i] - movementGrid.renderTo[i]) / movementGrid.spacing[i]):
			2 * Mathf.FloorToInt(movementGrid.size[i] / movementGrid.spacing[i]);
		}
				
		//build a default matrix
		allowedTiles = new bool[size[0], size[1]];
		//set all entries to true
		for(int i = 0; i < size[0]; i++){
			for( int j = 0; j < size[1]; j++){
				allowedTiles[i,j] = true;
			}
		}
	}
	
	//stores the grid coodinates of the lower left square. This is needed to properly map a grid's face to a matrix entry
	public static void SetOriginSquare(){
		//get the grid coordinates of the box; we get three coordinates, but we only use X and Y
		//we add 0.1f * Vector3.one to avoid unexpected behaviour for edge cases dues to rounding and float (in)accuracy
		Vector3 box = movementGrid.useCustomRenderRange? movementGrid.GetBoxCoordinates(movementGrid.transform.position + movementGrid.renderFrom + 0.1f * Vector3.one):
			movementGrid.GetBoxCoordinates(movementGrid.transform.position - movementGrid.size + 0.1f * Vector3.one);
		originSquare = new int[2]{Mathf.RoundToInt(box.x), Mathf.RoundToInt(box.y)};
	}
	
	//takes world coodinates, finds the corresponding square and sets that entry to either true or false. Use it to disable or enable squares
	public static void RegisterSquare(Vector3 vec, bool status){
		//first find the square that belongs to that world position
		int[] square = GetSquare(vec);
		//then set its value
		allowedTiles[square[0],square[1]] = status;
	}
	
	//takes world coodinates, finds the corresponding square and returns the value of that square. Use it to cheack if a square is forbidden or not
	public static bool CheckSquare(Vector3 vec){
		int[] square = GetSquare(vec);
		return allowedTiles[square[0],square[1]];
	}
	
	//takes world coodinates and finds the corresponding square. The result is returned as an int array that contains that square's position in the matrix
	private static int[] GetSquare(Vector3 vec){
		int[] square = new int [2];
		for(int i = 0; i < 2; i++){
			square[i] = Mathf.RoundToInt(movementGrid.GetBoxCoordinates(vec)[i]) - originSquare[i];
		}
		
		return square;
	}
}
