using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgroundParralax : MonoBehaviour
{

	public float parralaxScale;				

	private Transform cam;	
	private Vector3 originalPos;

	public bool useParralax;
	private bool lastFrameParralax;
	void Awake ()
	{
		// Setting up the reference shortcut.
		cam = Camera.main.transform;
	}

	void Start ()
	{
		useParralax = true;
		originalPos = transform.position;
		lastFrameParralax = useParralax;
	}


	Vector2 calculateParralax(Vector3 inPos) {
		return ((1.0f / 10.0f)*parralaxScale)*(cam.position - inPos);
	}
	void Update ()
	{
		if(lastFrameParralax != useParralax && useParralax) {
			float scaleFactor = (1.0f / 10.0f)*parralaxScale;

			originalPos = (transform.position - scaleFactor*cam.position) / (1.0f - scaleFactor);
		}
		if(useParralax) {
			Vector2 newPos = new Vector2(originalPos.x, originalPos.y) + calculateParralax(originalPos);
			transform.position = new Vector3(newPos.x, newPos.y, originalPos.z);
		} 

		lastFrameParralax = useParralax;
	}

}
