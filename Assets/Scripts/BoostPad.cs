using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPad : MonoBehaviour
{
	public Vector2 force;
	public PlayerMovement playerMove;
	[HideInInspector] public Vector2 unitVec;

    // Start is called before the first frame update
    void Start()
    {
        float rot = transform.rotation.z;
        unitVec = new Vector2(Mathf.Cos(rot), Mathf.Sin(rot));
        unitVec.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



     private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
        	playerMove.booster = this;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
        	playerMove.booster = null;
        }
    }

}
