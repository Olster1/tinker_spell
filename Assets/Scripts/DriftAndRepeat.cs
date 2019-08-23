using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriftAndRepeat : MonoBehaviour
{
	public float speed;
	public DriftAndRepeatParent parent;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
		Vector3 pos = transform.localPosition;
		pos.x += speed*Time.deltaTime;
		transform.localPosition = pos;
        
    }

    public void OnTriggerEnter2D(Collider2D other) {
    	GameObject gm = other.gameObject;
    	if(gm == parent) {
    		transform.localPosition = parent.lastObj.transform.localPosition + parent.offset;
    		parent.lastObj = gameObject;
    	}
    }
}
