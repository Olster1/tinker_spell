using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;
using UnityEngine.Assertions;

public class ZoomTrigger : MonoBehaviour
{
	private bool zoomTimer;
 	public float targetSize;
 	public float zoomSpeed;

 	private Transform camT;
 	private Transform parentT;

    void Start()
    {
        zoomTimer = false;

        camT = Camera.main.transform;
        parentT = Camera.main.transform.parent;
    }



    void Update()
    {
        if(zoomTimer) {
        	float localTarget = -(targetSize + parentT.position.z);
        	float z = Mathf.Lerp(camT.localPosition.z, localTarget, zoomSpeed);
        	Vector3 p = camT.localPosition;
        	camT.localPosition = new Vector3(p.x, p.y, z);

        	float d = Mathf.Abs(targetSize - z);
        	if(d < 0.01f) {
        		zoomTimer = false; //stop trying to zoom
        	}
        	
        }
    }

     public void StopZoom() {
    	zoomTimer = false;
    	
    }

     public void OnTriggerEnter2D(Collider2D other) {
     	string name = other.gameObject.name;
     	Debug.Log(other.gameObject.GetInstanceID()); 
		if (name == "Player" && !zoomTimer) {
			zoomTimer = true;
			if(name == "Player") {
				Debug.Log(gameObject.name);
				other.gameObject.GetComponent<PlayerMovement>().SetZoomTrigger(this);	
			} else {
				Assert.IsTrue(false); //NOTE(ol); not implemented
			}
		}
    }
}
