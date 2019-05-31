using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CheckGrounded : MonoBehaviour
{
    [HideInInspector] public int groundedCount;
    private List<int> idList;
    
    // Start is called before the first frame update
    private Animator parentAnimator;
    private PlayerMovement parentObj;
    public Rigidbody2D parentRigidBody;
    void Start()
    {
        idList = new List<int>();
        parentAnimator = gameObject.transform.parent.gameObject.GetComponent<Animator>();
        parentObj = gameObject.transform.parent.gameObject.GetComponent<PlayerMovement>();
        
    }
    
    public void OnTriggerEnter2D(Collider2D collision) {
        
        // Debug.Log("enter " + collision.gameObject.name);
        if(collision.gameObject.tag != "player" && !collision.isTrigger) {
            int id = collision.gameObject.GetInstanceID();
            // Assert.IsTrue(!idList.Contains(id));
            if(parentRigidBody.velocity.y <= 0) {
                groundedCount++;
                idList.Add(id);
            }
            
        } 
        
    }
    
    public void OnTriggerStay2D(Collider2D collision) {
        if(collision.gameObject.tag != "player" && !collision.isTrigger) {
            int id = collision.gameObject.GetInstanceID();
            if(parentRigidBody.velocity.y <= 0 && !idList.Contains(id)) {
                groundedCount++;
                idList.Add(id);
            }
            
        } 
    }
    
    public void OnTriggerExit2D(Collider2D collision)
    {
        // Debug.Log("exit " + collision.gameObject.name);
        if(collision.gameObject.tag != "player" && !collision.isTrigger) {
            int id = collision.gameObject.GetInstanceID();
            if(idList.Contains(id)) {
                groundedCount--;
                idList.Remove(id);
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
        //Debug.Log("grounded count" + (groundedCount));
        parentAnimator.SetBool("grounded", (groundedCount > 0));
        parentObj.isGrounded = (groundedCount > 0);

        // Debug.Log("grounded!" + (groundedCount > 0));
    }
}
