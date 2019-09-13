using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayerSimple : MonoBehaviour
{
	public Transform playerTransform;
	public float xDiff;
	public float yDiff;

	public float xForce;
	public float yForce;

	public Rigidbody2D rigidBody;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
    	Vector3 newPos = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z);
    	Vector3 difference = newPos - transform.position;
    	Vector2 forceAccel = new Vector2();
    	if (Mathf.Abs(difference.x) > xDiff)
    	{
    	    forceAccel.x = Mathf.Sign(difference.x)*xForce;
    	    // Debug.Log("outside of x region");
    	}
    	if (Mathf.Abs(difference.y) > yDiff)
    	{
    	    forceAccel.y = Mathf.Sign(difference.y) * yForce;
    	    // Debug.Log("outside of y region");
    	}

    	rigidBody.AddForce(forceAccel);
    }
}
