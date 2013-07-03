using UnityEngine;
using System.Collections;

public class BeatCollider : MonoBehaviour {
	
	public SoundElement CurrentSound { get; set; }

	void OnCheckSound(SoundType sound)
	{
		if (CurrentSound.Type == sound)
		{
			CurrentSound.SendMessage("OnSuccess");
		}
		else
		{
			CurrentSound.SendMessage("OnFail");
		}
	}
}
