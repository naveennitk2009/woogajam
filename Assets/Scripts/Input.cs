using UnityEngine;
using System.Collections;

public class Input : MonoBehaviour
{
	void OnTap(TapGesture gesture)
	{
		var middle = Screen.width/2;
		if (gesture.Position.x <= middle)
		{
			Debug.Log("Left tap");
		}
		else
		{
			Debug.Log("Right tap");
		}
	}
}
