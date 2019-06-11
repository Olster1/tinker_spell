using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieBox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D other) {
    	GameObject gm = other.gameObject;
    	
    	
    	if(gm.name == "Player") {
    	  PlayerMovement pm = gm.GetComponent<PlayerMovement>();
    	  pm.Die();
    	}
    }

}
