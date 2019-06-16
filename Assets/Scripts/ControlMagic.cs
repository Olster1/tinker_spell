using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMagic : MonoBehaviour
{
	public GameObject[] objects;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Fire4")) {
        	for(int i = 0; i < objects.Length; ++i) {
        		objects[i].SetActive(true);
        	}    		
    	} else {
			for(int i = 0; i < objects.Length; ++i) {
				objects[i].SetActive(false);
			}    		
    	}
    }
}
