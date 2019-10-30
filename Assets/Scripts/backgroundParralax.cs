using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgroundParralax : MonoBehaviour
{

	public float parralaxScale;				

	private Transform cam;	
	public Vector3 originalPos;

	public Vector3 newT;

	public bool useParralax;
	private bool lastFrameParralax;

	void Awake ()
	{
		cam = Camera.main.transform;
	}

	void Start ()
	{
		useParralax = true;
		originalPos = transform.position;
		lastFrameParralax = useParralax;
		newT = transform.InverseTransformPoint(transform.position);


	}


	Vector2 calculateParralax(Vector3 inPos) {
		return ((1.0f / 10.0f)*parralaxScale)*(cam.position - inPos);
	}
	void Update ()
	{
		// if(lastFrameParralax != useParralax && useParralax) {
		// 	float scaleFactor = (1.0f / 10.0f)*parralaxScale;

		// 	originalPos = (transform.position - scaleFactor*cam.position) / (1.0f - scaleFactor);

		// 	Vector3 p = transform.position;
		// 	transform.position = originalPos;
		// 	newT = transform.localPosition;

		// }
		// if(useParralax) {
		// 	Vector2 newPos = new Vector2(originalPos.x, originalPos.y) + calculateParralax(originalPos);
		// 	transform.position = new Vector3(newPos.x, newPos.y, 0);

		// } 

		// lastFrameParralax = useParralax;
	}

}
