using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopAmberBounce : MonoBehaviour
{
	public Rigidbody2D body;
	public AudioSource bounceSound;

    // Start is called before the first frame update
    void Start()
    {
        // body.solverIterations = 30;
    }

     // void OnCollisionEnter2D(Collision2D col) {
     // 	hitCount++;
     // 	if(hitCount > 5) {
     // 		body.gravityScale = 0;
     // 	}
     // }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        bounceSound.Play();
       
    }
}
