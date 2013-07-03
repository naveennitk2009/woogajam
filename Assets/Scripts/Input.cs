using UnityEngine;
using System.Collections;

public class Input : MonoBehaviour
{
	private BeatCollider beatCollider;

	void Awake()
	{
		beatCollider = GameObject.Find("Collider").GetComponent<BeatCollider>();
	}

	void OnStart()
	{
		
	}

	void OnTap(TapGesture gesture)
	{
		var middle = Screen.width/2;
		if (gesture.Position.x <= middle)
		{
			beatCollider.SendMessage("OnCheckSound", SoundType.Snare);
		}
		else
		{
			beatCollider.SendMessage("OnCheckSound", SoundType.Kick);
		}
	}
}
