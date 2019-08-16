using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyrdoDogRun : MonoBehaviour
{
	public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    	
    	Vector3 newP = transform.position;
    	newP.x +=  speed*Time.deltaTime;
        transform.position = newP;
    }
}
