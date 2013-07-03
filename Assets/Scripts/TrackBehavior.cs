using UnityEngine;
using System.Collections;

public class TrackBehavior : MonoBehaviour
{

	private Transform _transform;

	public float Speed;

	// Use this for initialization
	void Start ()
	{
		_transform = this.transform;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		_transform.Translate(0,(-Speed/100),0);
	}
}
