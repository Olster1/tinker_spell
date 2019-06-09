using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgroundParralax : MonoBehaviour
{

	public float parralaxScale;				// Array of all the backgrounds to be parallaxed.

	private Transform cam;						// Shorter reference to the main camera's transform.
	private Vector3 originalPos;

	void Awake ()
	{
		// Setting up the reference shortcut.
		cam = Camera.main.transform;
	}

	void Start ()
	{
		
		originalPos = transform.position;
	}


	void Update ()
	{
		Vector2 newPos = originalPos + ((1.0f / 10.0f)*parralaxScale)*(cam.position - originalPos);
		
		transform.position = new Vector3(newPos.x, newPos.y, originalPos.z);
			
	}

}
