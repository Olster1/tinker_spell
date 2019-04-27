using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGrounded : MonoBehaviour
{
    private int groundedCount;
    
    // Start is called before the first frame update
    private Animator parentAnimator;
    private PlayerMovement parentObj;
    void Start()
    {
        parentAnimator = gameObject.transform.parent.gameObject.GetComponent<Animator>();
        parentObj = gameObject.transform.parent.gameObject.GetComponent<PlayerMovement>();
        
    }
    
    public void OnTriggerEnter2D(Collider2D collision) {
        
        // Debug.Log("enter " + collision.gameObject.name);
        if(collision.gameObject.tag != "player" && !collision.isTrigger) {
            groundedCount++;
        } 
        
    }
    
    public void OnTriggerStay2D(Collider2D col) {
        //Debug.Log(col.gameObject.name);
    }
    
    public void OnTriggerExit2D(Collider2D collision)
    {
        // Debug.Log("exit " + collision.gameObject.name);
        if(collision.gameObject.tag != "player" && !collision.isTrigger) {
            groundedCount--;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
        //Debug.Log("grounded count" + (groundedCount));
        parentAnimator.SetBool("grounded", (groundedCount > 0));
        parentObj.isGrounded = (groundedCount > 0);
    }
}
