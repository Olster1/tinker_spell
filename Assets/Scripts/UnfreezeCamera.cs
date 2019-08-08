using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnfreezeCamera : MonoBehaviour
{
    public Rigidbody2D camRb;
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
	          camRb.constraints &= ~RigidbodyConstraints2D.FreezeAll;
	      }
    }
}
