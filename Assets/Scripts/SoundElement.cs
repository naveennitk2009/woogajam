using UnityEngine;
using System.Collections;

public class SoundElement : MonoBehaviour
{
	public SoundType Type;

	void OnTriggerEnter(Collider other)
	{
		var beatCollider = other.gameObject.GetComponent<BeatCollider>();
		beatCollider.CurrentSound = this;
	}

	void OnSuccess()
	{
		Debug.Log("Success");
	}

	void OnFail()
	{
		Debug.Log("Fail");
	}
}
